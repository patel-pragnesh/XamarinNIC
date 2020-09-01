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
        public CameraPage(string link)
        {
            InitializeComponent();
            BindingContext = this;
            Link = link;
        }
    }
}