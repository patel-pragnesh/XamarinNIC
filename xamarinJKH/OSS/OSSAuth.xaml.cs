using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamarinJKH.Server;
using xamarinJKH.Utils;

namespace xamarinJKH
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OSSAuth : ContentPage
    {
        readonly RestClientMP rc = new RestClientMP();

        Color hex = Color.FromHex(Settings.MobileSettings.color);
        
        const string forgotpinText = "Для восстановления/изменения пин-кода обратитесь пожалуйста в Вашу управляющую компанию";

        public OSSAuth()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

            UkName.Text = Settings.MobileSettings.main_name;
            IconViewLogin.Foreground = hex;
            IconViewPass.Foreground = hex;
            ImageClosePass.Foreground = hex;
            FrameBtnLogin.BackgroundColor = hex;
            LabelseparatorPass.BackgroundColor = hex;            
            RegistLabel.TextColor = hex;
            progress.Color = hex;

            var backClick = new TapGestureRecognizer();
            backClick.Tapped += async (s, e) => { ClosePage(); };
            BackStackLayout.GestureRecognizers.Add(backClick);

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
            startLogin.Tapped += ButtonClick;
            FrameBtnLogin.GestureRecognizers.Add(startLogin);

            var startRegForm = new TapGestureRecognizer();
            startRegForm.Tapped += async (s, e) => { OpenPage(new OSSRegisterPin()); };
            RegistLabel.GestureRecognizers.Add(startRegForm);

            
            var forgotPinTgr = new TapGestureRecognizer();
            forgotPinTgr.Tapped += async (s, e) => {
                await DisplayAlert("Внимание", forgotpinText, "OK");
            };
            ForgotPin.GestureRecognizers.Add(forgotPinTgr);

        }

        

        private async void ButtonClick(object sender, EventArgs e)
        {
            try
            {
                //делаем кнопку входа неактивной, отображаем индикатор занятости
                progress.IsVisible = true;
                FrameBtnLogin.IsVisible = false;
                //проверка пина на соответсвие с тем что сохранен на сервере 
                var checkResult = await CheckPinAsync(EntryPin.Text);

                if (string.IsNullOrWhiteSpace(checkResult))
                {
                    progress.IsVisible = false;
                    FrameBtnLogin.IsVisible = true;
                    OpenPage(new OSSMain());
                }
                else
                {
                    await DisplayAlert("Ошибка", checkResult, "OK");
                    progress.IsVisible = false;
                    FrameBtnLogin.IsVisible = true;
                }
            }
            catch(Exception ex)
            {
                progress.IsVisible = false;
                FrameBtnLogin.IsVisible = true;
            }
        }

        private async Task<string> CheckPinAsync(string text)
        {
            try
            {
                var r = await rc.OSSCheckPin(text);
                return r.Error;
            }
            catch(Exception ex)
            {
                return "Ошибка при проверки пин-кода";
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
    }
}