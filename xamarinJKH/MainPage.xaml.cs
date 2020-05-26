using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xamarinJKH.Navigation;
using xamarinJKH.Server;
using xamarinJKH.Server.RequestModel;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.PlatformConfiguration;
using xamarinJKH.Main;
using xamarinJKH.Utils;

namespace xamarinJKH
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        private RestClientMP server = new RestClientMP();

        public MainPage()
        {
            InitializeComponent();
            getSettings();
            var startRegForm = new TapGestureRecognizer();
            startRegForm.Tapped += async (s, e) => { await Navigation.PushModalAsync(new RegistrForm(this)); };
            RegistLabel.GestureRecognizers.Add(startRegForm);

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
            Login("79237173372", "123");
            // Login("79831727567", "123");
            // Login("79261270258", "19871987");
            // Login("79261937745", "123");
        }

        private async void getSettings()
        {
            Settings.MobileSettings = await server.MobileAppSettings("2.303", "1");
            if (Settings.MobileSettings.Error == null)
            {
                UkName.Text = Settings.MobileSettings.main_name;

                IconViewLogin.Foreground = Color.FromHex(Settings.MobileSettings.color);
                IconViewPass.Foreground = Color.FromHex(Settings.MobileSettings.color);
                ImageClosePass.Foreground = Color.FromHex(Settings.MobileSettings.color);

                FrameBtnLogin.BackgroundColor = Color.FromHex(Settings.MobileSettings.color);
                LabelseparatorPass.BackgroundColor = Color.FromHex(Settings.MobileSettings.color);
                LabelseparatorLogin.BackgroundColor = Color.FromHex(Settings.MobileSettings.color);
                SwitchLogin.OnColor = Color.FromHex(Settings.MobileSettings.color);
                SwitchLogin.ThumbColor = Color.White;
                RegistLabel.TextColor = Color.FromHex(Settings.MobileSettings.color);
                progress.Color = Color.FromHex(Settings.MobileSettings.color);
                Color.SetAccent(Color.FromHex(Settings.MobileSettings.color));

                StackLayoutContent.IsVisible = true;
                progress2.IsVisible = false;
                IconViewNameUkLoad.IsVisible = false;
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
                LoginResult login = await server.Login(replace, pass);
                if (login.Error == null)
                {
                    // await DisplayAlert("Успешно", login.ToString(), "OK");
                    Settings.Person = login;
                    Settings.EventBlockData = await server.GetEventBlockData();
                    ItemsList<NamedValue> result = await server.GetRequestsTypes();
                    Settings.TypeApp = result.Data;
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