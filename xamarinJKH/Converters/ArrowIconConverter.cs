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
                return "ic_arrow_down";
            }
            else
            {
                return "ic_arrow_up";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
