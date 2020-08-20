
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.Messaging;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamarinJKH.DialogViews;
using xamarinJKH.InterfacesIntegration;
using xamarinJKH.Server;
using xamarinJKH.Tech;
using xamarinJKH.Utils;

namespace xamarinJKH
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OSSRegisterPin : ContentPage
    {
        Color hex = Color.FromHex(Settings.MobileSettings.color);

        public OSSRegisterPin()
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
                        phoneDialer.MakePhoneCall(Settings.Person.companyPhone);
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

            //Запрос в смс/пуше кода подтверждения 
            GetAuthCode();

            NavigationPage.SetHasNavigationBar(this, false);

            UkName.Text = Settings.MobileSettings.main_name;
            LabelPhone.Text =  "+" + Settings.Person.companyPhone.Replace("+","");
            IconViewCode.Foreground = hex;

            // IconViewLogin.Foreground = hex;
            
            IconViewPass0.Foreground = hex;
            IconViewPass.Foreground = hex;

            ImageClosePass0.Foreground = hex;
            ImageClosePass.Foreground = hex;

            FrameBtnLogin.BackgroundColor = hex;
            LabelseparatorPass0.BackgroundColor = hex;
            LabelseparatorPass.BackgroundColor = hex;
            LabelseparatorPass4.BackgroundColor = hex;
            
            progress.Color = hex;

            var backClick = new TapGestureRecognizer();
            backClick.Tapped += async (s, e) => { ClosePage(); };
            BackStackLayout.GestureRecognizers.Add(backClick);

            var forgetPasswordVisible0 = new TapGestureRecognizer();
            forgetPasswordVisible0.Tapped += async (s, e) =>
            {
                EntryPin0.IsPassword = !EntryPin0.IsPassword;
                if (EntryPin0.IsPassword)
                {
                    ImageClosePass0.Foreground = Color.FromHex(Settings.MobileSettings.color);
                }
                else
                {
                    ImageClosePass0.Foreground = Color.DarkSlateGray;
                }

            };
            ImageClosePass0.GestureRecognizers.Add(forgetPasswordVisible0);

            var forgetPasswordVisible = new TapGestureRecognizer();
            forgetPasswordVisible.Tapped += async (s, e) =>
            {
                EntryPin.IsPassword = !EntryPin.IsPassword;
                if (EntryPin.IsPassword)
                {
                    ImageClosePass.Foreground = Color.FromHex(Settings.MobileSettings.color);
                }
                else
                {
                    ImageClosePass.Foreground = Color.DarkSlateGray;
                }

            };
            ImageClosePass.GestureRecognizers.Add(forgetPasswordVisible);

            var startLogin = new TapGestureRecognizer();
            startLogin.Tapped += RegisterPin;
            FrameBtnLogin.GestureRecognizers.Add(startLogin);
        }

        readonly RestClientMP rc = new RestClientMP();
        async void GetAuthCode()
        {
            if(Settings.Person.Accounts!=null && Settings.Person.Accounts.Count>0)
            {
                if(string.IsNullOrWhiteSpace(Settings.Person.Phone))
                {
                    await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorOSSRegisterAddPhone, "OK");
                    return;
                }
                //вызвали метод сервера для получения регстрационного кода по телефону/номеру счета 
                var r = await rc.OSSCheckCode(Settings.Person.Phone, Settings.Person.Accounts[0].Ident);
                if(!string.IsNullOrWhiteSpace( r.Error))
                {
                    await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorOSSRegisterCode+r.Error, "OK");
                    return;
                }
                await DisplayAlert(AppResources.ToGetSignature, AppResources.ToGetSignatureText, "OK");
            }
            else
            {
                await DisplayAlert(AppResources.ErrorTitle, AppResources.EnterIdent, "OK");
                return;
            }
        }

        async void RegisterPin(object sender, EventArgs e)
        {
            progress.IsVisible = true;
            FrameBtnLogin.IsVisible = false;

            //код
            var code = EntryCode.Text;
            if(string.IsNullOrWhiteSpace(code))
            {
                await DisplayAlert(AppResources.ErrorTitle, AppResources.EnterCode, "OK");
                progress.IsVisible = false;
                FrameBtnLogin.IsVisible = true;
                return;
            }

            int c0;
            var isInt0 = int.TryParse(code, out c0);
            if (!isInt0)
            {
                await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorOSSAuthNotNumber, "OK");
                progress.IsVisible = false;
                FrameBtnLogin.IsVisible = true;
                return;
            }
            if (c0 < 0)
            {
                await DisplayAlert(AppResources.ErrorTitle, $"{AppResources.ErrorOSSAuthPositive} \"{AppResources.CodeField}\"", "OK");
                progress.IsVisible = false;
                FrameBtnLogin.IsVisible = true;
                return;
            }


            int c;            
            var isInt = int.TryParse(EntryPin0.Text, out c);
            if (!isInt)
            {
                await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorOSSAuthNotNumber, "OK");
                progress.IsVisible = false;
                FrameBtnLogin.IsVisible = true;
                return;
            }
            if (c < 0)
            {
                await DisplayAlert(AppResources.ErrorTitle, $"{AppResources.ErrorOSSAuthPositive} \"{AppResources.PinField}\"", "OK");
                progress.IsVisible = false;
                FrameBtnLogin.IsVisible = true;
                return;
            }

            int c1;
            var isInt1 = int.TryParse(EntryPin.Text, out c1);
            if (!isInt1)
            {
                await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorOSSAuthNotNumber, "OK");
                progress.IsVisible = false;
                FrameBtnLogin.IsVisible = true;
                return;
            }
            if (c1 < 0)
            {
                await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorOSSAuthPositive, "OK");
                progress.IsVisible = false;
                FrameBtnLogin.IsVisible = true;
                return;
            }

            if (string.IsNullOrWhiteSpace( EntryPin0.Text) || string.IsNullOrWhiteSpace( EntryPin.Text))
            {
                //пин не введен
                await DisplayAlert(AppResources.ErrorTitle, AppResources.EnterBothPins, "OK");

                progress.IsVisible = false;
                FrameBtnLogin.IsVisible = true;

                return;
            }

            if (EntryPin0.Text!=EntryPin.Text)
            {
                //введенный пин не совпадает
                await DisplayAlert(AppResources.ErrorTitle, AppResources.PinsDifferent, "OK");
                
                progress.IsVisible = false;
                FrameBtnLogin.IsVisible = true;
                
                return;
            }

            //сохраняем пин на сервер, вместе с проверочным кодом
            var rez = await rc.OSSSavePin(Settings.Person.Phone, code, EntryPin0.Text);

            if(!string.IsNullOrWhiteSpace(rez.Error))
            {
                await DisplayAlert(AppResources.ErrorTitle, rez.Error, "OK");
                progress.IsVisible = false;
                FrameBtnLogin.IsVisible = true;
                return;

            }

            progress.IsVisible = false;
            FrameBtnLogin.IsVisible = true;

            //далее переход на форму голосования, или на форму ввода пина
            OpenPage(new OSSAuth());            
        }
        
        async void OpenPage(Page page)
        {
            try
            {
                await Navigation.PushAsync(page);
            }
            catch
            {
                await Navigation.PushModalAsync(page);
            }
        }
        async void ClosePage()
        {
            try
            {
                await Navigation.PopAsync();
            }
            catch (Exception e)
            {
                await Navigation.PopModalAsync();
            }
        }
    }
}