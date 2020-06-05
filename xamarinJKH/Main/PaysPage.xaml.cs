using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamarinJKH.Pays;
using xamarinJKH.Server;
using xamarinJKH.Server.RequestModel;
using xamarinJKH.Utils;

namespace xamarinJKH.Main
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PaysPage : ContentPage
    {
        public List<AccountAccountingInfo> _accountingInfo { get; set; }
        private RestClientMP _server = new RestClientMP();
        private bool _isRefreshing = false;
        private RestClientMP server = new RestClientMP();

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

                    await RefreshPaysData();

                    IsRefreshing = false;
                });
            }
        }

        public async Task RefreshPaysData()
        {
            getInfo();
            additionalList.ItemsSource = null;
            additionalList.ItemsSource = _accountingInfo;
        }

        public PaysPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
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
                        RelativeLayoutTop.Margin = new Thickness(0,0,0,-125);
                        BackStackLayout.Margin = new Thickness(5, 25, 0, 0);
                    }
                    break;
                default:
                    break;
            }

            SetTextAndColor();
            getInfo();
            additionalList.BackgroundColor = Color.Transparent;
            var goAddIdent = new TapGestureRecognizer();
            goAddIdent.Tapped += async (s, e) => { await Navigation.PushAsync(new AddIdent(this)); };
            FrameAddIdent.GestureRecognizers.Add(goAddIdent);
            var openSaldos = new TapGestureRecognizer();
            openSaldos.Tapped += async (s, e) => { await Navigation.PushAsync(new SaldosPage(_accountingInfo)); };
            FrameBtnSaldos.GestureRecognizers.Add(openSaldos);
            additionalList.Effects.Add(Effect.Resolve("MyEffects.ListViewHighlightEffect"));
        }
        
        protected override void OnAppearing()
        {
            base.OnAppearing();
            
            new Task(SyncSetup).Start(); // This could be an await'd task if need be
        }
        
        async void SyncSetup()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                // Assuming this function needs to use Main/UI thread to move to your "Main Menu" Page
                RefreshPaysData();
            });
        }

        void SetTextAndColor()
        {
            UkName.Text = Settings.MobileSettings.main_name;
            LabelPhone.Text = "+" + Settings.Person.Phone;
            FrameBtnHistory.BorderColor = Color.FromHex(Settings.MobileSettings.color);
            FrameBtnSaldos.BorderColor = Color.FromHex(Settings.MobileSettings.color);
            LabelSaldos.TextColor = Color.FromHex(Settings.MobileSettings.color);
            LabelHistory.TextColor = Color.FromHex(Settings.MobileSettings.color);
        }

        async void getInfo()
        {
            ItemsList<AccountAccountingInfo> info = await _server.GetAccountingInfo();
            if (info.Error == null)
            {
                _accountingInfo = info.Data;
                this.BindingContext = this;
            }
            else
            {
                await DisplayAlert("Ошибка", "Не удалось получить информацию о начислениях", "OK");
            }
        }

        private async void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            AccountAccountingInfo select = e.Item as AccountAccountingInfo;
            await Navigation.PushAsync(new CostPage(select, _accountingInfo));
        }

        private async void openSaldo(object sender, EventArgs e)
        {
            if (_accountingInfo.Count > 0)
            {
                await Navigation.PushAsync(new SaldosPage(_accountingInfo));
            }
            else
            {
                await DisplayAlert("Ошибка", "Лицевые счета не подключены", "OK");
            }
        }

        private async void OpenHistory(object sender, EventArgs e)
        {
            if (_accountingInfo.Count > 0)
            {
                await Navigation.PushAsync(new HistoryPayedPage(_accountingInfo));
            }
            else
            {
                await DisplayAlert("Ошибка", "Лицевые счета не подключены", "OK");
            }
        }
    }
}