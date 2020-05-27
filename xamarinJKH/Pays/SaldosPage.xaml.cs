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

namespace xamarinJKH.Pays
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SaldosPage : ContentPage
    {
        public List<BillInfo> BillInfos { get; set; }
        private RestClientMP _server = new RestClientMP();
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

            ItemsList<AccountAccountingInfo> info = await _server.GetAccountingInfo();
            if (info.Error == null)
            {
                SetBills(info.Data);
                additionalList.ItemsSource = null;
                additionalList.ItemsSource = BillInfos;
            }else
            {
                await DisplayAlert("Ошибка", "Не удалось получить информацию о квитанциях", "OK");
            }
        }
        public SaldosPage(List<AccountAccountingInfo> infos)
        {
            SetBills(infos);
            InitializeComponent();
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    ImageTop.Margin = new Thickness(0, 7, 0, 0);
                    StackLayout.Margin = new Thickness(0, 33, 0, 0);
                    IconViewNameUk.Margin = new Thickness(0, 33, 0, 0);
                    break;
                case Device.Android:
                default:
                    break;
            }
            NavigationPage.SetHasNavigationBar(this, false);
            var backClick = new TapGestureRecognizer();
            backClick.Tapped += async (s, e) => { _ = await Navigation.PopAsync(); };
            BackStackLayout.GestureRecognizers.Add(backClick);
            SetText();
            this.BindingContext = this;
            
        }

        void SetBills(List<AccountAccountingInfo> infos)
        {
            BillInfos = new List<BillInfo>();
            foreach (var each in infos)
            {
                foreach (var VARIABLE in each.Bills)
                {
                    BillInfos.Add(VARIABLE);
                }
                
            }
        }
        
        void SetText()
        {
            UkName.Text = Settings.MobileSettings.main_name;
            LabelPhone.Text = "+" + Settings.Person.Phone;
        }

        private void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            
        }
    }
}