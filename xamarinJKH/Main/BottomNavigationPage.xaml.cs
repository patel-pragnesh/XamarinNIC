﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamarinJKH.Utils;
using xamarinJKH.Server;
using System.Runtime.CompilerServices;
using Akavache;
using Microsoft.AppCenter.Analytics;
using Xamarin.Essentials;
using xamarinJKH.Server.RequestModel;
using xamarinJKH.Additional;
using xamarinJKH.Apps;

namespace xamarinJKH.Main
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BottomNavigationPage : TabbedPage
    {
        private RestClientMP server = new RestClientMP();

        public Command ChangeTheme { get; set; }
        int request_amount;
        public int RequestsAmount
        {
            get => request_amount;
            set
            {
                request_amount = value;
                OnPropertyChanged("RequestsAmount");
            }
        }

        int events_amount;
        public int EventsAmount
        {
            get => events_amount;
            set
            {
                events_amount = value;
                OnPropertyChanged(nameof(EventsAmount));
            }
        }

        public BottomNavigationPage()
        {
            Analytics.TrackEvent("Форма нижнего меню");
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            BindingContext = this;
            Color hex = (Color)Application.Current.Resources["MainColor"];
            SelectedTabColor = hex;
            GetBrand();
            OSAppTheme currentTheme = Application.Current.RequestedTheme;
            //только темная тема в ios
            //if (Xamarin.Essentials.DeviceInfo.Platform == DevicePlatform.iOS)
            //    currentTheme = OSAppTheme.Dark;

            Color unselect = hex.AddLuminosity(0.3);
            //switch (currentTheme)
            //{
            //    case OSAppTheme.Light: UnselectedTabColor = unselect;
            //        break;
            //    case OSAppTheme.Dark: UnselectedTabColor = Color.Gray;
            //        break;
            //}

            // if (AppInfo.PackageName == "rom.best.UkComfort" || AppInfo.PackageName == "sys_rom.ru.comfort_uk_app")
            // {
            // ProfPage.IsVisible = false;
            // }


            switch (currentTheme)
            {
                case OSAppTheme.Light:
                    UnselectedTabColor = unselect;
                    if (Xamarin.Essentials.DeviceInfo.Platform == Xamarin.Essentials.DevicePlatform.iOS)
                    {
                        EventsNavPage.BarTextColor = Color.Black;
                        PayPage.BarTextColor = Color.Black;
                        CounterPage.BarTextColor = Color.Black;
                        AppPage.BarTextColor = Color.Black;
                        ShopNavPage.BarTextColor = Color.Black;
                        ProfPage.BarTextColor = Color.Black;
                        ShopNavPage2.BarTextColor = Color.Black;
                    }

                    break;
                case OSAppTheme.Dark:
                    UnselectedTabColor = Color.Gray;
                    if (Xamarin.Essentials.DeviceInfo.Platform == Xamarin.Essentials.DevicePlatform.iOS)
                    {
                        EventsNavPage.BarTextColor = Color.White;
                        PayPage.BarTextColor = Color.White;
                        CounterPage.BarTextColor = Color.White;
                        AppPage.BarTextColor = Color.White;
                        ShopNavPage.BarTextColor = Color.White;
                        ProfPage.BarTextColor = Color.White;
                        ShopNavPage2.BarTextColor = Color.White;
                    }

                    break;
                case OSAppTheme.Unspecified:
                    if (Xamarin.Essentials.DeviceInfo.Platform == Xamarin.Essentials.DevicePlatform.iOS)
                    {
                        switch (Settings.MobileSettings.appTheme)
                        {
                            case "":
                            case "light":
                                EventsNavPage.BarTextColor = Color.Black;
                                PayPage.BarTextColor = Color.Black;
                                CounterPage.BarTextColor = Color.Black;
                                AppPage.BarTextColor = Color.Black;
                                ShopNavPage.BarTextColor = Color.Black;
                                ProfPage.BarTextColor = Color.Black;
                                ShopNavPage2.BarTextColor = Color.Black;
                                break;
                            case "dark":
                                EventsNavPage.BarTextColor = Color.White;
                                PayPage.BarTextColor = Color.White;
                                CounterPage.BarTextColor = Color.White;
                                AppPage.BarTextColor = Color.White;
                                ShopNavPage.BarTextColor = Color.White;
                                ProfPage.BarTextColor = Color.White;
                                ShopNavPage2.BarTextColor = Color.White;
                                break;
                        }
                        
                        //EventsNavPage.BarTextColor = Color.Black;
                        //PayPage.BarTextColor = Color.Black;
                        //CounterPage.BarTextColor = Color.Black;
                        //AppPage.BarTextColor = Color.Black;
                        //ShopNavPage.BarTextColor = Color.Black;
                        //ProfPage.BarTextColor = Color.Black;
                        //ShopNavPage2.BarTextColor = Color.Black;
                    }

                    break;
            }


            Application.Current.Resources["Saldo"] = true;
            VisibleMenu();
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
                //только темная тема в ios
                //if (Xamarin.Essentials.DeviceInfo.Platform == DevicePlatform.iOS)
                //    currentTheme = OSAppTheme.Dark;


                Color unselect = hex.AddLuminosity(0.3);
                switch (currentTheme)
                {
                    case OSAppTheme.Light:
                        UnselectedTabColor = unselect;
                        if (Xamarin.Essentials.DeviceInfo.Platform == Xamarin.Essentials.DevicePlatform.iOS)
                        {
                            EventsNavPage.BarTextColor = Color.Black;
                            PayPage.BarTextColor = Color.Black;
                            CounterPage.BarTextColor = Color.Black;
                            AppPage.BarTextColor = Color.Black;
                            ShopNavPage.BarTextColor = Color.Black;
                            ProfPage.BarTextColor = Color.Black;
                            ShopNavPage2.BarTextColor = Color.Black;
                        }

                        break;
                    case OSAppTheme.Dark:
                        UnselectedTabColor = Color.Gray;
                        if (Xamarin.Essentials.DeviceInfo.Platform == Xamarin.Essentials.DevicePlatform.iOS)
                        {
                            EventsNavPage.BarTextColor = Color.White;
                            PayPage.BarTextColor = Color.White;
                            CounterPage.BarTextColor = Color.White;
                            AppPage.BarTextColor = Color.White;
                            ShopNavPage.BarTextColor = Color.White;
                            ProfPage.BarTextColor = Color.White;
                            ShopNavPage2.BarTextColor = Color.White;
                        }

                        break;
                    case OSAppTheme.Unspecified:
                        if (Xamarin.Essentials.DeviceInfo.Platform == Xamarin.Essentials.DevicePlatform.iOS)
                        {
                            EventsNavPage.BarTextColor = Color.Black;
                            PayPage.BarTextColor = Color.Black;
                            CounterPage.BarTextColor = Color.Black;
                            AppPage.BarTextColor = Color.Black;
                            ShopNavPage.BarTextColor = Color.Black;
                            ProfPage.BarTextColor = Color.Black;
                            ShopNavPage2.BarTextColor = Color.Black;
                        }

                        break;
                }
            });

            Loadtab();

            MessagingCenter.Subscribe<Object>(this, "ChangeTheme", (sender) => ChangeTheme.Execute(null));

            MessagingCenter.Subscribe<Object, int>(this, "SetRequestsAmount", (sender, args) =>
            {
                if (args == -1)
                    RequestsAmount -= 1;
                else
                    RequestsAmount = args;
            });

            MessagingCenter.Subscribe<Object, int>(this, "SetEventsAmount", (sender, args) =>
            {
                if (args == -1)
                    EventsAmount -= 1;
                else
                    EventsAmount = args;
            });
        }

        void StartUpdateToken()
        {
            Device.StartTimer(TimeSpan.FromMinutes(5), OnTimerTick);
        }

        async void SetTab(string title)
        {
            var services = this.Children.FirstOrDefault(x => x.Title == title);
            if (services != null)
            {
                this.CurrentPage = services;
            }
        }

        async void Loadtab()
        {
            Analytics.TrackEvent("Установка данных для вкладок");
            if (Settings.MobileSettings == null)
            {
                Analytics.TrackEvent("Ожидание подгрузки данных");
            }
            while (Settings.MobileSettings == null)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(50));
            }
            if (Settings.MobileSettings != null)
                if (!string.IsNullOrEmpty(Settings.MobileSettings.startScreen))
                    switch (Settings.MobileSettings.startScreen.Trim())
                    {
                        case "Оплата":
                            {
                                SetTab(AppResources.Pays);
                                break;
                            }
                        case "События":
                            {
                                SetTab(AppResources.Events_NavBar);
                                break;
                            }
                        case "Показания":
                            {
                                SetTab(AppResources.Meters_NavBar);
                                break;
                            }
                        case "Наши услуги":
                            {
                                SetTab(AppResources.Shop_NavBar);
                                await Task.Delay(TimeSpan.FromMilliseconds(700));
                                MessagingCenter.Send<Object>(this, "LoadGoods");
                                break;
                            }
                        default:
                            SetTab(AppResources.Events_NavBar);
                            break;
                    }
        }

        private void GetBrand()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                ItemsList<string> vehicleMarks = await server.VehicleMarks();
                if (vehicleMarks.Error == null || vehicleMarks.Data == null)
                {
                    Settings.BrandCar = vehicleMarks.Data;
                }
                else
                {
                    Settings.BrandCar = new List<string>();
                }
            });
        }
        private bool OnTimerTick()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                string login = Preferences.Get("login", "");
                string pass = Preferences.Get("pass", "");
                if (!pass.Equals("") && !login.Equals(""))
                {
                    if (Xamarin.Essentials.Connectivity.NetworkAccess != Xamarin.Essentials.NetworkAccess.Internet)
                    {
                        Device.BeginInvokeOnMainThread(async () =>
                            await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorNoInternet, "OK"));
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

        void VisibleMenu()
        {
            try
            {
                // if (AppInfo.PackageName != "rom.best.UkComfort" && AppInfo.PackageName != "sys_rom.ru.comfort_uk_app")
                // {
                // if (RestClientMP.SERVER_ADDR.Contains("komfortnew"))
                // {
                Children.Remove(ShopNavPage);
                // }
                // else
                // {
                // Children.Remove(ShopNavPage2);
                // }
                // }
                // else
                // {
                //     Children.Remove(ShopNavPage);
                Children.Remove(ProfPage);
                // }



                foreach (var each in Settings.MobileSettings.menu)
                {
                    if (each.name_app.Equals("Заявки"))
                    {
                        if (each.visible == 0)
                        {
                            Children.Remove(AppPage);
                            Settings.AppIsVisible = false;
                        }
                    }
                    else if (each.name_app.Equals("Показания счетчиков") || each.name_app.Equals("Показания приборов"))
                    {
                        if (each.visible == 0)
                        {
                            Children.Remove(CounterPage);
                        }
                    }
                    else if (each.name_app.Equals("Оплата ЖКУ"))
                    {
                        if (each.visible == 0)
                        {
                            Children.Remove(PayPage);
                        }
                    }
                    else if (each.name_app.Equals("Уведомления"))
                    {
                        if (each.visible == 0)
                        {
                            Settings.NotifVisible = false;
                        }
                    }
                    else if (each.name_app.Equals("Опросы"))
                    {
                        if (each.visible == 0)
                        {
                            Settings.QuestVisible = false;
                        }
                    }
                    else if (each.name_app.Equals("Квитанции"))
                    {
                        if (each.visible == 0)
                        {
                            Application.Current.Resources["Saldo"] = false;
                        }
                    }
                    else if (each.name_app.Equals("Дополнительные услуги"))
                    {
                        if (each.visible == 0)
                        {
                            if (RestClientMP.SERVER_ADDR.Contains("komfortnew"))
                            {
                                Children.Remove(ShopNavPage2);
                            }
                            else
                            {
                                Children.Remove(ShopNavPage);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


        protected override void OnCurrentPageChanged()
        {
            base.OnCurrentPageChanged();
            var i = Children.IndexOf(CurrentPage);
            if (i == 0)
                MessagingCenter.Send<Object>(this, "UpdateEvents");

            if (CurrentPage != null)
            {
                if (CurrentPage is AppsPage || CurrentPage is xamarinJKH.MainConst.AppsConstPage ||
                    CurrentPage.Title == AppResources.App_NavBar)
                    MessagingCenter.Send<Object>(this, "AutoUpdate");

                if (CurrentPage.Title == AppResources.Shop_NavBar)
                    MessagingCenter.Send<Object>(this, "LoadGoods");
            }
        }


        async void RegisterNewDevice()
        {
            App.token = DependencyService.Get<xamarinJKH.InterfacesIntegration.IFirebaseTokenObtainer>().GetToken();
            var response = await (new RestClientMP()).RegisterDevice();
        }
    }
}