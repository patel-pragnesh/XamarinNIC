using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamarinJKH.Utils;
using xamarinJKH.Server;
using System.Runtime.CompilerServices;
using Xamarin.Essentials;
using xamarinJKH.Server.RequestModel;

namespace xamarinJKH.Main
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BottomNavigationPage : TabbedPage
    {
        private RestClientMP server = new RestClientMP();
        
        public Command ChangeTheme { get; set; }
        public BottomNavigationPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            Color hex = Color.FromHex(Utils.Settings.MobileSettings.color);
            SelectedTabColor = hex;

            OSAppTheme currentTheme = Application.Current.RequestedTheme;
            if (Xamarin.Essentials.DeviceInfo.Platform == DevicePlatform.iOS)
                currentTheme = OSAppTheme.Dark;
            Color unselect = hex.AddLuminosity(0.3);
            switch (currentTheme)
            {
                case OSAppTheme.Light: UnselectedTabColor = unselect;
                    break;
                case OSAppTheme.Dark: UnselectedTabColor = Color.Gray;
                    break;
            }
            CheckAccounts();
            Application.Current.Resources["Saldo"] = true;
            visibleMenu();
            StartUpdateToken();
            if (Device.RuntimePlatform == Device.Android)
                RegisterNewDevice();
            MessagingCenter.Subscribe<Object, int>(this, "SwitchToApps", (sender, index) =>
            {
                this.CurrentPage = this.Children[3];
                MessagingCenter.Send<Object, int>(this, "OpenApp", index);
            });
            
            ChangeTheme = new Command(async () =>
            {
                OSAppTheme currentTheme = Application.Current.RequestedTheme;
                if (Xamarin.Essentials.DeviceInfo.Platform == DevicePlatform.iOS)
                    currentTheme = OSAppTheme.Dark;


                Color unselect = hex.AddLuminosity(0.3);
                switch (currentTheme)
                {
                    case OSAppTheme.Light: UnselectedTabColor = unselect;
                        break;
                    case OSAppTheme.Dark: UnselectedTabColor = Color.Gray;
                        break;
                }
            });
            
            MessagingCenter.Subscribe<Object>(this, "ChangeTheme", (sender) => ChangeTheme.Execute(null));
        }

        void StartUpdateToken()
        {
            Device.StartTimer(TimeSpan.FromMinutes(5), OnTimerTick);
        }

        private  bool OnTimerTick()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                string login = Preferences.Get("login", "");
                string pass = Preferences.Get("pass", "");
                if (!pass.Equals("") && !login.Equals(""))
                {
                    if (Xamarin.Essentials.Connectivity.NetworkAccess != Xamarin.Essentials.NetworkAccess.Internet)
                    {
                        Device.BeginInvokeOnMainThread(async () => await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorNoInternet, "OK"));
                        return;
                    }
                    LoginResult loginResult = await server.Login(login, pass);
                    if (loginResult.Error == null)
                    {
                        Settings.Person = loginResult;
                    }
                }

            });
            return true;
        }

        void visibleMenu()
        {
            foreach (var each in Settings.MobileSettings.menu)
            {
                if (each.name_app.Equals("Заявки"))
                {
                    if (each.visible == 0)
                    {
                        Children.Remove(AppPage);
                        Settings.AppIsVisible = false;

                    }
                }else if (each.name_app.Equals("Показания счетчиков") || each.name_app.Equals("Показания приборов") )
                {
                    if (each.visible == 0)
                    {
                        Children.Remove(CounterPage);
                    }
                }else if (each.name_app.Equals("Оплата ЖКУ"))
                {
                    if (each.visible == 0)
                    {
                        Children.Remove(PayPage);
                    }
                }else if (each.name_app.Equals("Уведомления"))
                {
                    if (each.visible == 0)
                    {
                        Settings.NotifVisible = false;
                    }
                }else if (each.name_app.Equals("Опросы"))
                {
                    if (each.visible == 0)
                    {
                        Settings.QuestVisible = false;
                    }
                }else if (each.name_app.Equals("Квитанции"))
                {
                    if (each.visible == 0)
                    {
                        Application.Current.Resources["Saldo"] = false;
                    }
                }else if (each.name_app.Equals("Дополнительные услуги"))
                {
                    if (each.visible == 0)
                    {
                        Settings.AddVisible = false;
                    }
                }
            }
        }

        public async void CheckAccounts()
        {
            if (Settings.Person.Accounts.Count == 0)
            {
                await AiForms.Dialogs.Dialog.Instance.ShowAsync<xamarinJKH.DialogViews.AddAccountDialogView>();
            }
        }

        protected override void OnCurrentPageChanged()
        {
            base.OnCurrentPageChanged();
            var i = Children.IndexOf(CurrentPage);
            if (i == 0)
                MessagingCenter.Send<Object>(this, "UpdateEvents");

            if (CurrentPage is AppsPage || CurrentPage is xamarinJKH.MainConst.AppsConstPage || CurrentPage.Title == AppResources.App_NavBar)
                MessagingCenter.Send<Object>(this, "AutoUpdate");

            if (CurrentPage.Title == AppResources.Shop_NavBar)
                MessagingCenter.Send<Object>(this, "LoadGoods");
        }


        async void RegisterNewDevice()
        {
            App.token = DependencyService.Get<xamarinJKH.InterfacesIntegration.IFirebaseTokenObtainer>().GetToken();
            var response = await (new RestClientMP()).RegisterDevice();
        }
    }
}