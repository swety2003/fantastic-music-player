﻿using NAudio.Utils;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Fx;
using Un4seen.Bass.AddOn.Tags;
using static Un4seen.Bass.Bass;

namespace FantasticMusicPlayer
{
    public class BassPlayerImpl : IBassPlayer
    {



        private bool disposedValue;

        public float[] Spectrum => fft;

        public float peakDB = -96f;


#if DEBUG
        public static bool debug = true;
#else
        public static bool debug = false;
#endif

        public long CurrentPosition
        {
            get
            {
                long result = (long)(BASS_ChannelBytes2Seconds(currentPlaying, BASS_ChannelGetPosition(currentPlaying, BASSMode.BASS_POS_BYTE)) * 1000L);
                if (result < 0) { result = 0; }
                return result;
            }
            set => BASS_ChannelSetPosition(currentPlaying, Math.Min((TotalPosition - 10) / 1000d, value / 1000d));
        }

        public long TotalPosition => Math.Max(1, (long)(BASS_ChannelBytes2Seconds(currentPlaying, BASS_ChannelGetLength(currentPlaying, BASSMode.BASS_POS_BYTE)) * 1000L) + 1);

        private float _volume = 1;
        public float Volume
        {
            get
            {
                return _volume;
            }
            set
            {
                _volume = value;
                updateVolume();
            }
        }
        public bool IsPlaying
        {
            get => BASS_ChannelIsActive(currentPlaying) == BASSActive.BASS_ACTIVE_PLAYING; set
            {
                if (value)
                {
                    Play();
                }
                else
                {
                    Pause();
                }
            }
        }

        private bool _bassboosted = false;
        public bool BassBoost
        {
            get => _bassboosted; set
            {
                _bassboosted = value;
                applyFxStatus();
            }
        }

        public event EventHandler<EventArgs>? Stopped;
        public event EventHandler<AlbumEventArgs>? CoverAvailable;
        public event EventHandler<SongInfoEventArgs>? SongInfoAvailable;

        BASSTimer? updateTimer;

        public void init()
        {
            BASS_SetConfig(BASSConfig.BASS_CONFIG_FLOATDSP, true);
            if (BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero))
            {
                if (updateTimer == null)
                {
                    updateTimer = new BASSTimer(16);
                    updateTimer.Tick += UpdateTimer_Tick;
                }
            }
            else
            {
                BASSError berr = BASS_ErrorGetCode();
                if (berr == BASSError.BASS_ERROR_DEVICE)
                {
                    System.Windows.Forms.MessageBox.Show("没有可用的音频输出设备，请检查是否插入耳机/音响");
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show(berr.ToString(), "BASS_INIT_ERROR");
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
                    if (debug)
                    {
                        var minDB = (float)Decibels.DecibelsToLinear(-96);
                        var curDB = 0f;
                        var peakdata = BASS_ChannelGetLevels(currentPlaying);
                        if (peakdata != null && peakdata.Length > 0)
                        {
                            curDB = (float)peakdata.Max();
                        }
                        if (curDB < 0) { curDB = -curDB; }
                        if (curDB < minDB)
                        {
                            curDB = minDB;
                        }
                        curDB = (float)Decibels.LinearToDecibels(curDB);
                        peakDB -= 0.4f;
                        if (peakDB < -96) { peakDB = -96; }
                        if (peakDB < curDB)
                        {
                            peakDB = curDB;
                        }
                    }
                }

            }
            else
            {
                if (!loading && BASS_ChannelIsActive(currentPlaying) == BASSActive.BASS_ACTIVE_STOPPED && CurrentPosition > TotalPosition / 2)
                {
                    updateTimer.Stop();
                    Stopped?.Invoke(this, EventArgs.Empty);
                }
            }

