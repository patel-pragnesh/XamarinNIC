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
            
            var startLogin = new TapGestureRecognizer();
            startLogin.Tapped += async (s, e) =>
            {
                Login(EntryLogin.Text, EntryPass.Text);
            };
            FrameBtnLogin.GestureRecognizers.Add(startLogin);

            var forgetPasswordVisible = new TapGestureRecognizer();
            forgetPasswordVisible.Tapped += async (s, e) =>
            {
                EntryPass.IsPassword = !EntryPass.IsPassword;
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
            EntryPass.Text = "";
            // Login("79237173372", "123");
            
            // Login("79261270258", "19871987");
            // Login("79261937745", "123");
            string login = Preferences.Get("login","" );
            string pass = Preferences.Get("pass","" );
            if (Settings.IsFirsStart && !pass.Equals("") && !login.Equals(""))
            {
              
                Login(login, pass);
                Settings.IsFirsStart = false;
                EntryLogin.Text = login;
                EntryPass.Text = pass;
            }
            
        }

        private async void getSettings()
        {
            Settings.MobileSettings = await server.MobileAppSettings("2.303", "1");
            if (Settings.MobileSettings.Error == null)
            {
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
        }

        private async void ButtonClick(object sender, EventArgs e)
        {
            Login(EntryLogin.Text, EntryPass.Text);
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
                    Settings.EventBlockData = await server.GetEventBlockData();
                    ItemsList<NamedValue> result = await server.GetRequestsTypes();
                    Settings.TypeApp = result.Data;
                    Preferences.Set("login",replace );
                    Preferences.Set("pass",pass );
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
    }
}