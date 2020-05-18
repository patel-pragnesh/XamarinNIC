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
            NavigationPage.SetHasNavigationBar(this, false);
            var backClick = new TapGestureRecognizer();
            backClick.Tapped += async (s, e) => { _ = await Navigation.PopAsync(); };
            BackStackLayout.GestureRecognizers.Add(backClick);
            var openSaldos = new TapGestureRecognizer();
            openSaldos.Tapped += async (s, e) => { await Navigation.PushAsync(new SaldosPage(Accounts)); };
            FrameBtnSaldos.GestureRecognizers.Add(openSaldos);
            BindingContext = new AccountingInfoModel()
            {
                AllAcc = Accounts
            };
            SetText();
        }

        void SetText()
        {
            UkName.Text = Settings.MobileSettings.main_name;
            LabelPhone.Text = "+" + Settings.Person.Phone;
            Picker.Title = account.Ident;
            Picker.FontSize = 15;
            Picker.TextColor = Color.FromHex(Settings.MobileSettings.color);
            Picker.TitleColor = Color.FromHex(Settings.MobileSettings.color);
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
                FontSize = 15,
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
                TextColor = Color.Gray
            });
            LabelTotal.FormattedText = formatted;
            String[] month = account.DebtActualDate.Split('.');
            formatted = new FormattedString();

            formatted.Spans.Add(new Span
            {
                Text = "Оплата производится по квитанции за ",
                FontSize = 15,
                TextColor = Color.Black
            });
            formatted.Spans.Add(new Span
            {
                Text = Settings.months[Int32.Parse(month[1]) - 1] + " " + month[2],
                FontSize = 15,
                TextColor = Color.FromHex(Settings.MobileSettings.color),
                FontAttributes = FontAttributes.Bold
            });
            LabelMonth.FormattedText = formatted;
            Picker.Title = account.Ident;
        }

        private void picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            account = Accounts[Picker.SelectedIndex];
            SetPays();
        }

        private void Entry_Completed(object sender, EventArgs e)
        {
            
        }

        private void EntrySum_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            FormattedString formatted = new FormattedString();

            formatted.Spans.Add(new Span
            {
                Text = "Итого: ",
                FontSize = 20,
                TextColor = Color.Black
            });
            formatted.Spans.Add(new Span
            {
                Text = EntrySum.Text,
                FontSize = 25,
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
    }
}