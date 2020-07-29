using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Text.RegularExpressions;
using Xamarin.Essentials;
using xamarinJKH.Navigation;
using xamarinJKH.Server;
using xamarinJKH.Server.RequestModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Forms.Internals;
using xamarinJKH.Main;
using xamarinJKH.MainConst;
using xamarinJKH.Tech;
using xamarinJKH.Utils;
using XamEffects;
using NavigationPage = Xamarin.Forms.NavigationPage;
using AiForms.Dialogs.Abstractions;
using AiForms.Dialogs;
using xamarinJKH.DialogViews;

using Plugin.Permissions;
using Plugin.Permissions.Abstractions;

namespace xamarinJKH
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]

    public partial class MainPage : ContentPage
    {
        private RestClientMP server = new RestClientMP();
        public Color hex { get; set; }

        public MainPage()
        {
            InitializeComponent();
            getSettings();
           
            NavigationPage.SetHasNavigationBar(this, false);
            Device.BeginInvokeOnMainThread(() =>
            {
               ;

            });
            var startRegForm = new TapGestureRecognizer();
            startRegForm.Tapped += async (s, e) => { await Navigation.PushModalAsync(new RegistrForm(this)); };
            RegistLabel.GestureRecognizers.Add(startRegForm);
            
            var forgotPass = new TapGestureRecognizer();
            forgotPass.Tapped += async (s, e) =>
            {
                await DisplayAlert("Информация", "Для восстановления пароля, пройдите регистрацию повторно", "OK");
            };
            ForgotPass.GestureRecognizers.Add(forgotPass);

            var authConst = new TapGestureRecognizer();
            authConst.Tapped += ChoiceAuth;
            LabelSotr.GestureRecognizers.Add(authConst); 
            
            var techSend = new TapGestureRecognizer();
            techSend.Tapped += TechSend;
            LabelTech.GestureRecognizers.Add(techSend);

            var startLogin = new TapGestureRecognizer();
            startLogin.Tapped += ButtonClick;
            FrameBtnLogin.GestureRecognizers.Add(startLogin);

            var forgetPasswordVisible = new TapGestureRecognizer();
            forgetPasswordVisible.Tapped += async (s, e) =>
            {
                EntryPass.IsPassword = !EntryPass.IsPassword;
                EntryPassConst.IsPassword = !EntryPassConst.IsPassword;
                if (EntryPass.IsPassword)
                {
                    ImageClosePass.Foreground = Color.FromHex(Settings.MobileSettings.color);
                }
                else
                {
                    ImageClosePass.Foreground = Color.DarkSlateGray;
                }

            };
            ImageClosePass.GestureRecognizers.Add(forgetPasswordVisible);
            EntryLogin.Text = "";
            EntryLoginConst.Text = "";
            EntryPass.Text = "";
            // Login("79237173372", "123");

            // Login("79261270258", "19871987");
            // Login("79261937745", "123");
            string login = Preferences.Get("login", "");
            string pass = Preferences.Get("pass", "");
            string loginConst = Preferences.Get("loginConst", "");
            string passConst = Preferences.Get("passConst", "");
            bool isSave = Preferences.Get("isPass", false);
            Settings.ConstAuth = Preferences.Get("constAuth", false);
            if (Settings.ConstAuth && Settings.IsFirsStart && !passConst.Equals("") && !loginConst.Equals("") && !isSave)
            {
                LoginDispatcher(loginConst, passConst);
                Settings.IsFirsStart = false;
                EntryLogin.Text = login;
                EntryLoginConst.Text = loginConst;
                EntryPass.Text = pass;
                EntryPassConst.Text = passConst;
            }
            else if (Settings.IsFirsStart && !pass.Equals("") && !login.Equals("") && !isSave)
            {
                Login(login, pass);
                Settings.IsFirsStart = false;
                EntryLogin.Text = login;
                EntryLoginConst.Text = loginConst;
                EntryPass.Text = pass;
                EntryPassConst.Text = passConst;
            }

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    if (App.ScreenHeight < 600)
                    {
                        RegistLabel.FontSize = 13;
                        ForgotPass.FontSize = 13;
                        RegStackLayout.Margin = new Thickness(0, 5, 0, 0);
                        BottomStackLayout.Margin = new Thickness(0, -20, 0, 20);
                    }

                    if(Xamarin.Essentials.DeviceDisplay.MainDisplayInfo.Width<700)
                    {
                        LabelPhone.FontSize = 13;
                        LabelPassword.FontSize = 13;
                    }
                    break;
                case Device.Android:
                    break;
                default:
                    break;
            }
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            if (Device.RuntimePlatform == "Android")
            {
                var camera_perm = await Plugin.Permissions.CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
                if (camera_perm != PermissionStatus.Granted)
                {
                    await CrossPermissions.Current.RequestPermissionsAsync(Permission.Camera, Permission.Storage);
                }

                var file_perm = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);
                if (file_perm != PermissionStatus.Granted)
                {
                    await CrossPermissions.Current.RequestPermissionsAsync(Permission.Storage);
                }

            }
        }

        async void CheckForUpdate()
        {
            var version = Xamarin.Essentials.AppInfo.VersionString;
            var settings = await server.MobileAppSettings(version, "1");

            if (settings.Error != null && settings.Error.Contains("обновить"))
            {
                Device.BeginInvokeOnMainThread(async () => await Dialog.Instance.ShowAsync<UpdateNotificationDialog>());
            }
        }

        private async void getSettings()
        {
            
            Settings.MobileSettings = await server.MobileAppSettings("4.02", "0");
            if (Settings.MobileSettings.Error == null)
            {
                if (Device.RuntimePlatform == Device.Android)
                    CheckForUpdate();

                // if (RestClientMP.SERVER_ADDR.Contains("dgservicnew"))
                // {
                //     Settings.MobileSettings.main_name = "ООО \"ДОМЖИЛСЕРВИС\"";
                // }
                
                UkName.Text = Settings.MobileSettings.main_name;
                Version.Text ="ver " + Xamarin.Essentials.AppInfo.VersionString;
                hex = Color.FromHex(Settings.MobileSettings.color);
                IconViewLogin.Foreground = hex;
                IconViewPass.Foreground = hex;
                ImageClosePass.Foreground = hex;

                FrameBtnLogin.BackgroundColor = hex;
                LabelseparatorPass.BackgroundColor = hex;
                LabelseparatorLogin.BackgroundColor = hex;
                SwitchLogin.OnColor = hex;
                SwitchLogin.ThumbColor = Color.White;
                RegistLabel.TextColor = hex;
                progress.Color = hex;
                Color.SetAccent(hex);

                StackLayoutContent.IsVisible = true;
                progress2.IsVisible = false;
                IconViewNameUkLoad.IsVisible = false;
                FormattedString formatted = new FormattedString();
                formatted.Spans.Add(new Span
                {
                    Text = "Возникли проблемы? ",
                    TextColor = Color.Black,
                    FontAttributes = FontAttributes.Bold,
                    FontSize = 13
                });
                formatted.Spans.Add(new Span
                {
                    Text = "Напишите нам.",
                    TextColor = hex,
                    FontSize = 13,
                    TextDecorations = TextDecorations.Underline
                });
                LabelTech.FormattedText = formatted;
                BindingContext = this;
            }
            else
            {
                    await DisplayAlert("Ошибка", Settings.MobileSettings.Error, "OK");
                    getSettings();

                // BtnLogin.IsEnabled = false;
            }

            if (!Settings.MobileSettings.useDispatcherAuth)
                LabelSotr.IsVisible = false;
            if (Settings.ConstAuth)
            {
                Settings.ConstAuth = true;
                EntryLabel.Text = "Вход для сотрудника";
                LabelSotr.Text = "Вход для жителя";
                LabelPhone.Text = "Логин";
                RegStackLayout.IsVisible = false;
                EntryLogin.IsVisible = false;
                EntryLoginConst.IsVisible = true;
                EntryPass.IsVisible = false;
                EntryPassConst.IsVisible = true;
                IconViewLogin.Source = "ic_fio_reg";
            }
            else
            {
                Settings.ConstAuth = false;
                EntryLabel.Text = "Вход";
                LabelSotr.Text = "Вход для сотрудника";
                LabelPhone.Text = "Телефон";
                RegStackLayout.IsVisible = true;
                EntryLogin.IsVisible = true;
                EntryLoginConst.IsVisible = false;
                EntryPass.IsVisible = true;
                EntryPassConst.IsVisible = false;
                IconViewLogin.Source = "ic_phone_login";
            }
        }

        private async void ChoiceAuth(object sender, EventArgs e)
        {
            if (Settings.ConstAuth)
            {
                Settings.ConstAuth = false;
                EntryLabel.Text = "Вход";
                LabelSotr.Text = "Вход для сотрудника";
                LabelPhone.Text = "Телефон";
                RegStackLayout.IsVisible = true;
                EntryLogin.IsVisible = true;
                EntryLoginConst.IsVisible = false;
                EntryPass.IsVisible = true;
                EntryPassConst.IsVisible = false;
                IconViewLogin.Source = "ic_phone_login";
            }
            else
            {
                Settings.ConstAuth = true;
                EntryLabel.Text = "Вход для сотрудника";
                LabelSotr.Text = "Вход для жителя";
                LabelPhone.Text = "Логин";
                RegStackLayout.IsVisible = false;
                EntryLogin.IsVisible = false;
                EntryLoginConst.IsVisible = true;
                EntryPass.IsVisible = false;
                EntryPassConst.IsVisible = true;
                IconViewLogin.Source = "ic_fio_reg";
            }
        }

        private async void TechSend(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new TechSendPage(false));
        }

        private async void ButtonClick(object sender, EventArgs e)
        {
            if (Settings.ConstAuth)
            {
                LoginDispatcher(EntryLoginConst.Text, EntryPassConst.Text);
            }
            else
            {
                Login(EntryLogin.Text, EntryPass.Text);
            }            
        }

        void permiss()
        {
            
        }
        
        public async void Login(string loginAuth, string pass)
        {
            progress.IsVisible = true;
            FrameBtnLogin.IsVisible = false;

            var replace = loginAuth
                .Replace("+", "")
                .Replace(" ", "")
                .Replace("(", "")
                .Replace(")", "")
                .Replace("-", "");

            if (!replace.Equals("") && !pass.Equals(""))
            {
                if (replace.Length < 11)
                {
                    await DisplayAlert("Ошибка", "Номер телефона необходимо ввести в формате: +7 (ХХХ) ХХХ-ХХХХ", "OK");
                    progress.IsVisible = false;
                    FrameBtnLogin.IsVisible = true;
                    return;
                }

                LoginResult login = await server.Login(replace, pass);
                if (login.Error == null)
                {
                    // await DisplayAlert("Успешно", login.ToString(), "OK");
                    Settings.Person = login;
                    // Settings.EventBlockData = await server.GetEventBlockData();
                    ItemsList<NamedValue> result = await server.GetRequestsTypes();
                    Settings.TypeApp = result.Data;
                    Preferences.Set("login", replace);
                    Preferences.Set("pass", pass);
                    Preferences.Set("constAuth", false);
                    
                    await Navigation.PushModalAsync(new BottomNavigationPage());
                }
                else
                {
                    if (login.Error.ToLower().Contains("unauthorized"))
                    {
                        await DisplayAlert("Ошибка", "Пользователь не найден", "OK");
                    }
                    else
                    {
                        await DisplayAlert("Ошибка", login.Error, "OK");
                    }
                }
            }
            else
            {
                await DisplayAlert("Ошибка", "Заполните пустые поля", "OK");
            }

            progress.IsVisible = false;
            FrameBtnLogin.IsVisible = true;
        }

        public async void LoginDispatcher(string loginAuth, string pass)
        {
            progress.IsVisible = true;
            FrameBtnLogin.IsVisible = false;

            var replace = loginAuth
                .Replace("+", "")
                .Replace(" ", "")
                .Replace("(", "")
                .Replace(")", "")
                .Replace("-", "");

            if (!replace.Equals("") && !pass.Equals(""))
            {

                LoginResult login = await server.LoginDispatcher(replace, pass);
                if (login.Error == null)
                {
                    App.isCons = true;
                    // await DisplayAlert("Успешно", login.ToString(), "OK");
                    Settings.Person = login;
                    //Settings.EventBlockData = await server.GetEventBlockData();
                    ItemsList<NamedValue> result = await server.GetRequestsTypesConst();
                    Settings.TypeApp = result.Data;
                    Preferences.Set("loginConst", replace);
                    Preferences.Set("passConst", pass);
                    Preferences.Set("constAuth", true);
                    await Navigation.PushModalAsync(new BottomNavigationConstPage());
                }
                else
                {
                    if (login.Error.ToLower().Contains("unauthorized"))
                    {
                        await DisplayAlert("Ошибка", "Пользователь не найден", "OK");
                    }
                    else
                    {
                        await DisplayAlert("Ошибка", login.Error, "OK");
                    }
                }
            }
            else
            {
                await DisplayAlert("Ошибка", "Заполните пустые поля", "OK");
            }

            progress.IsVisible = false;
            FrameBtnLogin.IsVisible = true;
        }
    }
}