using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamarinJKH.Utils;
using xamarinJKH.InterfacesIntegration;
using Plugin.Messaging;
using Rg.Plugins.Popup.Services;
using xamarinJKH.DialogViews;

namespace xamarinJKH
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HeaderViewMain : ContentView
    {
        public static BindableProperty DarkProperty =
            BindableProperty.Create("DarkImage", typeof(string), typeof(string));

        public static BindableProperty LightProperty =
            BindableProperty.Create("LightImage", typeof(string), typeof(string));

        public static BindableProperty GoBackProperty =
            BindableProperty.Create("BackClick", typeof(Command), typeof(Command));

        public static BindableProperty PictureSource =
            BindableProperty.Create("Picture", typeof(string), typeof(string));

        public static BindableProperty TitleSource = BindableProperty.Create("Title", typeof(string), typeof(string));

        public string Phone
        {
            get => Settings.Person.companyPhone.Replace("+", "");
        }

        public string Name
        {
            get => Settings.MobileSettings.main_name;
        }

        public string Picture
        {
            get => (string) GetValue(PictureSource);
            set
            {
                SetValue(PictureSource, value);
                OnPropertyChanged("Picture");
            }
        }


        public string Title
        {
            get => (string) GetValue(TitleSource);
            set
            {
                SetValue(TitleSource, value);
                OnPropertyChanged("Title");
            }
        }

        public string DarkImage
        {
            get => (string) GetValue(DarkProperty);
            set
            {
                SetValue(DarkProperty, value);
                ImageFon.SetOnAppTheme(Image.SourceProperty, LightImage, value);
            }
        }

        public string LightImage
        {
            get => (string) GetValue(LightProperty);
            set
            {
                SetValue(LightProperty, value);
                ImageFon.SetOnAppTheme(Image.SourceProperty, value, DarkImage);
            }
        }

        public HeaderViewMain()
        {
            InitializeComponent();
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    int statusBarHeight = DependencyService.Get<IStatusBar>().GetHeight();
                    Pancake.Padding = new Thickness(0, statusBarHeight, 0, 0);
                    //BackgroundColor = Color.White;                    
                    break;
                default:
                    break;
            }
            var techClick = new TapGestureRecognizer();
            techClick.Tapped += async (s, e) => Tech();
            LayoutTech.GestureRecognizers.Add(techClick); 
            var backClick = new TapGestureRecognizer();
            backClick.Tapped += async (s, e) => Back();
            BackStackLayout.GestureRecognizers.Add(backClick);
            BindingContext = this;
        }

        public Command BackClick
        {
            get => (Command) GetValue(GoBackProperty);
            set => SetValue(GoBackProperty, value);
        }

        private void Call(object sender, EventArgs args)
        {
            if (Settings.Person.Phone != null)
            {
                IPhoneCallTask phoneDialer;
                phoneDialer = CrossMessaging.Current.PhoneDialer;
                if (phoneDialer.CanMakePhoneCall && !string.IsNullOrWhiteSpace(Settings.Person.companyPhone))
                    phoneDialer.MakePhoneCall(
                        System.Text.RegularExpressions.Regex.Replace(Settings.Person.companyPhone, "[^+0-9]", ""));
            }
        }

        private async void Tech()
        {
            await PopupNavigation.Instance.PushAsync(new TechDialog());
        }
        
        void Back()
        {
            // MessagingCenter.Send<HeaderViewStack>(this, "GoBack");
            if (BackClick != null)
                BackClick.Execute(null);
        }
    }
}