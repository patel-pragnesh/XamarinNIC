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
        EventsPageViewModel viewModel { get; set; }
        public EventsPage()
        {
            InitializeComponent();
            BindingContext = viewModel = new EventsPageViewModel();
            NavigationPage.SetHasNavigationBar(this, false);
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    int statusBarHeight = DependencyService.Get<IStatusBar>().GetHeight();                    
                    Pancake.Padding = new Thickness(0,statusBarHeight,0,0);
                    //BackgroundColor = Color.White;                    
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
                        phoneDialer.MakePhoneCall(Settings.Person.companyPhone);
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
            MessagingCenter.Subscribe<Object>(this, "UpdateEvents", (sender) => viewModel.LoadData.Execute(null));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            //new Task(SyncSetup).Start(); // This could be an await'd task if need be
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
                //funk();
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
            if (!Settings.MobileSettings.enableOSS || !Settings.Person.AccessOSS)
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

    public class EventsPageViewModel : xamarinJKH.ViewModels.BaseViewModel
    {
        bool _showNews;
        public bool ShowNews
        {
            get => _showNews;
            set
            {
                _showNews = value;
                OnPropertyChanged("ShowNews");
            }
        }

        bool _showPolls;
        public bool ShowPolls
        {
            get => _showPolls;
            set
            {
                _showPolls = value;
                OnPropertyChanged("ShowPolls");
            }
        }
        bool _showAnnouncements;
        public bool ShowAnnouncements
        {
            get => _showAnnouncements;
            set
            {
                _showAnnouncements = value;
                OnPropertyChanged("ShowAnnouncements");
            }
        }
        bool _showAddService;
        public bool ShowAdditionalServices
        {
            get => _showAddService;
            set
            {
                _showAddService = value;
                OnPropertyChanged("ShowAdditionalServices");
            }
        }
        public Command LoadData { get; set; }
        public EventsPageViewModel()
        {
            LoadData = new Command(async () =>
            {
                var server = new RestClientMP();
                var data = Settings.EventBlockData;
                data = await server.GetEventBlockData();
                ShowNews = data.News.Count == 0;
                ShowPolls = data.Polls.Count == 0;
                ShowAdditionalServices = data.AdditionalServices.Count == 0;
                ShowAnnouncements = data.Announcements.Count == 0;
            });
        }
    }
}