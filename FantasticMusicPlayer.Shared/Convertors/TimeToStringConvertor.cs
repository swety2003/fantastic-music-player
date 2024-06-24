using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace FantasticMusicPlayer.Shared.Convertors
{
    public class TimeToStringConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is long ms)
            {

                long minutes = ms / 60000;
                long second = (ms % 60000) / 1000;
                return String.Format("{0:D2}:{1:D2}", minutes, second);
            }
            return "--:--";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
