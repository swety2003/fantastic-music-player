using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows;

namespace FantasticMusicPlayer.Shared.Convertors
{

    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class InverseBoolToVisibilityConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool))
            {
                throw new ArgumentException("Invalid argument type. Expected argument: bool.", "value");
            }

            if (targetType != typeof(Visibility))
            {
                throw new ArgumentException("Invalid return type. Expected type: Visibility", "targetType");
            }

            if (!(bool)value)
            {
                return Visibility.Visible;
            }

            if (!(parameter is Visibility))
            {
                return Visibility.Collapsed;
            }

            return parameter;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Visibility))
            {
                throw new ArgumentException("Invalid argument type. Expected argument: Visibility.", "value");
            }

            if (targetType != typeof(bool))
            {
                throw new ArgumentException("Invalid return type. Expected type: bool", "targetType");
            }

            return (Visibility)value != Visibility.Visible;
        }

    }
}
