using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.FirebaseCrashlytics;
using Plugin.Messaging;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.Xaml;
using xamarinJKH.Additional;
using xamarinJKH.InterfacesIntegration;
using xamarinJKH.News;
using xamarinJKH.Questions;
using xamarinJKH.Server;
using xamarinJKH.Shop;
using xamarinJKH.Tech;
using xamarinJKH.Utils;
using Application = Xamarin.Forms.Application;
using NavigationPage = Xamarin.Forms.NavigationPage;
using VisualElement = Xamarin.Forms.VisualElement;

namespace xamarinJKH.Main
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EventsPage : ContentPage
    {
        public EventsPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    BackgroundColor = Color.White;
                    // ImageFon.Margin = new Thickness(0, 0, 0, 0);
                    // StackLayout.Margin = new Thickness(0, 33, 0, 0);
                    // IconViewNameUk.Margin = new Thickness(0, 33, 0, 0);
                    if (Application.Current.MainPage.Height < 800)
                    {
                        ScrollViewContainer.Margin = new Thickness(10, -180, 10, 0);
                        // BackStackLayout.Margin = new Thickness(5, 15, 0, 0);
                    }
                    else
                    {
                        ScrollViewContainer.Margin = new Thickness(10, -185, 10, 0);
                        // BackStackLayout.Margin = new Thickness(5, 35, 0, 0);
                    }

                    break;
                case Device.Android:
                    double or = Math.Round(((double) App.ScreenWidth / (double) App.ScreenHeight), 2);
                    // if (Math.Abs(or - 0.5) < 0.02)
                    // {
                    //     ScrollViewContainer.Margin = new Thickness(10, -135, 10, 0);
                    //     // BackStackLayout.Margin = new Thickness(5, 25, 0, 0);
                    // }
                    break;
                default:
                    break;
            }

            var techSend = new TapGestureRecognizer();
            techSend.Tapped += async (s, e) =>
            {
                if (Settings.MobileSettings.сheckCrashSystem)
                    CrossFirebaseCrashlytics.Current.Crash();
                await Navigation.PushAsync(new TechSendPage());
            };
            LabelTech.GestureRecognizers.Add(techSend);
            
            var call = new TapGestureRecognizer();
            call.Tapped += async (s, e) =>
            {
                if (Settings.Person.Phone != null)
                {
                    IPhoneCallTask phoneDialer;
                    phoneDialer = CrossMessaging.Current.PhoneDialer;
                    if (phoneDialer.CanMakePhoneCall) 
                        phoneDialer.MakePhoneCall(Settings.Person.Phone);
                }

            
            };
            LabelPhone.GestureRecognizers.Add(call);
            SetText();
            SetColor();
            StartNews();
            StartNotification();
            StartOffers();
            StartQuestions();
            // SetVisibleControls();
            StartOSS();
            CrossFirebaseCrashlytics.Current.SetUserIdentifier(Settings.Person.Login);
            CrossFirebaseCrashlytics.Current.SetUserName(Settings.Person.FIO);
            CrossFirebaseCrashlytics.Current.SetUserEmail(Settings.Person.Email);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            new Task(SyncSetup).Start(); // This could be an await'd task if need be
        }

        async void SyncSetup()
        {
            Device.BeginInvokeOnMainThread(SetVisibleControls);
        }

        async void SetVisibleControls()
        {
            RestClientMP server = new RestClientMP();
            Settings.EventBlockData = await server.GetEventBlockData();
            setVisible(Settings.EventBlockData.News.Count == 0, StartNews, FrameNews);
            setVisible(Settings.EventBlockData.Polls.Count == 0, StartQuestions, FrameQuestions);
            setVisible(Settings.EventBlockData.Announcements.Count == 0, StartNotification, FrameNotification);
            setVisible(Settings.EventBlockData.AdditionalServices.Count == 0, StartOffers, FrameOffers);
            setVisible(false, StartShop, FrameShop);

            //для ОСС
            setVisible(false, StartOSS, FrameOSS);
        }

        void setVisible(bool visible, Action funk, VisualElement frame)
        {
            if (visible)
            {
                frame.IsVisible = false;
            }
            else
            {
                // funk();
            }
        }

        private void StartNews()
        {
            var startNews = new TapGestureRecognizer();
            startNews.Tapped += async (s, e) => { await Navigation.PushAsync(new NewsPage()); };
            FrameNews.GestureRecognizers.Add(startNews);
        }

        private void StartQuestions()
        {
            var startQuest = new TapGestureRecognizer();
            startQuest.Tapped += async (s, e) => { await Navigation.PushAsync(new QuestionsPage()); };
            FrameQuestions.GestureRecognizers.Add(startQuest);
        }

        private async void StartOffers()
        {
            var startAdditional = new TapGestureRecognizer();
            startAdditional.Tapped += async (s, e) => { await Navigation.PushAsync(new AdditionalPage()); };
            FrameOffers.GestureRecognizers.Add(startAdditional);
        }

        private void StartNotification()
        {
            var startNotif = new TapGestureRecognizer();
            startNotif.Tapped += async (s, e) => { await Navigation.PushAsync(new NotificationsPage()); };
            FrameNotification.GestureRecognizers.Add(startNotif);
        }

        private void StartOSS()
        {
            var startOSSTGR = new TapGestureRecognizer();
            //startOSSTGR.Tapped += async (s, e) => { await Navigation.PushAsync(new OSSMain()); };
            startOSSTGR.Tapped += async (s, e) => { await Navigation.PushAsync(new OSSAuth()); };
            FrameOSS.GestureRecognizers.Add(startOSSTGR);
            if (!Settings.MobileSettings.enableOSS)
            {
                FrameOSS.IsVisible = false;
            }
        }

        private void StartShop()
        {
        }

        void SetText()
        {
            UkName.Text = Settings.MobileSettings.main_name;
            LabelPhone.Text = "+" + Settings.Person.companyPhone.Replace("+","");
            // LabelTech.TextColor = Color.FromHex(Settings.MobileSettings.color);
            // IconViewTech.Foreground = Color.FromHex(Settings.MobileSettings.color);
        }

        void SetColor()
        {
            Color hexColor = Color.FromHex(Settings.MobileSettings.color);
            // IconViewLogin.Foreground = hexColor;
            // IconViewTech.Foreground = hexColor;
            IconViewNotification.Foreground = hexColor;
            IconViewShop.Foreground = hexColor;
            IconViewForvardShop.Foreground = hexColor;
            IconViewForvardNotification.Foreground = hexColor;
            IconViewNews.Foreground = hexColor;
            IconViewForvardNews.Foreground = hexColor;
            IconViewQuestions.Foreground = hexColor;
            IconViewForvardQuestions.Foreground = hexColor;
            IconViewOffers.Foreground = hexColor;
            IconViewForvardOffers.Foreground = hexColor;


            IconViewOss.Foreground = hexColor;
            IconViewForvardOss.Foreground = hexColor;


            // LabelTech.TextColor = hexColor;
        }
    }
}