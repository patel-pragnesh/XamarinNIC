﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AiForms.Dialogs;
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

        bool TimerStart = false;
        int TimerTime = 59;

        private MainPage _mainPage;

        public Color hex { get; set; }

        private bool isNext = false;

        public RegistrForm(MainPage mainPage)
        {
            InitializeComponent();
            setColors();
            hex = Color.FromHex(Settings.MobileSettings.color);
            this.BindingContext = this;
            
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    BackgroundColor = Color.White;
                    BackStackLayout.Margin = new Thickness(10, 40, 0, 0);
                    RegLbl.Margin = new Thickness(20, 60, 0, 0);
                    break;
                case Device.Android:
                    double or = Math.Round(((double) App.ScreenWidth / (double) App.ScreenHeight), 2);
                    if (Math.Abs(or - 0.5) < 0.02)
                    {
                        BackStackLayout.Margin = new Thickness(10, -150, 0, 0);
                    }
                    break;
                default:
                    break;
            }
            NavigationPage.SetHasNavigationBar(this, false);
            var backClick = new TapGestureRecognizer();
            backClick.Tapped += BackClick;
            BackStackLayout.GestureRecognizers.Add(backClick);
            FrameTimer.IsVisible = false;
            var nextReg = new TapGestureRecognizer();
            nextReg.Tapped += async (s, e) => { FirstStepReg(); };
            FrameBtnLogin.GestureRecognizers.Add(nextReg);

            var nextReg2 = new TapGestureRecognizer();
            nextReg2.Tapped += async (s, e) =>
            {
                if (isNext)
                    SecondStep();
            };
            FrameBtnNextTwo.GestureRecognizers.Add(nextReg2); 
            
            var finalReg = new TapGestureRecognizer();
            finalReg.Tapped += async (s, e) =>
            {
                FinalRegg();
            };
            FrameBtnRegFinal.GestureRecognizers.Add(finalReg);

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

            IconViewLogin.Foreground = Color.FromHex(Settings.MobileSettings.color);
            IconViewFio.Foreground = Color.FromHex(Settings.MobileSettings.color);
            IconViewCode.Foreground = Color.FromHex(Settings.MobileSettings.color);
            IconViewPassNew.Foreground = Color.FromHex(Settings.MobileSettings.color);
            ImageClosePass.Foreground = Color.FromHex(Settings.MobileSettings.color);
            IconViewConfirmPass.Foreground = Color.FromHex(Settings.MobileSettings.color);

            SwitchConsent.OnColor = Color.FromHex(Settings.MobileSettings.color);
            FrameBtnLogin.BackgroundColor = Color.FromHex(Settings.MobileSettings.color);
            FrameBtnReg.BackgroundColor = Color.FromHex(Settings.MobileSettings.color);
            FrameTimer.BackgroundColor = Color.FromHex(Settings.MobileSettings.color);
            // FrameBtnNextTwo.BackgroundColor = Color.FromHex(Settings.MobileSettings.color);
            FrameBtnRegFinal.BackgroundColor = Color.FromHex(Settings.MobileSettings.color);
            FrameTech.BorderColor = Color.FromHex(Settings.MobileSettings.color);
            SwitchConsent.ThumbColor = Color.Black;
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
            string phone = EntryPhone.Text
                .Replace("+", "")
                .Replace(" ", "")
                .Replace("(", "")
                .Replace(")", "")
                .Replace("-", "");
            string fio = EntryFio.Text;
            string date = DatePicker.Date.ToString("dd.MM.yyyy");

            if (phone.Equals(""))
            {
                await DisplayAlert("Ошибка", "Заполните поле номер телефона", "OK");
            }else if (phone.Length < 11)
            {
                await DisplayAlert("Ошибка", "Номер телефона необходимо ввести в формате: +7 (ХХХ) ХХХ-ХХХХ", "OK");
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
                Person.Phone = phone;
                Person.FIO = fio;
                Person.Birthday = date;
                FormattedString formatted = new FormattedString();
                formatted.Spans.Add(new Span
                {
                    Text =
                        "Чтобы получить код доступа нажмите «Запросить звонок с кодом».\n    Вам позвонит робот на номер ",
                });
                formatted.Spans.Add(new Span
                {
                    Text = EntryPhone.Text,
                    TextColor = Color.Black,
                });
                formatted.Spans.Add(new Span
                {
                    Text = " и сообщит код",
                });

                LabelTitleRequestCode.FormattedText = formatted;
                StepsImage.Source = ImageSource.FromFile("ic_steps_two");
                RegistrationFrameStep1.IsVisible = false;
                RegistrationFrameStep2.IsVisible = true;
                LabelSteps.Text = "Шаг 2";
                if (Settings.TimerStart)
                {
                    Settings.TimerStart = false;
                    TimerStart = true;
                    TimerTime = Settings.TimerTime;
                    LabelTimer.Text = "ЗАПРОСИТЬ ПОВТОРНЫЙ ЗВОНОК МОЖНО БУДЕТ ЧЕРЕЗ: " + TimerTime + " секунд";
                    FrameBtnReg.IsVisible = false;
                    FrameTimer.IsVisible = true;                    
                    Device.StartTimer(TimeSpan.FromSeconds(1), OnTimerTick);
                }
            }
        }

        private async void BackClick(object sender, EventArgs e)
        {
            if (TimerStart)
            {
                TimerStart = false;
                Settings.TimerStart = true;
                Settings.TimerTime = TimerTime;
                Device.StartTimer(TimeSpan.FromSeconds(1), Settings.OnTimerTick);
            }
            _ = await Navigation.PopModalAsync();
        }

        private async void NextTwoReg(object sender, EventArgs e)
        {
            SecondStep();
        }

        private async void SecondStep()
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
                FrameBtnNextTwo.BackgroundColor = hex;
                isNext = true;
            }
            else
            {
                FrameBtnNextTwo.BackgroundColor = Color.FromHex("#CFCFCF");
                isNext = false;
            }
        }

        private bool OnTimerTick()
        {
            if (TimerStart)
            {
                if (LabelTimer != null)
                {
                    LabelTimer.Text = "ЗАПРОСИТЬ ПОВТОРНЫЙ ЗВОНОК МОЖНО БУДЕТ ЧЕРЕЗ: " + TimerTime + " секунд";
                }                
                TimerTime -= 1;
                if (TimerTime < 0)
                {
                    TimerStart = false;
                    TimerTime = 59;
                    if (FrameTimer != null)
                    {
                        FrameTimer.IsVisible = false;
                        FrameBtnReg.IsVisible = true;
                    }                                        
                }
            }
            if (TimerStart == false)
            {
                if (FrameTimer != null)
                {
                    FrameTimer.IsVisible = false;
                    FrameBtnReg.IsVisible = true;
                }                
            }
            return TimerStart;
        }

        private async void RegCodeRequest(object sender, EventArgs e)
        {
            // FrameBtnReg.IsVisible = false;
            // progress.IsVisible = true;
            await Settings.StartProgressBar(title: "Подождите", opacity: 0.4);
            CommonResult result = await _server.RequestAccessCode(Person.Phone);
            if (result.Error == null)
            {
                Console.WriteLine("Отправлено");
                TimerStart = true;
                FrameBtnReg.IsVisible = false;
                FrameTimer.IsVisible = true;
                Device.StartTimer(TimeSpan.FromSeconds(1), OnTimerTick);
                if (Device.RuntimePlatform == Device.iOS)
                {
                    await DisplayAlert("", "Запрос с кодом доступа отправлен", "OK");
                }
                else
                {
                    DependencyService.Get<IMessage>().ShortAlert("Запрос с кодом доступа отправлен");
                }                
                // FrameBtnReg.IsVisible = true;
                // progress.IsVisible = false;
                Loading.Instance.Hide();
            }
            else
            {
                // FrameBtnReg.IsVisible = true;
                // progress.IsVisible = false;
                Loading.Instance.Hide();
                if (Device.RuntimePlatform == Device.iOS)
                {
                    await DisplayAlert("", result.Error, "OK");
                }
                else
                {
                    DependencyService.Get<IMessage>().ShortAlert(result.Error);
                }
            }
        }

        private async void ButtonReg(object sender, EventArgs e)
        {
            FinalRegg();
        }

        private async void FinalRegg()
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
                CommonResult result =
                    await _server.RegisterByPhone(Person.FIO, Person.Phone, pass, Person.Code, Person.Birthday);
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

        private void datePicker_DateSelected(object sender, DateChangedEventArgs e)
        {
        }
    }
}