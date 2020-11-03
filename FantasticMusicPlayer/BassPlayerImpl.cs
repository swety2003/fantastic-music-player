using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Un4seen.Bass;
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
            set  {BASS_ChannelSetAttribute(currentPlaying, BASSAttribute.BASS_ATTRIB_VOL, value);_volume = value;
        } }
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

        public event EventHandler<EventArgs> Stopped;
        public event EventHandler<AlbumEventArgs> CoverAvailable;
        public event EventHandler<SongInfoEventArgs> SongInfoAvailable;

        BASSTimer updateTimer;

        public void init()
        {
            
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
        }

        int currentPlaying = 0;
        object loadingSync = new object();
        bool loading = false;
        public void Load(string filename)
        {
                loading = true;
            
            updateTimer.Stop();
            BASS_StreamFree(currentPlaying);

            TAG_INFO tag = Un4seen.Bass.AddOn.Tags.BassTags.BASS_TAG_GetFromFile(filename);
            if (tag.title == null || tag.title == "") { tag.title = tag.filename; }

            bool isHiResAudio = filename.ToLower().EndsWith(".flac") || filename.ToLower().EndsWith(".ape") || filename.ToLower().EndsWith(".wav");

            SongInfoAvailable?.Invoke(this,new SongInfoEventArgs(tag.title, tag.artist, tag.album) { HiResAudio = isHiResAudio});

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
            currentPlaying = BASS_StreamCreateFile(filename, 0, 0, BASSFlag.BASS_DEFAULT);
            if (currentPlaying == 0) {
                System.Windows.Forms.MessageBox.Show(BASS_ErrorGetCode().ToString(),"播放失败");
                Stopped?.Invoke(this, EventArgs.Empty);
                return;
            };
            BASS_ChannelSetAttribute(currentPlaying, BASSAttribute.BASS_ATTRIB_VOL, _volume);
            loading = false;
            updateTimer.Start();
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
                    // TODO: 释放托管状态(托管对象)
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
}
