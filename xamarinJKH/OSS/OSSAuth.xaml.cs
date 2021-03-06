﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Plugin.Messaging;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamarinJKH.DialogViews;
using xamarinJKH.InterfacesIntegration;
using xamarinJKH.Main;
using xamarinJKH.Server;
using xamarinJKH.Tech;
using xamarinJKH.Utils;

namespace xamarinJKH
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OSSAuth : ContentPage
    {
        public bool forsvg { get; set;}

        readonly RestClientMP rc = new RestClientMP();

        Color hex = (Color)Application.Current.Resources["MainColor"];
        
        string forgotpinText = AppResources.OSSPincodeTroub;

        public OSSAuth()
        {
            InitializeComponent();
            forsvg = false;
            BindingContext = this;
            NavigationPage.SetHasNavigationBar(this, false);
            var techSend = new TapGestureRecognizer();
            techSend.Tapped += async (s, e) => {     await Navigation.PushAsync(new AppPage());};
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

            UkName.Text = Settings.MobileSettings.main_name;
            if (!string.IsNullOrWhiteSpace(Settings.Person.companyPhone))
            {
                LabelPhone.Text = "+" + Settings.Person.companyPhone.Replace("+", "");
            }
            else
            {
                IconViewLogin.IsVisible = false;
                LabelPhone.IsVisible = false;
            }
            
            FrameBtnLogin.BackgroundColor = hex;
            LabelseparatorPass.BackgroundColor = hex;            
            RegistLabel.TextColor = hex;
            progress.Color = hex;

            var backClick = new TapGestureRecognizer();

            var profile = new TapGestureRecognizer();
            profile.Tapped += async (s, e) =>
            {
                if (Navigation.NavigationStack.FirstOrDefault(x => x is ProfilePage) == null)
                    await Navigation.PushAsync(new ProfilePage());
            };
            IconViewProfile.GestureRecognizers.Add(profile);

            backClick.Tapped += async (s, e) => { ClosePage(); };
            BackStackLayout.GestureRecognizers.Add(backClick);

            var forgetPasswordVisible = new TapGestureRecognizer();
            forgetPasswordVisible.Tapped += async (s, e) =>
            {
                EntryPin.IsPassword = !EntryPin.IsPassword;                
                if (EntryPin.IsPassword)
                {
                    //ImageClosePass.Foreground = (Color)Application.Current.Resources["MainColor"];
                }
                else
                {
                    //ImageClosePass.Foreground = Color.DarkSlateGray;
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
                await DisplayAlert(AppResources.Attention, forgotpinText, "OK");
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
                    await DisplayAlert(AppResources.ErrorTitle, checkResult, "OK");
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
                    return AppResources.ErrorOSSAuthNotNumber;
                }
                if (c < 0)
                {
                    return AppResources.ErrorOSSAuthPositive;
                }

                var r = await rc.OSSCheckPin(text);
                return r.Error;
            }
            catch(Exception ex)
            {
                return AppResources.ErrorOSSAuthPin;
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