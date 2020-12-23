using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

using Xamarin.Forms;

namespace xamarinJKH.Converters
{
    public class AccountSelectedBorderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                return (Color)App.Current.Resources["MainColor"];
            }
            else
            {
                var theme = App.Current.RequestedTheme;
                return theme == OSAppTheme.Dark ? Color.White : Color.FromHex("#8D8D8D");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
