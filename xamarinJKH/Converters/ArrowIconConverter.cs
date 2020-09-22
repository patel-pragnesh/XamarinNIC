using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

using Xamarin.Forms;

namespace xamarinJKH.Converters
{
    public class ArrowIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool asending = (bool)value;
            if (asending)
            {
                return "resource://xamarinJKH.Resources.ic_arrow_down.svg";
            }
            else
            {
                return "resource://xamarinJKH.Resources.ic_arrow_up.svg";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
