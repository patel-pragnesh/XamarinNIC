using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Xsl;
using AiForms.Dialogs;
using AiForms.Dialogs.Abstractions;
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
        public Color hex { get; set; }
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
            Settings.mainPage = this;
            NavigationPage.SetHasNavigationBar(this, false);
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    BackgroundColor = Color.White;
                    ImageFon.Margin = new Thickness(0, 0, 0, 0);
                    StackLayout.Margin = new Thickness(0, 33, 0, 0);
                    IconViewNameUk.Margin = new Thickness(0, 33, 0, 0);
                    IconViewSaldos.Margin = new Thickness(-20,0,5,0);
                    break;
                case Device.Android:
                    double or = Math.Round(((double) App.ScreenWidth / (double) App.ScreenHeight), 2);
                    if (Math.Abs(or - 0.5) < 0.02)
                    {
                        RelativeLayoutTop.Margin = new Thickness(0, 0, 0, -125);
                        BackStackLayout.Margin = new Thickness(5, 15, 0, 0);
                    }

                    break;
                default:
                    break;
            }

            hex = Color.FromHex(Settings.MobileSettings.color);
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

            FrameBtnHistory.BorderColor = hex;
            FrameBtnSaldos.BorderColor = hex;
            LabelSaldos.TextColor = hex;
            LabelHistory.TextColor = hex;
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

        public async void DellLs(string Ident)
        {
            bool answer = await Settings.mainPage.DisplayAlert("Удалить?", "Удалить лицевой счет: " + Ident + " ?",
                "Да", "Отмена");
            if (answer)
            {
                await DellIdent(Ident);
            }
        }

        public async Task DellIdent(string ident)
        {
            // Loading settings
            Configurations.LoadingConfig = new LoadingConfig
            {
                IndicatorColor = Color.FromHex(Settings.MobileSettings.color),
                OverlayColor = Color.Black,
                Opacity = 0.8,
                DefaultMessage = "Удаление ЛС",
            };
            RestClientMP server = new RestClientMP();
            await Loading.Instance.StartAsync(async progress =>
            {
                // some heavy process.
                await DellIdentTask(ident, server);
            });
        }

        private async Task DellIdentTask(string ident, RestClientMP server)
        {
            CommonResult result = await server.DellIdent(ident);
            if (result.Error == null)
            {
                // Settings.EventBlockData = await server.GetEventBlockData();
                ItemsList<NamedValue> resultN = await server.GetRequestsTypes();
                Settings.TypeApp = resultN.Data;
                removeLs(ident);
            }
            else
            {
                await DisplayAlert("Ошибка", result.Error, "ОК");
            }
        }

        void removeLs(string ident)
        {
            foreach (var each in _accountingInfo)
            {
                if (each.Ident.Equals(ident))
                {
                    _accountingInfo.Remove(each);
                    break;
                }
            }

            additionalList.ItemsSource = null;
            additionalList.ItemsSource = _accountingInfo;
        }
    }
}