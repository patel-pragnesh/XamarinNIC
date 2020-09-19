using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.PancakeView;
using Xamarin.Forms.Xaml;
using xamarinJKH.AppsConst;
using xamarinJKH.DialogViews;
using xamarinJKH.Server;
using xamarinJKH.InterfacesIntegration;
using xamarinJKH.Server.RequestModel;
using xamarinJKH.Tech;
using xamarinJKH.Utils;
using xamarinJKH.ViewModels;

namespace xamarinJKH.MainConst
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AppsConstPage : ContentPage
    {
        public ObservableCollection<RequestInfo> RequestInfos { get; set; }
        public ObservableCollection<RequestInfo> RequestInfosAlive { get; set; }
        public ObservableCollection<RequestInfo> RequestInfosClose { get; set; }
        private RequestList _requestList;
        private RestClientMP _server = new RestClientMP();
        private bool _isRefreshing = false;
        public Color hex { get; set; }
        
        private HashSet<RequestInfo> CheckRequestInfos = new HashSet<RequestInfo>();
        
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
        
        
        public async Task RefreshData()
        {
            getApps();
            additionalList.ItemsSource = null;
            additionalList.ItemsSource = RequestInfos;
        }

        public AppsConstPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            this.BindingContext = this;

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    int statusBarHeight = DependencyService.Get<IStatusBar>().GetHeight();
                    Pancake.Padding = new Thickness(0, statusBarHeight, 0, 0);
                    break;
                default:
                    break;
            }

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    //BackgroundColor = Color.White;
                    // ImageFon.Margin = new Thickness(0, 0, 0, 0);
                    // StackLayout.Margin = new Thickness(0, 33, 0, 0);
                    // IconViewNameUk.Margin = new Thickness(0, 33, 0, 0);
                    break;
                case Device.Android:
                    // double or = Math.Round(((double)App.ScreenWidth / (double)App.ScreenHeight), 2);
                    // if (Math.Abs(or - 0.5) < 0.02)
                    // {
                    //     ScrollViewContainer.Margin = new Thickness(0, 0, 0, -125);
                    //     BackStackLayout.Margin = new Thickness(5, 25, 0, 0);
                    // }

                    break;
                default:
                    break;
            }
            var techSend = new TapGestureRecognizer();
            techSend.Tapped += async (s, e) => {     await PopupNavigation.Instance.PushAsync(new TechDialog()); };
            LabelTech.GestureRecognizers.Add(techSend);
            
            var addClick = new TapGestureRecognizer();
            addClick.Tapped += async (s, e) => { startNewApp(FrameBtnAdd, null); };
            FrameBtnAdd.GestureRecognizers.Add(addClick);
            hex = Color.FromHex(Settings.MobileSettings.color);
            SetText();
            additionalList.BackgroundColor = Color.Transparent;
            additionalList.Effects.Add(Effect.Resolve("MyEffects.ListViewHighlightEffect"));
            MessagingCenter.Subscribe<Object>(this, "UpdateAppCons", (sender) => RefreshData());
            // Assuming this function needs to use Main/UI thread to move to your "Main Menu" Page
            getApps();
            ChangeTheme = new Command(async () =>
            {
                SetAdminName();
            });
            MessagingCenter.Subscribe<Object>(this, "ChangeAdminApp", (sender) => ChangeTheme.Execute(null));
            MessagingCenter.Subscribe<Object, string>(this, "ChechApp", async (sender, args) =>
            {
                RequestInfo requestInfo = getRequestInfo(args);
                if (requestInfo != null)
                {
                    CheckRequestInfos.Add(requestInfo);
                }

                IsVisibleFunction();
            });  
            MessagingCenter.Subscribe<Object, string>(this, "ChechDownApp", async (sender, args) =>
            {
                RequestInfo requestInfo = getRequestInfo(args);
                if (requestInfo != null)
                {
                    CheckRequestInfos.Remove(requestInfo);
                }

                IsVisibleFunction();
            });

        }

        private RequestInfo getRequestInfo(string number)
        {
            foreach (var each in RequestInfos)
            {
                if (number.Equals(each.RequestNumber))
                {
                    return each;
                }
            }

            return null;
        }

        private void IsVisibleFunction()
        {
            if (CheckRequestInfos.Count > 0)
            {
                StackLayoutFunction.IsVisible = true;
                StackLayoutBot.IsVisible = false;
            }
            else
            {
                StackLayoutFunction.IsVisible = false;
                StackLayoutBot.IsVisible = true;
            }
        }
        
        private void SetAdminName()
        {
            FormattedString formattedName = new FormattedString();
            OSAppTheme currentTheme = Application.Current.RequestedTheme;
            //if (Xamarin.Essentials.DeviceInfo.Platform == Xamarin.Essentials.DevicePlatform.iOS)
            //    currentTheme = OSAppTheme.Dark;
            formattedName.Spans.Add(new Span
            {
                Text = Settings.Person.FIO,
                TextColor = currentTheme.Equals(OSAppTheme.Dark) ? Color.White : Color.Black,
                FontAttributes = FontAttributes.Bold,
                FontSize = 16
            });
            formattedName.Spans.Add(new Span
            {
                Text = AppResources.GoodDay,
                TextColor = currentTheme.Equals(OSAppTheme.Dark) ? Color.White : Color.Black,
                FontAttributes = FontAttributes.None,
                FontSize = 16
            });
            LabelName.FormattedText = formattedName;
        }

        public Command ChangeTheme { get; set; }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // new Task(SyncSetup).Start(); // This could be an await'd task if need be
        }

        async void SyncSetup()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                // Assuming this function needs to use Main/UI thread to move to your "Main Menu" Page
                RefreshData();

            });
        }

        void SetText()
        {
            UkName.Text = Settings.MobileSettings.main_name;
            SetAdminName();
            SwitchApp.OnColor = hex;
            FrameBtnAdd.BackgroundColor = hex;
            //IconAddApp.Foreground = Color.White;
            
            Color hexColor = (Color) Application.Current.Resources["MainColor"];
            //IconViewLogin.SetAppThemeColor(IconView.ForegroundProperty, hexColor, Color.White);
            //IconViewTech.SetAppThemeColor(IconView.ForegroundProperty, hexColor, Color.White);
            Pancake.SetAppThemeColor(PancakeView.BorderColorProperty, hexColor, Color.Transparent);
            PancakeViewIcon.SetAppThemeColor(PancakeView.BorderColorProperty, hexColor, Color.Transparent);
            GoodsLayot.SetAppThemeColor(PancakeView.BorderColorProperty, hexColor, Color.Transparent);
            LabelTech.SetAppThemeColor(Label.TextColorProperty, hexColor, Color.White);
        }


        async void getApps()
        {

            if (Xamarin.Essentials.Connectivity.NetworkAccess != Xamarin.Essentials.NetworkAccess.Internet)
            {
                Device.BeginInvokeOnMainThread(async () => await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorNoInternet, "OK"));
                return;
            }
            _requestList = await _server.GetRequestsListConst();
            if (_requestList.Error == null)
            {
                setCloses(_requestList.Requests);
                Settings.UpdateKey = _requestList.UpdateKey;
            }
            else
            {
                await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorAppsInfo, "OK");
            }
        }

        void setCloses(List<RequestInfo> infos)
        {
            RequestInfosAlive = new ObservableCollection<RequestInfo>();
            RequestInfosClose = new ObservableCollection<RequestInfo>();
            foreach (var each in infos)
            {
                if (each.IsClosed)
                {
                    RequestInfosClose.Add(each);
                }
                else
                {
                    RequestInfosAlive.Add(each);
                }
            }

            if (SwitchApp.IsToggled)
            {
                RequestInfos = RequestInfosClose;
            }
            else
            {
                RequestInfos = RequestInfosAlive;
            }

            if (RequestInfos != null)
            {
                BindingContext = this;
                additionalList.ItemsSource = null;
                additionalList.ItemsSource = RequestInfos;
            }
        }

        private async void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            RequestInfo select = e.Item as RequestInfo;
            await Navigation.PushAsync(new AppConstPage(select));
        }

        private async void startNewApp(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NewAppConstPage(this));
        }

        private async void change(object sender, PropertyChangedEventArgs e)
        {
            if (SwitchApp.IsToggled)
            {
                RequestInfos = RequestInfosClose;
            }
            else
            {
                RequestInfos = RequestInfosAlive;
            }
            additionalList.ItemsSource = null;
            additionalList.ItemsSource = RequestInfos;
        }
    }
}