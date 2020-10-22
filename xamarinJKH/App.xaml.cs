using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Plugin.FirebasePushNotification;
using Xamarin.Essentials;
using Xamarin.Forms.Xaml;
using xamarinJKH.Server;
using xamarinJKH.Server.RequestModel;
using xamarinJKH.Utils;
using Application = Xamarin.Forms.Application;
using System.Linq;
using System.Text;
using Akavache;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using xamarinJKH.Apps;
using xamarinJKH.Main;
using xamarinJKH.Notifications;
using Device = Xamarin.Forms.Device;

namespace xamarinJKH
{
    public partial class App : Application
    {
        public static int ScreenHeight { get; set; }
        public static int ScreenWidth { get; set; }
        public static string version { get; set; }
        public static string model { get; set; }
        public static string token { get; set; }
        public static bool isCons { get; set; } = false;
        public static bool isConnected { get; set; } = true;
        private RestClientMP server = new RestClientMP();

        public App()
        {
            InitializeComponent();
            Crashes.SendingErrorReport += SendingErrorReportHandler;
            Crashes.SentErrorReport += SentErrorReportHandler;
            Crashes.FailedToSendErrorReport += FailedToSendErrorReportHandler;
            Crashes.GetErrorAttachments += GetErrorAttachmentHandler;
            //только темная тема в ios
            if (Device.RuntimePlatform == Device.iOS && Application.Current.UserAppTheme == OSAppTheme.Unspecified)
                Application.Current.UserAppTheme = OSAppTheme.Light;

            DependencyService.Register<RestClientMP>();
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:

                    Color color;

                    int theme = Preferences.Get("Theme", 1);


                    if (theme!=1)
                        color = Color.Black;
                    else
                        color = Color.White;

                    //var color = Application.Current.UserAppTheme == OSAppTheme.Light /*|| Application.Current.UserAppTheme == OSAppTheme.Unspecified*/ ? Color.Black : Color.White;
                    var nav = new Xamarin.Forms.NavigationPage(new MainPage())
                    {
                        BarBackgroundColor = Color.Black,
                        BarTextColor = color
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

            CrossFirebasePushNotification.Current.Subscribe("general");
            CrossFirebasePushNotification.Current.OnTokenRefresh += async (s, p) =>
            {
                System.Diagnostics.Debug.WriteLine($"TOKEN : {p.Token}");
                token = p.Token;
                await server.RegisterDevice(isCons);
            };


            CrossFirebasePushNotification.Current.OnNotificationReceived += (s, p) =>
            {
                System.Diagnostics.Debug.WriteLine("Received");

                Device.BeginInvokeOnMainThread(async () =>
                {
                    try
                    {
                        bool displayAlert = false;
                        string o = string.Empty;

                        if (p.Data.ContainsKey("title") && p.Data.ContainsKey("body"))
                        {
                            var current_page =
                                (App.Current.MainPage.Navigation.ModalStack.ToList()[0] as Xamarin.Forms.TabbedPage)
                                .CurrentPage;
                            if (!(current_page is AppPage))
                            {
                                displayAlert = await MainPage.DisplayAlert(p.Data["title"].ToString(),
                                    p.Data["body"].ToString(), "OK", "Отмена");
                                if (p.Data.ContainsKey("type_push"))
                                    o = p.Data["type_push"].ToString();
                            }
                        }

                        //ios
                        if (p.Data.ContainsKey("aps.alert.title") && p.Data.ContainsKey("aps.alert.body"))
                        {
                            var current_page =
                                (App.Current.MainPage.Navigation.ModalStack.ToList()[0] as Xamarin.Forms.TabbedPage)
                                .CurrentPage;
                            if (!(current_page is AppPage))
                            {
                                displayAlert = await MainPage.DisplayAlert(p.Data["aps.alert.title"].ToString(),
                                    p.Data["aps.alert.body"].ToString(), "OK", "Отмена");
                                if (p.Data.ContainsKey("gcm.notification.type_push"))
                                    o = p.Data["gcm.notification.type_push"].ToString();
                            }
                        }

                        if (displayAlert && o.ToLower().Equals("осс"))
                        {
                            await MainPage.Navigation.PushModalAsync(new OSSMain());
                        }

                        if (displayAlert && o.ToLower().Equals("comment"))
                        {
                            var tabbedpage = App.Current.MainPage.Navigation.ModalStack.ToList()[0];
                            if (tabbedpage is xamarinJKH.Main.BottomNavigationPage)
                            {
                                var stack = (tabbedpage as Xamarin.Forms.TabbedPage).Children[3].Navigation
                                    .NavigationStack;
                                if (stack.Count == 2)
                                {
                                    var app_page = stack.ToList()[0];
                                }
                                else
                                {
                                    MessagingCenter.Send<Object, int>(this, "SwitchToApps",
                                        int.Parse(p.Data["id_request"].ToString()));
                                }
                            }
                        }
                    }
                    catch
                    {
                    }
                });
            };
            CrossFirebasePushNotification.Current.OnNotificationOpened += (s, p) =>
            {
                System.Diagnostics.Debug.WriteLine("Opened");
                if (p.Data.ContainsKey("type_push") || p.Data.ContainsKey("gcm.notification.type_push"))
                {
                    string o = "";
                    if (Device.RuntimePlatform == Device.Android)
                        o = p.Data["type_push"].ToString();
                    else
                        o = p.Data["gcm.notification.type_push"].ToString();

                    if (o.ToLower().Equals("announcement"))
                    {
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            string login = Preferences.Get("login", "");
                            string pass = Preferences.Get("pass", "");
                            if (!pass.Equals("") && !login.Equals(""))
                            {
                                LoginResult loginResult = await server.Login(login, pass);
                                if (loginResult.Error == null)
                                {
                                    Settings.EventBlockData = await server.GetEventBlockData();
                                    foreach (var each in Settings.EventBlockData.Announcements)
                                    {
                                        if (p.Data.ContainsKey("aps.alert.title") && p.Data.ContainsKey("aps.alert.body"))
                                        {
                                            if (p.Data["aps.alert.title"].Equals(each.Header) & p.Data["aps.alert.body"].Equals(each.Text))
                                            {
                                                await MainPage.Navigation.PushModalAsync(new NotificationOnePage(each));
                                            }
                                        }
                                        if (p.Data.ContainsKey("title") && p.Data.ContainsKey("body"))
                                        {
                                            if (p.Data["title"].Equals(each.Header) & p.Data["body"].Equals(each.Text))
                                            {
                                                await MainPage.Navigation.PushModalAsync(new NotificationOnePage(each));
                                            }
                                        }                                        
                                    }
                                }
                            }
                        });
                    }

                    if (o.ToLower().Equals("осс"))
                    {
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            string login = Preferences.Get("login", "");
                            string pass = Preferences.Get("pass", "");
                            if (!pass.Equals("") && !login.Equals(""))
                            {
                                LoginResult loginResult = await server.Login(login, pass);
                                if (loginResult.Error == null)
                                {
                                    Settings.Person = loginResult;
                                    await MainPage.Navigation.PushModalAsync(new OSSMain());
                                }
                            }
                        });
                    }
                }
            };
            CrossFirebasePushNotification.Current.OnNotificationAction += (s, p) =>
            {
                System.Diagnostics.Debug.WriteLine("Action");

                if (!string.IsNullOrEmpty(p.Identifier))
                {
                    System.Diagnostics.Debug.WriteLine($"ActionId: {p.Identifier}");
                    foreach (var data in p.Data)
                    {
                        System.Diagnostics.Debug.WriteLine($"{data.Key} : {data.Value}");
                    }
                }
            };
        }

        private IEnumerable<ErrorAttachmentLog> GetErrorAttachmentHandler(ErrorReport report)
        {
            string accountsJson = Newtonsoft.Json.JsonConvert.SerializeObject(Settings.Person.Accounts);
            // Your code goes here.
            return new ErrorAttachmentLog[]
            {
                ErrorAttachmentLog.AttachmentWithBinary(Encoding.UTF8.GetBytes(accountsJson), "Accounts.json", "application/json")
            };
        }

        private void FailedToSendErrorReportHandler(object sender, FailedToSendErrorReportEventArgs e)
        {
            AppCenterLog.Info(LogTag, "Failed to send error report");

            var args = e as FailedToSendErrorReportEventArgs;
            ErrorReport report = args.Report;
            string AccountsJson = Newtonsoft.Json.JsonConvert.SerializeObject(Settings.Person.Accounts);
            //test some values
            if (report.Exception != null)
            {
                AppCenterLog.Info(LogTag, report.Exception.ToString());
            }
            else if (report.AndroidDetails != null)
            {
                AppCenterLog.Info(LogTag, report.AndroidDetails.ThreadName );
            }

            if (e.Exception != null)
            {
                AppCenterLog.Info(LogTag, "There is an exception associated with the failure");
            }
        }

        private void SentErrorReportHandler(object sender, SentErrorReportEventArgs e)
        {
            AppCenterLog.Info(LogTag, "Sent error report");

            var args = e as SentErrorReportEventArgs;
            ErrorReport report = args.Report;
            string AccountsJson = Newtonsoft.Json.JsonConvert.SerializeObject(Settings.Person.Accounts);

            //test some values
            if (report.Exception != null)
            {
                AppCenterLog.Info(LogTag, report.Exception.ToString());
            }
            else
            {
                AppCenterLog.Info(LogTag, "No system exception was found" );
            }

            if (report.AndroidDetails != null)
            {
                AppCenterLog.Info(LogTag, report.AndroidDetails.ThreadName );
            }
        }

        private void SendingErrorReportHandler(object sender, SendingErrorReportEventArgs e)
        {
            AppCenterLog.Info(LogTag, "Sending error report");

            var args = e as SendingErrorReportEventArgs;
            ErrorReport report = args.Report;

            //test some values
            if (report.Exception != null)
            {
                AppCenterLog.Info(LogTag, report.Exception.ToString());
            }
            else if (report.AndroidDetails != null)
            {
                AppCenterLog.Info(LogTag, report.AndroidDetails.ThreadName);
            }
        }

        public string LogTag { get; set; } = "CrashLog";

        protected override void OnStart()
        {
            Registrations.Start("XamarinJKH");
                                    
            int theme = Preferences.Get("Theme", 1);

            //только темная тема в ios
            //if (Xamarin.Essentials.DeviceInfo.Platform == DevicePlatform.iOS)
            //    theme = Preferences.Get("Theme", 1);

            switch (theme)
                {
                    case 0:
                        Current.UserAppTheme = OSAppTheme.Unspecified;
                        break;
                    case 1:
                        Current.UserAppTheme = OSAppTheme.Dark;
                        break;
                    case 2:
                        Current.UserAppTheme = OSAppTheme.Light;
                        break;
                }
            


            AppCenter.Start("android=4384b8c4-8639-411c-b011-9d9e8408acde;ios=4a45a15f-a591-4860-b748-a856636cf982;", typeof(Analytics), typeof(Crashes));
            AppCenter.LogLevel = LogLevel.Verbose;
            
        
            
            // Handle when your app starts
            CrossFirebasePushNotification.Current.Subscribe("general");
            CrossFirebasePushNotification.Current.OnTokenRefresh += (s, p) =>
            {
                System.Diagnostics.Debug.WriteLine($"TOKEN REC: {p.Token}");
            };
            System.Diagnostics.Debug.WriteLine($"TOKEN: {CrossFirebasePushNotification.Current.Token}");

            CrossFirebasePushNotification.Current.OnNotificationReceived += (s, p) =>
            {
                try
                {
                    System.Diagnostics.Debug.WriteLine("Received");
                    if (p.Data.ContainsKey("body"))
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            System.Diagnostics.Debug.WriteLine($"{p.Data["body"]}");
                        });
                    }

                    if (p.Data.ContainsKey("aps.alert.body"))
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            System.Diagnostics.Debug.WriteLine($"{p.Data["aps.alert.body"]}");
                        });
                    }
                }
                catch (Exception ex)
                {
                }
            };

            CrossFirebasePushNotification.Current.OnNotificationOpened += (s, p) =>
            {
                //System.Diagnostics.Debug.WriteLine(p.Identifier);

                System.Diagnostics.Debug.WriteLine("Opened");
                foreach (var data in p.Data)
                {
                    System.Diagnostics.Debug.WriteLine($"{data.Key} : {data.Value}");
                }

                if (!string.IsNullOrEmpty(p.Identifier))
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        // MainPage.Message = p.Identifier;
                        System.Diagnostics.Debug.WriteLine("123");
                    });
                }
                else if (p.Data.ContainsKey("color"))
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        MainPage.Navigation.PushAsync(new ContentPage()
                        {
                            BackgroundColor = Color.FromHex($"{p.Data["color"]}")
                        });
                    });
                }
                else if (p.Data.ContainsKey("aps.alert.title"))
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        // MainPage.Message = $"{p.Data["aps.alert.title"]}";
                        System.Diagnostics.Debug.WriteLine($"Пушшшш2 ==== {p.Data["aps.alert.title"]}");
                    });
                }
            };

            CrossFirebasePushNotification.Current.OnNotificationAction += (s, p) =>
            {
                System.Diagnostics.Debug.WriteLine("Action");

                if (!string.IsNullOrEmpty(p.Identifier))
                {
                    System.Diagnostics.Debug.WriteLine($"ActionId: {p.Identifier}");
                    foreach (var data in p.Data)
                    {
                        System.Diagnostics.Debug.WriteLine($"{data.Key} : {data.Value}");
                    }
                }
            };

            CrossFirebasePushNotification.Current.OnNotificationDeleted += (s, p) =>
            {
                System.Diagnostics.Debug.WriteLine("Dismissed");
            };
        }

        protected override void OnSleep()
        {
           
        }

        protected override void OnResume()
        {
        }
        
        
    }
}