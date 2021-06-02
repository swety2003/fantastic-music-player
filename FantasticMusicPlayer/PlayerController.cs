using FantasticMusicPlayer.dbo.Model;
using FantasticMusicPlayer.lib;
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
                shuffeForwardLog.push(CurrentPlaying);

                SongEntry prev = null;
                if (shuffeBackLog.pop(out prev))
                {
                    CurrentPlaying = prev;
                }
                else
                {
                    CurrentPlaying = CurrentList.Songs[rnd.Next(CurrentList.Songs.Count)];
                }
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
                shuffeBackLog.push(CurrentPlaying);
                SongEntry next = null;
                if (shuffeForwardLog.pop(out next))
                {
                    CurrentPlaying = next;
                }
                else
                {
                    CurrentPlaying = CurrentList.Songs[rnd.Next(CurrentList.Songs.Count)];
                }
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

        SizedStack<SongEntry> shuffeBackLog = new SizedStack<SongEntry>(30);
        SizedStack<SongEntry> shuffeForwardLog = new SizedStack<SongEntry>(30);

        /// <summary>
        /// 0 - 全部播放 1 - 单曲循环 2 - 列表循环
        /// </summary>
        public int LoopMode { get; set; } = 0;

        private bool _shuffe = false;
        public bool Shuffe { get=>_shuffe; set {
                _shuffe = value;
                if (value) {
                    shuffeBackLog.Clear();
                    shuffeForwardLog.Clear();
                }
            } 
        }

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
            if (_shuffe) { 
                
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
