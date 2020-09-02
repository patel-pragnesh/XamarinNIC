using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace xamarinJKH.VideoStreaming
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CameraPage : ContentPage
    {
        string _link;
        public string Link
        {
            get => _link;
            set
            {
                _link = value;
                OnPropertyChanged("Link");
            }
        }
        bool isLoading;
        public bool IsLoading
        {
            get => isLoading;
            set
            {
                isLoading = value;
                OnPropertyChanged(nameof(IsLoading));
            }
        }
        double _pHeight;
        public double PlayerHeight
        {
            get => _pHeight;
            set
            {
                _pHeight = value;
                OnPropertyChanged(nameof(PlayerHeight));
            }
        }
        public CameraPage(string link)
        {
            InitializeComponent();
            BindingContext = this;
            IsLoading = true;
            Link = link;
            PlayerHeight = 100;
            MessagingCenter.Subscribe<object>(this, "StopLoadingPlayer", sender =>
            {
                IsLoading = false;
                Video.Opacity = 1;
            });

            MessagingCenter.Subscribe<object, float>(this, "SetRatio", (sender, args) =>
            {
                var width = Video.Width;
                PlayerHeight = Convert.ToDouble(width * args);
            });
        }
    }
}