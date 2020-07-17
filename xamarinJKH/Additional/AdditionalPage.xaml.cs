﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Plugin.Messaging;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamarinJKH.InterfacesIntegration;
using xamarinJKH.Server;
using xamarinJKH.Server.RequestModel;
using xamarinJKH.Shop;
using xamarinJKH.Tech;
using xamarinJKH.Utils;

namespace xamarinJKH.Additional
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AdditionalPage : ContentPage
    {
        public ObservableCollection<AdditionalService> Additional { get; set; }
        private bool _isRefreshing = false;
        private RestClientMP server = new RestClientMP();

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

        private async Task RefreshData()
        {
            Settings.EventBlockData = await server.GetEventBlockData();
            if (Settings.EventBlockData.Error == null)
            {
                SetAdditional();
                additionalList.ItemsSource = null;
                additionalList.ItemsSource = Additional;
            }
            else
            {
                await DisplayAlert("Ошибка", "Не удалось получить информацию об услугах", "OK");
            }
        }
        public AdditionalPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
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
            additionalList.BackgroundColor = Color.Transparent;
            additionalList.Effects.Add(Effect.Resolve("MyEffects.ListViewHighlightEffect"));
            MainColor = "#" + Settings.MobileSettings.color;
            Additional = new ObservableCollection<AdditionalService>();
            Groups = new ObservableCollection<string>();
            this.BindingContext = this;
        }
        void SetText()
        {
            UkName.Text = Settings.MobileSettings.main_name;
            LabelPhone.Text = "+" + Settings.Person.companyPhone.Replace("+", "");

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            SetText();
            SetAdditional();
        }

        void SetAdditional()
        {
            Groups.Clear();
            Additional.Clear();
            foreach (var each in Settings.EventBlockData.AdditionalServices)
            {
                if (each.HasLogo && !each.ShowInAdBlock.ToLower().Equals("не отображать"))
                    Additional.Add(each);
            }

            var groups = Additional.GroupBy(x => x.Group).Select(x => x.First()).Select(y => y.Group).ToList();

            foreach (var group in groups)
            {
                Groups.Add(group);
            }

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
                if (service.HasLogo && !service.ShowInAdBlock.ToLower().Equals("не отображать"))
                    Additional.Add(service);
            }
        }
    }
}