            if (Bass.BASS_GetDeviceInfo(Bass.BASS_GetDevice(), deviceinfo))
            {
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

            TAG_INFO tag = BassTags.BASS_TAG_GetFromFile(filename);

            if (tag.title == null || tag.title == "") { tag.title = tag.filename; }

            bool isHiResAudio = filename.ToLower().EndsWith(".flac") || filename.ToLower().EndsWith(".ape") || filename.ToLower().EndsWith(".wav");

            LyricManager? lyricManager = null;

            var expectedLyricFile = Path.Combine(Path.GetDirectoryName(filename), Path.GetFileNameWithoutExtension(filename) + ".lrc");

            if (File.Exists(expectedLyricFile))
            {
                lyricManager = new LyricManager(File.ReadAllText(expectedLyricFile));
                if (lyricManager.lyricEntries.Count < 2)
                {
                    lyricManager = null;
                }
            }

            SongInfoAvailable?.Invoke(this,
                new SongInfoEventArgs(tag.title, tag.artist, tag.album, lyricManager) { HiResAudio = isHiResAudio });

            if (tag.PictureCount > 0)
            {
                try
                {
                    TagPicture tagp = null;
                    for (int i = 0; i < 32; i++)
                    {
                        tagp = tag.PictureGet(i);
                        if (tagp != null) { break; }
                    }
                    Debug.Assert(tagp != null);
                    CoverAvailable?.Invoke(this, new AlbumEventArgs(new Bitmap(tagp.PictureImage), false));
                }
                catch
                {
                    CoverAvailable?.Invoke(this, new AlbumEventArgs(null, true));
                }
            }
            else
            {
                CoverAvailable?.Invoke(this, new AlbumEventArgs(null, true));
            }
            currentPlaying = BASS_StreamCreateFile(filename, 0, 0, BASSFlag.BASS_DEFAULT);
            if (currentPlaying == 0)
            {
                System.Windows.Forms.MessageBox.Show(BASS_ErrorGetCode().ToString(), "播放失败");
                Stopped?.Invoke(this, EventArgs.Empty);
                return;
            };
            //防止编译器优化
            bool b = Looping;
            Looping = b.ToString().ToLower().StartsWith("t");
            initFx();
            applyFxStatus();
            //BASS_ChannelSetAttribute(currentPlaying, BASSAttribute.BASS_ATTRIB_VOL, _volume);
            BASS_CHANNELINFO info = new BASS_CHANNELINFO();
            BASS_ChannelGetInfo(currentPlaying, info);
            int samplerate = info.freq;
            int channels = info.chans;

            SurroundSound.init(samplerate, channels);

            BASS_ChannelSetDSP(currentPlaying, SurroundSound.DspDelegate, IntPtr.Zero, 30);
            loading = false;
            updateTimer.Start();
        }

        public DSPClass SurroundSound = new DspSwitcher();

        int fxcompressor = 0;
        int fxgainparam = 0;
        int fxcompressorF1 = 0;
        int fxcompressorF2 = 0;
        private int fxvolume = 0;
        private List<int> appliedFx = new List<int>();
        private List<FxObject> fxobjects = new List<FxObject>();
        private float baseGAIN = 1;

        void initFx()
        {
            fxgainparam = Bass.BASS_ChannelSetFX(currentPlaying, BASSFXType.BASS_FX_BFX_DAMP, 1);
            fxvolume = Bass.BASS_ChannelSetFX(currentPlaying, BASSFXType.BASS_FX_BFX_DAMP, 200);
            BASS_BFX_DAMP param3 = new BASS_BFX_DAMP();
            param3.fGain = baseGAIN;
            Bass.BASS_FXSetParameters(fxgainparam, param3);
            param3.fGain = this._volume;
            Bass.BASS_FXSetParameters(fxvolume, param3);
            fxcompressor = Bass.BASS_ChannelSetFX(currentPlaying, BASSFXType.BASS_FX_BFX_DAMP, 10);

            setCompressorState(_drcompress);


        }

