using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Services;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamarinJKH.Apps;
using xamarinJKH.InterfacesIntegration;
using xamarinJKH.Utils;

namespace xamarinJKH.DialogViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TechDialog : Rg.Plugins.Popup.Pages.PopupPage
    {
        public TechDialog(bool isVisibleApp = true)
        {
            InitializeComponent();
            LabelInfo.Text =
                AppResources.TechAdditionalText1 +
                Settings.MobileSettings.main_name + AppResources.TechAdditionalText2;
            BtnApp.Text = AppResources.TechAdditionalText3 + Settings.MobileSettings.main_name;
            var appOpen = new TapGestureRecognizer();
            StackLayoutApp.IsVisible = isVisibleApp;
            var close = new TapGestureRecognizer();
            close.Tapped += async (s, e) => { await PopupNavigation.Instance.PopAsync(); };
            IconViewClose.GestureRecognizers.Add(close);
            appOpen.Tapped += async (s, e) =>
            {
                if (isVisibleApp)
                {
                    if (Settings.Person.Accounts.Count > 0)
                    {
                        if (Settings.TypeApp.Count > 0)
                        {
                            await Navigation.PushAsync(new NewAppPage());
                        }
                        else
                        {
                            await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorAppsNoTypes, "OK");
                        }
                    }
                    else
                    {
                        await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorAppsNoIdent, "OK");
                    }
                }
            };
            FrameBtnApp.GestureRecognizers.Add(appOpen);
            var openUrlWhatsapp = new TapGestureRecognizer();
            openUrlWhatsapp.Tapped += async (s, e) => { await LoadUrl("https://wa.me/79955052402"); };
            ImageWhatsapp.GestureRecognizers.Add(item: openUrlWhatsapp);
            var openUrlTelegram = new TapGestureRecognizer();
            openUrlTelegram.Tapped += async (s, e) => { await LoadUrl("https://teleg.run/oiCVE7GCt3Z0bot"); };
            ImageTelegram.GestureRecognizers.Add(item: openUrlTelegram);
            var openUrlVider = new TapGestureRecognizer();
            openUrlVider.Tapped += async (s, e) =>
            {
                DependencyService.Get<IOpenApp>().OpenExternalApp("viber://pa/info?uri=smcenter");
            };
            ImageViber.GestureRecognizers.Add(item: openUrlVider);
        }

        private async Task LoadUrl(string url)
        {
            try
            {
                await Launcher.OpenAsync(url);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorAdditionalLink, "OK");
            }
        }
    }
}