using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamarinJKH.Main;
using xamarinJKH.Pays;
using xamarinJKH.Server;
using xamarinJKH.Server.RequestModel;

namespace xamarinJKH.DialogViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EnterPinAddLs :Rg.Plugins.Popup.Pages.PopupPage
    {
        RestClientMP server = new RestClientMP();
        public string Ident { get; set; }
        private PaysPage paysPage;
        private AddIdent AddIdent;
        public EnterPinAddLs(string ident, PaysPage paysPage, AddIdent addIdent)
        {
            Ident = ident;
            this.paysPage = paysPage;
            AddIdent = addIdent;
            InitializeComponent();
            var close = new TapGestureRecognizer();
            close.Tapped += async (s, e) => { await PopupNavigation.Instance.PopAsync(); };
            IconViewClose.GestureRecognizers.Add(close);
            BindingContext = this;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            PinCode.Focus();

        }

        private async void Button_OnClicked(object sender, EventArgs e)
        {
            string pinCodeText = PinCode.Text;
            if(!string.IsNullOrEmpty(pinCodeText))
            {
                AddAccountResult result = await server.AddIdent(Ident,false, pinCodeText);
                if (result.Error == null)
                {
                    await DisplayAlert("", $"{AppResources.Acc} " + Ident + $"{AppResources.AddLsString}", "ОК");
                    await paysPage.RefreshPaysData();
                    await PopupNavigation.Instance.PopAsync();
                }
            }
        }
    }
}