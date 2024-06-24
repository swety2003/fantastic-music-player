using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace FantasticMusicPlayer.Shared.Convertors
{
    public class MultiplyValueConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var a = double.Parse(parameter as string??"1");
            if (value is int num)
            {
                return num * a;
            }else if (value is float fnum)
            {
                return fnum * a;
            }
            else if (value is double dnum)
            {
                return dnum * a;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
