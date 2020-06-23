using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AiForms.Dialogs;
using AiForms.Dialogs.Abstractions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamarinJKH.Server;
using xamarinJKH.Utils;
using xamarinJKH.Server.RequestModel;

namespace xamarinJKH.Pays
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PayServicePage : ContentPage
    {
        private RestClientMP server = new RestClientMP();

        public PayServicePage(string ident, decimal sum)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            var backClick = new TapGestureRecognizer();
            backClick.Tapped += async (s, e) => { _ = await Navigation.PopAsync(); };
            BackStackLayout.GestureRecognizers.Add(backClick);
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
                        BackStackLayout.Margin = new Thickness(-5, 15, 0, 0);
                    }

                    break;
                default:
                    break;
            }

            SetText();

            GetPayLink(ident, sum);
        }

        async void GetPayLink(string ident, decimal sum)
        {
            await Settings.StartProgressBar();
            PayService payLink = await server.GetPayLink(ident, sum);
            if (payLink.payLink != null)
            {
                webView.Source = payLink.payLink;
            }
            else
            {
                Loading.Instance.Hide();
                await DisplayAlert("Ошибка", payLink.Error, "OK");
                await Navigation.PopAsync();
            }
        }

        void SetText()
        {
            UkName.Text = Settings.MobileSettings.main_name;
            LabelPhone.Text = "+" + Settings.Person.Phone;
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
                    await DisplayAlert("Ошибка", result.error, "OK");
                    await Navigation.PopAsync();
                }
                else
                {
                    await DisplayAlert("Успешно", result.message, "OK");
                    await Navigation.PopToRootAsync();
                }
            });
        }
    }
}