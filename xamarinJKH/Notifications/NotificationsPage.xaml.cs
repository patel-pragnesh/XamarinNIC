﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.AppCenter.Analytics;
using Plugin.Messaging;
using Rg.Plugins.Popup.Services;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PancakeView;
using Xamarin.Forms.Xaml;
using xamarinJKH.DialogViews;
using xamarinJKH.InterfacesIntegration;
using xamarinJKH.Notifications;
using xamarinJKH.Server;
using xamarinJKH.Server.RequestModel;
using xamarinJKH.Tech;
using xamarinJKH.Utils;

namespace xamarinJKH
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NotificationsPage : ContentPage
    {
        public List<AnnouncementInfo> Notifications { get; set; }
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

                    await RefreshData();

                    IsRefreshing = false;
                });
            }
        }

        private async Task RefreshData()
        {
            if (Xamarin.Essentials.Connectivity.NetworkAccess != Xamarin.Essentials.NetworkAccess.Internet)
            {
                Device.BeginInvokeOnMainThread(async () => await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorNoInternet, "OK"));
                return;
            }
            Settings.EventBlockData = await server.GetEventBlockData();
            if (Settings.EventBlockData.Error == null)
            {
                Notifications = Settings.EventBlockData.Announcements.Take(3).ToList();
                NotificationList.ItemsSource = null;
                NotificationList.ItemsSource = Notifications;
            }
            else
            {
                await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorNotifInfo, "OK");
            }
        }

        public NotificationsPage()
        {
            InitializeComponent();
            Analytics.TrackEvent("Список уведомлений");
            NavigationPage.SetHasNavigationBar(this, false);

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    int statusBarHeight = DependencyService.Get<IStatusBar>().GetHeight();
                    Pancake.Padding = new Thickness(0, statusBarHeight, 0, 0);
                    //BackgroundColor = Color.White;
                    break;
                default:
                    break;
            }

            var backClick = new TapGestureRecognizer();
            backClick.Tapped += async (s, e) => {
                try
                {
                    _ = await Navigation.PopAsync();
                }
                catch { }
            };
            BackStackLayout.GestureRecognizers.Add(backClick);
            var techSend = new TapGestureRecognizer();
            techSend.Tapped += async (s, e) => {     await PopupNavigation.Instance.PushAsync(new TechDialog());; };
            LabelTech.GestureRecognizers.Add(techSend);
            var call = new TapGestureRecognizer();
            call.Tapped += async (s, e) =>
            {
                if (Settings.Person.Phone != null)
                {
                    IPhoneCallTask phoneDialer;
                    phoneDialer = CrossMessaging.Current.PhoneDialer;
                    if (phoneDialer.CanMakePhoneCall && !string.IsNullOrWhiteSpace(Settings.Person.companyPhone)) 
                        phoneDialer.MakePhoneCall(System.Text.RegularExpressions.Regex.Replace(Settings.Person.companyPhone, "[^+0-9]", ""));
                }

            
            };
            LabelPhone.GestureRecognizers.Add(call);
            SetText();
            Notifications = Settings.EventBlockData.Announcements.Take(3).ToList();
            this.BindingContext = this;
            NotificationList.BackgroundColor = Color.Transparent;
            NotificationList.Effects.Add(Effect.Resolve("MyEffects.ListViewHighlightEffect"));
        }

        void SetText()
        {
            UkName.Text = Settings.MobileSettings.main_name;
            if (!string.IsNullOrWhiteSpace(Settings.Person.companyPhone))
            {
                LabelPhone.Text = "+" + Settings.Person.companyPhone.Replace("+", "");
            }
            else
            {
                IconViewLogin.IsVisible = false;
                LabelPhone.IsVisible = false;
            }
            Color hexColor = (Color) Application.Current.Resources["MainColor"];
            IconViewLogin.SetAppThemeColor(IconView.ForegroundProperty, hexColor, Color.White);
            IconViewTech.SetAppThemeColor(IconView.ForegroundProperty, hexColor, Color.White);
            Pancake.SetAppThemeColor(PancakeView.BorderColorProperty, hexColor, Color.Transparent);
            PancakeViewIcon.SetAppThemeColor(PancakeView.BorderColorProperty, hexColor, Color.Transparent);if (Device.RuntimePlatform == Device.iOS){ if (AppInfo.PackageName == "rom.best.saburovo" || AppInfo.PackageName == "sys_rom.ru.tsg_saburovo"){PancakeViewIcon.Padding = new Thickness(0);}}
            LabelTech.SetAppThemeColor(Label.TextColorProperty, hexColor, Color.White);
        }

        private async void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            AnnouncementInfo select = e.Item as AnnouncementInfo;

            if (Navigation.NavigationStack.FirstOrDefault(x => x is NotificationOnePage) == null)
                await Navigation.PushAsync(new NotificationOnePage(select));
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NotificationAllPage());
        }
    }
}