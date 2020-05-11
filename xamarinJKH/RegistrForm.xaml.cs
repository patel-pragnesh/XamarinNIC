using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xamarinJKH.InterfacesIntegration;
using xamarinJKH.Server;
using xamarinJKH.Server.RequestModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Forms.PlatformConfiguration;
using xamarinJKH.Utils;

namespace xamarinJKH
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegistrForm : ContentPage
    {
        private LoginResult Person = new LoginResult();
        private RestClientMP _server = new RestClientMP();
        public string LoginAuth { get; set; }
        public string passAuth { get; set; }

        private MainPage _mainPage;

        public RegistrForm(MainPage mainPage)
        {
            InitializeComponent();
            setColors();
            var backClick = new TapGestureRecognizer();
            backClick.Tapped += async (s, e) => { await Navigation.PopModalAsync(); };
            BackStackLayout.GestureRecognizers.Add(backClick);
            _mainPage = mainPage;
            var passwordVisible = new TapGestureRecognizer();
            passwordVisible.Tapped += async (s, e) =>
            {
                EntryPassNew.IsPassword = !EntryPassNew.IsPassword;
                if (EntryPassNew.IsPassword)
                {
                    ImageClosePass.Foreground = Color.FromHex(Settings.MobileSettings.color);
                }
                else
                {
                    ImageClosePass.Foreground = Color.DarkSlateGray;
                }
            };
            ImageClosePass.GestureRecognizers.Add(passwordVisible);
        }

        private void NextReg(object sender, EventArgs e)
        {
            FirstStepReg();
        }

        private void setColors()
        {
            UkName.Text = Settings.MobileSettings.main_name;

            IconViewNameUk.Foreground = Color.FromHex(Settings.MobileSettings.color);
            IconViewLogin.Foreground = Color.FromHex(Settings.MobileSettings.color);
            IconViewFio.Foreground = Color.FromHex(Settings.MobileSettings.color);
            IconViewCode.Foreground = Color.FromHex(Settings.MobileSettings.color);
            IconViewPassNew.Foreground = Color.FromHex(Settings.MobileSettings.color);
            ImageClosePass.Foreground = Color.FromHex(Settings.MobileSettings.color);
            IconViewConfirmPass.Foreground = Color.FromHex(Settings.MobileSettings.color);

            SwitchConsent.OnColor = Color.FromHex(Settings.MobileSettings.color);
            FrameBtnLogin.BackgroundColor = Color.FromHex(Settings.MobileSettings.color);
            FrameBtnReg.BackgroundColor = Color.FromHex(Settings.MobileSettings.color);
            FrameBtnNextTwo.BackgroundColor = Color.FromHex(Settings.MobileSettings.color);
            FrameBtnRegFinal.BackgroundColor = Color.FromHex(Settings.MobileSettings.color);
            FrameTech.BorderColor = Color.FromHex(Settings.MobileSettings.color);
            SwitchConsent.ThumbColor = Color.White;
            BtnTech.TextColor = Color.FromHex(Settings.MobileSettings.color);
            progress.Color = Color.FromHex(Settings.MobileSettings.color);
            
            LabelseparatorPassConfirm.BackgroundColor = Color.FromHex(Settings.MobileSettings.color);
            LabelseparatorPass.BackgroundColor = Color.FromHex(Settings.MobileSettings.color);
            LabelseparatorCode.BackgroundColor = Color.FromHex(Settings.MobileSettings.color);
            LabelseparatorPhone.BackgroundColor = Color.FromHex(Settings.MobileSettings.color);
            LabelseparatorFio.BackgroundColor = Color.FromHex(Settings.MobileSettings.color);
        }

        private async void FirstStepReg()
        {
            string phone = EntryPhone.Text;
            string fio = EntryFio.Text;

            if (phone.Equals(""))
            {
                await DisplayAlert("Ошибка", "Заполните поле номер телефона", "OK");
            }
            else if (fio.Equals(""))
            {
                await DisplayAlert("Ошибка", "Заполните поле ФИО", "OK");
            }
            else if (!SwitchConsent.IsToggled)
            {
                await DisplayAlert("Ошибка", "Подтвердите согласие на обработку персональных данных", "OK");
            }
            else
            {
                Person.Phone = phone.Replace("+", "")
                    .Replace(" ", "")
                    .Replace("(", "")
                    .Replace(")", "")
                    .Replace("-", "");
                Person.FIO = fio;
                LabelTitleRequestCode.Text = string.Format(
                    "Чтобы получить код доступа нажмите «Запросить звонок с кодом».{0}Вам позвонит робот на номер {1} и сообщит код",
                    Environment.NewLine, phone);
                StepsImage.Source = ImageSource.FromFile("ic_steps_two");
                RegistrationFrameStep1.IsVisible = false;
                RegistrationFrameStep2.IsVisible = true;
                LabelSteps.Text = "Шаг 2";
            }
        }


        private async void NextTwoReg(object sender, EventArgs e)
        {
            var entryCodeText = EntryCode.Text;
            CheckResult result = await _server.RequestChechCode(Person.Phone, entryCodeText);
            if (result.IsCorrect)
            {
                StepsImage.Source = ImageSource.FromFile("ic_steps_three");
                RegistrationFrameStep2.IsVisible = false;
                RegistrationFrameStep3.IsVisible = true;
                LabelSteps.Text = "Шаг 3";
                Person.Code = entryCodeText;
            }
            else
            {
                await DisplayAlert("Ошибка", "Неверный код доступа", "OK");
            }
        }

        private void EntryCode_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!EntryCode.Text.Equals(""))
            {
                FrameBtnNextTwo.IsVisible = true;
            }
            else
            {
                FrameBtnNextTwo.IsVisible = false;
            }
        }

        private async void RegCodeRequest(object sender, EventArgs e)
        {
            FrameBtnReg.IsVisible = false;
            progress.IsVisible = true;
            CommonResult result = await _server.RequestAccessCode(Person.Phone);
            if (result.Error == null)
            {
                Console.WriteLine("Отправлено");
                DependencyService.Get<IMessage>().ShortAlert("Запрос с кодом доступа отправлен");
                FrameBtnReg.IsVisible = true;
                progress.IsVisible = false;
            }
            else
            {
                FrameBtnReg.IsVisible = true;
                progress.IsVisible = false;
                DependencyService.Get<IMessage>().ShortAlert(result.Error);
            }
        }

        private async void ButtonReg(object sender, EventArgs e)
        {
            string pass = EntryPassNew.Text;
            string passConfirm = EntryPassCommit.Text;
            if (pass.Equals(""))
            {
                await DisplayAlert("Ошибка", "Заполните поле пароль", "OK");
            }
            else if (passConfirm.Equals(""))
            {
                await DisplayAlert("Ошибка", "Заполните поле подтвердите пароль", "OK");
            }
            else if (!pass.Equals(passConfirm))
            {
                await DisplayAlert("Ошибка", "Пароли не совпадают", "OK");
            }
            else
            {
                CommonResult result = await _server.RegisterByPhone(Person.FIO, Person.Phone, pass, Person.Code);
                if (result.Error == null)
                {
                    LoginAuth = "79237173372";
                    passAuth = "qw";
                    await Navigation.PopModalAsync();
                    _mainPage.Login(Person.Phone, pass);
                }
                else
                {
                    await DisplayAlert("Ошибка", result.Error, "OK");
                }
            }
        }
    }
}