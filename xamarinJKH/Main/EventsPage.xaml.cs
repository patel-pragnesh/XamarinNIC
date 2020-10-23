using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Plugin.FirebaseCrashlytics;
using Plugin.Messaging;
using Rg.Plugins.Popup.Services;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PancakeView;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.Xaml;
using xamarinJKH.Additional;
using xamarinJKH.CustomRenderers;
using xamarinJKH.DialogViews;
using xamarinJKH.InterfacesIntegration;
using xamarinJKH.News;
using xamarinJKH.Questions;
using xamarinJKH.Server;
using xamarinJKH.Server.RequestModel;
using xamarinJKH.Shop;
using xamarinJKH.Tech;
using xamarinJKH.Utils;
using xamarinJKH.VideoStreaming;
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
            Analytics.TrackEvent("События");
            Analytics.TrackEvent(Newtonsoft.Json.JsonConvert.SerializeObject(Settings.Person.Accounts));
            BindingContext = viewModel = new EventsPageViewModel();
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

            var techSend = new TapGestureRecognizer();
            techSend.Tapped += async (s, e) =>
            {
                if (Settings.MobileSettings.сheckCrashSystem)
                    Crashes.GenerateTestCrash();
               
                await PopupNavigation.Instance.PushAsync(new TechDialog());
            };
            LabelTech.GestureRecognizers.Add(techSend); 
            
            var profile = new TapGestureRecognizer();
            profile.Tapped += async (s, e) =>
            {
                if (Navigation.NavigationStack.FirstOrDefault(x => x is ProfilePage) == null)
                    await Navigation.PushAsync(new ProfilePage());
            };

            // if (AppInfo.PackageName == "rom.best.UkComfort" || AppInfo.PackageName == "sys_rom.ru.comfort_uk_app")
            IconViewProfile.IsVisible = true;

            IconViewProfile.GestureRecognizers.Add(profile);

            var call = new TapGestureRecognizer();
            call.Tapped += async (s, e) =>
            {
                if (Settings.Person.Phone != null)
                {
                    IPhoneCallTask phoneDialer;
                    phoneDialer = CrossMessaging.Current.PhoneDialer;
#if DEBUG
                    //Settings.Person.companyPhone = null;
#endif
                    try
                    {

                        if (phoneDialer.CanMakePhoneCall && !string.IsNullOrWhiteSpace(Settings.Person.companyPhone))
                            phoneDialer.MakePhoneCall(System.Text.RegularExpressions.Regex.Replace(Settings.Person.companyPhone, "[^+0-9]", ""));
                    }
                    catch(Exception ex)
                    {

                    }
                    
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
            MessagingCenter.Subscribe<Object>(this, "UpdateEvents", (sender) =>
            {
                viewModel.LoadData.Execute(null);

                if (Xamarin.Essentials.Connectivity.NetworkAccess != Xamarin.Essentials.NetworkAccess.Internet)
                {
                    Device.BeginInvokeOnMainThread(async () => await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorNoInternet, "OK"));
                    return;
                }


            }); 
            MessagingCenter.Subscribe<Object>(this, "StartTech", async (sender) =>
            {
                await PopupNavigation.Instance.PushAsync(new TechDialog());
            });
            MessagingCenter.Subscribe<Object>(this, "ChangeThemeCounter", (sender) =>
            {

                OSAppTheme currentTheme = Application.Current.RequestedTheme;
                var colors = new Dictionary<string, string>();
                var arrowcolor = new Dictionary<string, string>();
                if (currentTheme == OSAppTheme.Light || currentTheme == OSAppTheme.Unspecified)
                {
                    colors.Add("#000000", ((Color)Application.Current.Resources["MainColor"]).ToHex());
                    arrowcolor.Add("#000000", "#494949");
                }
                else
                {
                    colors.Add("#000000", "#FFFFFF");
                    arrowcolor.Add("#000000", "#FFFFFF");
                }
                IconViewLogin.ReplaceStringMap = colors;
                IconViewTech.ReplaceStringMap = colors;
            });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            MessagingCenter.Send<Object>(this, "ChangeThemeCounter");
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
            startNews.Tapped += async (s, e) => { if (Navigation.NavigationStack.FirstOrDefault(x => x is NewsPage) == null) await Navigation.PushAsync(new NewsPage()); };
            FrameNews.GestureRecognizers.Add(startNews);
        }

        private void StartQuestions()
        {
            var startQuest = new TapGestureRecognizer();
            startQuest.Tapped += async (s, e) => { if (Navigation.NavigationStack.FirstOrDefault(x => x is QuestionsPage) == null)  await Navigation.PushAsync(new QuestionsPage()); };
            FrameQuestions.GestureRecognizers.Add(startQuest);
        }

        private async void StartOffers()
        {
            var startAdditional = new TapGestureRecognizer();
            startAdditional.Tapped += async (s, e) => { if (Navigation.NavigationStack.FirstOrDefault(x => x is AdditionalPage) == null) await Navigation.PushAsync(new AdditionalPage()); };
            FrameOffers.GestureRecognizers.Add(startAdditional);
        }

        private void StartNotification()
        {
            var startNotif = new TapGestureRecognizer();
            startNotif.Tapped += async (s, e) => { if (Navigation.NavigationStack.FirstOrDefault(x => x is NotificationsPage) == null)  await Navigation.PushAsync(new NotificationsPage()); };
            FrameNotification.GestureRecognizers.Add(startNotif);
        }

        private void StartOSS()
        {
            var startOSSTGR = new TapGestureRecognizer();
            startOSSTGR.Tapped += async (s, e) => { if (Navigation.NavigationStack.FirstOrDefault(x => x is OSSMain) == null) await Navigation.PushAsync(new OSSMain()); };
            // startOSSTGR.Tapped += async (s, e) => { await Navigation.PushAsync(new OSSAuth()); };
            FrameOSS.GestureRecognizers.Add(startOSSTGR);
            if (!Settings.MobileSettings.enableOSS || !Settings.Person.accessOSS)
            {
                FrameOSS.IsVisible = false;
            }

            if (RestClientMP.SERVER_ADDR.ToLower().Contains("water"))
            {
                FrameOSS.IsVisible = true;
            }
        }

        private void StartShop()
        {
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

            // LabelTech.TextColor = (Color)Application.Current.Resources["MainColor"];
            // IconViewTech.Foreground = (Color)Application.Current.Resources["MainColor"];
        }

        void SetColor()
        {
            Color hexColor = (Color) Application.Current.Resources["MainColor"];
            //IconViewLogin.SetAppThemeColor(IconView.ForegroundProperty, hexColor, Color.White);
            //IconViewTech.SetAppThemeColor(IconView.ForegroundProperty, hexColor, Color.White);
            Pancake.SetAppThemeColor(PancakeView.BorderColorProperty, hexColor, Color.Transparent);
            PancakeViewIcon.SetAppThemeColor(PancakeView.BorderColorProperty, hexColor, Color.Transparent);if (Device.RuntimePlatform == Device.iOS){ if (AppInfo.PackageName == "rom.best.saburovo" || AppInfo.PackageName == "sys_rom.ru.tsg_saburovo"){PancakeViewIcon.Padding = new Thickness(0);}}
            LabelTech.SetAppThemeColor(Label.TextColorProperty, hexColor, Color.White);

            FrameNews.SetAppThemeColor(MaterialFrame.BorderColorProperty, hexColor, Color.White);
            FrameQuestions.SetAppThemeColor(MaterialFrame.BorderColorProperty, hexColor, Color.White);
            FrameOffers.SetAppThemeColor(MaterialFrame.BorderColorProperty, hexColor, Color.White);
            FrameOSS.SetAppThemeColor(MaterialFrame.BorderColorProperty, hexColor, Color.White);
            FrameNotification.SetAppThemeColor(MaterialFrame.BorderColorProperty, hexColor, Color.White);
        }

        private async void Cameras(object sender, EventArgs e)
        {
            if (Navigation.NavigationStack.FirstOrDefault(x => x is CameraListPage) == null)
                await Navigation.PushAsync(new CameraListPage());
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
        bool _showCameras;
        public bool ShowCameras
        {
            get
            {
                try
                {
                    MobileMenu mobileMenu = Settings.MobileSettings.menu.Find(x => x.name_app == "Web-камеры");
                    return mobileMenu != null && mobileMenu.visible != 0 &&
                           Device.RuntimePlatform == "Android";
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return true;
                }
                
            }
        }

        public Command LoadData { get; set; }
        public Command CountNew { get; set; }

        int announcementsCount;
        public int AnnounsmentsCount
        {
            get => announcementsCount;
            set
            {
                announcementsCount = value;
                OnPropertyChanged(nameof(AnnounsmentsCount));
            }
        }

        int pollsCount;
        public int PollsCount
        {
            get => pollsCount;
            set
            {
                pollsCount = value;
                OnPropertyChanged(nameof(PollsCount));
            }
        }
        public EventsPageViewModel()
        {
            LoadData = new Command(async () =>
            {
                var server = new RestClientMP();
                var data = Settings.EventBlockData;
                data = await server.GetEventBlockData();
                Settings.EventBlockData = data;
                if (data != null)
                {
                    if (data.News != null)
                        ShowNews =  data.News.Count != 0;
                    if (data.Polls != null)
                        ShowPolls = data.Polls.Count != 0 && Settings.QuestVisible;
                    if (data.AdditionalServices != null)
                        ShowAdditionalServices = data.AdditionalServices.Count != 0 && Settings.AddVisible;
                    if (data.Announcements != null)
                        ShowAnnouncements = data.Announcements.Count != 0 && Settings.NotifVisible;
                }
                ShowAdditionalServices = false;
                CountNew.Execute(null);
            });
            CountNew = new Command(() =>
            {
                AnnounsmentsCount = Settings.EventBlockData.Announcements.Where(x => !x.IsReaded).Count();
                PollsCount = Settings.EventBlockData.Polls.Where(x => !x.IsReaded).Count();
            });
        }
    }
}