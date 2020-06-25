using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Essentials;
using xamarinJKH.Navigation;
using xamarinJKH.Server;
using xamarinJKH.Server.RequestModel;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using xamarinJKH.Main;
using xamarinJKH.MainConst;
using xamarinJKH.Utils;
using XamEffects;
using NavigationPage = Xamarin.Forms.NavigationPage;

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
            
            NavigationPage.SetHasNavigationBar(this, false);
            getSettings();
            var startRegForm = new TapGestureRecognizer();
            startRegForm.Tapped += async (s, e) => { await Navigation.PushModalAsync(new RegistrForm(this)); };
            RegistLabel.GestureRecognizers.Add(startRegForm);

            var authConst = new TapGestureRecognizer();
            authConst.Tapped += ChoiceAuth;
            LabelSotr.GestureRecognizers.Add(authConst);

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
        }

        private async void getSettings()
        {
            
            Settings.MobileSettings = await server.MobileAppSettings("2.303", "1");
            if (Settings.MobileSettings.Error == null)
            {
                if (RestClientMP.SERVER_ADDR.Contains("dgservicnew"))
                {
                    Settings.MobileSettings.main_name = "ООО \"ДОМЖИЛСЕРВИС\"";
                }
                UkName.Text = Settings.MobileSettings.main_name;

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
                    // await DisplayAlert("Успешно", login.ToString(), "OK");
                    Settings.Person = login;
                    //Settings.EventBlockData = await server.GetEventBlockData();
                    ItemsList<NamedValue> result = await server.GetRequestsTypes();
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