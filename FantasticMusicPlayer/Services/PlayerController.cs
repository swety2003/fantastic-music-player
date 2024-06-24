using FantasticMusicPlayer.Models;
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
        public PlayList CurrentList { get => PlayListProvider.LastPlayList;
            set
            {
                PlayListProvider.LastPlayList = value;
            }
        }
        public List<PlayList> AllPlayList { get => PlayListProvider.PlayLists; }
        public List<SongEntry> SongInList { get; }
        Random rnd = new Random();
        void prev() {
            if (Shuffe && CurrentList.Songs.Count >0) {
                shuffePtr--;
                shuffedPlayed--;
                if ((-shuffedPlayed) >= shuffedSongs.Count)
                {
                    resetShuffe();
                    shuffeList(shuffedSongs);
                }
                if (shuffePtr < 0)
                {
                    shuffePtr = shuffedSongs.Count-1;
                }

                CurrentPlaying = shuffedSongs[shuffePtr];
                changeSong(CurrentPlaying);
                return;
            }
            int songptr = CurrentList.Songs.IndexOf(CurrentPlaying);
            int listptr = AllPlayList.IndexOf(CurrentList);

            songptr--;
            if (songptr < 0)
            {
                if (LoopMode == 0 || CurrentList.Songs.Count<1)
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
            if (Shuffe && CurrentList.Songs.Count >0)
            {
                shuffePtr++;
                shuffedPlayed++;
                if (shuffedPlayed >= shuffedSongs.Count) {
                    resetShuffe(); 
                    shuffeList(shuffedSongs);
                }
                if (shuffePtr >= shuffedSongs.Count)
                {
                    shuffePtr = 0;
                }
                CurrentPlaying = shuffedSongs[shuffePtr];
                changeSong(CurrentPlaying);
                return;
            }

            int songptr = CurrentList.Songs.IndexOf(CurrentPlaying);
            int listptr = AllPlayList.IndexOf(CurrentList);

            songptr++;
            if (songptr > CurrentList.Songs.Count - 1) {

                songptr = 0;

                if (LoopMode == 0 || CurrentList.Songs.Count<1) {
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

        /// <summary>
        /// 0 - 全部播放 1 - 单曲循环 2 - 列表循环
        /// </summary>
        private int _loopMode = 0;
        public int LoopMode { get => _loopMode; set {
                _loopMode = value;
            }
        }

        private List<SongEntry> shuffedSongs = new List<SongEntry>();
        private int shuffePtr = 0;
        private int shuffedPlayed = 0;

        private void resetShuffe() {
            shuffedSongs.Clear();
            shuffedPlayed = 0;
            shuffePtr = 0;
            if (CurrentList != null)
            {
                shuffedSongs.AddRange(CurrentList.Songs);
                shuffeList(shuffedSongs);
                if (CurrentPlaying != null)
                {
                    int current = shuffedSongs.IndexOf(CurrentPlaying);
                    swapListItem(shuffedSongs, 0, current);
                }
            }
        }

        public List<SongEntry> ShuffedPlaylist { get { return shuffedSongs; } }

        private void shuffeList<T>(List<T> list) {
            Random rnd = new Random();
            for (int i = 0; i < list.Count - 1; i++)
            {
                swapListItem(list, i, i + rnd.Next(list.Count - 1 - i) + 1);
            }
        }

        private void swapListItem<T>(List<T> list,int i1,int i2) {
            if (i1 == i2) { return; }
            T item = list[i1];
            list[i1] = list[i2];
            list[i2] = item;
        }

        private bool _shuffe = false;
        public bool Shuffe { get=>_shuffe; set {
                _shuffe = value;
                if (value) {
                    resetShuffe();
                }
            } 
        }

        public PlayerController(IPlayListProvider playlistProvider) {
            this.PlayListProvider = playlistProvider;

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
            if (_shuffe) {
                resetShuffe();
            }
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
