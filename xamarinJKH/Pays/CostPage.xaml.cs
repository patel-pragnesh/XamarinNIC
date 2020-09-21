using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.Messaging;
using Rg.Plugins.Popup.Services;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PancakeView;
using Xamarin.Forms.Xaml;
using xamarinJKH.DialogViews;
using xamarinJKH.InterfacesIntegration;
using xamarinJKH.Server;
using xamarinJKH.Server.RequestModel;
using xamarinJKH.Tech;
using xamarinJKH.Utils;

namespace xamarinJKH.Pays
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CostPage : ContentPage
    {
        private AccountAccountingInfo account { get; set; }
        RestClientMP server = new RestClientMP();
        private List<AccountAccountingInfo> Accounts { get; set; }
        private bool isComission = false;

        public CostPage(AccountAccountingInfo account, List<AccountAccountingInfo> accounts)
        {
            this.account = account;
            Accounts = accounts;
            InitializeComponent();
            var techSend = new TapGestureRecognizer();
            techSend.Tapped += async (s, e) => {     await PopupNavigation.Instance.PushAsync(new TechDialog()); };
            LabelTech.GestureRecognizers.Add(techSend);
            var call = new TapGestureRecognizer();
            call.Tapped += async (s, e) =>
            {
                if (Settings.Person.Phone != null)
                {
                    IPhoneCallTask phoneDialer;
                    phoneDialer = CrossMessaging.Current.PhoneDialer;
                    if (phoneDialer.CanMakePhoneCall && string.IsNullOrWhiteSpace(Settings.Person.companyPhone)) 
                        phoneDialer.MakePhoneCall(System.Text.RegularExpressions.Regex.Replace(Settings.Person.companyPhone, "[^+0-9]", ""));
                }

            
            };
            LabelPhone.GestureRecognizers.Add(call);

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    int statusBarHeight = DependencyService.Get<IStatusBar>().GetHeight();
                    Pancake.Padding = new Thickness(0, statusBarHeight, 0, 0);
                    if (Xamarin.Essentials.DeviceDisplay.MainDisplayInfo.Width < 700)
                    {
                        LabelHistory.FontSize = 10;
                        LabelSaldos.FontSize = 10;
                    }
                    IconViewSaldos.Margin = new Thickness(0, 0, 5, 0);
                    break;
                default:
                    break;
            }

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    //BackgroundColor = Color.White;
                    // ImageTop.Margin = new Thickness(0, 0, 0, 0);
                    // StackLayout.Margin = new Thickness(0, 33, 0, 0);
                    // IconViewNameUk.Margin = new Thickness(0, 33, 0, 0);
                    break;
                case Device.Android:
                    double or = Math.Round(((double) App.ScreenWidth / (double) App.ScreenHeight), 2);
                    if (Math.Abs(or - 0.5) < 0.02)
                    {
                        // ScrollViewContainer.Margin = new Thickness(0, 0, 0, -120);
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
            LabelPhone.Text =  "+" + Settings.Person.companyPhone.Replace("+","");
            Picker.Title = account.Ident;
            IconViewUslugi.Foreground = Color.FromHex(Settings.MobileSettings.color);
            Labelseparator.BackgroundColor = Color.FromHex(Settings.MobileSettings.color);
            FrameBtnLogin.BackgroundColor = Color.FromHex(Settings.MobileSettings.color);
            LabelSaldos.TextColor = Color.FromHex(Settings.MobileSettings.color);
            LabelHistory.TextColor = Color.FromHex(Settings.MobileSettings.color);
            FrameBtnHistory.BorderColor = Color.FromHex(Settings.MobileSettings.color);
            FrameBtnSaldos.BorderColor = Color.FromHex(Settings.MobileSettings.color);
            Color hexColor = (Color) Application.Current.Resources["MainColor"];
            IconViewLogin.SetAppThemeColor(IconView.ForegroundProperty, hexColor, Color.White);
            IconViewTech.SetAppThemeColor(IconView.ForegroundProperty, hexColor, Color.White);
            Pancake.SetAppThemeColor(PancakeView.BorderColorProperty, hexColor, Color.Transparent);
            PancakeViewIcon.SetAppThemeColor(PancakeView.BorderColorProperty, hexColor, Color.Transparent);if (Device.RuntimePlatform == Device.iOS){ if (AppInfo.PackageName == "rom.best.saburovo" || AppInfo.PackageName == "sys_rom.ru.tsg_saburovo"){PancakeViewIcon.Padding = new Thickness(0);}}
            GoodsLayot.SetAppThemeColor(PancakeView.BorderColorProperty, hexColor, Color.Transparent);
            LabelTech.SetAppThemeColor(Label.TextColorProperty, hexColor, Color.White);
            Frame.SetAppThemeColor(Xamarin.Forms.Frame.BorderColorProperty, hexColor, Color.White);
            
            SetPays();
        }

        async void SetPays()
        {
            EntrySum.Text = account.Sum.ToString();
            FormattedString formatted = new FormattedString();
            ComissionModel result = await server.GetSumWithComission(account.Sum.ToString());
            string totalSum = EntrySum.Text;
            if (result.Error == null && result.Comission != 0)
            {
                isComission = true;
                LabelCommision.Text = $"{AppResources.Commision} " + result.Comission + $" {AppResources.Currency}";
                totalSum = result.TotalSum.ToString();
            }

            LayoutInsurance.IsVisible = account.InsuranceSum != 0;
            
            LabelInsurance.Text = AppResources.InsuranceText.Replace("111", account.InsuranceSum.ToString());
            formatted.Spans.Add(new Span
            {
                Text = $"{AppResources.Total}: ",
                FontSize = 17,
                TextColor = Color.Black
            });
            formatted.Spans.Add(new Span
            {
                Text = totalSum,
                FontSize = 20,
                TextColor = Color.FromHex(Settings.MobileSettings.color),
                FontAttributes = FontAttributes.Bold
            });
            formatted.Spans.Add(new Span
            {
                Text = $" {AppResources.Currency}",
                FontSize = 15,
                TextColor = Color.FromHex("#777777")
            });
            LabelTotal.FormattedText = formatted;
            String[] month = account.DebtActualDate.Split('.');
            formatted = new FormattedString();

            formatted.Spans.Add(new Span
            {
                Text = $"{AppResources.PaymentOf} ",
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
            // var identLength = Settings.Person.Accounts[Picker.SelectedIndex].Ident.Length;
            // if (identLength < 6)
            // {
            //     Picker.WidthRequest = identLength * 9;
            // }

            account = Accounts[Picker.SelectedIndex];
            SetPays();
        }

        private void Entry_Completed(object sender, EventArgs e)
        {
        }

        private async void EntrySum_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            await SetSumPay();
        }

        private async Task SetSumPay()
        {
            FormattedString formatted = new FormattedString();


            string sumText = EntrySum.Text.Equals("") ? "0" : EntrySum.Text;

            decimal totalSum = Decimal.Parse(sumText);

            // if (isComission)
            // {
            if (SwitchInsurance.IsToggled && SwitchInsurance.IsVisible)
            {
                totalSum += account.InsuranceSum;
            }
            ComissionModel result = await server.GetSumWithComission(totalSum.ToString());
            if (result.Error == null && !result.Comission.Equals("0"))
            {
                isComission = true;
                LabelCommision.Text = $"{AppResources.Commision} " + result.Comission + $" {AppResources.Currency}";
                totalSum = result.TotalSum;
            }
            // }

           
            
            formatted.Spans.Add(new Span
            {
                Text = $"{AppResources.Total}: ",
                FontSize = 17,
                TextColor = Color.Black
            });

            formatted.Spans.Add(new Span
            {
                Text = totalSum.ToString(),
                FontSize = 20,
                TextColor = Color.FromHex(Settings.MobileSettings.color),
                FontAttributes = FontAttributes.Bold
            });
            formatted.Spans.Add(new Span
            {
                Text = $" {AppResources.Currency}",
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
            
            if (!sumText.Equals("") && !sumText.Equals("0") && !sumText.Equals("-"))
            {
                decimal sumPay = Decimal.Parse(sumText);
                if (sumPay < 1 && sumPay > 0)
                {
                    await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorEnterSumOne, "OK");
                    return;
                }
                decimal sum = Decimal.Parse(sumText);
                if (sum > 0)
                {
                    await Navigation.PushAsync(new PayServicePage(account.Ident, sum, null, SwitchInsurance.IsToggled && SwitchInsurance.IsVisible));
                }
                else
                {
                    await DisplayAlert(AppResources.ErrorTitle,AppResources.ErrorOverpay, "OK");
                }
            }
            else
            {
                await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorEnterSum, "OK");
            }
        }

        private async void SwitchLogin_OnToggled(object sender, ToggledEventArgs e)
        {
            await SetSumPay();
        }
    }
}