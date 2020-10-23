using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamarinJKH.Server.RequestModel;
using xamarinJKH.Utils;

namespace xamarinJKH.Notifications
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NotificationAllPage : ContentPage
    {
        public ObservableCollection<AnnouncementInfo> Notification { get; set; }
        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        public NotificationAllPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            Notification = new ObservableCollection<AnnouncementInfo>(Settings.EventBlockData.Announcements);
            InitializeComponent();
            HeaderViewMain.BackClick = new Command(async () =>
            {
                if (Application.Current.MainPage.Navigation.ModalStack.Count > 1)
                {
                    await Navigation.PopModalAsync();
                }
                else
                {
                    await Navigation.PopAsync();
                }
            });
            var pickDo = new TapGestureRecognizer();
            pickDo.Tapped += async (s, e) => {  
                Device.BeginInvokeOnMainThread(() =>
                {
                    DatePicker.Focus();
                });
            };
            StackLayoutDo.GestureRecognizers.Add(pickDo);
            var pickPosle = new TapGestureRecognizer();
            pickPosle.Tapped += async (s, e) => {  
                Device.BeginInvokeOnMainThread(() =>
                {
                    DatePicker2.Focus();
                });
            };
            StackLayoutDo.GestureRecognizers.Add(pickPosle);
            HeaderViewMain.DarkImage = "ic_notification_top";
            HeaderViewMain.LightImage = "ic_notification_top_light";
            BindingContext = this;
        }

        private async void NotifSelect(object sender, SelectionChangedEventArgs e)
        {
            AnnouncementInfo select =  e.CurrentSelection.FirstOrDefault()  as AnnouncementInfo;

            if (Navigation.NavigationStack.FirstOrDefault(x => x is NotificationOnePage) == null)
                await Navigation.PushAsync(new NotificationOnePage(select));
            try
            {
                CollectionView.SelectedItem = null;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void DatePicker2_OnDateSelected(object sender, DateChangedEventArgs e)
        {
            SetDate();
        }

        private void DatePicker_OnDateSelected(object sender, DateChangedEventArgs e)
        {
            SetDate();
        }

        void SetDate()
        {
            DateTime one = DatePicker.Date;
            DateTime two = DatePicker2.Date;
            Notification.Clear();
            foreach (var each in Settings.EventBlockData.Announcements)
            {
                DateTime timeNotif = new DateTime();
                try
                {
                    timeNotif =  DateTime.ParseExact(each.Created, "dd.MM.yyyy",
                        System.Globalization.CultureInfo.InvariantCulture);
                    if (timeNotif >= one && timeNotif <= two)
                    {
                        Device.BeginInvokeOnMainThread(async () => Notification.Add(each));
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                
            }
        }
    }
}