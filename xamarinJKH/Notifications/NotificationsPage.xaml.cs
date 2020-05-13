using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamarinJKH.Notifications;
using xamarinJKH.Server.RequestModel;
using xamarinJKH.Utils;

namespace xamarinJKH
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NotificationsPage : ContentPage
    {
        public List<AnnouncementInfo> Notifications { get; set; }
        public NotificationsPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            var backClick = new TapGestureRecognizer();
            backClick.Tapped += async (s, e) => { _ = await Navigation.PopAsync(); };
            BackStackLayout.GestureRecognizers.Add(backClick);
            SetText();
            Notifications = Settings.EventBlockData.Announcements;
            this.BindingContext = this;
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