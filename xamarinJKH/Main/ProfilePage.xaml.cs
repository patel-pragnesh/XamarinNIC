using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.Xaml;
using xamarinJKH.Utils;

namespace xamarinJKH.Main
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfilePage : ContentPage
    {
        public ProfilePage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    ImageTop.Margin = new Thickness(0, 0, 0, 0);
                    ImageFon.Margin = new Thickness(0, 0, 0, 0);
                    StackLayout.Margin = new Thickness(0, 26, 0, 0);
                    IconViewNameUk.Margin = new Thickness(0, 26, 0, 0);
                    break;
                case Device.Android:
                default:
                    ImageTop.Margin = new Thickness();
                    ImageFon.Margin = new Thickness();
                    break;
            }
            SetText();
            SetColor();
        }
        
        private async void ButtonClick(object sender, EventArgs e)
        {
            SaveInfoAccount(EntryFio.Text, EntryEmail.Text);
        }
        
        public async void SaveInfoAccount(string fio, string email)
        {
            progress.IsVisible = true;
            FrameBtnLogin.IsVisible = false;
            // if (!fio.Equals("") && !email.Equals(""))
            // {
            //     LoginResult login = await server.Login(replace, pass);
            //     if (login.Error == null)
            //     {
            //         // await DisplayAlert("Успешно", login.ToString(), "OK");
            //         Settings.Person = login;
            //         Settings.EventBlockData = await server.GetEventBlockData();
            //         await Navigation.PushModalAsync(new BottomNavigationPage());
            //     }
            //     else
            //     {
            //         if (login.Error.ToLower().Contains("unauthorized"))
            //         {
            //             await DisplayAlert("Ошибка", "Пользователь не найден", "OK");
            //         }
            //         else
            //         {
            //             await DisplayAlert("Ошибка", login.Error, "OK");
            //         }
            //     }
            // }
            // else
            // {
            //     await DisplayAlert("Ошибка", "Заполните пустые поля", "OK");
            // }

            progress.IsVisible = false;
            FrameBtnLogin.IsVisible = true;
        }
        
        void SetText()
        {
            UkName.Text = Settings.MobileSettings.main_name;
            LabelPhone.Text = "+" + Settings.Person.Phone;
        }
        
        void SetColor()
        {
            Color hexColor = Color.FromHex(Settings.MobileSettings.color);
            UkName.Text = Settings.MobileSettings.main_name;

            IconViewNameUk.Foreground = hexColor;
            IconViewFio.Foreground = hexColor;
            IconViewEmail.Foreground = hexColor;
            IconViewExit.Foreground = hexColor;

            FrameBtnExit.BackgroundColor = Color.White;
            FrameBtnExit.BorderColor = hexColor;
            FrameBtnLogin.BackgroundColor = hexColor;
            LabelseparatorEmail.BackgroundColor = hexColor;
            LabelseparatorFio.BackgroundColor = hexColor;
            SwitchSavePass.OnColor = hexColor;
            SwitchSavePass.ThumbColor = Color.White;
            BtnExit.TextColor = hexColor;
            progress.Color = hexColor;
        }
    }
}