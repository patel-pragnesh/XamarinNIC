using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AiForms.Dialogs;
using AiForms.Dialogs.Abstractions;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Rg.Plugins.Popup.Services;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamarinJKH.Apps;
using xamarinJKH.InterfacesIntegration;
using xamarinJKH.Server;
using xamarinJKH.Server.RequestModel;
using xamarinJKH.Utils;

namespace xamarinJKH.DialogViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TechDialog : Rg.Plugins.Popup.Pages.PopupPage
    {
        public string number { get; set; }
        private RestClientMP server = new RestClientMP();
        public TechDialog(bool isVisibleApp = true)
        {
            var stack = Rg.Plugins.Popup.Services.PopupNavigation.PopupStack;
            if (stack.Count == 0)
                Device.BeginInvokeOnMainThread(async () => await SendTechTask());
            else if (stack != null)
                Device.BeginInvokeOnMainThread(async () => await Rg.Plugins.Popup.Services.PopupNavigation.PopAsync());
            InitializeComponent();
            Analytics.TrackEvent("Диалог тех.поддержки");
            LabelInfo.Text =
                AppResources.TechAdditionalText1 +
                Settings.MobileSettings.main_name + AppResources.TechAdditionalText2;
            BtnApp.Text = AppResources.TechAdditionalText3 + Settings.MobileSettings.main_name;
            var appOpen = new TapGestureRecognizer();
            StackLayoutApp.IsVisible = isVisibleApp && Settings.AppIsVisible;
            var close = new TapGestureRecognizer();
            close.Tapped += async (s, e) => { await PopupNavigation.Instance.PopAsync(); };
            IconViewClose.GestureRecognizers.Add(close);
            appOpen.Tapped += async (s, e) =>
            {
                if (isVisibleApp)
                {
                    if (Settings.Person.Accounts.Count > 0)
                    {
                        if (Settings.TypeApp.Count > 0)
                        {
                            await Navigation.PushModalAsync(new NewAppPage());
                        }
                        else
                        {
                            await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorAppsNoTypes, "OK");
                        }
                    }
                    else
                    {
                        await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorAppsNoIdent, "OK");
                    }
                    await PopupNavigation.Instance.PopAsync();
                }
            };
            FrameBtnApp.GestureRecognizers.Add(appOpen);
            var openUrlWhatsapp = new TapGestureRecognizer();
            openUrlWhatsapp.Tapped += async (s, e) => { await LoadUrl("https://wa.me/79955052402","com.whatsapp"); };
            ImageWhatsapp.GestureRecognizers.Add(item: openUrlWhatsapp);
            var openUrlTelegram = new TapGestureRecognizer();
            openUrlTelegram.Tapped += async (s, e) => { await LoadUrl("https://teleg.run/oiCVE7GCt3Z0bot","org.telegram.messenger"); };
            ImageTelegram.GestureRecognizers.Add(item: openUrlTelegram);
            var openUrlVider = new TapGestureRecognizer();
            openUrlVider.Tapped += async (s, e) => { await LoadUrl("https://clc.am/4YSqZg", "com.viber.voip"); };
            ImageViber.GestureRecognizers.Add(item: openUrlVider);

            var openMail = new TapGestureRecognizer();
            openMail.Tapped += async (s, e) => {
                try
                {
                    var message = new EmailMessage
                    {
                        Subject = $"{AppResources.PhoneNumber+" "+Settings.Person.Phone}, {AppResources.application+": "+Settings.MobileSettings.main_name}",
                        //Body= ,
                        To = new List<string> { "mobile@sm-center.ru" }

                    };
                    await Email.ComposeAsync(message);
                }
                catch (FeatureNotSupportedException fbsEx)
                {
                    Analytics.TrackEvent("Диалог тех.поддержки, невозможно отправить письмо, не поддерживается устройством");
                    Crashes.TrackError(fbsEx);
                    // Email is not supported on this device
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                    // Some other exception occurred
                }
            };
            mailSpan.GestureRecognizers.Add(openMail);
        }

        //private async Task OpenMail_TappedAsync(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        var message = new EmailMessage
        //        {
        //            Subject = "тема письма",
        //            //Body= ,
        //            To = new List<string> { "mobile@sm-center.ru" }
                    
        //        };
        //        await Email.ComposeAsync(message);
        //    }
        //    catch (FeatureNotSupportedException fbsEx)
        //    {
        //        Analytics.TrackEvent("Диалог тех.поддержки, невозможно отправить письмо, не поддерживается устройством");
        //        Crashes.TrackError(fbsEx);
        //        // Email is not supported on this device
        //    }
        //    catch (Exception ex)
        //    {
        //        Crashes.TrackError(ex);
        //        // Some other exception occurred
        //    }
            
        //}

        bool launched;
        private async Task SendTechTask()
        {
            // Loading settings
            Configurations.LoadingConfig = new LoadingConfig
            {
                IndicatorColor = (Color) Application.Current.Resources["MainColor"],
                OverlayColor = Color.Black,
                Opacity = 0.8,
                DefaultMessage = "Загрузка",
            };

            await Loading.Instance.StartAsync(async progress =>
            {
                // some heavy process.
                if (!launched)
                    await sendTech();
            });

            Frame.IsVisible = true;
        }
        private async Task LoadUrl(string url, string package)
        {
            try
            {
                if (Device.RuntimePlatform == "Android")
                {
                    if (DependencyService.Get<IOpenApp>().IsOpenApp(package))
                    {
                        await Launcher.OpenAsync(url);
                    }
                    else
                    {
                        await Launcher.OpenAsync($"https://play.google.com/store/apps/details?id={package}");
                    }
                }
                else
                {
                    if (DependencyService.Get<IOpenApp>().IsOpenApp(package))
                    {
                        await Launcher.OpenAsync(url);
                    }
                    else
                    {
                        var uri = $"https://itunes.apple.com/us/app/";

                        if (package.Contains("what"))
                            uri += "whatsapp-messenger/id310633997";

                        if (package.Contains("teleg"))
                            uri += "telegram-messenger/id686449807";

                        if (package.Contains("viber"))
                            uri += "viber-messenger-chats-calls/id382617920";

                        //await Launcher.OpenAsync(url);
                        await Launcher.OpenAsync(uri);

                    }

                }
                // await PopupNavigation.Instance.PopAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorAdditionalLink, "OK");
            }

        }
        
        async Task sendTech()
        {
            Analytics.TrackEvent("Запрос кода для тех.поддержки");
            TechSupportAppealArguments arguments = new TechSupportAppealArguments();
            arguments.OS = Device.RuntimePlatform;
            arguments.Phone = Settings.Person.Phone;
            arguments.Text = "Заявка из МП для мессенджеров";
            arguments.Mail = "example@gmail.com";
            arguments.AppVersion = Xamarin.Essentials.AppInfo.VersionString;
            arguments.Info = GetIdent();
            arguments.Address = GetAdres();
            arguments.Login = Settings.Person.Login;

            TechId result = await server.TechSupportAppeal(arguments);
            if (result.Error == null)
            {

                number = result.requestId.ToString();
                Analytics.TrackEvent("Получили код: " + number);
                BindingContext = this;

            }

            number = "-1";
            launched = true;
        }
            
        string GetAdres()
        {
            try
            {
                return Settings.Person.Accounts[0].Address;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        string GetIdent()
        {
            try
            {
                return Settings.Person.Accounts[0].Ident;
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        
    }
}