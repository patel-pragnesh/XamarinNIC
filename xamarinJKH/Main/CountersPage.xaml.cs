using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Plugin.Messaging;
using Rg.Plugins.Popup.Services;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Markup;
using Xamarin.Forms.PancakeView;
using Xamarin.Forms.Xaml;
using xamarinJKH.Counters;
using xamarinJKH.CustomRenderers;
using xamarinJKH.DialogViews;
using xamarinJKH.InterfacesIntegration;
using xamarinJKH.Main;
using xamarinJKH.Server;
using xamarinJKH.Server.RequestModel;
using xamarinJKH.Tech;
using xamarinJKH.Utils;

namespace xamarinJKH.Main
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CountersPage : ContentPage
    {
        public List<MeterInfo> _meterInfo { get; set; }
        public List<MeterInfo> _meterInfoAll { get; set; }
        private string account = "";
        public List<string> Accounts = new List<string>();
        private RestClientMP _server = new RestClientMP();
        private bool _isRefreshing = false;

        public bool IsRefreshing
        {
            get { return _isRefreshing; }
            set
            {
                _isRefreshing = value;
                OnPropertyChanged(nameof(IsRefreshing));
            }
        }
        public Command ChangeTheme { get; set; }
        public ICommand RefreshCommand
        {
            get
            {
                return new Command(async () =>
                {
                    IsRefreshing = true;
                    await RefreshCountersData();
                    IsRefreshing = false;
                });
            }
        }

        public async Task RefreshCountersData()
        {
            if (Xamarin.Essentials.Connectivity.NetworkAccess != Xamarin.Essentials.NetworkAccess.Internet)
            {
                Device.BeginInvokeOnMainThread(async () => await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorNoInternet, "OK"));
                return;
            }
            ItemsList<MeterInfo> info = await _server.GetThreeMeters();
            _meterInfoAll = info.Data;
            if (account == "Все")
            {
                _meterInfo = _meterInfoAll;
            }
            else
            {
                List<MeterInfo> meters = new List<MeterInfo>();
                foreach (var meterInfo in _meterInfoAll)
                {
                    if (meterInfo.Ident == account)
                    {
                        meters.Add(meterInfo);
                    }
                }

                _meterInfo = meters;
            }

            baseForCounters.Children.Clear();

            foreach (var mi in _meterInfo)
            {
                var mtc = new MetersThreeCell(mi);
                TapGestureRecognizer tap = new TapGestureRecognizer();
                tap.Tapped += Tap_Tapped;
                mtc.GestureRecognizers.Add(tap);

                baseForCounters.Children.Add(mtc);
            }

            //countersList.ItemsSource = null;
            //countersList.ItemsSource = _meterInfo;
        }

        private async void Tap_Tapped(object sender, EventArgs e)
        {
            int currDay = DateTime.Now.Day;

            MeterInfo select = ((MetersThreeCell)sender).meterInfo;// as MeterInfo;
            if (select.IsDisabled)
            {
                return;
            }
            if ((Settings.Person.Accounts[0].MetersStartDay <= currDay &&
                 Settings.Person.Accounts[0].MetersEndDay >= currDay) ||
                (Settings.Person.Accounts[0].MetersStartDay == 0 &&
                 Settings.Person.Accounts[0].MetersEndDay == 0))
            {
                if (select.Values.Count >= 1 && int.Parse(select.Values[0].Period.Split('.')[1]) == DateTime.Now.Month)
                {
                    var counterThisMonth = (select.Values.Count >= 1) ? select.Values[0].Value : 0;
                    var counterThisMonth2 = (select.Values.Count >= 2) ? select.Values[1].Value : 0;
                    await Navigation.PushAsync(new AddMetersPage(select, _meterInfo, this, counterThisMonth,
                        counterThisMonth2));
                }
                else
                {
                    var counterThisMonth = (select.Values.Count >= 1) ? select.Values[0].Value : 0;
                    await Navigation.PushAsync(new AddMetersPage(select, _meterInfo, this, 0, counterThisMonth));
                }
            }
        }

        public CountersPage()
        {
            InitializeComponent();
            Settings.mainPage = this;
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
            techSend.Tapped += async (s, e) => { await PopupNavigation.Instance.PushAsync(new TechDialog()); };
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
            SetTextAndColor();
            getInfo();
            SetTitle();

            //countersList.BackgroundColor = Color.Transparent;
            baseForCounters.BackgroundColor = Color.Transparent;
            //if (Device.RuntimePlatform != Device.iOS)
            //    countersList.Effects.Add(Effect.Resolve("MyEffects.ListViewHighlightEffect"));

            Color hexColor = (Color)Application.Current.Resources["MainColor"];
            IconViewLogin.SetAppThemeColor(IconView.ForegroundProperty, hexColor, Color.White);
            IconViewTech.SetAppThemeColor(IconView.ForegroundProperty, hexColor, Color.White);
            Pancake.SetAppThemeColor(PancakeView.BorderColorProperty, hexColor, Color.Transparent);
            PancakeViewIcon.SetAppThemeColor(PancakeView.BorderColorProperty, hexColor, Color.Transparent);if (Device.RuntimePlatform == Device.iOS){ if (AppInfo.PackageName == "rom.best.saburovo" || AppInfo.PackageName == "sys_rom.ru.tsg_saburovo"){PancakeViewIcon.Padding = new Thickness(0);}}
            LabelTech.SetAppThemeColor(Label.TextColorProperty, hexColor, Color.White);
            FrameTop.SetAppThemeColor(MaterialFrame.BorderColorProperty, hexColor, Color.FromHex("#494949"));
            ChangeTheme = new Command(async () =>
            {
                SetTitle();
            });
            MessagingCenter.Subscribe<Object>(this, "ChangeThemeCounter", (sender) => ChangeTheme.Execute(null));
            MessagingCenter.Subscribe<Object>(this, "UpdateCounters", (sender) => RefreshCommand.Execute(null));
        }

        private void SetTitle()
        {
            OSAppTheme currentTheme = Application.Current.RequestedTheme;
            var day = DateTime.Now.Day;
            //if (Xamarin.Essentials.DeviceInfo.Platform == Xamarin.Essentials.DevicePlatform.iOS)
            //    currentTheme = OSAppTheme.Dark;
            if (Settings.Person != null)
                if (Settings.Person.Accounts != null)
                    if (Settings.Person.Accounts.Count > 0)
                    {
                        FormattedString formattedResource = new FormattedString();
                        formattedResource.Spans.Add(new Span
                        {
                            Text = AppResources.CountersInfo1,
                            TextColor = currentTheme.Equals(OSAppTheme.Light) ? Color.Black : Color.White,
                            FontAttributes = FontAttributes.None,
                            FontSize = 15
                        });
                        if (Settings.Person.Accounts[0].MetersStartDay != null &&
                            Settings.Person.Accounts[0].MetersEndDay != null)
                        {
                            if (Settings.Person.Accounts[0].MetersStartDay != 0 &&
                                Settings.Person.Accounts[0].MetersEndDay != 0)
                            {
                                if (Settings.Person.Accounts[0].MetersStartDay > Settings.Person.Accounts[0].MetersEndDay)
                                {
                                    if (day <= Settings.Person.Accounts[0].MetersEndDay)
                                    {
                                        formattedResource.Spans.Add(new Span
                                        {
                                            Text = $" {AppResources.From} {Settings.Person.Accounts[0].MetersStartDay} " +
                                            $"{AppResources.PreviousMonth} {AppResources.To} {Settings.Person.Accounts[0].MetersEndDay} ",
                                            TextColor = currentTheme.Equals(OSAppTheme.Light) ? Color.Black : Color.White,
                                            FontAttributes = FontAttributes.Bold,
                                            FontSize = 15
                                        });
                                        formattedResource.Spans.Add(new Span
                                        {
                                            Text = AppResources.CountersCurrentMonth,
                                            TextColor = currentTheme.Equals(OSAppTheme.Light) ? Color.Black : Color.White,
                                            FontAttributes = FontAttributes.None,
                                            FontSize = 15
                                        });
                                    }
                                    else
                                    {
                                        formattedResource.Spans.Add(new Span
                                        {
                                            Text = $" {AppResources.From} {Settings.Person.Accounts[0].MetersStartDay} " +
                                            $"{AppResources.CountersCurrentMonth} {AppResources.To} {Settings.Person.Accounts[0].MetersEndDay} ",
                                            TextColor = currentTheme.Equals(OSAppTheme.Light) ? Color.Black : Color.White,
                                            FontAttributes = FontAttributes.Bold,
                                            FontSize = 15
                                        });
                                        formattedResource.Spans.Add(new Span
                                        {
                                            Text = AppResources.NextMonth,
                                            TextColor = currentTheme.Equals(OSAppTheme.Light) ? Color.Black : Color.White,
                                            FontAttributes = FontAttributes.None,
                                            FontSize = 15
                                        });
                                    }
                                }
                                else
                                {
                                    formattedResource.Spans.Add(new Span
                                    {
                                        Text = AppResources.From + Settings.Person.Accounts[0].MetersStartDay + AppResources.To +
                                           Settings.Person.Accounts[0].MetersEndDay + AppResources.DayOfMounth,
                                        TextColor = currentTheme.Equals(OSAppTheme.Light) ? Color.Black : Color.White,
                                        FontAttributes = FontAttributes.Bold,
                                        FontSize = 15
                                    });
                                    formattedResource.Spans.Add(new Span
                                    {
                                        Text = AppResources.CountersCurrentMonth,
                                        TextColor = currentTheme.Equals(OSAppTheme.Light) ? Color.Black : Color.White,
                                        FontAttributes = FontAttributes.None,
                                        FontSize = 15
                                    });
                                }

                            }
                            else
                            {
                                formattedResource.Spans.Add(new Span
                                {
                                    Text = AppResources.CountersThisMonth,
                                    TextColor = currentTheme.Equals(OSAppTheme.Light) ? Color.Black : Color.White,
                                    FontAttributes = FontAttributes.Bold,
                                    FontSize = 15
                                });
                            }
                        }
                        else
                        {
                            formattedResource.Spans.Add(new Span
                            {
                                Text = AppResources.CountersThisMonth,
                                TextColor = currentTheme.Equals(OSAppTheme.Light) ? Color.Black : Color.White,
                                FontAttributes = FontAttributes.Bold,
                                FontSize = 15
                            });
                        }

                        PeriodSendLbl.FormattedText = formattedResource;
                    }
                    else
                    {
                        PeriodSendLbl.Text = AppResources.NoAccounts;
                    }
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
                RefreshCountersData();
            });
        }

        private void picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                var identLength = Settings.Person.Accounts[Picker.SelectedIndex - 1].Ident.Length;
                if (identLength < 6)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Picker.WidthRequest = identLength * 10;
                        //Picker.MinimumWidthRequest = identLength * 9;
                    });
                }
            }
            catch (Exception ex)
            {
                if (Picker.SelectedIndex != -1 && Settings.Person.Accounts.Count != 0)
                {
                    var identLength = Settings.Person.Accounts[Picker.SelectedIndex].Ident.Length;
                    if (identLength < 6)
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            Picker.WidthRequest = identLength * 9;
                            // Picker.MinimumWidthRequest = identLength * 9;
                        });
                    }
                }
            }

            if (Accounts != null)
            {
                if (Accounts.Count > 0)
                {
                    account = Accounts[Picker.SelectedIndex];
                    SetIdents();
                }
            }
        }

        void SetIdents()
        {
            Picker.SetAppThemeColor(Xamarin.Forms.Picker.TextColorProperty, Color.Black, Color.White);
            Picker.SetAppThemeColor(Xamarin.Forms.Picker.TitleColorProperty, Color.Black, Color.White);
            Picker.Title = account;
            _meterInfo = null;
            if (account == "Все")
            {
                _meterInfo = _meterInfoAll;
            }
            else
            {
                List<MeterInfo> meters = new List<MeterInfo>();
                foreach (var meterInfo in _meterInfoAll)
                {
                    if (meterInfo.Ident == account)
                    {
                        meters.Add(meterInfo);
                    }
                }

                _meterInfo = meters;
            }

            baseForCounters.Children.Clear();

            foreach (var mi in _meterInfo)
            {
                baseForCounters.Children.Add(new MetersThreeCell(mi));
            }

            //countersList.ItemsSource = null;
            //countersList.ItemsSource = _meterInfo;
        }

        private async void ButtonClick(object sender, EventArgs e)
        {
        }

        void SetTextAndColor()
        {
            UkName.Text = Settings.MobileSettings.main_name;
            LabelPhone.Text = "+" + Settings.Person.companyPhone.Replace("+", "");
        }

        async void getInfo()
        {
            if (Xamarin.Essentials.Connectivity.NetworkAccess != Xamarin.Essentials.NetworkAccess.Internet)
            {
                Device.BeginInvokeOnMainThread(async () => await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorNoInternet, "OK"));
                return;
            }
            ItemsList<MeterInfo> info = await _server.GetThreeMeters();
            if (info.Error == null)
            {
                _meterInfo = info.Data;
                _meterInfoAll = info.Data;
                this.BindingContext = this;
                if (_meterInfo != null)
                {
                    account = "Все";
                    Accounts.Add("Все");
                    foreach (var meterInfo in _meterInfo)
                    {
                        Boolean k = false;
                        foreach (var s in Accounts)
                        {
                            if (s == meterInfo.Ident)
                            {
                                k = true;
                            }
                        }

                        if (k == false)
                        {
                            Accounts.Add(meterInfo.Ident);
                        }
                    }

                    Picker.ItemsSource = Accounts;
                    Picker.SelectedIndex = 0;
                }
            }
            else
            {
                await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorCountersNoData, "OK");
            }
        }

        private async void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            int currDay = DateTime.Now.Day;
            MeterInfo select = e.Item as MeterInfo;
            if (select.IsDisabled)
            {
                return;
            }
            if (Settings.Person != null)
                if (Settings.Person.Accounts != null)
                    if (Settings.Person.Accounts.Count > 0)
                        if ((Settings.Person.Accounts[0].MetersStartDay <= currDay &&
                             Settings.Person.Accounts[0].MetersEndDay >= currDay) ||
                            (Settings.Person.Accounts[0].MetersStartDay == 0 &&
                             Settings.Person.Accounts[0].MetersEndDay == 0))
                        {
                            if (select.Values.Count >= 1 && int.Parse(select.Values[0].Period.Split('.')[1]) == DateTime.Now.Month)
                            {
                                var counterThisMonth = (select.Values.Count >= 1) ? select.Values[0].Value : 0;
                                var counterThisMonth2 = (select.Values.Count >= 2) ? select.Values[1].Value : 0;
                                await Navigation.PushAsync(new AddMetersPage(select, _meterInfo, this, counterThisMonth,
                                    counterThisMonth2));
                            }
                            else
                            {
                                var counterThisMonth = (select.Values.Count >= 1) ? select.Values[0].Value : 0;
                                await Navigation.PushAsync(new AddMetersPage(select, _meterInfo, this, 0, counterThisMonth));
                            }
                        }
        }

        private async void RefreshView_RefreshingAsync(object sender, EventArgs e)
        {
            IsRefreshing = true;
            await RefreshCountersData();
            IsRefreshing = false;
        }


    }
}