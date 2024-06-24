using FantasticMusicPlayer.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace FantasticMusicPlayer.Convertors
{
    public class SongPlayingToBoolConvertor : IValueConverter
    {
        static PlayerController controller = App.CurrentApp.GetService<PlayerController>();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SongEntry)
            {
                return controller.CurrentPlaying == value;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
