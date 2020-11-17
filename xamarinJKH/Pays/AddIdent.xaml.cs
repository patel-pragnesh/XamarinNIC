using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AppCenter.Analytics;
using Plugin.Messaging;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.PancakeView;
using Xamarin.Forms.Xaml;
using xamarinJKH.DialogViews;
using xamarinJKH.Server;
using xamarinJKH.Main;
using xamarinJKH.InterfacesIntegration;
using xamarinJKH.Server.RequestModel;
using xamarinJKH.Tech;
using xamarinJKH.Utils;
using Xamarin.Essentials;

namespace xamarinJKH.Pays
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddIdent : ContentPage
    {
        private RestClientMP _server = new RestClientMP();
        private PaysPage _paysPage;
        public AddIdent(PaysPage paysPage)
        {
            InitializeComponent();
            Analytics.TrackEvent("Добавление ЛС");
            NavigationPage.SetHasNavigationBar(this, false);
            var techSend = new TapGestureRecognizer();
            techSend.Tapped += async (s, e) => {     await PopupNavigation.Instance.PushAsync(new TechDialog()); };
            LabelTech.GestureRecognizers.Add(techSend);
            var call = new TapGestureRecognizer();
            call.Tapped += async (s, e) =>
            {
                if (Settings.Person.Phone != null)
                {
                    IPhoneCallTask phoneDialer;
                    phoneDialer = CrossMessaging.Current.PhoneDialer;
                    if (phoneDialer.CanMakePhoneCall && !string.IsNullOrWhiteSpace(Settings.Person.companyPhone)) 
                        phoneDialer.MakePhoneCall(System.Text.RegularExpressions.Regex.Replace(Settings.Person.companyPhone, "[^+0-9]", ""));
                }

            
            };
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    int statusBarHeight = DependencyService.Get<IStatusBar>().GetHeight();
                    //BackgroundColor = Color.White;
                    break;
                default:
                    break;
            }

            _paysPage = paysPage;
            SetText();
            var backClick = new TapGestureRecognizer();
            backClick.Tapped += async (s, e) => {
                try
                {
                    _ = await Navigation.PopAsync();
                }
                catch { }
            };
            BackStackLayout.GestureRecognizers.Add(backClick);
            MessagingCenter.Subscribe<Object>(this, "ClosePage", async (sender) =>
            {
                _ = await Navigation.PopAsync();
            });
        }
        
        private async void AddButtonClick(object sender, EventArgs e)
        {
            AddIdentAccount(EntryIdent.Text);
        }
        public bool hex { get; set; }
        void SetText()
        {
            UkName.Text = Settings.MobileSettings.main_name;
           
            FrameBtnAdd.BackgroundColor = (Color)Application.Current.Resources["MainColor"];
            progress.Color = (Color)Application.Current.Resources["MainColor"];
            Labelseparator.BackgroundColor = (Color)Application.Current.Resources["MainColor"];
            IconViewFio.Foreground = (Color)Application.Current.Resources["MainColor"];
            
            Color hexColor = (Color) Application.Current.Resources["MainColor"];
            IconViewTech.SetAppThemeColor(IconView.ForegroundProperty, hexColor, Color.White);
            LabelTech.SetAppThemeColor(Label.TextColorProperty, hexColor, Color.White);
            Frame.SetAppThemeColor(Xamarin.Forms.Frame.BorderColorProperty, hexColor, Color.White);
        }
        
        public async void AddIdentAccount(string ident)
        {
            if (ident != "")
            {
                progress.IsVisible = true;
                FrameBtnAdd.IsVisible = false;
                progress.IsVisible = true;
                AddAccountResult result = await _server.AddIdent(ident, true);
                if (result.Error == null)
                {
                    Console.WriteLine(result.Address);
                    Console.WriteLine("Отправлено");
                    bool answer = await DisplayAlert("", result.Address, AppResources.AddIdent, AppResources.Cancel);
                    if (answer)
                    {
                        if (Settings.MobileSettings.useAccountPinCode)
                        {
                            await PopupNavigation.Instance.PushAsync(
                                new EnterPinAddLs(ident, _paysPage, this));
                        }
                        else
                        {
                            AcceptAddIdentAccount(ident, Settings.Person.Email);
                        }
                    }
                    else
                    {
                        FrameBtnAdd.IsVisible = true;
                        progress.IsVisible = false;
                    }
                }
                else
                {
                    Console.WriteLine("---ОШИБКА---");
                    Console.WriteLine(result.ToString());
                    FrameBtnAdd.IsVisible = true;
                    progress.IsVisible = false;
                    if (Device.RuntimePlatform == Device.iOS)
                    {
                        await DisplayAlert(AppResources.ErrorTitle, result.Error, "OK");
                    }
                    else
                    {
                        DependencyService.Get<IMessage>().ShortAlert(result.Error);
                    }
                }

                progress.IsVisible = false;
                FrameBtnAdd.IsVisible = true;
            }
            else
            {
                if (ident == "")
                {
                    await DisplayAlert("", AppResources.ErrorFillIdent , "OK");
                }
            }
        }
        
        public async void AcceptAddIdentAccount(string ident, string email)
        {
            if (ident != "")
            {
                progress.IsVisible = true;
                FrameBtnAdd.IsVisible = false;
                progress.IsVisible = true;
                AddAccountResult result = await _server.AddIdent(ident);
                if (result.Error == null)
                {
                    Console.WriteLine(result.Address);
                    Console.WriteLine("Отправлено");
                    await DisplayAlert("", $"{AppResources.Acc} " + ident + $"{AppResources.AddLsString}", "ОК");
                    FrameBtnAdd.IsVisible = true;
                    progress.IsVisible = false;
                    if (!string.IsNullOrWhiteSpace(result.acx))
                    {
                       
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            string login = Preferences.Get("login", "");
                            string pass = Preferences.Get("pass", "");
                            if (!pass.Equals("") && !login.Equals(""))
                            {
                                if (Xamarin.Essentials.Connectivity.NetworkAccess != Xamarin.Essentials.NetworkAccess.Internet)
                                {
                                    Device.BeginInvokeOnMainThread(async () =>
                                        await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorNoInternet, "OK"));
                                    return;
                                }

                                LoginResult loginResult = await _server.Login(login, pass);
                                if (loginResult.Error == null)
                                {
                                    Settings.Person = loginResult;
                                }
                                Settings.Person.acx = result.acx;
                                MessagingCenter.Send<Object>(this, "UpdateCounters");
                                MessagingCenter.Send<Object>(this, "AutoUpdate");
                            }
                        });
                    }
                    try
                    {
                        _ = await Navigation.PopAsync();
                    }
                    catch { }

                    if (_paysPage != null) await _paysPage.RefreshPaysData();
                }
                else
                {
                    Console.WriteLine("---ОШИБКА---");
                    Console.WriteLine(result.ToString());
                    FrameBtnAdd.IsVisible = true;
                    progress.IsVisible = false;
                    if (Device.RuntimePlatform == Device.iOS)
                    {
                        await DisplayAlert(AppResources.ErrorTitle, result.Error, "OK");
                    }
                    else
                    {
                        DependencyService.Get<IMessage>().ShortAlert(result.Error);
                    }
                }

                progress.IsVisible = false;
                FrameBtnAdd.IsVisible = true;
            }
            else
            {
                if (ident == "")
                {
                    await DisplayAlert("", AppResources.ErrorFillIdent, "OK");
                }
            }
        }
    }
}