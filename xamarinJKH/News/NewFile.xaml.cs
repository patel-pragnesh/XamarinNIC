using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AppCenter.Analytics;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace xamarinJKH.News
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewFile : AiForms.Dialogs.Abstractions.DialogView
    {
        string image;
        public string Image
        {
            get => image;
            set
            {
                image = value;
                OnPropertyChanged("Image");
            }
        }
        public NewFile(string img)
        {
            InitializeComponent();
            Analytics.TrackEvent("Файл новостей");

            Image = img;
            BindingContext = this;
        }

        void Close(object sender, EventArgs args)
        {
            DialogNotifier.Cancel();
        }
    }
}