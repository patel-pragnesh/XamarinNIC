using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

using Xamarin.Forms;

namespace xamarinJKH.Converters
{
    public class AccountSelectedBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                return Color.White;
            }
            else
            {
                var theme = App.Current.RequestedTheme;
                return theme == OSAppTheme.Dark ? Color.FromHex("#8D8D8D") : Color.White;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
