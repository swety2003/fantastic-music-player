using FantasticMusicPlayer.dbo.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FantasticMusicPlayer
{
    public class PlayerController
    {
        public IPlayListProvider PlayListProvider { get; set; }
        public event EventHandler<SongSwitchedEventArgs> SongChanged;
        public SongEntry CurrentPlaying { get => PlayListProvider.LastSong; set => PlayListProvider.LastSong = value; }
        public PlayList CurrentList { get => PlayListProvider.LastPlayList; set => PlayListProvider.LastPlayList = value; }
        public List<PlayList> AllPlayList { get; }
        public List<SongEntry> SongInList { get; }
        Random rnd = new Random();
        void prev() {
            if (Shuffe) {
                CurrentPlaying = CurrentList.Songs[rnd.Next(CurrentList.Songs.Count)];
                changeSong(CurrentPlaying);
                return;
            }
            int songptr = CurrentList.Songs.IndexOf(CurrentPlaying);
            int listptr = AllPlayList.IndexOf(CurrentList);

            songptr--;
            if (songptr < 0)
            {
                if (LoopMode == 0)
                {
                    do
                    {
                        listptr--;
                        if (listptr <0) { listptr = AllPlayList.Count-1; }
                    } while (AllPlayList[listptr].Songs.Count == 0);
                }
                songptr = AllPlayList[listptr].Songs.Count-1;
            }
            CurrentList = AllPlayList[listptr];
            CurrentPlaying = CurrentList.Songs[songptr];
            changeSong(CurrentPlaying);
        }
        void next() {
            if (Shuffe)
            {
                CurrentPlaying = CurrentList.Songs[rnd.Next(CurrentList.Songs.Count)];
                changeSong(CurrentPlaying);
                return;
            }

            int songptr = CurrentList.Songs.IndexOf(CurrentPlaying);
            int listptr = AllPlayList.IndexOf(CurrentList);

            songptr++;
            if (songptr > CurrentList.Songs.Count - 1) {

                songptr = 0;

                if (LoopMode == 0) {
                    do
                    {
                        listptr++;
                        if (listptr > AllPlayList.Count - 1) { listptr = 0; }
                    } while (AllPlayList[listptr].Songs.Count == 0);
                }
            }
            CurrentList = AllPlayList[listptr];
            CurrentPlaying = CurrentList.Songs[songptr];
            changeSong(CurrentPlaying);
        }

        private bool HasSong = false;

        public int LoopMode { get; set; } = 0;
        public bool Shuffe { get; set; } = false;

        public PlayerController(IPlayListProvider playlistProvider) {
            this.PlayListProvider = playlistProvider;
            AllPlayList = playlistProvider.PlayLists;

            if (playlistProvider.GetTotalSongCount() > 0)
            {

                playlistProvider.loadProgress();
                CurrentList = playlistProvider.LastPlayList;
                CurrentPlaying = playlistProvider.LastSong;
                HasSong = true;
                changeSong(CurrentPlaying);
            }
        }

        public void ImReady() {
            changeSong(CurrentPlaying);
        }

        private void changeSong(SongEntry se) => SongChanged?.Invoke(this, new SongSwitchedEventArgs(se));

        public void onPrevButtonClick() {
            if (HasSong)
                prev();
        }

        public void onNextButtonClick() {
            if (HasSong)
                next();
        }

        public void onSongStop() {
            if (HasSong)
            {
                if (LoopMode == 1)
                {
                    changeSong(CurrentPlaying);
                    return;
                }
                next();
            }
        }
        
        


    }
}
