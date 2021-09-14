using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Fx;
using Un4seen.Bass.AddOn.Tags;
using static Un4seen.Bass.Bass;

namespace FantasticMusicPlayer
{
    class BassPlayerImpl : IBassPlayer
    {

        

        private bool disposedValue;

        public float[] Spectrum => fft;

        public long CurrentPosition { get {
                long result = (long)(BASS_ChannelBytes2Seconds(currentPlaying, BASS_ChannelGetPosition(currentPlaying, BASSMode.BASS_POS_BYTE)) * 1000L);
                if (result < 0) { result = 0; }return result;
                    } set => BASS_ChannelSetPosition(currentPlaying, Math.Min((TotalPosition-10) / 1000d,value / 1000d)); }

        public long TotalPosition =>Math.Max(1,(long)(BASS_ChannelBytes2Seconds(currentPlaying, BASS_ChannelGetLength(currentPlaying, BASSMode.BASS_POS_BYTE)) * 1000L) + 1);

        private float _volume = 1;
        public float Volume
        {
            get
            {
                return _volume;
            }
            set  {
                _volume = value;
                updateVolume();
            }
        }
        public bool IsPlaying { get => BASS_ChannelIsActive(currentPlaying) == BASSActive.BASS_ACTIVE_PLAYING; set {
                if (value)
                {
                    Play();
                }
                else {
                    Pause();
                }
            }
        }

        private bool _bassboosted = false;
        public bool BassBoost { get => _bassboosted; set {
                _bassboosted = value;
                applyFxStatus();
            } 
        }

        public event EventHandler<EventArgs> Stopped;
        public event EventHandler<AlbumEventArgs> CoverAvailable;
        public event EventHandler<SongInfoEventArgs> SongInfoAvailable;

        BASSTimer updateTimer;

        public void init()
        {
            BASS_SetConfig(BASSConfig.BASS_CONFIG_FLOATDSP, true);
            if (BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero))
            {
                updateTimer = new BASSTimer(1);
                updateTimer.Tick += UpdateTimer_Tick;
            }
            else {
                BASSError berr = BASS_ErrorGetCode();
                if (berr == BASSError.BASS_ERROR_DEVICE)
                {
                    System.Windows.Forms.MessageBox.Show("没有可用的音频输出设备，请检查是否插入耳机/音响");
                }
                else {
                    System.Windows.Forms.MessageBox.Show(berr.ToString(),"BASS_INIT_ERROR");
                }

                Environment.Exit(1);
            }
        }

        private float[] fft = new float[512];

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            if (IsPlaying)
            {
                if (currentPlaying != 0)
                {
                    BASS_ChannelGetData(currentPlaying, fft, (int)BASSData.BASS_DATA_FFT1024);
                    
                }

            }
            else {
                if (!loading && BASS_ChannelIsActive(currentPlaying) == BASSActive.BASS_ACTIVE_STOPPED && CurrentPosition > TotalPosition / 2)
                {
                    updateTimer.Stop();
                    Stopped?.Invoke(this, EventArgs.Empty);
                }
            }

