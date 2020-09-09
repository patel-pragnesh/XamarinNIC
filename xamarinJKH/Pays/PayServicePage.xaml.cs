﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AiForms.Dialogs;
using AiForms.Dialogs.Abstractions;
using Rg.Plugins.Popup.Services;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamarinJKH.DialogViews;
using xamarinJKH.Server;
using xamarinJKH.Utils;
using xamarinJKH.Server.RequestModel;
using xamarinJKH.InterfacesIntegration;

namespace xamarinJKH.Pays
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PayServicePage : ContentPage
    {
        private RestClientMP server = new RestClientMP();
        private int? idRequset;

        public PayServicePage(string ident, decimal sum, int? idRequset = null)
        {
            this.idRequset = idRequset;
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            var backClick = new TapGestureRecognizer();
            backClick.Tapped += async (s, e) => { _ = await Navigation.PopAsync(); };
            BackStackLayout.GestureRecognizers.Add(backClick);
            if (Device.RuntimePlatform == Device.iOS)
            {
                int statusBarHeight = DependencyService.Get<IStatusBar>().GetHeight();
                //BackStackLayout.Padding = new Thickness(0, statusBarHeight, 0, 0);

                iosBarSeparator.IsVisible = true;
                iosBarSeparator.IsEnabled = true;
                iosBarSeparator.HeightRequest = statusBarHeight;
            }

            SetText();

            if (idRequset == null)
            {
                GetPayLink(ident, sum);
            }
            else
            {
                GetPayLinkRequest(idRequset, sum);
            }
        }

        async void GetPayLink(string ident, decimal sum)
        {
            if (Xamarin.Essentials.Connectivity.NetworkAccess != Xamarin.Essentials.NetworkAccess.Internet)
            {
                Device.BeginInvokeOnMainThread(async () =>
                    await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorNoInternet, "OK"));
                return;
            }

            await Settings.StartProgressBar();
            PayService payLink = await server.GetPayLink(ident, sum);
            if (payLink.payLink != null)
            {
                webView.Source = payLink.payLink;
            }
            else
            {
                Loading.Instance.Hide();
                await DisplayAlert(AppResources.ErrorTitle, payLink.Error, "OK");
                await Navigation.PopAsync();
            }
        }

        async void GetPayLinkRequest(int? id, decimal sum)
        {
            if (Xamarin.Essentials.Connectivity.NetworkAccess != Xamarin.Essentials.NetworkAccess.Internet)
            {
                Device.BeginInvokeOnMainThread(async () =>
                    await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorNoInternet, "OK"));
                return;
            }

            await Settings.StartProgressBar();
            PayService payLink = await server.GetPayLink(id, sum);
            if (payLink.payLink != null)
            {
                webView.Source = payLink.payLink;
            }
            else
            {
                Loading.Instance.Hide();
                await DisplayAlert(AppResources.ErrorTitle, payLink.Error, "OK");
                await Navigation.PopAsync();
            }
        }

        void SetText()
        {
            // UkName.Text = Settings.MobileSettings.main_name;
            // LabelPhone.Text = "+" + Settings.Person.Phone;
        }

        private void WebView_OnNavigated(object sender, WebNavigatedEventArgs e)
        {
            var eUrl = e.Url;
            Loading.Instance.Hide();
            var findByName = webView.FindByName("objectBox objectBox-string");
        }

        private async void WebView_OnNavigating(object sender, WebNavigatingEventArgs e)
        {
            var eUrl = e.Url;
            if (eUrl.Contains("GetPayResult"))
            {
                string url = eUrl.Replace(RestClientMP.SERVER_ADDR + "/", "");
                await StartProgressBar(url);
            }

            Loading.Instance.Hide();
        }

        public async Task StartProgressBar(string url)
        {
            bool rate = Preferences.Get("rate", true);
            if (Xamarin.Essentials.Connectivity.NetworkAccess != Xamarin.Essentials.NetworkAccess.Internet)
            {
                Device.BeginInvokeOnMainThread(async () =>
                    await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorNoInternet, "OK"));
                return;
            }

            // Loading settings
            Configurations.LoadingConfig = new LoadingConfig
            {
                IndicatorColor = Color.FromHex(Settings.MobileSettings.color),
                OverlayColor = Color.Black,
                Opacity = 0.6,
                DefaultMessage = "Оплата",
            };

            await Loading.Instance.StartAsync(async progress =>
            {
                // some heavy process.
                PayResult result = await server.GetPayResult(url);
                if (result.error != null && result.Equals(""))
                {
                    await DisplayAlert(AppResources.ErrorTitle, result.error, "OK");
                    

                    await Navigation.PopAsync();
                }
                else
                {
                    await DisplayAlert(AppResources.AlertSuccess, result.message, "OK");
                    if (rate)
                    {
                        await PopupNavigation.Instance.PushAsync(new RatingAppMarketDialog());
                    }
                    if (idRequset != null)
                    {
                        await GetCodePay();
                    }
                    await Navigation.PopToRootAsync();
                }
            });
        }

        private async Task GetCodePay()
        {
            CommonResult resultCode =
                await server.SendPaidRequestCompleteCodeOnlineAndCah(idRequset, Settings.Person.Phone);
            if (resultCode.Error != null)
            {
                await DisplayAlert(AppResources.ErrorTitle, resultCode.Error, "OK");
            }
            else
            {
                if (Device.RuntimePlatform == Device.iOS)
                {
                    await DisplayAlert("", AppResources.AlertCodeSent, "OK");
                }
                else
                {
                    DependencyService.Get<IMessage>().ShortAlert(AppResources.AlertCodeSent);
                }
            }
        }
    }
}