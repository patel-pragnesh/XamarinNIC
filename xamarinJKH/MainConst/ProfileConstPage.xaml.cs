﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Services;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PancakeView;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.Xaml;
using xamarinJKH.DialogViews;
using xamarinJKH.InterfacesIntegration;
using xamarinJKH.Server;
using xamarinJKH.Server.RequestModel;
using xamarinJKH.Tech;
using xamarinJKH.Utils;
using System.Globalization;
using System.Threading;
using Microsoft.AppCenter.Analytics;
using xamarinJKH.PushNotification;

namespace xamarinJKH.MainConst
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfileConstPage : ContentPage
    {
        private LoginResult Person = new LoginResult();
        private RestClientMP _server = new RestClientMP();
        public bool isSave { get; set; }
        public string svg2 { get; set; }

        private async void GoBack(object sender, EventArgs args)
        {
            try
            {
                _ = await Navigation.PopAsync();
            }
            catch { }
        }

        private async void TechSend(object sender, EventArgs e)
        {

            // await PopupNavigation.Instance.PushAsync(new TechDialog(false));
            if (Settings.Person != null && !string.IsNullOrWhiteSpace(Settings.Person.Phone))
            {
                await Navigation.PushModalAsync(new Tech.AppPage());
            }
            else
            {
                await PopupNavigation.Instance.PushAsync(new EnterPhoneDialog());
            }
        }

        public ProfileConstPage()
        {
            InitializeComponent();
            Analytics.TrackEvent("Профиль сотрудника");

            isSave = Preferences.Get("isPass", false);
            NavigationPage.SetHasNavigationBar(this, false);
            var exitClick = new TapGestureRecognizer();
            exitClick.Tapped += async (s, e) =>
            {
                _ = await Navigation.PopModalAsync();
            };
            FrameBtnExit.GestureRecognizers.Add(exitClick);
            var techSend = new TapGestureRecognizer();
            techSend.Tapped += TechSend;// async (s, e) => {    await Navigation.PushAsync(new AppPage());};
            LabelTech.GestureRecognizers.Add(techSend);
            var saveClick = new TapGestureRecognizer();
            saveClick.Tapped += async (s, e) =>
            {

                ButtonClick(FrameBtnLogin, null);
            };
            FrameBtnLogin.GestureRecognizers.Add(saveClick);

            var createPush = new TapGestureRecognizer();
            createPush.Tapped += async (s, e) =>
            {
                await Navigation.PushAsync(new SendPushPage());
            };
            FrameOffers.GestureRecognizers.Add(createPush);

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    int statusBarHeight = DependencyService.Get<IStatusBar>().GetHeight();
                    Pancake.Padding = new Thickness(0, statusBarHeight, 0, 0);

                    break;
                default:
                    break;
            }

            Russian.Effects.Add(Effect.Resolve("MyEffects.RadioButtonEffect"));
            Ukranian.Effects.Add(Effect.Resolve("MyEffects.RadioButtonEffect"));
            English.Effects.Add(Effect.Resolve("MyEffects.RadioButtonEffect"));

            if (!Application.Current.Properties.ContainsKey("Culture"))
            {
                Application.Current.Properties.Add("Culture", string.Empty);
            }
            var culture = CultureInfo.InstalledUICulture;

            switch (Application.Current.Properties["Culture"])
            {
                case "en-EN":
                case "en":
                    English.IsChecked = true;
                    break;
                case "ru-RU":
                case "ru":
                    Russian.IsChecked = true;
                    break;
                case "uk-UA":
                case "uk":
                    Ukranian.IsChecked = true;
                    break;
            }

            var tAutoClick = new TapGestureRecognizer();
            tAutoClick.Tapped += (s, e) => { Device.BeginInvokeOnMainThread(() => RadioButtonAuto.IsChecked = true); };
            tAuto.GestureRecognizers.Add(tAutoClick);

            var tBlackClick = new TapGestureRecognizer();
            tBlackClick.Tapped += (s, e) => { Device.BeginInvokeOnMainThread(() => RadioButtonDark.IsChecked = true); };
            tDark.GestureRecognizers.Add(tBlackClick);

            var tLightClick = new TapGestureRecognizer();
            tLightClick.Tapped += (s, e) => { Device.BeginInvokeOnMainThread(() => RadioButtonLigth.IsChecked = true); };
            tLight.GestureRecognizers.Add(tLightClick);

            var lRuClick = new TapGestureRecognizer();
            lRuClick.Tapped += (s, e) => { Device.BeginInvokeOnMainThread(() => Russian.IsChecked = true); };
            lRu.GestureRecognizers.Add(lRuClick);

            var lEnClick = new TapGestureRecognizer();
            lEnClick.Tapped += (s, e) => { Device.BeginInvokeOnMainThread(() => English.IsChecked = true); };
            lEn.GestureRecognizers.Add(lEnClick);

            var lUaClick = new TapGestureRecognizer();
            lUaClick.Tapped += (s, e) => { Device.BeginInvokeOnMainThread(() => Ukranian.IsChecked = true); };
            lUa.GestureRecognizers.Add(lUaClick);

            SetText();
            SetColor();
            EntryFio.Text = Settings.Person.FIO;
            EntryEmail.Text = Settings.Person.Email;

#if DEBUG
            FrameOffers.IsVisible = true;
#else
            FrameOffers.IsVisible = Settings.Person.UserSettings.RightCreateAnnouncements;
#endif

            MessagingCenter.Subscribe<Object>(this, "ChangeThemeConst", (sender) =>
            {
                OSAppTheme currentTheme = Application.Current.RequestedTheme;
                var colors = new Dictionary<string, string>();
                if (currentTheme == OSAppTheme.Light || currentTheme == OSAppTheme.Unspecified)
                {
                    colors.Add("#000000", ((Color)Application.Current.Resources["MainColor"]).ToHex());
                }
                else
                {
                    colors.Add("#000000", "#FFFFFF");
                }

                ImageBack.ReplaceStringMap = colors;
            });

            BindingContext = this;
        }

        bool svg { get; set; }

        private async void ButtonClick(object sender, EventArgs e)
        {
            SaveInfoAccount(EntryFio.Text, EntryEmail.Text);
        }

        public interface ICloseApplication
        {
            void closeApplication();
        }

        public async void SaveInfoAccount(string fio, string email)
        {
            if (fio != "" && email != "")
            {
                progress.IsVisible = true;
                FrameBtnLogin.IsVisible = false;
                progress.IsVisible = true;
                CommonResult result = await _server.UpdateProfileConst(email, fio);
                if (result.Error == null)
                {
                    Console.WriteLine(result.ToString());
                    Console.WriteLine("Отправлено");
                    await DisplayAlert("", AppResources.SuccessProfile, "OK");
                    FrameBtnLogin.IsVisible = true;
                    progress.IsVisible = false;
                }
                else
                {
                    Console.WriteLine("---ОШИБКА---");
                    Console.WriteLine(result.ToString());
                    FrameBtnLogin.IsVisible = true;
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
                FrameBtnLogin.IsVisible = true;
            }
            else
            {
                if (fio == "" && email == "")
                {
                    await DisplayAlert("", $"{AppResources.ErrorFills} {AppResources.FIO} {AppResources.And} E-mail", "OK");
                }
                else if (fio == "")
                {
                    await DisplayAlert("", $"{AppResources.ErrorFill} {AppResources.FIO}", "OK");
                }
                else if (email == "")
                {
                    await DisplayAlert("", $"{AppResources.ErrorFill} E-mail", "OK");
                }
            }
        }

        void SetText()
        {
            Color hexColor = (Color) Application.Current.Resources["MainColor"];
            UkName.Text = Settings.MobileSettings.main_name;
            SetAdminName();
            //IconViewTech.SetAppThemeColor(IconView.ForegroundProperty, hexColor, Color.White);
            //LabelTech.SetAppThemeColor(Label.TextColorProperty, hexColor, Color.White);
            FrameTop.SetAppThemeColor(Frame.BorderColorProperty, hexColor, Color.White);
            FrameSettings.SetAppThemeColor(Frame.BorderColorProperty, hexColor, Color.White);
        }

        private void SetAdminName()
        {
            FormattedString formattedName = new FormattedString();
            OSAppTheme currentTheme = Application.Current.RequestedTheme;
            //if (Xamarin.Essentials.DeviceInfo.Platform == Xamarin.Essentials.DevicePlatform.iOS)
            //    currentTheme = OSAppTheme.Dark;
            formattedName.Spans.Add(new Span
            {
                Text = Settings.Person.FIO,
                TextColor = currentTheme.Equals(OSAppTheme.Dark) ? Color.White : Color.Black,
                FontAttributes = FontAttributes.Bold,
                FontSize = 16
            });
            formattedName.Spans.Add(new Span
            {
                Text = AppResources.GoodDay,
                TextColor = currentTheme.Equals(OSAppTheme.Dark) ? Color.White : Color.Black,
                FontAttributes = FontAttributes.None,
                FontSize = 16
            });
        }

        void SetColor()
        {
            Color hexColor = (Color)Application.Current.Resources["MainColor"];
            UkName.Text = Settings.MobileSettings.main_name;
            //IconViewSave.Foreground = Color.White;
            // IconViewNameUk.Foreground = hexColor;
            //IconViewFio.Foreground = hexColor;
            //IconViewEmail.Foreground = hexColor;
            //IconViewExit.Foreground = hexColor;

            FrameBtnExit.BackgroundColor = Color.White;
            FrameBtnExit.BorderColor = hexColor;
            FrameBtnLogin.BackgroundColor = hexColor;
            LabelseparatorEmail.BackgroundColor = hexColor;
            LabelseparatorFio.BackgroundColor = hexColor;
            BtnExit.TextColor = hexColor;
            progress.Color = hexColor;

            FrameOffers.BorderColor = hexColor;

            RadioButtonAuto.Effects.Add(Effect.Resolve("MyEffects.RadioButtonEffect"));
            RadioButtonDark.Effects.Add(Effect.Resolve("MyEffects.RadioButtonEffect"));
            RadioButtonLigth.Effects.Add(Effect.Resolve("MyEffects.RadioButtonEffect"));

            int theme = Preferences.Get("Theme", 1);

            //задание темы для ios
            //if (Xamarin.Essentials.DeviceInfo.Platform == DevicePlatform.iOS)
            //    theme = Preferences.Get("Theme", 1);

            switch (theme)
            {
                case 0:
                    RadioButtonAuto.IsChecked = true;
                    break;
                case 1:
                    RadioButtonDark.IsChecked = true;
                    break;
                case 2:
                    RadioButtonLigth.IsChecked = true;
                    break;
            }

        }

        private void SwitchSavePass_OnPropertyChanged(object sender, ToggledEventArgs toggledEventArgs)
        {
            Preferences.Set("isPass", isSave);
        }

        private void RadioButtonAuto_OnCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            //только темная тема в ios
            //if (Xamarin.Essentials.DeviceInfo.Platform != DevicePlatform.iOS)
            //{
                Application.Current.UserAppTheme = OSAppTheme.Unspecified;
                Preferences.Set("Theme", 0);
                MessagingCenter.Send<Object>(this, "ChangeThemeConst");
                MessagingCenter.Send<Object>(this, "ChangeAdminApp");
                MessagingCenter.Send<Object>(this, "ChangeAdminMonitor");
                SetAdminName();
            //}
                
        }

        private void RadioButtonDark_OnCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            Application.Current.UserAppTheme = OSAppTheme.Dark;
            Preferences.Set("Theme", 1);
            MessagingCenter.Send<Object>(this, "ChangeThemeConst");
            MessagingCenter.Send<Object>(this, "ChangeAdminApp");
            MessagingCenter.Send<Object>(this, "ChangeAdminMonitor");
            SetAdminName();
        }

        private void RadioButtonLigth_OnCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            Application.Current.UserAppTheme = OSAppTheme.Light;
            Preferences.Set("Theme", 2);
            MessagingCenter.Send<Object>(this, "ChangeThemeConst");
            MessagingCenter.Send<Object>(this, "ChangeAdminApp");
            MessagingCenter.Send<Object>(this, "ChangeAdminMonitor");
            SetAdminName();
        }

        private async void Russian_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (Application.Current.Properties["Culture"].ToString() != "ru")
            {
                await DisplayAlert(null, "Для того, чтобы изменения вступили в силу, необходимо перезапустить приложение", "OK");
            }
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru");

            AppResources.Culture = new CultureInfo("ru");
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("ru");

            Application.Current.Properties["Culture"] = "ru";
            await Application.Current.SavePropertiesAsync();
        }

        private async void English_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (Application.Current.Properties["Culture"].ToString() != "en")
            {
                await DisplayAlert(null, "In order for the changes to take effect, you must restart the application", "OK");
            }
            var cultures = CultureInfo.GetCultures(CultureTypes.NeutralCultures);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");

            AppResources.Culture = new CultureInfo("en");
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en");
            Application.Current.Properties["Culture"] = "en";
            await Application.Current.SavePropertiesAsync();
        }

        private async void Ukranian_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (Application.Current.Properties["Culture"].ToString() != "uk")
            {
                await DisplayAlert(null, "Для того, щоб зміни вступили в силу, необхідно перезапустити програму", "OK");
            }
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("uk");

            AppResources.Culture = new CultureInfo("uk");
            Application.Current.Properties["Culture"] = "uk";
            await Application.Current.SavePropertiesAsync();
        }
    }
}