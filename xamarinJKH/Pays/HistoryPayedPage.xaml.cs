using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamarinJKH.Server;
using xamarinJKH.Server.RequestModel;
using xamarinJKH.Utils;

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
                    BackgroundColor = Color.White;
                    ImageTop.Margin = new Thickness(0, 0, 0, 0);
                    StackLayout.Margin = new Thickness(0, 33, 0, 0);
                    IconViewNameUk.Margin = new Thickness(0, 33, 0, 0);
                    break;
                case Device.Android:
                    double or = Math.Round(((double) App.ScreenWidth / (double) App.ScreenHeight), 2);
                    if (Math.Abs(or - 0.5) < 0.02)
                    {
                        ScrollViewContainer.Margin = new Thickness(0,0,0,-120);
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
            SetText();
            Payments = Accounts[0].Payments;
            SelectedAcc = Accounts[0];
            BindingContext = this;
            additionalList.Effects.Add(Effect.Resolve("MyEffects.ListViewHighlightEffect"));
        }

        void SetText()
        {
            UkName.Text = Settings.MobileSettings.main_name;
            LabelPhone.Text = "+" + Settings.Person.Phone;
        }

        private void picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            // var identLength = Settings.Person.Accounts[Picker.SelectedIndex].Ident.Length;
            // if (identLength < 6)
            // {
            //     Picker.WidthRequest = identLength * 9;
            // }
            additionalList.ItemsSource = null;
            additionalList.ItemsSource = Accounts[Picker.SelectedIndex].Payments;
        }

        private void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
        }
    }
}