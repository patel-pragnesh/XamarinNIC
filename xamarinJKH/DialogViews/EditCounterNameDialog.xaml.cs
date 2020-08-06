using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AiForms.Dialogs;
using AiForms.Dialogs.Abstractions;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamarinJKH.Server;
using xamarinJKH.Server.RequestModel;

namespace xamarinJKH.DialogViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditCounterNameDialog :Rg.Plugins.Popup.Pages.PopupPage
    {
        RestClientMP server = new RestClientMP();
        public Color hex { get; set; }
        public string UniqueNum { get; set; }
        
        public EditCounterNameDialog(Color hexColor, string uniqName)
        {
            hex = hexColor;
            UniqueNum = uniqName;
            InitializeComponent();
            var close = new TapGestureRecognizer();
            close.Tapped += async (s, e) => { await PopupNavigation.Instance.PopAsync(); };
            IconViewClose.GestureRecognizers.Add(close);
            BindingContext = this;
        }

        private async void Button_OnClicked(object sender, EventArgs e)
        {
            string name = EditName.Text;
            if (!name.Equals(""))
            {
                Configurations.LoadingConfig = new LoadingConfig {
                    IndicatorColor = hex,
                    OverlayColor = Color.Black,
                    Opacity = 0.8,
                    DefaultMessage = "",
                };

                await Loading.Instance.StartAsync(async progress =>
                {
                    // Device.BeginInvokeOnMainThread(async () =>
                    // {
                        CommonResult result = await server.SetMeterCustomName(UniqueNum, name);
                        if (result.Error == null)
                        {
                            MessagingCenter.Send<Object>(this, "UpdateCounters");
                            await PopupNavigation.Instance.PopAsync();
                        }
                        else
                        {
                            await DisplayAlert(AppResources.Error, result.Error, "OK");
                        }
                    // });
                });
            }
            else
            {
                await DisplayAlert(AppResources.Error, AppResources.EnterName, "OK");
            }
        }
    }
}