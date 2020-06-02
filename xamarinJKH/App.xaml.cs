using System;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.Xaml;
using Application = Xamarin.Forms.Application;

namespace xamarinJKH
{
    public partial class App : Application
    {
        public static int ScreenHeight {get; set;}
        public static int ScreenWidth {get; set;}
        public App()
        {
            InitializeComponent();

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    var nav = new Xamarin.Forms.NavigationPage(new MainPage())
                    {
                        BarBackgroundColor = Color.Black,
                        BarTextColor = Color.White
                    };

                    nav.On<iOS>().SetIsNavigationBarTranslucent(true);
                    nav.On<iOS>().SetStatusBarTextColorMode(StatusBarTextColorMode.MatchNavigationBarTextLuminosity);

                    MainPage = nav;
                    break;
                case Device.Android:
                    MainPage = new MainPage();
                    break;
                default:
                    break;
            }
            
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
