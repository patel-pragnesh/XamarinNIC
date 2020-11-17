﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.AppCenter.Analytics;
using Plugin.FirebaseCrashlytics;
using Plugin.Messaging;
using Rg.Plugins.Popup.Services;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PancakeView;
using Xamarin.Forms.Xaml;
using xamarinJKH.CustomRenderers;
using xamarinJKH.DialogViews;
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
            if (Xamarin.Essentials.Connectivity.NetworkAccess != Xamarin.Essentials.NetworkAccess.Internet)
            {
                Device.BeginInvokeOnMainThread(async () => await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorNoInternet, "OK"));
                return;
            }
            ItemsList<AccountAccountingInfo> info = await _server.GetAccountingInfo();
            if (info.Error == null)
            {
                Accounts = info.Data;
                additionalList.ItemsSource = null;
                additionalList.ItemsSource = setPays(Accounts[Picker.SelectedIndex]);
            }
            else
            {
                await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorPayInfo, "OK");
            }
        }

        public HistoryPayedPage(List<AccountAccountingInfo> accounts)
        {
            this.Accounts = accounts;
            InitializeComponent();
            Analytics.TrackEvent("История платежей");
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    int statusBarHeight = DependencyService.Get<IStatusBar>().GetHeight();
                    //BackgroundColor = Color.White;
                    break;
                default:
                    break;
            }

            NavigationPage.SetHasNavigationBar(this, false);
            var backClick = new TapGestureRecognizer();
            backClick.Tapped += async (s, e) => {
                try
                {
                    _ = await Navigation.PopAsync();
                }
                catch { }
            };
            BackStackLayout.GestureRecognizers.Add(backClick);
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
            var techSend = new TapGestureRecognizer();
            techSend.Tapped += async (s, e) => { await PopupNavigation.Instance.PushAsync(new TechDialog()); };
            LabelTech.GestureRecognizers.Add(techSend);
            var pickLs = new TapGestureRecognizer();
            pickLs.Tapped += async (s, e) => {  
                Device.BeginInvokeOnMainThread(() =>
                {
                    Picker.Focus();
                });
            };
            StackLayoutLs.GestureRecognizers.Add(pickLs);
            SetText();
            Payments = setPays(Accounts[0]);
            SelectedAcc = Accounts[0];
            BindingContext = this;
            additionalList.Effects.Add(Effect.Resolve("MyEffects.ListViewHighlightEffect"));
        }


        List<PaymentInfo> setPays(AccountAccountingInfo accountingInfo)
        {
            List<PaymentInfo> paymentInfo = new List<PaymentInfo>(accountingInfo.Payments);
            List<PaymentInfo> paymentUO = new List<PaymentInfo>(accountingInfo.PendingPayments);
            List<MobilePayment> mobile = new List<MobilePayment>(accountingInfo.MobilePayments);
            paymentInfo.AddRange(paymentUO);
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
            paymentInfo.Reverse();
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
           
            
            Color hexColor = (Color) Application.Current.Resources["MainColor"];
            // IconViewLogin.SetAppThemeColor(IconView.ForegroundProperty, hexColor, Color.White);
            IconViewTech.SetAppThemeColor(IconView.ForegroundProperty, hexColor, Color.White);
            LabelTech.SetAppThemeColor(Label.TextColorProperty, hexColor, Color.White);
            FrameSaldo.SetAppThemeColor(MaterialFrame.BorderColorProperty, hexColor, Color.FromHex("#494949"));
            FrameHistory.SetAppThemeColor(MaterialFrame.BorderColorProperty, hexColor, Color.White);
        }

        private void picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            // var identLength = Settings.Person.Accounts[Picker.SelectedIndex].Ident.Length;
            // if (identLength < 6)
            // {
            //     Picker.WidthRequest = identLength * 9;
            // }
            additionalList.ItemsSource = null;
            AccountAccountingInfo account = Accounts[Picker.SelectedIndex];
            additionalList.ItemsSource = setPays(account);
            Analytics.TrackEvent("Смена лс на " + account.Ident);

        }

        private void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
        }
    }
}