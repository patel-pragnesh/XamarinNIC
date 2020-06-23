using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamarinJKH.Server.RequestModel;
using xamarinJKH.Utils;

namespace xamarinJKH.Pays
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CostPage : ContentPage
    {
        private AccountAccountingInfo account { get; set; }

        private List<AccountAccountingInfo> Accounts { get; set; }

        public CostPage(AccountAccountingInfo account, List<AccountAccountingInfo> accounts)
        {
            this.account = account;
            Accounts = accounts;
            InitializeComponent();
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    BackgroundColor = Color.White;
                    ImageTop.Margin = new Thickness(0, 0, 0, 0);
                    StackLayout.Margin = new Thickness(0, 33, 0, 0);
                    IconViewNameUk.Margin = new Thickness(0, 33, 0, 0);
                    break;
                case Device.Android:
                    double or = Math.Round(((double) App.ScreenWidth / (double) App.ScreenHeight), 2);
                    if (Math.Abs(or - 0.5) < 0.02)
                    {
                        ScrollViewContainer.Margin = new Thickness(0, 0, 0, -120);
                        BackStackLayout.Margin = new Thickness(-5, 15, 0, 0);
                    }

                    break;
                default:
                    break;
            }

            NavigationPage.SetHasNavigationBar(this, false);
            var backClick = new TapGestureRecognizer();
            backClick.Tapped += async (s, e) => { _ = await Navigation.PopAsync(); };
            BackStackLayout.GestureRecognizers.Add(backClick);
            var openSaldos = new TapGestureRecognizer();
            openSaldos.Tapped += async (s, e) => { await Navigation.PushAsync(new SaldosPage(Accounts)); };
            FrameBtnSaldos.GestureRecognizers.Add(openSaldos);
            var openHistory = new TapGestureRecognizer();
            openHistory.Tapped += async (s, e) => { await Navigation.PushAsync(new HistoryPayedPage(Accounts)); };
            FrameBtnHistory.GestureRecognizers.Add(openHistory);
            BindingContext = new AccountingInfoModel()
            {
                AllAcc = Accounts,
                hex = Color.FromHex(Settings.MobileSettings.color)
            };
            SetText();
        }

        void SetText()
        {
            UkName.Text = Settings.MobileSettings.main_name;
            LabelPhone.Text = "+" + Settings.Person.Phone;
            Picker.Title = account.Ident;
            IconViewUslugi.Foreground = Color.FromHex(Settings.MobileSettings.color);
            Labelseparator.BackgroundColor = Color.FromHex(Settings.MobileSettings.color);
            FrameBtnLogin.BackgroundColor = Color.FromHex(Settings.MobileSettings.color);
            LabelSaldos.TextColor = Color.FromHex(Settings.MobileSettings.color);
            LabelHistory.TextColor = Color.FromHex(Settings.MobileSettings.color);
            FrameBtnHistory.BorderColor = Color.FromHex(Settings.MobileSettings.color);
            FrameBtnSaldos.BorderColor = Color.FromHex(Settings.MobileSettings.color);
            SetPays();
        }

        void SetPays()
        {
            EntrySum.Text = account.Sum.ToString();
            FormattedString formatted = new FormattedString();

            formatted.Spans.Add(new Span
            {
                Text = "Итого: ",
                FontSize = 17,
                TextColor = Color.Black
            });
            formatted.Spans.Add(new Span
            {
                Text = EntrySum.Text,
                FontSize = 20,
                TextColor = Color.FromHex(Settings.MobileSettings.color),
                FontAttributes = FontAttributes.Bold
            });
            formatted.Spans.Add(new Span
            {
                Text = " руб.",
                FontSize = 15,
                TextColor = Color.FromHex("#777777")
            });
            LabelTotal.FormattedText = formatted;
            String[] month = account.DebtActualDate.Split('.');
            formatted = new FormattedString();

            formatted.Spans.Add(new Span
            {
                Text = "Оплата производится по квитанции за ",
                FontSize = 12,
                TextColor = Color.Black
            });
            formatted.Spans.Add(new Span
            {
                Text = Settings.months[Int32.Parse(month[1]) - 1] + " " + month[2],
                FontSize = 12,
                TextColor = Color.Black,
                FontAttributes = FontAttributes.Bold
            });
            LabelMonth.FormattedText = formatted;
            Picker.Title = account.Ident;
        }

        private void picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            var identLength = Settings.Person.Accounts[Picker.SelectedIndex].Ident.Length;
            if (identLength < 6)
            {
                Picker.WidthRequest = identLength * 9;
            }

            account = Accounts[Picker.SelectedIndex];
            SetPays();
        }

        private void Entry_Completed(object sender, EventArgs e)
        {
        }

        private void EntrySum_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            FormattedString formatted = new FormattedString();
            

            string sumText = EntrySum.Text.Equals("") ? "0" : EntrySum.Text;

            formatted.Spans.Add(new Span
            {
                Text = "Итого: ",
                FontSize = 17,
                TextColor = Color.Black
            });

            formatted.Spans.Add(new Span
            {
                Text = sumText,
                FontSize = 20,
                TextColor = Color.FromHex(Settings.MobileSettings.color),
                FontAttributes = FontAttributes.Bold
            });
            formatted.Spans.Add(new Span
            {
                Text = " руб.",
                FontSize = 15,
                TextColor = Color.Gray
            });
            LabelTotal.FormattedText = formatted;
        }

        private async void openSaldo(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SaldosPage(Accounts));
        }

        private async void OpenHistory(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new HistoryPayedPage(Accounts));
        }

        private async void Pay(object sender, EventArgs e)
        {
            string sumText = EntrySum.Text;
            if (!sumText.Equals("") && !sumText.Equals("0")&& !sumText.Equals("-"))
            {
                decimal sum = Decimal.Parse(sumText);
                if (sum > 0)
                {
                    await Navigation.PushAsync(new PayServicePage(account.Ident, sum));
                }
                else
                {
                    await DisplayAlert("Ошибка", "Имеется переплата по л.с", "OK");
                }
            }
            else
            {
                await DisplayAlert("Ошибка", "Введите сумму", "OK");
            }
        }
    }
}