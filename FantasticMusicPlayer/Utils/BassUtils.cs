using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FantasticMusicPlayer.Utils
{
    static class BassUtils
    {
        public static void CheckLibrary()
        {
            String rootpath = Path.Combine(App.APP_LOCATION, "lib");
            String pluginpath = Path.Combine(rootpath, "plugins");
            //_fftModulePath = Path.Combine(rootpath, "fftconvolver02.dll");
            if (Un4seen.Bass.Bass.LoadMe(Path.Combine(rootpath)))
            {
                Un4seen.Bass.Bass.BASS_PluginLoadDirectory(pluginpath);
            }
            else
            {
                try
                {
                    var err = Un4seen.Bass.Bass.BASS_ErrorGetCode().ToString();

                    MessageBox.Show(err,"错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message,"错误",MessageBoxButton.OK,MessageBoxImage.Error);
                    Environment.Exit(-1);
                }
            }
            if (Un4seen.Bass.AddOn.Fx.BassFx.LoadMe(Path.Combine(pluginpath)))
            {

            }
            else
            {
                MessageBox.Show(Un4seen.Bass.Bass.BASS_ErrorGetCode().ToString(), "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }


        }
    }
}
