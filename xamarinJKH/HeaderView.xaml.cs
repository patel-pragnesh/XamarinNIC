using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamarinJKH.Utils;

namespace xamarinJKH
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HeaderView : ContentView
    {
        public static BindableProperty HeaderPicture = BindableProperty.Create("HeaderPicture", typeof(string), typeof(string));
        public static BindableProperty ShowBackButton = BindableProperty.Create("ShowBackButton", typeof(bool), typeof(bool));
        public static BindableProperty TitleProperty = BindableProperty.Create("TitleProperty", typeof(string), typeof(string));

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set
            {
                SetValue(TitleProperty, value);
                OnPropertyChanged("Title");
            }
        }
        public string Picture
        {
            get => (string)GetValue(HeaderPicture);
            set
            {
                SetValue(HeaderPicture, value);
                OnPropertyChanged("Picture");
            }
        }

        public bool ShowBack
        {
            get => (bool)GetValue(ShowBackButton);
            set
            {
                SetValue(ShowBackButton, value);
                OnPropertyChanged("ShowBack");
            }
        }

        public string Phone
        {
            get => Settings.Person.companyPhone.Replace("+", "");
        }

        public string Company
        {
            get => Settings.MobileSettings.main_name;
        }

        public HeaderView()
        {
            InitializeComponent();
            BindingContext = this;
        }

        protected async void GoBack(object sender, EventArgs args)
        {
            var page = this.Parent;
        }
    }
}