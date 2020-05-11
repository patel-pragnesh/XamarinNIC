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

namespace xamarinJKH
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            var startRegForm = new TapGestureRecognizer();
            startRegForm.Tapped += async (s, e) => { await Navigation.PushModalAsync(new RegistrForm()); };
            RegistLabel.GestureRecognizers.Add(startRegForm);

            var forgetPasswordVisible = new TapGestureRecognizer();
            forgetPasswordVisible.Tapped += async (s, e) =>
            {
                EntryPass.IsPassword = !EntryPass.IsPassword;
                if (EntryPass.IsPassword)
                {
                    ImageClosePass.Foreground = Color.Red;
                }
                else
                {
                    ImageClosePass.Foreground = Color.DarkGray;
                }
            };
            ImageClosePass.GestureRecognizers.Add(forgetPasswordVisible);

            EntryLogin.Text = "";
            EntryPass.Text = "";
        }

        private async void ButtonClick(object sender, EventArgs e)
        {
            progress.IsVisible = true;
            FrameBtnLogin.IsVisible = false;

            RestClientMP server = new RestClientMP();
            var replace = EntryLogin.Text
                .Replace("+", "")
                .Replace(" ", "")
                .Replace("(", "")
                .Replace(")", "")
                .Replace("-", "");
            var entryPassText = EntryPass.Text.ToString();
            if (!replace.Equals("") && !entryPassText.Equals(""))
            {
                LoginResult login = await server.Login(replace, entryPassText);
                await DisplayAlert("Успешно", login.ToString(), "OK");
            }
            else
            {
                await DisplayAlert("Ошибка", "Заполните пустые поля", "OK");
            }
            progress.IsVisible = false;
            FrameBtnLogin.IsVisible = true;
        }

        // private async Task ButtonClick(object sender, EventArgs e)
        // {
        //     BtnLogin.Text = "Войдено";
        //     await Navigation.PushModalAsync(new RegistrForm());
        // }

        private static async Task StartRegistrForm()
        {
            await NavigationService.NavigateToAsync(new RegistrForm());
        }
    }
}
