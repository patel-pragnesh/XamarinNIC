using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamarinJKH.DialogViews;
using xamarinJKH.Tech;
using xamarinJKH.Utils;

namespace xamarinJKH.PushNotification
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SendPushPage : ContentPage
    {
        public bool isRefresh { get; set; }

        public SendPushPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            UkName.Text = Settings.MobileSettings.main_name;
            var backClick = new TapGestureRecognizer();
            backClick.Tapped += async (s, e) =>
            {
                try
                {
                    _ = await Navigation.PopAsync();
                }
                catch
                {
                }
            };
            var techSend = new TapGestureRecognizer();
            techSend.Tapped += async (s, e) =>
            {
                if (Settings.Person != null && !string.IsNullOrWhiteSpace(Settings.Person.Phone))
                {
                    await Navigation.PushModalAsync(new AppPage());
                }
                else
                {
                    await PopupNavigation.Instance.PushAsync(new EnterPhoneDialog());
                }
            };
            LabelTech.GestureRecognizers.Add(techSend);
            BackStackLayout.GestureRecognizers.Add(backClick);
            BindingContext = this;
        }
        Thickness frameMargin = new Thickness();
        private void BordlessEditor_Focused(object sender, FocusEventArgs e)
        {
            if (Xamarin.Essentials.DeviceDisplay.MainDisplayInfo.Width < 800)
            {
                frameMargin = Frame.Margin;
                Device.BeginInvokeOnMainThread(()=> { Frame.Margin = new Thickness(15, 0, 15, 15); });
            }
        }

        private void BordlessEditor_Unfocused(object sender, FocusEventArgs e)
        {
            if (Xamarin.Essentials.DeviceDisplay.MainDisplayInfo.Width < 800)
            {
                Device.BeginInvokeOnMainThread(() => { Frame.Margin = frameMargin; }) ;
            }
        }

        private void CheckBox_OnCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
        }
    }
}