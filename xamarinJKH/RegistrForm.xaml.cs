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

namespace xamarinJKH
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegistrForm : ContentPage
    {
        private LoginResult Person = new LoginResult();
        private RestClientMP _server = new RestClientMP();

        public RegistrForm()
        {
            InitializeComponent();
            var backClick = new TapGestureRecognizer();
            backClick.Tapped += async (s, e) => { await Navigation.PopModalAsync(); };
            BackStackLayout.GestureRecognizers.Add(backClick);
            
            var passwordVisible = new TapGestureRecognizer();
            passwordVisible.Tapped += async (s, e) => { EntryPassNew.IsPassword = !EntryPassNew.IsPassword; };
            ImageClosePass.GestureRecognizers.Add(passwordVisible);
        }

        private void NextReg(object sender, EventArgs e)
        {
            FirstStepReg();
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


        private void NextTwoReg(object sender, EventArgs e)
        {
            StepsImage.Source = ImageSource.FromFile("ic_steps_three");
            RegistrationFrameStep2.IsVisible = false;
            RegistrationFrameStep3.IsVisible = true;
            LabelSteps.Text = "Шаг 3";
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
    }
}