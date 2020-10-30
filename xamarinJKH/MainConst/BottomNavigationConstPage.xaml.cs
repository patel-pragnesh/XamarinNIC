using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamarinJKH.Server;
using xamarinJKH.Server.RequestModel;
using xamarinJKH.Utils;

namespace xamarinJKH.MainConst
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BottomNavigationConstPage : TabbedPage
    {
        private RestClientMP server = new RestClientMP();
        public Command ChangeTheme { get; set; }
        public BottomNavigationConstPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            Color hex = (Color) Application.Current.Resources["MainColor"];
            SelectedTabColor = hex;
            UnselectedTabColor = Color.Gray;
            OSAppTheme currentTheme = Application.Current.RequestedTheme;
            //if (Xamarin.Essentials.DeviceInfo.Platform == Xamarin.Essentials.DevicePlatform.iOS)
            //    currentTheme = OSAppTheme.Dark;
            Color unselect = hex.AddLuminosity(0.3);
            //switch (currentTheme)
            //{
            //    case OSAppTheme.Light: UnselectedTabColor = unselect;
            //        break;
            //    case OSAppTheme.Dark: UnselectedTabColor = Color.Gray;
            //        break;
            //}

            switch (currentTheme)
            {
                case OSAppTheme.Light:
                    UnselectedTabColor = unselect;
                    if (Xamarin.Essentials.DeviceInfo.Platform == Xamarin.Essentials.DevicePlatform.iOS)
                    {
                        appNavBar.BarTextColor = Color.Black;
                        monNavBar.BarTextColor = Color.Black;
                        profNavBar.BarTextColor = Color.Black;
                    }
                    break;
                case OSAppTheme.Dark:
                    UnselectedTabColor = Color.Gray;
                    if (Xamarin.Essentials.DeviceInfo.Platform == Xamarin.Essentials.DevicePlatform.iOS)
                    {
                        appNavBar.BarTextColor = Color.White;
                        monNavBar.BarTextColor = Color.White;
                        profNavBar.BarTextColor = Color.White;
                    }
                    break;
                case OSAppTheme.Unspecified:
                    if (Xamarin.Essentials.DeviceInfo.Platform == Xamarin.Essentials.DevicePlatform.iOS)
                    {
                        appNavBar.BarTextColor = Color.Black;
                        monNavBar.BarTextColor = Color.Black;
                        profNavBar.BarTextColor = Color.Black;
                    }
                    break;
            }

            StartUpdateToken();
            ChangeTheme = new Command(async () =>
            {
                OSAppTheme currentTheme = Application.Current.RequestedTheme;
                
                Color unselect = hex.AddLuminosity(0.3);
                switch (currentTheme)
                {
                    case OSAppTheme.Light: 
                        UnselectedTabColor = unselect;
                        if (Xamarin.Essentials.DeviceInfo.Platform == Xamarin.Essentials.DevicePlatform.iOS)
                        {
                            appNavBar.BarTextColor = Color.Black;
                            monNavBar.BarTextColor = Color.Black;
                            profNavBar.BarTextColor = Color.Black;
                        }
                            break;
                    case OSAppTheme.Dark: 
                        UnselectedTabColor = Color.Gray;
                        if (Xamarin.Essentials.DeviceInfo.Platform == Xamarin.Essentials.DevicePlatform.iOS)
                        {
                            appNavBar.BarTextColor = Color.White;
                            monNavBar.BarTextColor = Color.White;
                            profNavBar.BarTextColor = Color.White;
                        }
                        break;
                    case OSAppTheme.Unspecified:
                        if (Xamarin.Essentials.DeviceInfo.Platform == Xamarin.Essentials.DevicePlatform.iOS)
                        {
                            appNavBar.BarTextColor = Color.Black;
                            monNavBar.BarTextColor = Color.Black;
                            profNavBar.BarTextColor = Color.Black;
                        }
                        break;
                }
            });
            MessagingCenter.Subscribe<Object>(this, "ChangeThemeConst", (sender) => ChangeTheme.Execute(null));
            if (Device.RuntimePlatform == "Android")
                RegisterNewDevice();

            MessagingCenter.Subscribe<Object, int>(this, "SwitchToAppsConst", (sender, args) =>
            {
                this.CurrentPage = this.Children[0];
                MessagingCenter.Send<Object, int>(this, "OpenAppConst", args);
            });
            

        }
        void StartUpdateToken()
        {
            Device.StartTimer(TimeSpan.FromMinutes(5), OnTimerTick);
        }

        private  bool OnTimerTick()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                string login = Preferences.Get("loginConst", "");
                string pass = Preferences.Get("passConst", "");
                if (!pass.Equals("") && !login.Equals(""))
                {

                    if (Xamarin.Essentials.Connectivity.NetworkAccess != Xamarin.Essentials.NetworkAccess.Internet)
                    {
                        Device.BeginInvokeOnMainThread(async () => await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorNoInternet, "OK"));
                        return;
                    }
                    LoginResult loginResult = await server.LoginDispatcher(login, pass);
                    if (loginResult.Error == null)
                    {
                        Settings.Person = loginResult;
                    }
                }

            });
            return true;
        }
        protected override void OnCurrentPageChanged()
        {
            base.OnCurrentPageChanged();
            var i = Children.IndexOf(CurrentPage);
            if (i == 0)
                MessagingCenter.Send<Object>(this, "UpdateAppCons");
        }

        async void RegisterNewDevice()
        {
            App.token = DependencyService.Get<xamarinJKH.InterfacesIntegration.IFirebaseTokenObtainer>().GetToken();
            var response = await (new RestClientMP()).RegisterDevice(true);
        }
    }
}
