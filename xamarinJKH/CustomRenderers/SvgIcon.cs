using System;
using System.Collections.Generic;
using System.Text;

using Xamarin.Forms;
using FFImageLoading.Svg.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace xamarinJKH.CustomRenderers
{
    public class SvgIcon:SvgCachedImage
    {
        public static BindableProperty ForegroundProperty = BindableProperty.Create("Foreground", typeof(Color), typeof(Color));
        public Color Foreground
        {
            get => (Color)GetValue(ForegroundProperty);
            set => SetValue(ForegroundProperty, value);
        }

        void SetColor(string color)
        {
            var svg = this.Source;
        }
    }
}
