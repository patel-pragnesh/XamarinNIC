using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                    if (phoneDialer.CanMakePhoneCall) 
                        phoneDialer.MakePhoneCall(System.Text.RegularExpressions.Regex.Replace(Settings.Person.companyPhone, "[^+0-9]", ""));
                }

            
            };
            LabelPhone.GestureRecognizers.Add(call);
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

            _paysPage = paysPage;
            SetText();
            var backClick = new TapGestureRecognizer();
            backClick.Tapped += async (s, e) => { _ = await Navigation.PopAsync(); };
            BackStackLayout.GestureRecognizers.Add(backClick);
        }
        
        private async void AddButtonClick(object sender, EventArgs e)
        {
            AddIdentAccount(EntryIdent.Text);
        }
        
        void SetText()
        {
            UkName.Text = Settings.MobileSettings.main_name;
            LabelPhone.Text = "+" + Settings.Person.companyPhone.Replace("+","");
            FrameBtnAdd.BackgroundColor = Color.FromHex(Settings.MobileSettings.color);
            progress.Color = Color.FromHex(Settings.MobileSettings.color);
            Labelseparator.BackgroundColor = Color.FromHex(Settings.MobileSettings.color);
            IconViewFio.Foreground = Color.FromHex(Settings.MobileSettings.color);
            
            Color hexColor = (Color) Application.Current.Resources["MainColor"];
            IconViewLogin.SetAppThemeColor(IconView.ForegroundProperty, hexColor, Color.White);
            IconViewTech.SetAppThemeColor(IconView.ForegroundProperty, hexColor, Color.White);
            Pancake.SetAppThemeColor(PancakeView.BorderColorProperty, hexColor, Color.Transparent);
            PancakeViewIcon.SetAppThemeColor(PancakeView.BorderColorProperty, hexColor, Color.Transparent);if (Device.RuntimePlatform == Device.iOS){ if (AppInfo.PackageName == "rom.best.saburovo" || AppInfo.PackageName == "sys_rom.ru.tsg_saburovo"){PancakeViewIcon.Padding = new Thickness(0);}}
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
                        AcceptAddIdentAccount(ident, Settings.Person.Email);
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
                    await DisplayAlert("", $"{AppResources.Acc} " + ident + " успешно подключён, для дальнейшей работы перезагрузите приложение", "ОК");
                    FrameBtnAdd.IsVisible = true;
                    progress.IsVisible = false;
                   
                    await Navigation.PopAsync();
                    _paysPage.RefreshPaysData();
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