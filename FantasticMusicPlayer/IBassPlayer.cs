using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FantasticMusicPlayer
{
    public interface IBassPlayer : IDisposable
    {
        void init();
        float[] Spectrum { get; }

        long CurrentPosition { get; set; }

        long TotalPosition { get; }

        event EventHandler<AlbumEventArgs> CoverAvailable;
        event EventHandler<SongInfoEventArgs> SongInfoAvailable;

        

        void Load(String filename);

        void PlayOrPause();
        void Play();
        void Pause();
        void Stop();
        event EventHandler<EventArgs> Stopped;
        void Replay();
        float Volume { get; set; }
        bool IsPlaying { get; set; }

        bool BassBoost { get; set; }
    }

    public class AlbumEventArgs : EventArgs {
        public Bitmap cover { get; set; }
        public bool Fallback { get; set; }

        public AlbumEventArgs(Bitmap cover, bool fallback)
        {
            this.cover = cover;
            Fallback = fallback;
        }
    }

    public class SongInfoEventArgs : EventArgs { 
        public String title { get; set; }
        public String artist { get; set; }
        public String album { get; set; }

        public bool HiResAudio { get; set; }

        public SongInfoEventArgs(string title, string artist, string album)
        {
            this.title = title;
            this.artist = artist;
            this.album = album;
        }
    }
}
