using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Plugin.FirebaseCrashlytics;
using Plugin.Messaging;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamarinJKH.InterfacesIntegration;
using xamarinJKH.Server;
using xamarinJKH.Server.RequestModel;
using xamarinJKH.Tech;
using xamarinJKH.Utils;
using xamarinJKH.Utils.Compatator;

namespace xamarinJKH.Pays
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HistoryPayedPage : ContentPage
    {
        public List<AccountAccountingInfo> Accounts { get; set; }
        public List<PaymentInfo> Payments { get; set; }
        public AccountAccountingInfo SelectedAcc { get; set; }

        private RestClientMP _server = new RestClientMP();
        private bool _isRefreshing = false;

        public bool IsRefreshing
        {
            get { return _isRefreshing; }
            set
            {
                _isRefreshing = value;
                OnPropertyChanged(nameof(IsRefreshing));
            }
        }

        public ICommand RefreshCommand
        {
            get
            {
                return new Command(async () =>
                {
                    IsRefreshing = true;

                    await RefreshData();

                    IsRefreshing = false;
                });
            }
        }

        private async Task RefreshData()
        {
            ItemsList<AccountAccountingInfo> info = await _server.GetAccountingInfo();
            if (info.Error == null)
            {
                Accounts = info.Data;
                additionalList.ItemsSource = null;
                additionalList.ItemsSource = Accounts[Picker.SelectedIndex].Payments;
            }
            else
            {
                await DisplayAlert("Ошибка", "Не удалось получить информацию об оплатах", "OK");
            }
        }

        public HistoryPayedPage(List<AccountAccountingInfo> accounts)
        {
            this.Accounts = accounts;
            InitializeComponent();

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    int statusBarHeight = DependencyService.Get<IStatusBar>().GetHeight();
                    Pancake.Padding = new Thickness(0, statusBarHeight, 0, 0);
                    BackgroundColor = Color.White;
                    break;
                default:
                    break;
            }

            NavigationPage.SetHasNavigationBar(this, false);
            var backClick = new TapGestureRecognizer();
            backClick.Tapped += async (s, e) => { _ = await Navigation.PopAsync(); };
            BackStackLayout.GestureRecognizers.Add(backClick);
            var call = new TapGestureRecognizer();
            call.Tapped += async (s, e) =>
            {
                if (Settings.Person.Phone != null)
                {
                    IPhoneCallTask phoneDialer;
                    phoneDialer = CrossMessaging.Current.PhoneDialer;
                    if (phoneDialer.CanMakePhoneCall)
                        phoneDialer.MakePhoneCall(Settings.Person.companyPhone);
                }
            };
            LabelPhone.GestureRecognizers.Add(call);
            var techSend = new TapGestureRecognizer();
            techSend.Tapped += async (s, e) => { await Navigation.PushAsync(new TechSendPage()); };
            LabelTech.GestureRecognizers.Add(techSend);

            SetText();
            Payments = setPays(Accounts[0]);
            SelectedAcc = Accounts[0];
            BindingContext = this;
            additionalList.Effects.Add(Effect.Resolve("MyEffects.ListViewHighlightEffect"));
        }


        List<PaymentInfo> setPays(AccountAccountingInfo accountingInfo)
        {
            List<PaymentInfo> paymentInfo = new List<PaymentInfo>(accountingInfo.Payments);
            List<MobilePayment> mobile = new List<MobilePayment>(accountingInfo.MobilePayments);

            foreach (var each in mobile)
            {
                paymentInfo.Add(new PaymentInfo()
                {
                    Date = each.Date.Split(' ')[0],
                    Ident = each.Ident,
                    Period = "Мобильный",
                    Sum = each.Sum
                });
            }
            
            HistoryPayComparable comparable = new HistoryPayComparable();
            
            paymentInfo.Sort(comparable);

            return paymentInfo;
        }


        string period(string date)
        {
            string[] split = date.Split(' ')[0].Split('.');
            int month = Int32.Parse(split[0]);
            if (month > 0)
                return Settings.months[month - 1] + " " + split[2];
            return "";
        }

        void SetText()
        {
            UkName.Text = Settings.MobileSettings.main_name;
            LabelPhone.Text = "+" + Settings.Person.companyPhone.Replace("+", "");
        }

        private void picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            // var identLength = Settings.Person.Accounts[Picker.SelectedIndex].Ident.Length;
            // if (identLength < 6)
            // {
            //     Picker.WidthRequest = identLength * 9;
            // }
            additionalList.ItemsSource = null;
            additionalList.ItemsSource = setPays(Accounts[Picker.SelectedIndex]);
        }

        private void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
        }
    }
}