using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamarinJKH.Server;
using xamarinJKH.Server.RequestModel;
using xamarinJKH.Utils;

namespace xamarinJKH.Main
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CountersPage : ContentPage
    {
        public List<MeterInfo> _meterInfo { get; set; }
        private RestClientMP _server = new RestClientMP();
        private bool _isRefreshing = false;
        
        public bool IsRefreshing
        {
            get { return _isRefreshing; }
            set {
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

        private async Task RefreshData()
        {
            ItemsList<MeterInfo> info = await _server.GetThreeMeters();
            _meterInfo = info.Data;
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
                    ImageTop.Margin = new Thickness(0, 7, 0, 0);
                    ImageFon.Margin = new Thickness(0, 7, 0, 0);
                    StackLayout.Margin = new Thickness(0, 33, 0, 0);
                    IconViewNameUk.Margin = new Thickness(0, 33, 0, 0);
                    break;
                case Device.Android:
                default:
                    ImageTop.Margin = new Thickness();
                    ImageFon.Margin = new Thickness();
                    break;
            }
            SetTextAndColor();
            getInfo();
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
                Console.WriteLine(info.Data.Count);
                _meterInfo = info.Data;
                this.BindingContext = this;
            }
            else
            {
                await DisplayAlert("Ошибка", "Не удалось получить информацию о начислениях", "OK");
            }
        }
        
        private async void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            MeterInfo select = e.Item as MeterInfo;
            // await Navigation.PushAsync(new CostPage(select, _accountingInfo));
        }
    }
}