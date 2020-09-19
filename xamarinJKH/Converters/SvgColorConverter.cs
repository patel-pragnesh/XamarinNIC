using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace xamarinJKH.Converters
{
    public class SvgColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color mainColor = (Color)Application.Current.Resources["MainColor"];
            var colors = new string[2];
            if (parameter != null)
            {
                var colors_ = parameter.ToString().Split('|');
                for (int i = 0; i < colors_.Length; i++)
                {
                    colors[i] = colors_[i];
                }
            }
            else
                colors[0] = mainColor.ToHex();
            if (colors[1] != null)
            {
                mainColor = Color.FromHex(colors[1]);
            }
            if (mainColor != null)
            {
                var colorReplace = Application.Current.RequestedTheme == OSAppTheme.Light || Application.Current.RequestedTheme == OSAppTheme.Unspecified ? ColorToString(mainColor) : colors[0];
                return new Dictionary<string, string>
                {
                    { "#000000", colorReplace }
                };
            }
            return new Dictionary<string, string>
            {
                {"#000000", "#FF0000" }
            };
        }

        string ColorToString(Color color)
        {
            var red = (int)(color.R * 255);
            var green = (int)(color.G * 255);
            var blue = (int)(color.B * 255);
            var hex = $"#{red:X2}{green:X2}{blue:X2}";

            return hex;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
