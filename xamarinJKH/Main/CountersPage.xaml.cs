using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamarinJKH.Counters;
using xamarinJKH.Main;
using xamarinJKH.Server;
using xamarinJKH.Server.RequestModel;
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

            countersList.ItemsSource = null;
            countersList.ItemsSource = _meterInfo;
        }

        public CountersPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    BackgroundColor = Color.White;
                    ImageFon.Margin = new Thickness(0, 0, 0, 0);
                    StackLayout.Margin = new Thickness(0, 33, 0, 0);
                    IconViewNameUk.Margin = new Thickness(0, 33, 0, 0);
                    break;
                case Device.Android:
                    double or = Math.Round(((double) App.ScreenWidth / (double) App.ScreenHeight), 2);
                    if (Math.Abs(or - 0.5) < 0.02)
                    {
                        RelativeLayoutTop.Margin = new Thickness(0, 0, 0, -125);
                        BackStackLayout.Margin = new Thickness(5, 25, 0, 0);
                    }

                    break;
                default:
                    break;
            }
            
            SetTextAndColor();
            getInfo();
            if (Settings.Person.Accounts.Count > 0)
            {
                FormattedString formattedResource = new FormattedString();
                formattedResource.Spans.Add(new Span
                {
                    Text = "Возможность передавать показания доступна ",
                    TextColor = Color.White,
                    FontAttributes = FontAttributes.None,
                    FontSize = 15
                });
                if (Settings.Person.Accounts[0].MetersStartDay != null &&
                    Settings.Person.Accounts[0].MetersEndDay != null)
                {
                    if (Settings.Person.Accounts[0].MetersStartDay != 0 &&
                        Settings.Person.Accounts[0].MetersEndDay != 0)
                    {
                        formattedResource.Spans.Add(new Span
                        {
                            Text = "c " + Settings.Person.Accounts[0].MetersStartDay + " по " +
                                   Settings.Person.Accounts[0].MetersEndDay + " числа ",
                            TextColor = Color.White,
                            FontAttributes = FontAttributes.Bold,
                            FontSize = 15
                        });
                        formattedResource.Spans.Add(new Span
                        {
                            Text = "текущего месяца!",
                            TextColor = Color.White,
                            FontAttributes = FontAttributes.None,
                            FontSize = 15
                        });
                    }
                    else
                    {
                        formattedResource.Spans.Add(new Span
                        {
                            Text = "в текущем месяце!",
                            TextColor = Color.White,
                            FontAttributes = FontAttributes.Bold,
                            FontSize = 15
                        });
                    }
                }
                else
                {
                    formattedResource.Spans.Add(new Span
                    {
                        Text = "в текущем месяце!",
                        TextColor = Color.White,
                        FontAttributes = FontAttributes.Bold,
                        FontSize = 15
                    });
                }

                PeriodSendLbl.FormattedText = formattedResource;
            }
            else
            {
                PeriodSendLbl.Text = "Лицевые счета не подключены";
            }

            countersList.BackgroundColor = Color.Transparent;
            countersList.Effects.Add(Effect.Resolve("MyEffects.ListViewHighlightEffect"));
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
            Picker.TextColor = Color.White;
            Picker.TitleColor = Color.White;
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

            countersList.ItemsSource = null;
            countersList.ItemsSource = _meterInfo;
        }

        private async void ButtonClick(object sender, EventArgs e)
        {
        }

        void SetTextAndColor()
        {
            UkName.Text = Settings.MobileSettings.main_name;
            LabelPhone.Text = "+" + Settings.Person.Phone;
        }

        async void getInfo()
        {
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
                await DisplayAlert("Ошибка", "Не удалось получить информацию о начислениях", "OK");
            }
        }

        private async void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            int currDay = DateTime.Now.Day;
            if ((Settings.Person.Accounts[0].MetersStartDay <= currDay &&
                 Settings.Person.Accounts[0].MetersEndDay >= currDay) ||
                (Settings.Person.Accounts[0].MetersStartDay == 0 &&
                 Settings.Person.Accounts[0].MetersEndDay == 0))
            {
                MeterInfo select = e.Item as MeterInfo;
                if (int.Parse(select.Values[0].Period.Split('.')[1]) == DateTime.Now.Month)
                {
                    await Navigation.PushAsync(new AddMetersPage(select, _meterInfo, this, select.Values[0].Value,
                        select.Values[1].Value));
                }
                else
                {
                    await Navigation.PushAsync(new AddMetersPage(select, _meterInfo, this));
                }
            }
        }
    }
}