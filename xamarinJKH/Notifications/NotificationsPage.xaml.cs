using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamarinJKH.Notifications;
using xamarinJKH.Server;
using xamarinJKH.Server.RequestModel;
using xamarinJKH.Utils;

namespace xamarinJKH
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NotificationsPage : ContentPage
    {
        public List<AnnouncementInfo> Notifications { get; set; }
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
            Settings.EventBlockData = await server.GetEventBlockData();
            if (Settings.EventBlockData.Error == null)
            {
                Notifications = Settings.EventBlockData.Announcements;
                NotificationList.ItemsSource = null;
                NotificationList.ItemsSource = Notifications;
            }
            else
            {
                await DisplayAlert("Ошибка", "Не удалось получить информацию о уведомлениях", "OK");
            }
        }

        public NotificationsPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    ImageTop.Margin = new Thickness(0, 7, 0, 0);
                    StackLayout.Margin = new Thickness(0, 33, 0, 0);
                    IconViewNameUk.Margin = new Thickness(0, 33, 0, 0);
                    break;
                case Device.Android:
                    RelativeLayoutTop.Margin = new Thickness(0,0,0,-135);
                    break;
                default:
                    break;
            }
            var backClick = new TapGestureRecognizer();
            backClick.Tapped += async (s, e) => { _ = await Navigation.PopAsync(); };
            BackStackLayout.GestureRecognizers.Add(backClick);
            SetText();
            Notifications = Settings.EventBlockData.Announcements;
            this.BindingContext = this;
            NotificationList.BackgroundColor = Color.Transparent;
        }

        void SetText()
        {
            UkName.Text = Settings.MobileSettings.main_name;
            LabelPhone.Text = "+" + Settings.Person.Phone;
        }

        private async void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            AnnouncementInfo select = e.Item as AnnouncementInfo;
            await Navigation.PushAsync(new NotificationOnePage(select));
        }
    }
}