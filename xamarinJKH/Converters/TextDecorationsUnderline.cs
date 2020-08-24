using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

using Xamarin.Forms;

namespace xamarinJKH.Converters
{
    public class TextDecorationsUnderline : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color color = (Color)value;
            if (color != Color.Black || color != Color.White)
            {
                return TextDecorations.Underline;
            }
            else
            {
                return TextDecorations.None;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return TextDecorations.None;
        }
    }
}
