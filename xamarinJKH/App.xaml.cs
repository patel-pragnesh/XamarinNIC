using System;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Plugin.FirebasePushNotification;
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
            //
            // // Handle when your app starts
            // CrossFirebasePushNotification.Current.Subscribe("general");
            // CrossFirebasePushNotification.Current.OnTokenRefresh += (s, p) =>
            // {
            //     System.Diagnostics.Debug.WriteLine($"TOKEN REC: {p.Token}");
            // };
            // System.Diagnostics.Debug.WriteLine($"TOKEN: {CrossFirebasePushNotification.Current.Token}");
            //
            // CrossFirebasePushNotification.Current.OnNotificationReceived += (s, p) =>
            // {
            //     try
            //     {
            //         System.Diagnostics.Debug.WriteLine("Received");
            //         if (p.Data.ContainsKey("body"))
            //         {
            //             Device.BeginInvokeOnMainThread(() =>
            //             {
            //                 System.Diagnostics.Debug.WriteLine($"{p.Data["body"]}");
            //             });
            //
            //         }
            //     }
            //     catch (Exception ex)
            //     {
            //
            //     }
            //
            // };
            //
            // CrossFirebasePushNotification.Current.OnNotificationOpened += (s, p) =>
            // {
            //     //System.Diagnostics.Debug.WriteLine(p.Identifier);
            //
            //     System.Diagnostics.Debug.WriteLine("Opened");
            //     foreach (var data in p.Data)
            //     {
            //         System.Diagnostics.Debug.WriteLine($"{data.Key} : {data.Value}");
            //     }
            //
            //     if (!string.IsNullOrEmpty(p.Identifier))
            //     {
            //         Device.BeginInvokeOnMainThread(() =>
            //         {
            //             // MainPage.Message = p.Identifier;
            //             System.Diagnostics.Debug.WriteLine("123");
            //         });
            //     }
            //     else if (p.Data.ContainsKey("color"))
            //     {
            //         Device.BeginInvokeOnMainThread(() =>
            //         {
            //             MainPage.Navigation.PushAsync(new ContentPage()
            //             {
            //                 BackgroundColor = Color.FromHex($"{p.Data["color"]}")
            //
            //             });
            //         });
            //
            //     }
            //     else if (p.Data.ContainsKey("aps.alert.title"))
            //     {
            //         Device.BeginInvokeOnMainThread(() =>
            //         {
            //             // MainPage.Message = $"{p.Data["aps.alert.title"]}";
            //             System.Diagnostics.Debug.WriteLine($"{p.Data["aps.alert.title"]}");
            //         });
            //
            //     }
            // };
            //
            // CrossFirebasePushNotification.Current.OnNotificationAction += (s, p) =>
            // {
            //     System.Diagnostics.Debug.WriteLine("Action");
            //
            //     if (!string.IsNullOrEmpty(p.Identifier))
            //     {
            //         System.Diagnostics.Debug.WriteLine($"ActionId: {p.Identifier}");
            //         foreach (var data in p.Data)
            //         {
            //             System.Diagnostics.Debug.WriteLine($"{data.Key} : {data.Value}");
            //         }
            //
            //     }
            //
            // };
            //
            // CrossFirebasePushNotification.Current.OnNotificationDeleted += (s, p) =>
            // {
            //     System.Diagnostics.Debug.WriteLine("Dismissed");
            // };
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
