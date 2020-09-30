using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace xamarinJKH.VideoStreaming
{
    public class VideoPlayerExo : View
    {
        public static BindableProperty SourceProperty = BindableProperty.Create("SourceProperty", typeof(string), typeof(VideoPlayerExo));
        public string Source
        {
            get => (string)GetValue(SourceProperty);
            set
            {
                SetValue(SourceProperty, value);
                OnPropertyChanged("Source");
            }
        }
        public VideoPlayerExo()
        {

        }
    }
}