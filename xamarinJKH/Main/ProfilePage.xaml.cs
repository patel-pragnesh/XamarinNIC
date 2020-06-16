using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.Xaml;
using xamarinJKH.InterfacesIntegration;
using xamarinJKH.Server;
using xamarinJKH.Server.RequestModel;
using xamarinJKH.Utils;

namespace xamarinJKH.Main
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfilePage : ContentPage
    {
        private LoginResult Person = new LoginResult();
        private RestClientMP _server = new RestClientMP();
        public bool isSave  {get;set;}
        public ProfilePage()
        {
            InitializeComponent();
            isSave = Preferences.Get("isPass", false);
            NavigationPage.SetHasNavigationBar(this, false);
            var exitClick = new TapGestureRecognizer();
            exitClick.Tapped += async (s, e) =>
            {
                _ = await Navigation.PopModalAsync();
            };
            FrameBtnExit.GestureRecognizers.Add(exitClick);
            var saveClick = new TapGestureRecognizer();
            saveClick.Tapped += async (s, e) => { ButtonClick(FrameBtnLogin, null); };
            FrameBtnLogin.GestureRecognizers.Add(saveClick);
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    BackgroundColor = Color.White;
                    ImageFon.Margin = new Thickness(0, 0, 0, 0);
                    StackLayout.Margin = new Thickness(0, 33, 0, 0);
                    IconViewNameUk.Margin = new Thickness(0, 33, 0, 0);
                    break;
                case Device.Android:
                    double or = Math.Round(((double) App.ScreenWidth / (double) App.ScreenHeight), 2);
                    if (Math.Abs(or - 0.5) < 0.02)
                    {
                        ScrollViewContainer.Margin = new Thickness(10, -150, 10, 0);
                        BackStackLayout.Margin = new Thickness(5, 25, 0, 0);
                    }
                    break;
                default:
                    break;
            }
            SetText();
            SetColor();
            EntryFio.Text = Settings.Person.FIO;
            EntryEmail.Text = Settings.Person.Email;
            BindingContext = this;
        }
        
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
                CommonResult result = await _server.UpdateProfile(email, fio);
                if (result.Error == null)
                {
                    Console.WriteLine(result.ToString());
                    Console.WriteLine("Отправлено");
                    await DisplayAlert("", "Ваши данные успешно сохранены", "OK");             
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
                        await DisplayAlert("ОШИБКА", result.Error, "OK");
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
                    await DisplayAlert("", "Заполните поля ФИО и E-mail", "OK");
                }else if (fio == "")
                {
                    await DisplayAlert("", "Заполните поле ФИО", "OK");
                }else if (email == "")
                {
                    await DisplayAlert("", "Заполните поле E-mail", "OK");
                }
            }
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
            IconViewSave.Foreground = Color.White;
            // IconViewNameUk.Foreground = hexColor;
            IconViewFio.Foreground = hexColor;
            IconViewEmail.Foreground = hexColor;
            IconViewExit.Foreground = hexColor;

            FrameBtnExit.BackgroundColor = Color.White;
            FrameBtnExit.BorderColor = hexColor;
            FrameBtnLogin.BackgroundColor = hexColor;
            LabelseparatorEmail.BackgroundColor = hexColor;
            LabelseparatorFio.BackgroundColor = hexColor;
            SwitchSavePass.OnColor = hexColor;
            SwitchSavePass.ThumbColor = Color.Black;
            BtnExit.TextColor = hexColor;
            progress.Color = hexColor;

            
        }

        private void SwitchSavePass_OnPropertyChanged(object sender, ToggledEventArgs toggledEventArgs)
        {
            Preferences.Set("isPass",isSave);
        }
     
    }
}