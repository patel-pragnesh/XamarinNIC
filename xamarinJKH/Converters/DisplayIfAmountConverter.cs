using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;


using Xamarin.Forms;

namespace xamarinJKH.Converters
{
    public class DisplayIfAmountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var amount = (int)value;
            if (amount > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return false;
        }
    }
}
