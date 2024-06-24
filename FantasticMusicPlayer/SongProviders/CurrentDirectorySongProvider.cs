using FantasticMusicPlayer.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FantasticMusicPlayer.SongProviders
{
    class CurrentDirectorySongProvider : IPlayListProvider
    {
        private string rootPath;

        private List<PlayList>? unfilteredPlaylist;

        public CurrentDirectorySongProvider(IConfiguration cfg)
        {
            rootPath = cfg.GetValue<string>("HomeDir")??".";
            EnumrateDir(rootPath);
            unfilteredPlaylist = _playlists;
            _playlists = unfilteredPlaylist.Where(p => p.Songs.Count > 0).ToList();
            if (_playlists.Count == 0)
            {
                System.Windows.Forms.MessageBox.Show("当前文件夹下没有找到音乐文件。");
                Environment.Exit(1);
            }

            this.cfg = cfg;
        }

        List<string> availableExtenstions = new List<string>(new string[] { ".mp3", ".wav", ".flac", ".ape", ".aac" });

        public void EnumrateDir(string root)
        {
            if (root.Contains("/.") || root.Contains("\\."))
            {
                return;
            }
            PlayList pl = new PlayList(Path.GetFileName(root));
            PlayLists.Add(pl);
            Directory.EnumerateFiles(root, "*.pl").OrderBy(o => o).ToList().ForEach(f => SetupVirtualList(f));
            Directory.EnumerateFiles(root).Where(o => availableExtenstions.Any(a => o.ToLower().EndsWith(a))).OrderBy(o => o).ToList().ForEach(f => pl.Songs.Add(new SongEntry(f)));
            Directory.EnumerateDirectories(root).OrderBy(o => o).ToList().ForEach(o => EnumrateDir(o));
        }

        public void UpdatePlaylist()
        {
            _playlists = unfilteredPlaylist.Where(p => p.Songs.Count > 0).ToList();
        }

        public void SetupVirtualList(string virtualList)
        {
            string? name = Path.GetFileNameWithoutExtension(virtualList);
            PlayLists.Add(new VirtualDir(name, virtualList, rootPath));
        }

        private List<PlayList> _playlists = new List<PlayList>();
        private readonly IConfiguration cfg;

        public List<PlayList> PlayLists => _playlists;

        public PlayList LastPlayList { get; set; }
        public SongEntry LastSong { get; set; }

        public void loadProgress()
        {
            int folderpos = 0;
            int songpos = 0;
            if (File.Exists("point.conf"))
            {
                try
                {
                    string[] content = File.ReadAllLines("point.conf");
                    folderpos = int.Parse(content[0]);
                    songpos = int.Parse(content[1]);
                }
                catch { }
            }
            LastPlayList = PlayLists[Math.Min(folderpos, PlayLists.Count - 1)];
            LastSong = LastPlayList.Songs[Math.Min(songpos, LastPlayList.Songs.Count - 1)];
        }

        public void saveProgress()
        {
            File.WriteAllLines("point.conf", new string[] { PlayLists.IndexOf(LastPlayList) + "", LastPlayList.Songs.IndexOf(LastSong) + "" });
        }
    }

    class VirtualDir : PlayList
    {
        private string fileName;
        private DateTime? lastmodified = DateTime.MinValue;
        private string rootdir;
        private List<SongEntry> songs = new List<SongEntry>();
        public VirtualDir(string name, string file, string root) : base(name)
        {
            fileName = file;
            rootdir = root;
            checkUpdate();
        }

        void checkUpdate()
        {
            if (File.GetLastWriteTime(fileName) != lastmodified)
            {
                songs.Clear();
                songs.AddRange(File.ReadAllLines(fileName).Where(f => File.Exists(Path.Combine(rootdir, f))).Select(f => new SongEntry(f.Replace('/', Path.DirectorySeparatorChar))));
                Console.WriteLine("Loaded playlist " + Name + " with " + songs.Count + " songs.");
                lastmodified = File.GetLastWriteTime(fileName);
            }
        }

        public static string getReletivePath(SongEntry se)
        {
            string? path = Path.GetFullPath(se.Path);
            path = path.Replace(Path.GetFullPath("."), "").Replace(Path.DirectorySeparatorChar, '/');
            if (path.StartsWith("/"))
            {
                path = path.Substring(1);
            }
            return path;
        }

        public void save()
        {
            File.WriteAllLines(fileName, songs.Select(s => getReletivePath(s)));
        }

        public override List<SongEntry> Songs { get { checkUpdate(); return songs; } set { } }
    }
}
