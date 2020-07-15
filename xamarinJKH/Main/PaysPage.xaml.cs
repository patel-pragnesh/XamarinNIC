using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Xsl;
using AiForms.Dialogs;
using AiForms.Dialogs.Abstractions;
using Plugin.Messaging;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamarinJKH.DialogViews;
using xamarinJKH.InterfacesIntegration;
using xamarinJKH.Pays;
using xamarinJKH.Server;
using xamarinJKH.Server.RequestModel;
using xamarinJKH.Tech;
using xamarinJKH.Utils;
using xamarinJKH.ViewModels;

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
            //additionalList.ItemsSource = null;
            //additionalList.ItemsSource = _accountingInfo;
        }

        PaysPageViewModel viewModel { get; set; }

        public PaysPage()
        {
            InitializeComponent();
            BindingContext = viewModel = new PaysPageViewModel();
            Settings.mainPage = this;
            NavigationPage.SetHasNavigationBar(this, false);

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    int statusBarHeight = DependencyService.Get<IStatusBar>().GetHeight();
                    Pancake.Padding = new Thickness(0, statusBarHeight, 0, 0);
                    break;
                default:
                    break;
            }

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    //BackgroundColor = Color.White;
                    // ImageFon.Margin = new Thickness(0, 0, 0, 0);
                    // StackLayout.Margin = new Thickness(0, 33, 0, 0);
                    // IconViewNameUk.Margin = new Thickness(0, 33, 0, 0);
                    // IconViewSaldos.Margin = new Thickness(-20,0,5,0);
                    // if (Application.Current.MainPage.Height < 800)
                    // {
                    //     BackStackLayout.Margin = new Thickness(5, 15, 0, 0);
                    // }
                    // else
                    // {
                    //     BackStackLayout.Margin = new Thickness(5, 35, 0, 0);
                    //     RelativeLayoutTop.Margin = new Thickness(0, 0, 0, -180);
                    // }
                    break;
                case Device.Android:
                    double or = Math.Round(((double) App.ScreenWidth / (double) App.ScreenHeight), 2);
                    // if (Math.Abs(or - 0.5) < 0.02)
                    // {
                    //     RelativeLayoutTop.Margin = new Thickness(0, 0, 0, -125);
                    //     BackStackLayout.Margin = new Thickness(5, 15, 0, 0);
                    // }

                    break;
                default:
                    break;
            }

            hex = Color.FromHex(Settings.MobileSettings.color);
            var techSend = new TapGestureRecognizer();
            techSend.Tapped += async (s, e) => {     await Navigation.PushAsync(new TechSendPage()); };
            LabelTech.GestureRecognizers.Add(techSend);
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
            SetTextAndColor();
            //getInfo();
            additionalList.BackgroundColor = Color.Transparent;
            var goAddIdent = new TapGestureRecognizer();
            goAddIdent.Tapped += async (s, e) => { /*await Dialog.Instance.ShowAsync<AddAccountDialogView>();*/await Navigation.PushAsync(new AddIdent(this)); };
            FrameAddIdent.GestureRecognizers.Add(goAddIdent);
            var openSaldos = new TapGestureRecognizer();
            openSaldos.Tapped += async (s, e) => { await Navigation.PushAsync(new SaldosPage(_accountingInfo)); };
            FrameBtnSaldos.GestureRecognizers.Add(openSaldos);
            additionalList.Effects.Add(Effect.Resolve("MyEffects.ListViewHighlightEffect"));
            MessagingCenter.Subscribe<Object>(this, "UpdateIdent", (sender) => SyncSetup());
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
            LabelPhone.Text = "+" + Settings.Person.companyPhone.Replace("+","");

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
                viewModel.LoadAccounts.Execute(info.Data);
                //this.BindingContext = this;
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
                viewModel.RemoveAccount.Execute(ident);//removeLs(ident);
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

    public class PaysPageViewModel : BaseViewModel
    {
        public ObservableCollection<AccountAccountingInfo> Accounts { get; set; }
        public Command LoadAccounts { get; set; }
        public Color hex { get; set; }
        public Command RemoveAccount 
        {
            get => new Command<string>(ident =>
            {
                var account_to_delete = Accounts.FirstOrDefault(x => x.Ident == ident);
                if (account_to_delete != null)
                {
                    Accounts.Remove(account_to_delete);
                    OnPropertyChanged("Accounts");
                }
            });
        }

        bool _isRefreshing;
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set
            {
                _isRefreshing = value;
                OnPropertyChanged(nameof(IsRefreshing));
            }
        }

        public Command RefreshCommand
        {
            get => new Command(() =>
            {
                IsRefreshing = true;
                LoadAccounts.Execute(null);

            });
        }

        public PaysPageViewModel()
        {
            hex = Color.FromHex(Settings.MobileSettings.color);
            Accounts = new ObservableCollection<AccountAccountingInfo>();
            LoadAccounts = new Command<List<AccountAccountingInfo>>(async (accounts) =>
            {
                Accounts.Clear();
                if (accounts == null)
                    accounts = (await (new RestClientMP()).GetAccountingInfo()).Data;
                foreach (var account in accounts)
                {
                    Device.BeginInvokeOnMainThread(() => Accounts.Add(account));
                }
                IsRefreshing = false;
            });
        }
    }
}