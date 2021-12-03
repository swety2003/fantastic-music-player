using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FantasticMusicPlayer
{
    static class Program
    {

        public const string appid = "com.zyfdroid.fantasymusicplayer_v100000000";

        public static void checkLibrary() {
            String rootpath = Path.Combine(Path.GetTempPath(), appid, "libs");
            String pluginpath = Path.Combine(rootpath, "plugins");
            makeFileExists(Properties.Resources.bass, Path.Combine(rootpath, "bass.dll"));
            makeFileExists(Properties.Resources.bassflac, Path.Combine(pluginpath, "bass_flac.dll"));
            makeFileExists(Properties.Resources.bass_aac, Path.Combine(pluginpath, "bass_aac.dll"));
            makeFileExists(Properties.Resources.bass_ape, Path.Combine(pluginpath, "bass_ape.dll"));
            makeFileExists(Properties.Resources.bass_fx, Path.Combine(pluginpath, "bass_fx.dll"));

            if (Un4seen.Bass.Bass.LoadMe(Path.Combine(rootpath)))
            {

                Un4seen.Bass.Bass.BASS_PluginLoadDirectory(pluginpath);
            }
            else {
                Console.WriteLine(Un4seen.Bass.Bass.BASS_ErrorGetCode());
            }
            if (Un4seen.Bass.AddOn.Fx.BassFx.LoadMe(Path.Combine(pluginpath)))
            {   
            }
            else
            {
                Console.WriteLine(Un4seen.Bass.Bass.BASS_ErrorGetCode());
            }


        }

        public static void makeFileExists(byte[] data,String filename) {
            if (!Directory.Exists(Path.GetDirectoryName(filename))) {
                Directory.CreateDirectory(Path.GetDirectoryName(filename));
            }
            if(!File.Exists(filename))
            {
                File.WriteAllBytes(filename + ".tmp",data);
                File.Move(filename + ".tmp", filename);
            }
        }


        [STAThread]
        static void Main()
        {
            checkLibrary();
            #if DEBUG
            Environment.CurrentDirectory = "Q:\\MP3Player";
#endif
            if (!File.Exists("收藏.pl")) { File.Create("收藏.pl").Dispose(); }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
