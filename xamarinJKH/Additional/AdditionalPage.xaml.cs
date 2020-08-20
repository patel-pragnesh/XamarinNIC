using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Plugin.Messaging;
using Xamarin.Forms;
using Xamarin.Forms.PancakeView;
using Xamarin.Forms.Xaml;
using xamarinJKH.InterfacesIntegration;
using xamarinJKH.Server;
using xamarinJKH.Server.RequestModel;
using xamarinJKH.Shop;
using xamarinJKH.Tech;
using xamarinJKH.Utils;
using xamarinJKH.ViewModels.Additional;
using static Akavache.BlobCache;

using Xamarin.Forms.Maps;
using AiForms.Dialogs;
using Rg.Plugins.Popup.Services;
using xamarinJKH.DialogViews;

namespace xamarinJKH.Additional
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AdditionalPage : ContentPage
    {
        public ObservableCollection<AdditionalService> Additional { get; set; }
        private bool _isRefreshing = false;
        private RestClientMP server = new RestClientMP();

        string _selectedGroup;

        public string SelectedGroup
        {
            get => _selectedGroup;
            set
            {
                _selectedGroup = value;
                OnPropertyChanged(nameof(SelectedGroup));
            }
        }

        public bool IsRefreshing
        {
            get { return _isRefreshing; }
            set
            {
                _isRefreshing = value;
                OnPropertyChanged(nameof(IsRefreshing));
            }
        }

        public ObservableCollection<string> Groups { get; set; }
        string _mainColor;

        public string MainColor
        {
            get => _mainColor;
            set
            {
                _mainColor = value;
                OnPropertyChanged(nameof(MainColor));
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

        bool _busy;

        public bool IsBusy
        {
            get => _busy;
            set
            {
                _busy = value;
                OnPropertyChanged("IsBusy");
            }
        }

        private async Task RefreshData()
        {
            if (Xamarin.Essentials.Connectivity.NetworkAccess != Xamarin.Essentials.NetworkAccess.Internet)
            {
                Device.BeginInvokeOnMainThread(async () =>
                    await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorNoInternet, "OK"));
                return;
            }

            Settings.EventBlockData = await server.GetEventBlockData();
            if (Settings.EventBlockData.Error == null)
            {
                SetAdditional();
                additionalList.ItemsSource = null;
                additionalList.ItemsSource = Additional;
                
            }
            else
            {
                await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorAdditional, "OK");
            }
        }

        public AdditionalPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            Map.BindingContext = new MapPageViewModel();
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    //BackgroundColor = Color.White;
                    // ImageTop.Margin = new Thickness(0, 0, 0, 0);
                    // StackLayout.Margin = new Thickness(0, 33, 0, 0);
                    // IconViewNameUk.Margin = new Thickness(0, 33, 0, 0);
                    break;
                case Device.Android:
                    // double or = Math.Round(((double) App.ScreenWidth / (double) App.ScreenHeight), 2);
                    // if (Math.Abs(or - 0.5) < 0.02)
                    // {
                    //     RelativeLayoutTop.Margin = new Thickness(0,0,0,-90);
                    // }
                    break;
                default:
                    break;
            }

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    int statusBarHeight = DependencyService.Get<IStatusBar>().GetHeight();
                    Pancake.Padding = new Thickness(0, statusBarHeight, 0, 0);
                    break;
                default:
                    break;
            }

            var backClick = new TapGestureRecognizer();
            backClick.Tapped += async (s, e) => { _ = await Navigation.PopAsync(); };
            BackStackLayout.GestureRecognizers.Add(backClick);
            var techSend = new TapGestureRecognizer();
            techSend.Tapped += async (s, e) => { await PopupNavigation.Instance.PushAsync(new TechDialog()); };
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
            additionalList.BackgroundColor = Color.Transparent;
            additionalList.Effects.Add(Effect.Resolve("MyEffects.ListViewHighlightEffect"));
            MainColor = "#" + Settings.MobileSettings.color;
            Additional = new ObservableCollection<AdditionalService>();
            Groups = new ObservableCollection<string>();
            this.BindingContext = this;
            CatalogMenu.TextColor = (Color)Application.Current.Resources["MainColor"];
            MessagingCenter.Subscribe<Object>(this, "LoadGoods", async (s) =>
            {
                IsBusy = true;
                await Task.Delay(TimeSpan.FromMilliseconds(500));
                SetAdditional();
            });

            MessagingCenter.Subscribe<MapPageViewModel, Xamarin.Forms.Maps.Position>(this, "FocusMap", (sender, args) =>
            {
                (Map.Children[0] as Xamarin.Forms.Maps.Map).MoveToRegion(MapSpan.FromCenterAndRadius(args, Distance.FromKilometers(2)));
            });

            MessagingCenter.Subscribe<Object, AdditionalService>(this, "OpenService", async (sender, args) =>
            {
                var select = args;
                if (select.ShopID == null)
                {
                    await Navigation.PushAsync(new AdditionalOnePage(select));
                }
                else
                {
                    await Navigation.PushAsync(new ShopPage(select));
                }
            });
        }

        void SetText()
        {
            UkName.Text = Settings.MobileSettings.main_name;
            LabelPhone.Text = "+" + Settings.Person.companyPhone.Replace("+", "");
            Color hexColor = (Color) Application.Current.Resources["MainColor"];
            IconViewLogin.SetAppThemeColor(IconView.ForegroundProperty, hexColor, Color.White);
            IconViewTech.SetAppThemeColor(IconView.ForegroundProperty, hexColor, Color.White);
            Pancake.SetAppThemeColor(PancakeView.BorderColorProperty, hexColor, Color.Transparent);
            PancakeViewIcon.SetAppThemeColor(PancakeView.BorderColorProperty, hexColor, Color.Transparent);
            FrameKind.SetAppThemeColor(Frame.BorderColorProperty, hexColor, Color.FromHex("#494949"));
            LabelTech.SetAppThemeColor(Label.TextColorProperty, hexColor, Color.White);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            //AiForms.Dialogs.Loading.Instance.Show();
            IsBusy = true;
            SetText();

            //await RefreshData();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            Groups.Clear();
            Additional.Clear();
        }

        void SetAdditional()
        {
            if (Xamarin.Essentials.Connectivity.NetworkAccess != Xamarin.Essentials.NetworkAccess.Internet)
            {
                Device.BeginInvokeOnMainThread(async () => await DisplayAlert(AppResources.ErrorTitle, null, "OK"));
                return;
            }

            Groups.Clear();
            Additional.Clear();
            Task.Run(() =>
            {
                if (Settings.EventBlockData.AdditionalServices != null)
                {
                    var l = Settings.EventBlockData.AdditionalServices.Where(x => x.HasLogo && x.ShowInAdBlock != null && !x.ShowInAdBlock.ToLower().Equals("не отображать"));
                    var groups = l.GroupBy(x => x.Group).Select(x => x.First())
                        .Select(y => y.Group ).ToList();

                    var groups1 = Settings.EventBlockData.AdditionalServices.GroupBy(x => x.Group).Select(x => x.First())
                        .Select(y => y.Group).ToList();

                    foreach (var group in groups)
                    {
                        Device.BeginInvokeOnMainThread(() => Groups.Add(group));
                    }

                    foreach (var each in Settings.EventBlockData.AdditionalServices)
                    {
                        //try
                        //{
                        if (each.HasLogo)
                            if (each.ShowInAdBlock != null)
                                if (!each.ShowInAdBlock.ToLower().Equals("не отображать"))
                                {
                                    if (SelectedGroup != null)
                                    {
                                        if (each.Group == SelectedGroup)
                                        {
                                            Device.BeginInvokeOnMainThread(() => Additional.Add(each));
                                        }
                                    }
                                    else
                                    {
                                        Device.BeginInvokeOnMainThread(() => Additional.Add(each));
                                    }
                                }
                    }


                    if (SelectedGroup == null)
                    {
                        if (groups.Count > 0)
                            Device.BeginInvokeOnMainThread(() => SelectedGroup = Groups[0]);
                    }
                }

                IsBusy = false;
            });

            //AiForms.Dialogs.Loading.Instance.Hide();
        }

     


        private async void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            AdditionalService select = e.Item as AdditionalService;
            if (select.ShopID == null)
            {
                await Navigation.PushAsync(new AdditionalOnePage(select));
            }
            else
            {
                await Navigation.PushAsync(new ShopPage(select));
            }
        }

        private void GroupChanged(object sender, SelectionChangedEventArgs e)
        {
            string group = e.CurrentSelection[0] as string;
            Additional.Clear();
            foreach (var service in Settings.EventBlockData.AdditionalServices.Where(x => x.Group == group))
            {
                if (service.HasLogo && service.ShowInAdBlock != null)
                    if (!service.ShowInAdBlock.ToLower().Equals("не отображать"))
                        Additional.Add(service);
            }
        }

        private void SwitchList(object sender, EventArgs args)
        {
            var label = sender as Label;
            var mainColor = Application.Current.Resources["MainColor"];
            label.TextColor = (Color)mainColor;
            MapMenu.TextColor = Application.Current.UserAppTheme == OSAppTheme.Dark ? Color.White : Color.Black;
            Map.IsVisible = false;
            SetAdditional();
        }

        private void SwitchMap(object sender, EventArgs args)
        {
            var label = sender as Label;
            var mainColor = Application.Current.Resources["MainColor"];
            label.TextColor = (Color)mainColor;
            CatalogMenu.TextColor = Application.Current.UserAppTheme == OSAppTheme.Dark ? Color.White : Color.Black;
            Map.IsVisible = true;
            (Map.BindingContext as MapPageViewModel).GetPermission.Execute(Additional);
            (Map.BindingContext as MapPageViewModel).LoadPins.Execute(Additional);
        }

        private async void Pin_Clicked(object sender, EventArgs e)
        {
            var shop = Additional.FirstOrDefault(x => x.ID == Convert.ToInt32((sender as Xamarin.Forms.Maps.Pin).ClassId));
            if (shop != null)
            {
                await Dialog.Instance.ShowAsync(new MapShopDialogView(shop));
            }
        }
        private void CollectionView_Scrolled(object sender, ItemsViewScrolledEventArgs e)
        {
            var index = e.FirstVisibleItemIndex;
            if (index >= 0)
            {
                if (index + 1 <= Groups.Count - 1 && e.HorizontalOffset < 160)
                {
                    SelectedGroup = Groups[index];
                }
                else
                {
                    SelectedGroup = Groups[e.LastVisibleItemIndex];
                }
            }
        }
    }
}