        private void setCompressorState(bool state)
        {
            BASS_BFX_DAMP compressor = new BASS_BFX_DAMP();
            BASS_ChannelRemoveFX(currentPlaying, fxcompressorF1);
            BASS_ChannelRemoveFX(currentPlaying, fxcompressorF2);
            if (state)
            {
                float bassBypass = -8;
                compressor.fGain = (float)Decibels.DecibelsToLinear(bassBypass - 0.5);
                compressor.fTarget = (float)Decibels.DecibelsToLinear(bassBypass - 0.5);
                compressor.fDelay = 0.30f;
                compressor.fRate = 0.015f;
                compressor.fQuiet = (float)Decibels.DecibelsToLinear(-42);

                BASS_BFX_BQF bfxparam = new BASS_BFX_BQF();
                bfxparam.fBandwidth = 0;
                bfxparam.fQ = 0f;
                bfxparam.fS = 0.9f;
                bfxparam.fCenter = 168;
                bfxparam.fGain = bassBypass;
                bfxparam.lFilter = BASSBFXBQF.BASS_BFX_BQF_LOWSHELF;
                fxcompressorF1 = BASS_ChannelSetFX(currentPlaying, BASSFXType.BASS_FX_BFX_BQF, 11);
                BASS_FXSetParameters(fxcompressorF1, bfxparam);
                bfxparam.fGain = -bfxparam.fGain;
                fxcompressorF2 = BASS_ChannelSetFX(currentPlaying, BASSFXType.BASS_FX_BFX_BQF, 9);
                BASS_FXSetParameters(fxcompressorF2, bfxparam);
            }
            else
            {
                compressor.fGain = 1;
            }

            Bass.BASS_FXSetParameters(fxcompressor, compressor);
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
            param.fQ = 1 / octivate;
            param.fBandwidth = 0;
            param.fCenter = freq;
            param.fGain = gain;
            int fxhandle = BASS_ChannelSetFX(currentPlaying, BASSFXType.BASS_FX_BFX_PEAKEQ, 5);
            BASS_FXSetParameters(fxhandle, param);
            appliedFx.Add(fxhandle);
        }

        private void ClearFx()
        {
            appliedFx.ForEach(fx => BASS_ChannelRemoveFX(currentPlaying, fx));
        }

        private bool _looping = false;
        public bool Looping
        {
            get => _looping;
            set
            {
                this._looping = value;
                BASSFlag loopFlag = this._looping ? BASSFlag.BASS_SAMPLE_LOOP : ~BASSFlag.BASS_SAMPLE_LOOP;
                BASSFlag result = Bass.BASS_ChannelFlags(currentPlaying, loopFlag, BASSFlag.BASS_SAMPLE_LOOP);
            }
        }

        private bool _drcompress = false;
        public bool DynamicRangeCompressed
        {
            get { return _drcompress; }
            set
            {
                this._drcompress = value;
                setCompressorState(value);
            }
        }

        public void LoadFx(string fxfile)
        {
            if (fxfile != null)
            {
                if (fxfile.ToLower().EndsWith(".eq"))
                {
                    ApplyLegacyFx(fxfile);
                    return;
                }
                if (fxfile.ToLower().EndsWith(".wav"))
                {
                    ApplyLegacyFx(null);
                    ApplyConvolverFx(fxfile);
                    return;
                }
            }


            ApplyLegacyFx(null);
        }


        void ApplyConvolverFx(string fxfile)
        {
            using (var convReader = new NAudio.Wave.WaveFileReader(fxfile))
            {
                WaveFormat format = convReader.WaveFormat;
                if (format.Channels != 4)
                {
                    throw new InvalidDataException("无效的脉冲响应文件");
                }
                if (convReader.TotalTime > TimeSpan.FromMilliseconds(10000))
                {
                    throw new InvalidDataException("文件过大");
                }
            }
            DspSwitcher switcher = this.SurroundSound as DspSwitcher;
            if (switcher != null)
            {
                switcher.WrappedDSP = new SpeakerInRoomDSP(fxfile);
                switcher.Enabled = true;
            }
        }

        void ApplyLegacyFx(string fxfile)
        {
            DspSwitcher dspSwitcher = this.SurroundSound as DspSwitcher;
            if (dspSwitcher.WrappedDSP != null && dspSwitcher.WrappedDSP is SpeakerInRoomDSP)
            {
                SpeakerInRoomDSP dsp = dspSwitcher.WrappedDSP as SpeakerInRoomDSP;
                if (dsp.firSource != null)
                {
                    dspSwitcher.WrappedDSP = null;
                }
            }

            if (fxfile == null || fxfile == "")
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
                        fxObjects.Add(new FxObject() { Frequent = p1, BandwidthOctivates = p2, Gain = p3 });
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
            Bass.BASS_FXGetParameters(fxgainparam, param3);
            param3.fGain = baseGAIN;
            Bass.BASS_FXSetParameters(fxgainparam, param3);
            ClearFx();
            fxobjects.ForEach(fo => AppendFx(fo.Frequent, fo.BandwidthOctivates, fo.Gain));

        }

        public void Pause()
        {
            BASS_ChannelPause(currentPlaying);
        }

        public void Play()
        {
            updateTimer.Enabled = true;
            BASS_ChannelPlay(currentPlaying, false);
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
