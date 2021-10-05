using FantasticMusicPlayer.dbo.Model;
using FantasticMusicPlayer.lib;
using System;
using System.Collections.Generic;
using System.Deployment.Application;
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
                shuffeBackLog.Clear();
                shuffeForwardLog.Clear();
            }
        }
        public List<PlayList> AllPlayList { get => PlayListProvider.PlayLists; }
        public List<SongEntry> SongInList { get; }
        Random rnd = new Random();
        void prev() {
            if (Shuffe && CurrentList.Songs.Count >0) {
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

        SongEntry pollRandomNext()
        {
            if (CurrentList.Songs.Count>3)
            {
                SongEntry se = null;
                bool valid = false;
                while (!valid)
                {
                    se = CurrentList.Songs[rnd.Next(0, CurrentList.Songs.Count)];
                    int count = CurrentList.Songs.Count / 2;
                    int begin = shuffeBackLog.InnerList.Count - count;
                    if (begin < 0)
                    {
                        begin = 0;
                    }
                    valid = true;
                    for (int i = begin; i < shuffeBackLog.InnerList.Count; i++)
                    {
                        if (shuffeBackLog.InnerList[i].Equals(se))
                        {
                            valid = false;
                            break;
                        }
                    }
                }
                return se;
            }
            else
            {
                int idx = CurrentList.Songs.IndexOf(CurrentPlaying);
                idx++;
                if (idx >= CurrentList.Songs.Count)
                {
                    idx = 0;
                }
                return CurrentList.Songs[idx];
            }
        }

        void next() {
            if (Shuffe && CurrentList.Songs.Count >0)
            {
                shuffeBackLog.push(CurrentPlaying);
                SongEntry next = null;
                if (shuffeForwardLog.pop(out next))
                {
                    CurrentPlaying = next;
                }
                else
                {
                    CurrentPlaying = pollRandomNext();
                }
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

        SizedStack<SongEntry> shuffeBackLog = new SizedStack<SongEntry>(80);
        SizedStack<SongEntry> shuffeForwardLog = new SizedStack<SongEntry>(20);

        /// <summary>
        /// 0 - 全部播放 1 - 单曲循环 2 - 列表循环
        /// </summary>
        private int _loopMode = 0;
        public int LoopMode { get => _loopMode; set {
                _loopMode = value;
            }
        }

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
