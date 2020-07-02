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

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    BackgroundColor = Color.White;
                    BackgroundColor = Color.White;
                    ImageTop.Margin = new Thickness(0, 0, 0, 0);
                    StackLayout.Margin = new Thickness(0, 33, 0, 0);
                    IconViewNameUk.Margin = new Thickness(0, 33, 0, 0);
                    RelativeLayoutTop.Margin = new Thickness(0, 0, 0, 0);
                    if (App.ScreenHeight <= 667)//iPhone6
                    {
                        RelativeLayoutTop.Margin = new Thickness(0, 0, 0, -110);
                    }
                    else if (App.ScreenHeight <= 736)//iPhone8Plus Height=736
                    {
                        RelativeLayoutTop.Margin = new Thickness(0, 0, 0, -145);
                    }
                    else
                    {
                        RelativeLayoutTop.Margin = new Thickness(0, 0, 0, -145);
                    }


                    break;
                case Device.Android:
                    RelativeLayoutTop.Margin = new Thickness(0, 0, 0, -135);
                    //var h = Xamarin.Essentials.DeviceDisplay.MainDisplayInfo.Height;
                    //var w = Xamarin.Essentials.DeviceDisplay.MainDisplayInfo.Width;
                    //  var isLongScreen =h/w >=1.9;
                    ////if (isLongScreen)
                    ////{
                    //    int m = 8;
                    //    var x = Convert.ToInt32(h / 300);
                    //    switch (x)
                    //    {
                    //        case 5:
                    //            m = 20;
                    //            break;
                    //        case 6:
                    //            m = 18; 
                    //            break;
                    //        case 7:
                    //            m = 16;
                    //            break;
                    //        case 8:
                    //            m = 14;
                    //            break;
                    //        case 9:
                    //            m = 12;
                    //            break;
                    //        case 10:
                    //            m = 10;
                    //            break;
                    //        case 11:
                    //            m = 8;
                    //            break;
                    //        case 12:
                    //            m = 6;
                    //            break;
                    //        default:

                    //            break;
                    //    }

                    //    int k = Convert.ToInt32( h / m);
                    //    pageContent.Margin = new Thickness(0, -300, 0, 300); 
                    ////}
                    ////else
                    ////{
                    ////    int m = Convert.ToInt32(h / 14);
                    ////    pageContent.Margin = new Thickness(0, -m, 0, m);
                    ////}

                    double or = Math.Round(((double)App.ScreenWidth / (double)App.ScreenHeight), 2);
                    if (Math.Abs(or - 0.5) < 0.02)
                    {
                        //RelativeLayoutTop.Margin = new Thickness(0, 0, 0, -90);
                        pageContent.Margin = new Thickness(0, -150, 0, 250);
                    }
                    else
                    {
                        pageContent.Margin = new Thickness(0, -150, 0, 150);
                    }

                    break;
                default:
                    break;
            }

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
                int c;
                var isInt = int.TryParse(text, out c);
                if(!isInt)
                {
                    return "Введенный пин-код не является числом";
                }
                if (c < 0)
                {
                    return "Введите положительное число в поле пин-код";
                }

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