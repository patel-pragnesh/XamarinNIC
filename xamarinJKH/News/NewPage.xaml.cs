using System;
using System.Collections.Generic;
using System.IO;
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
using xamarinJKH.Additional;
using xamarinJKH.DialogViews;
using xamarinJKH.InterfacesIntegration;
using xamarinJKH.Server;
using xamarinJKH.Server.RequestModel;
using xamarinJKH.Tech;
using xamarinJKH.Utils;
using xamarinJKH.Converters;
using xamarinJKH.Main;

namespace xamarinJKH.News
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewPage : ContentPage
    {
        private NewsInfo newsInfo;
        private NewsInfoFull newsInfoFull;
        private RestClientMP _server = new RestClientMP();
       
        public string hex { get; set; }

        public NewPage(NewsInfo newsInfo)
        {
            this.newsInfo = newsInfo;
            InitializeComponent();
            Analytics.TrackEvent("Новость");

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    int statusBarHeight = DependencyService.Get<IStatusBar>().GetHeight();
                    Pancake2.HeightRequest = statusBarHeight;
                    //Pancake2.Padding = new Thickness(0, statusBarHeight, 0, 0);
                    //BackgroundColor = Color.White;
                    break;
                default:
                    break;
            }
            
            NavigationPage.SetHasNavigationBar(this, false);

            var profile = new TapGestureRecognizer();
            profile.Tapped += async (s, e) =>
            {
                if (Navigation.NavigationStack.FirstOrDefault(x => x is ProfilePage) == null)
                    await Navigation.PushAsync(new ProfilePage());
            };
            IconViewProfile.GestureRecognizers.Add(profile);

            var techSend = new TapGestureRecognizer();
            techSend.Tapped += async (s, e) => {     await Navigation.PushAsync(new AppPage());};
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
            var backClick = new TapGestureRecognizer();
            backClick.Tapped += async (s, e) => {
                try
                {
                    _ = await Navigation.PopAsync();
                }
                catch { }
            };
            BackStackLayout.GestureRecognizers.Add(backClick);
            SetText();
            BindingContext = this;
            if (!newsInfo.IsReaded)
            {
                Task.Run(async () =>
                {
                    var response = await _server.SetNewReadFlag(newsInfo.ID);
                    MessagingCenter.Send<Object, int>(this, "SetEventsAmount", -1);
                    MessagingCenter.Send<Object>(this, "ReduceNews");
                });
            }
        }

        bool navigated;
        async void SetText()
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
            LabelTitle.Text = newsInfo.Header;
            LabelDate.Text = newsInfo.Created;
            newsInfoFull = await _server.GetNewsFull(newsInfo.ID.ToString());
            //LabelText.Text = newsInfoFull.Text;
            //LabelText.FormattedText = (FormattedString)(new HtmlTextConverter()).Convert(newsInfoFull.Text, null);
            MainText.Source = new HtmlWebViewSource { Html = newsInfoFull.Text };
            MainText.Navigated += (s, e) =>
            {
                if (!navigated)
                {
                    (s as WebView).Source = new UrlWebViewSource() { Url = e.Url };
                    navigated = true;
                }
            };
            Color hexColor = (Color) Application.Current.Resources["MainColor"];
            IconViewLogin.SetAppThemeColor(IconView.ForegroundProperty, hexColor, Color.White);
            IconViewTech.SetAppThemeColor(IconView.ForegroundProperty, hexColor, Color.Black);
            Pancake.SetAppThemeColor(PancakeView.BorderColorProperty, hexColor, Color.Transparent);
            PancakeViewIcon.SetAppThemeColor(PancakeView.BorderColorProperty, hexColor, Color.Transparent);{ if (AppInfo.PackageName == "rom.best.saburovo" || AppInfo.PackageName == "sys_rom.ru.tsg_saburovo"){PancakeViewIcon.Padding = new Thickness(0);}}
            LabelTech.SetAppThemeColor(Label.TextColorProperty, hexColor, Color.Black);
            if (Xamarin.Essentials.Connectivity.NetworkAccess != Xamarin.Essentials.NetworkAccess.Internet)
            {
                Device.BeginInvokeOnMainThread(async () => await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorNoInternet, "OK"));
                return;
            }

            Files.IsVisible = newsInfoFull.HasImage;
        }

        public async void OpenFile(object sender, EventArgs args)
        {
            Analytics.TrackEvent("Открытие файла новости");
            try
            {
                var link = RestClientMP.SERVER_ADDR + "/News/Image/" + newsInfoFull.ID.ToString();
                await AiForms.Dialogs.Dialog.Instance.ShowAsync(new NewFile(link));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
        }
    }
}