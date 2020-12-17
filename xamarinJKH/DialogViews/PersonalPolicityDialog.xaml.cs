using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamarinJKH.Server;

namespace xamarinJKH.DialogViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PersonalPolicityDialog : Rg.Plugins.Popup.Pages.PopupPage
    {
        private RestClientMP server = new RestClientMP();
        public PersonalPolicityDialog()
        {
            InitializeComponent();
            var close = new TapGestureRecognizer();
            close.Tapped += async (s, e) => { await PopupNavigation.Instance.PopAsync(); };
            IconViewClose.GestureRecognizers.Add(close);
            GetText();
        }

        async void GetText()
        {
            string text = await server.MobilePersonalDataPolicy();
            Text.Text = text;
        }
    }
}