using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Plugin.Messaging;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamarinJKH.Apps;
using xamarinJKH.Server;
using xamarinJKH.InterfacesIntegration;
using xamarinJKH.Server.RequestModel;
using xamarinJKH.Tech;
using xamarinJKH.Utils;
using System.Threading;
using Xamarin.Forms.PancakeView;
using xamarinJKH.ViewModels.Main;

namespace xamarinJKH.Main
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AppsPage : ContentPage
    {
        public List<RequestInfo> RequestInfos { get; set; }
        public List<RequestInfo> RequestInfosAlive { get; set; }
        public List<RequestInfo> RequestInfosClose { get; set; }
        private RequestList _requestList;
        private RestClientMP _server = new RestClientMP();
        private bool _isRefreshing = false;
        public Color hex { get; set; }
        public AppsPageViewModel viewModel { get; set; }
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
                    try
                    {
                        CancellationTokenSource.Cancel();
                        CancellationTokenSource.Dispose();
                    }
                    catch
                    {

                    }

                    //await RefreshData();
                    StartAutoUpdate();

                    IsRefreshing = false;
                });
            }
        }
        Task UpdateTask;

        public async Task ShowMessage(string message,
            string title,
            string buttonText,
            Action afterHideCallback)
        {
            await DisplayAlert(
                title,
                message,
                buttonText);

            afterHideCallback?.Invoke();
        }

        bool showNoInetWindow = true;

        void StartAutoUpdate()
        {
            CancellationTokenSource = new CancellationTokenSource();
            this.CancellationToken = CancellationTokenSource.Token;
            UpdateTask = null;
            UpdateTask = new Task(async () =>
            {
                while (!this.CancellationToken.IsCancellationRequested)
                {
                    if (Xamarin.Essentials.Connectivity.NetworkAccess != Xamarin.Essentials.NetworkAccess.Internet)
                    {
                        Device.BeginInvokeOnMainThread(async () => 
                        {
                            if (showNoInetWindow)
                            {
                                showNoInetWindow = false;

                                   await ShowMessage(AppResources.ErrorNoInternet, AppResources.ErrorTitle, "OK", () =>
                                {
                                    showNoInetWindow = true;
                                    //await ShowMessage("OK was pressed", "Message", "OK", null);
                                });
                            }
                            //await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorNoInternet, "OK");

                            await Task.Delay(TimeSpan.FromSeconds(5));
                        });

                    }
                    await viewModel.UpdateTask();
                    await Task.Delay(TimeSpan.FromSeconds(5));
                }
                return;
            }, this.CancellationToken);
            UpdateTask.Start();
        }

        static bool inUpdateNow = false;
        public async Task RefreshData()
        {

            try
            {
                if (inUpdateNow)
                    return;
                inUpdateNow = true;


                await getAppsAsync();
                //Device.BeginInvokeOnMainThread(() =>
                //{
                //    additionalList.ItemsSource = null;
                //    additionalList.ItemsSource = RequestInfos;
                //});

                inUpdateNow = false;
            }
            catch (Exception e)
            {
                inUpdateNow = false;
            }
            finally
            {
                inUpdateNow = false;
            }


        }

        public CancellationTokenSource CancellationTokenSource { get; set; }
        public CancellationToken CancellationToken { get; set; }

        public AppsPage()
        {
            InitializeComponent();
            hex = Color.FromHex(Settings.MobileSettings.color);
            BindingContext = viewModel = new AppsPageViewModel();
            
            NavigationPage.SetHasNavigationBar(this, false);
            MessagingCenter.Subscribe<Object>(this, "AutoUpdate", (sender) =>
            {
                StartAutoUpdate();
                viewModel.LoadRequests.Execute(null);
            });


            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    int statusBarHeight = DependencyService.Get<IStatusBar>().GetHeight();
                    Pancake.Padding = new Thickness(0, statusBarHeight, 0, 0);
                    if (Xamarin.Essentials.DeviceDisplay.MainDisplayInfo.Width < 700)
                        LabelSwitch.FontSize = 12;

                    FrameBtnAdd.IsVisible = false;
                    FrameBtnAddIos.IsVisible = true;

                    break;
                case Device.Android:
                    FrameBtnAdd.IsVisible = true;
                    FrameBtnAddIos.IsVisible = false;

                    break;
                default:
                    break;
            }

           
            var techSend = new TapGestureRecognizer();
            techSend.Tapped += async (s, e) => { await Navigation.PushAsync(new TechSendPage()); };
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
            var addClick = new TapGestureRecognizer();
            addClick.Tapped += async (s, e) => { startNewApp(FrameBtnAdd, null); };
            FrameBtnAdd.GestureRecognizers.Add(addClick);
            var addClickIOS = new TapGestureRecognizer();
            addClickIOS.Tapped += async (s, e) => { startNewApp(FrameBtnAddIos, null); };
            FrameBtnAddIos.GestureRecognizers.Add(addClickIOS);
            SetText();
            //getApps();
            additionalList.BackgroundColor = Color.Transparent;
            additionalList.Effects.Add(Effect.Resolve("MyEffects.ListViewHighlightEffect"));
            this.CancellationTokenSource = new CancellationTokenSource();

            MessagingCenter.Subscribe<Object, int>(this, "OpenApp", async (sender, args) =>
            {
                await RefreshData();
                var request = RequestInfos?.Find(x => x.ID == args);
                if (request != null)
                {
                    try
                    {
                        CancellationTokenSource.Cancel();
                        CancellationTokenSource.Dispose();
                    }
                    catch
                    {

                    }
                    Device.BeginInvokeOnMainThread(async () => await Navigation.PushAsync(new AppPage(request)));
                }
            });

            //            PropertyChanged = "change"

            SwitchApp.Toggled += SwitchApp_Toggled; 
        }

        private void SwitchApp_Toggled(object sender, ToggledEventArgs e)
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

        public AppsPage(string app_id) : base ()
        {
            var request = RequestInfos.Find(x => x.ID == int.Parse(app_id));
            if (request != null)
            {
                Device.BeginInvokeOnMainThread(async () => await Navigation.PushAsync(new AppPage(request)));
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            try
            {

                CancellationTokenSource.Cancel();
                CancellationTokenSource.Dispose();
                
            }
            catch
            {

            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        async void SyncSetup()
        {
            //Device.BeginInvokeOnMainThread(() =>
            //{
            // Assuming this function needs to use Main/UI thread to move to your "Main Menu" Page
            while (true)
            {
                await Task.Delay(TimeSpan.FromSeconds(5));
                await RefreshData();
            }
            //});
        }

        void SetText()
        {
            UkName.Text = Settings.MobileSettings.main_name;
            LabelPhone.Text = "+" + Settings.Person.companyPhone.Replace("+", "");
            SwitchApp.OnColor = hex;
            IconAddApp.Foreground = Color.White;
            Color hexColor = (Color) Application.Current.Resources["MainColor"];
            IconViewLogin.SetAppThemeColor(IconView.ForegroundProperty, hexColor, Color.White);
            IconViewTech.SetAppThemeColor(IconView.ForegroundProperty, hexColor, Color.White);
            Pancake.SetAppThemeColor(PancakeView.BorderColorProperty, hexColor, Color.Transparent);
            PancakeViewIcon.SetAppThemeColor(PancakeView.BorderColorProperty, hexColor, Color.Transparent);
            GoodsLayot.SetAppThemeColor(PancakeView.BorderColorProperty, hexColor, Color.Transparent);
            LabelTech.SetAppThemeColor(Label.TextColorProperty, hexColor, Color.White);
        }

        async Task getAppsAsync()
        {
            if (Xamarin.Essentials.Connectivity.NetworkAccess != Xamarin.Essentials.NetworkAccess.Internet)
            {
                Device.BeginInvokeOnMainThread(async () => await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorNoInternet, "OK"));
                return;
            }
            _requestList = await _server.GetRequestsList();
            if (_requestList.Error == null)
            {
                if(Settings.UpdateKey != _requestList.UpdateKey)
                {
                    RequestInfos = null;
                    setCloses(_requestList.Requests);
                    Settings.UpdateKey = _requestList.UpdateKey;
                    this.BindingContext = this;                    
                }                
            }
            else
            {
                await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorAppsInfo, "OK");
            }
        }

        async void getApps()
        {
            _requestList = await _server.GetRequestsList();
            if (_requestList.Error == null)
            {
                setCloses(_requestList.Requests);
                Settings.UpdateKey = _requestList.UpdateKey;
                this.BindingContext = this;
            }
            else
            {
                await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorAppsInfo, "OK");
            }
        }

        void setCloses(List<RequestInfo> infos)
        {
            RequestInfosAlive = new List<RequestInfo>();
            RequestInfosClose = new List<RequestInfo>();
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

            Device.BeginInvokeOnMainThread(() =>
            {
                additionalList.ItemsSource = null;
                additionalList.ItemsSource = RequestInfos;
            });
        }

        private async void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            RequestInfo select = e.Item as RequestInfo;
            await Navigation.PushAsync(new AppPage(select));
        }

        private async void startNewApp(object sender, EventArgs e)
        {
            if (Settings.Person.Accounts.Count > 0)
            {
                if (Settings.TypeApp.Count > 0)
                {
                    await Navigation.PushAsync(new NewAppPage());
                }
                else
                {
                    await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorAppsNoTypes, "OK");
                }
            }
            else
            {
                await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorAppsNoIdent, "OK");
            }
        }

        private void change(object sender, PropertyChangedEventArgs e)
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