            if (Bass.BASS_GetDeviceInfo(Bass.BASS_GetDevice(), deviceinfo)) {
                if (!deviceinfo.IsDefault)
                {
                    switchDevice();
                }
            }
        }

        private void switchDevice()
        {
            updateTimer.Stop();
            bool paused = IsPlaying;
            long position = CurrentPosition;
            BASS_Free();
            init();
            Load(lastname);
            CurrentPosition = position;
            IsPlaying = paused;
            updateTimer.Start();
        }

        BASS_DEVICEINFO deviceinfo = new BASS_DEVICEINFO();



        int currentPlaying = 0;
        bool loading = false;

        private string lastname = null;

        public void Load(string filename)
        {
            lastname = filename;
            loading = true;
            updateTimer.Stop();
            BASS_StreamFree(currentPlaying);

            TAG_INFO tag = Un4seen.Bass.AddOn.Tags.BassTags.BASS_TAG_GetFromFile(filename);
            if (tag.title == null || tag.title == "") { tag.title = tag.filename; }

            bool isHiResAudio = filename.ToLower().EndsWith(".flac") || filename.ToLower().EndsWith(".ape") || filename.ToLower().EndsWith(".wav");

            SongInfoAvailable?.Invoke(this,
                new SongInfoEventArgs(tag.title, tag.artist, tag.album) {HiResAudio = isHiResAudio});

            if (tag.PictureCount > 0)
            {
                try
                {
                    CoverAvailable?.Invoke(this, new AlbumEventArgs(new Bitmap(tag.PictureGet(0).PictureImage), false));
                }
                catch {
                    CoverAvailable?.Invoke(this, new AlbumEventArgs(new Bitmap(Properties.Resources.default_cover), true));
                }
            }
            else { 
                CoverAvailable?.Invoke(this, new AlbumEventArgs(new Bitmap(Properties.Resources.default_cover), true));
            }
            currentPlaying = BASS_StreamCreateFile(filename, 0, 0, BASSFlag.BASS_DEFAULT | BASSFlag.BASS_STREAM_PRESCAN);
            if (currentPlaying == 0) {
                System.Windows.Forms.MessageBox.Show(BASS_ErrorGetCode().ToString(),"播放失败");
                Stopped?.Invoke(this, EventArgs.Empty);
                return;
            };
            Looping = Looping;
            initFx();
            applyFxStatus();
            BASS_ChannelSetAttribute(currentPlaying, BASSAttribute.BASS_ATTRIB_VOL, _volume);
            BASS_CHANNELINFO info = new BASS_CHANNELINFO();
            BASS_ChannelGetInfo(currentPlaying, info);
            samplerate = info.freq;
            channels = info.chans;
            canProcSurround = bitdepth > 0 && samplerate > 0 && channels==2;
            
            if (surroundProc == null) {
                surroundProc = new DSPPROC(surroundSoundDspProc);
            }
            if (canProcSurround) {
                delaybuffer = new byte[samplerate / 200 * bitdepth];
                Console.WriteLine("Using buffer size " + delaybuffer.Length);
                BASS_ChannelSetDSP(currentPlaying, surroundProc, IntPtr.Zero, 50);
            }
            loading = false;
            updateTimer.Start();
        }


        public bool SurroundSound = false;
        private int samplerate;
        private int channels = 2;
        private byte[] delaybuffer;
        private int bitdepth = 4;
        private int delayBufferPtr = 0;
        private DSPPROC surroundProc;
        private bool canProcSurround = true;

        private void surroundSoundDspProc(int handle, int channel, IntPtr buffer, int bufferlen, IntPtr user) {
            if (bufferlen == 0 || buffer == IntPtr.Zero || (!(SurroundSound && canProcSurround))) {
                return;
            }
            unsafe
            {
                byte* data = (byte*)buffer;
                
                    for (int i = 0; i < bufferlen; i++)
                    {
                        if (((i / 4) & 1) == 1)
                        {
                            byte d = data[i];
                            data[i] = delaybuffer[delayBufferPtr];
                            delaybuffer[delayBufferPtr] = d;
                            delayBufferPtr++;
                            if (delayBufferPtr >= delaybuffer.Length)
                            {
                                delayBufferPtr = 0;
                            }
                        }
                    }
                
            }

        }



        int fxgainparam = 0;
        private int fxvolume = 0;
        private List<int> appliedFx = new List<int>();
        private List<FxObject> fxobjects = new List<FxObject>();
        private float baseGAIN = 1;
        
        void initFx() {
             fxgainparam = Bass.BASS_ChannelSetFX(currentPlaying, BASSFXType.BASS_FX_BFX_DAMP, 1);
             fxvolume =  Bass.BASS_ChannelSetFX(currentPlaying, BASSFXType.BASS_FX_BFX_DAMP, 1);
             BASS_BFX_DAMP param3 = new BASS_BFX_DAMP();
             param3.fGain = baseGAIN;
             Bass.BASS_FXSetParameters(fxgainparam, param3);
             param3.fGain = this._volume;
             Bass.BASS_FXSetParameters(fxvolume, param3);
        }

        void updateVolume()
        {
            BASS_BFX_DAMP param3 = new BASS_BFX_DAMP();
            param3.fGain = this._volume;
            Bass.BASS_FXSetParameters(fxvolume, param3);
        }

        private void AppendFx(float freq, float octivate, float gain)
        {
            BASS_BFX_PEAKEQ param = new BASS_BFX_PEAKEQ();
            param.fBandwidth = octivate;
            param.fCenter = freq;
            param.fGain = gain;
            int fxhandle = BASS_ChannelSetFX(currentPlaying, BASSFXType.BASS_FX_BFX_PEAKEQ, 0);
            BASS_FXSetParameters(fxhandle, param);
            appliedFx.Add(fxhandle);
        }
        
        private void ClearFx()
        {
            appliedFx.ForEach(fx=>BASS_ChannelRemoveFX(currentPlaying,fx));
        }

        private bool _looping = false;
        public bool Looping {
            get => _looping;
            set  {
                this._looping = value;
                BASSFlag loopFlag = this._looping ? BASSFlag.BASS_SAMPLE_LOOP : ~BASSFlag.BASS_SAMPLE_LOOP;
                BASSFlag result =  Bass.BASS_ChannelFlags(currentPlaying, loopFlag, BASSFlag.BASS_SAMPLE_LOOP);
            }
        }

        public void LoadFx(string fxfile)
        {
            if (fxfile == null || fxfile=="")
            {
                fxobjects.Clear();
                baseGAIN = 1;
                applyFxStatus();
                return;
            }

            String[] lines = File.ReadAllLines(fxfile);
            List<FxObject> fxObjects = new List<FxObject>();
            float gain = 0;
            try
            {
                gain = float.Parse(lines[0]);
                for (int i = 1; i < lines.Length; i++)
                {
                    string line = lines[i];
                    if (line.Trim() != "")
                    {
                        string[] param = line.Split(',');
                        float p1 = float.Parse(param[0].Trim());
                        float p2 = float.Parse(param[1].Trim());
                        float p3 = float.Parse(param[2].Trim());
                        fxObjects.Add(new FxObject(){Frequent = p1,BandwidthOctivates = p2,Gain = p3});
                    }
                }
                
                fxobjects.Clear();
                fxobjects.AddRange(fxObjects);
                baseGAIN = gain;
                applyFxStatus();
            }
            catch
            {
                throw;
            }
        }

        void applyFxStatus()
        {
             BASS_BFX_DAMP param3 = new BASS_BFX_DAMP();
             Bass.BASS_FXGetParameters(fxgainparam,param3);
             param3.fGain = baseGAIN;
             Bass.BASS_FXSetParameters(fxgainparam, param3);
             ClearFx();
             fxobjects.ForEach(fo => AppendFx(fo.Frequent,fo.BandwidthOctivates,fo.Gain));
        }

        public void Pause()
        {
            BASS_ChannelPause(currentPlaying);
        }

        public void Play()
        {
            updateTimer.Enabled = true;
            BASS_ChannelPlay(currentPlaying,false);
        }

        public void PlayOrPause()
        {
            IsPlaying = !IsPlaying;
        }

        public void Replay()
        {
            BASS_ChannelPlay(currentPlaying, true);
        }

        public void Stop()
        {
            BASS_ChannelPause(currentPlaying);
            BASS_ChannelSetPosition(currentPlaying, 0);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Stop();
                    updateTimer.Stop();
                    updateTimer.Dispose();
                }
                BASS_SampleFree(currentPlaying);
                BASS_Free();
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    public class FxObject
    {
        public float Frequent { get; set; }
        public float BandwidthOctivates { get; set; }
        public float Gain { get; set; }
    }
    
}
