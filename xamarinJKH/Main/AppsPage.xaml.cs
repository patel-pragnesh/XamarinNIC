using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamarinJKH.Apps;
using xamarinJKH.Server;
using xamarinJKH.InterfacesIntegration;
using xamarinJKH.Server.RequestModel;
using xamarinJKH.Utils;

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

        public AppsPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    FrameBtnAdd.IsVisible = false;
                    FrameBtnAddIos.IsVisible = true;
                    BackgroundColor = Color.White;
                    ImageFon.Margin = new Thickness(0, 0, 0, 0);
                    StackLayout.Margin = new Thickness(0, 33, 0, 0);
                    IconViewNameUk.Margin = new Thickness(0, 33, 0, 0);
                    if (Application.Current.MainPage.Height < 800)
                    {
                        ScrollViewContainer.Margin = new Thickness(0, 0, 0, -170);
                        BackStackLayout.Margin = new Thickness(5, 15, 0, 0);
                    }
                    else
                    {
                        ScrollViewContainer.Margin = new Thickness(0, 0, 0, -180);
                        BackStackLayout.Margin = new Thickness(5, 35, 0, 0);
                    }                  
                    break;
                case Device.Android:
                    FrameBtnAdd.IsVisible = true;
                    FrameBtnAddIos.IsVisible = false;
                    ScrollViewContainer.Margin = new Thickness(0,0,0,-170);
                    BackStackLayout.Margin = new Thickness(5,25,0,0);
                    double or = Math.Round(((double) App.ScreenWidth / (double) App.ScreenHeight), 2);
                    if (Math.Abs(or - 0.5) < 0.02)
                    {
                        ScrollViewContainer.Margin = new Thickness(0, 0, 0, -125);
                        BackStackLayout.Margin = new Thickness(5, 25, 0, 0);
                    }

                    break;
                default:
                    break;
            }
            var addClick = new TapGestureRecognizer();
            addClick.Tapped += async (s, e) => { startNewApp(FrameBtnAdd, null); };
            FrameBtnAdd.GestureRecognizers.Add(addClick);
            var addClickIOS = new TapGestureRecognizer();
            addClickIOS.Tapped += async (s, e) => { startNewApp(FrameBtnAddIos, null); };
            FrameBtnAddIos.GestureRecognizers.Add(addClickIOS);
            hex = Color.FromHex(Settings.MobileSettings.color);
            SetText();
            getApps();
            additionalList.BackgroundColor = Color.Transparent;
            additionalList.Effects.Add(Effect.Resolve("MyEffects.ListViewHighlightEffect"));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            new Task(SyncSetup).Start(); // This could be an await'd task if need be
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
            LabelPhone.Text = Settings.Person.Phone;
            SwitchApp.OnColor = hex;
            IconAddApp.Foreground = Color.White;
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
                await DisplayAlert("Ошибка", "Не удалось получить информацию о заявках", "OK");
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
                await Navigation.PushAsync(new NewAppPage(this));
            }
            else
            {
                await DisplayAlert("Ошибка", "Лицевые счета не подключены", "OK");
            }
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