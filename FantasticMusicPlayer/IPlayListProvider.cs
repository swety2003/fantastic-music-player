using FantasticMusicPlayer.dbo.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FantasticMusicPlayer
{
    public interface IPlayListProvider
    {
        List<PlayList> PlayLists { get; }

        PlayList LastPlayList { get; set; }
        SongEntry LastSong { get; set; }
        void saveProgress();
        void loadProgress();
        
    }

    public static class PlayListProviderExtendable {
        public static int GetTotalSongCount(this IPlayListProvider provider) {
            int sum = 0;
            foreach (PlayList item in provider.PlayLists)
            {
                sum += item.Songs.Count;
            }
            return sum;
        }
    }

    public class SongSwitchedEventArgs : EventArgs {
        public SongEntry CurrentSong { get; set; }

        public SongSwitchedEventArgs(SongEntry currentSong)
        {
            CurrentSong = currentSong;
        }
    }
}
