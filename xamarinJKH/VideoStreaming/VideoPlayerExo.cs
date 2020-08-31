using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace xamarinJKH.VideoStreaming
{
    public class VideoPlayerExo : View
    {
        public static BindableProperty SourceUrl = BindableProperty.Create("Source", typeof(string), typeof(VideoPlayerExo));
        public string Source
        {
            get => (string)GetValue(SourceUrl);
            set
            {
                SetValue(SourceUrl, value);
                OnPropertyChanged("Source");
            }
        }
        public VideoPlayerExo()
        {

        }
    }
}