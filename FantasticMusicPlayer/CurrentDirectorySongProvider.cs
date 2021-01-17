using FantasticMusicPlayer.dbo.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FantasticMusicPlayer
{
    class CurrentDirectorySongProvider : IPlayListProvider
    {
        public CurrentDirectorySongProvider() {
            EnumrateDir(Path.GetFullPath("."));
            PlayLists.RemoveAll(a => a.Songs.Count < 1);
        }

        List<String> availableExtenstions = new List<string>(new string[] {".mp3",".wav",".flac",".ape",".aac" });

        public void EnumrateDir(String root) {
            PlayList pl = new PlayList(Path.GetFileName(root));
            PlayLists.Add(pl);
            Directory.EnumerateFiles(root).Where(o => availableExtenstions.Any(a => o.ToLower().EndsWith(a))).OrderBy(o => o).ToList().ForEach(f => pl.Songs.Add(new SongEntry(f)));
            Directory.EnumerateDirectories(root).OrderBy(o => o).ToList().ForEach(o => EnumrateDir(o));
        }

        private List<PlayList> _playlists = new List<PlayList>();
        public List<PlayList> PlayLists => _playlists;

        public PlayList LastPlayList { get; set ; }
        public SongEntry LastSong { get ; set ; }

        public void loadProgress()
        {
            int folderpos = 0;
            int songpos = 0;
            if (File.Exists("point.conf")) {
                try
                {
                    String[] content = File.ReadAllLines("point.conf");
                    folderpos = int.Parse(content[0]);
                    songpos = int.Parse(content[1]);
                }
                catch { }
            }

            LastPlayList = PlayLists[Math.Min(folderpos,PlayLists.Count-1)];
            LastSong = LastPlayList.Songs[Math.Min(songpos, LastPlayList.Songs.Count - 1)];
        }

        public void saveProgress()
        {
            File.WriteAllLines("point.conf", new string[] {PlayLists.IndexOf(LastPlayList)+"",LastPlayList.Songs.IndexOf(LastSong)+"" });
        }
    }
}
