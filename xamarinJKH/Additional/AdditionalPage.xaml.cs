﻿using System;
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
using Xamarin.Essentials;
using System.Text.RegularExpressions;

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

                    if (Xamarin.Essentials.Connectivity.NetworkAccess != Xamarin.Essentials.NetworkAccess.Internet)
                    {
                        Device.BeginInvokeOnMainThread(async () => await DisplayAlert(AppResources.ErrorTitle, null, "OK"));
                        IsRefreshing = false;
                    }
                    else
                    {
                        if (Settings.EventBlockData.AdditionalServices != null)
                        {
                            var l = Settings.EventBlockData.AdditionalServices.Where(x => x.HasLogo && x.ShowInAdBlock != null && !x.ShowInAdBlock.ToLower().Equals("не отображать"));
                            var groups = l.GroupBy(x => x.Group).Select(x => x.First())
                                .Select(y => y.Group).ToList();

                            var groups1 = Settings.EventBlockData.AdditionalServices.GroupBy(x => x.Group).Select(x => x.First())
                                .Select(y => y.Group).ToList();
                            Additional.Clear();

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
                        }
                    }

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
                //additionalList.ItemsSource = null;
                //additionalList.ItemsSource = Additional;
                
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
                    int statusBarHeight = DependencyService.Get<IStatusBar>().GetHeight();
                    Pancake.Padding = new Thickness(0, statusBarHeight, 0, 0);

                    //headerImg.HorizontalOptions = LayoutOptions.Center;
                    //headerImg.Aspect = Aspect.AspectFit;

                    //MenuDelimiter.IsEnabled = false;
                    //MenuDelimiter.IsVisible = false;

                    //MapMenu.IsEnabled = false;
                    //MapMenu.IsVisible = false;
                    break;
                default:
                    break;
            }

            var backClick = new TapGestureRecognizer();
            backClick.Tapped += async (s, e) => {
                try
                {
                    _ = await Navigation.PopAsync();
                }
                catch { }
            };
            BackStackLayout.GestureRecognizers.Add(backClick);
            var techSend = new TapGestureRecognizer();
            techSend.Tapped += async (s, e) => { await PopupNavigation.Instance.PushAsync(new TechDialog()); };
            StackLayoutTech.GestureRecognizers.Add(techSend);
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
            additionalList.BackgroundColor = Color.Transparent;
            additionalList.Effects.Add(Effect.Resolve("MyEffects.ListViewHighlightEffect"));
            MainColor = "#" + Settings.MobileSettings.color;
            Additional = new ObservableCollection<AdditionalService>();
            Groups = new ObservableCollection<string>();
            this.BindingContext = this;
            CatalogMenu.TextColor = (Color)Application.Current.Resources["MainColor"];
            MessagingCenter.Subscribe<Object>(this, "LoadGoods", async (s) =>
            {
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
                    await Navigation.PushAsync(new ShopPageNew(select));
                }
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
            Color hexColor = (Color) Application.Current.Resources["MainColor"];
            //IconViewLogin.SetAppThemeColor(IconView.ForegroundProperty, hexColor, Color.White);
            //IconViewTech.SetAppThemeColor(IconView.ForegroundProperty, hexColor, Color.White);
            Pancake.SetAppThemeColor(PancakeView.BorderColorProperty, hexColor, Color.Transparent);
            PancakeViewIcon.SetAppThemeColor(PancakeView.BorderColorProperty, hexColor, Color.Transparent);if (Device.RuntimePlatform == Device.iOS){ if (AppInfo.PackageName == "rom.best.saburovo" || AppInfo.PackageName == "sys_rom.ru.tsg_saburovo"){PancakeViewIcon.Padding = new Thickness(0);}}
            FrameKind.SetAppThemeColor(Frame.BorderColorProperty, hexColor, Color.FromHex("#494949"));
            LabelTech.SetAppThemeColor(Label.TextColorProperty, hexColor, Color.White);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            //AiForms.Dialogs.Loading.Instance.Show();
            SetText();
            //SetAdditional();
            //await RefreshData();
        }

        void SetAdditional()
        {
            if (Xamarin.Essentials.Connectivity.NetworkAccess != Xamarin.Essentials.NetworkAccess.Internet)
            {
                Device.BeginInvokeOnMainThread(async () => await DisplayAlert(AppResources.ErrorTitle, null, "OK"));
                return;
            }

            IsBusy = true;
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


                        if (groups.Count > 0)
                        {
                            SelectedGroup = null;
                            Device.BeginInvokeOnMainThread(() => SelectedGroup = Groups[0]);

                        }
                }

                IsBusy = false;
            });

            //AiForms.Dialogs.Loading.Instance.Hide();
        }

     


        private async void OnItemTapped(object sender, SelectionChangedEventArgs e)
        {
            AdditionalService select = e.CurrentSelection[0] as AdditionalService;
            if (select.ShopID == null)
            {
                await Navigation.PushAsync(new AdditionalOnePage(select));
            }
            else
            { 
                await Navigation.PushAsync(new ShopPageNew(select));
            }
        }

        private void GroupChanged(object sender, SelectionChangedEventArgs e)
        {
            try
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
            catch
            {

            }
            
        }

        private void SwitchList(object sender, EventArgs args)
        {
            var label = sender as Label;
            var mainColor = Application.Current.Resources["MainColor"];
            label.TextColor =  (Color)mainColor;
            if(DevicePlatform.iOS==DevicePlatform.iOS)
                MapMenu.TextColor = Application.Current.UserAppTheme == OSAppTheme.Light || Application.Current.UserAppTheme == OSAppTheme.Unspecified ? Color.Black : Color.White;
            else
                MapMenu.TextColor = Application.Current.UserAppTheme == OSAppTheme.Dark || Application.Current.UserAppTheme == OSAppTheme.Unspecified ? Color.White : Color.Black;
            Map.IsVisible = false;
            SetAdditional();
        }

        private void SwitchMap(object sender, EventArgs args)
        {
            var label = sender as Label;
            var mainColor = Application.Current.Resources["MainColor"];
            label.TextColor = (Color)mainColor;

            if (DevicePlatform.iOS == DevicePlatform.iOS)
                CatalogMenu.TextColor = Application.Current.UserAppTheme == OSAppTheme.Light || Application.Current.UserAppTheme == OSAppTheme.Unspecified ? Color.Black : Color.White;
            else
                CatalogMenu.TextColor = Application.Current.UserAppTheme == OSAppTheme.Dark || Application.Current.UserAppTheme == OSAppTheme.Unspecified ? Color.White : Color.Black;
            
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