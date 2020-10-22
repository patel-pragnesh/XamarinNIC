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
            HeaderViewMain.DarkImage = "ic_notification_top";
            HeaderViewMain.LightImage = "ic_notification_top_light";
            BindingContext = this;
        }

        private async void NotifSelect(object sender, SelectionChangedEventArgs e)
        {
            AnnouncementInfo select =  e.CurrentSelection[0] as AnnouncementInfo;

            if (Navigation.NavigationStack.FirstOrDefault(x => x is NotificationOnePage) == null)
                await Navigation.PushAsync(new NotificationOnePage(select));
        }
    }
}