using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Services;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamarinJKH.Pays;
using xamarinJKH.Server;
using xamarinJKH.Server.RequestModel;
using xamarinJKH.Utils;

namespace xamarinJKH.DialogViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PayAppDialog : Rg.Plugins.Popup.Pages.PopupPage
    {
        public Color hex { get; set; }
        public bool isBonusVisible { get; set; }
        public decimal bonusCount { get; set; }
        public string title { get; set; }
        private RequestContent request;
        private Page appPage;


        public PayAppDialog(Color hexColor, RequestContent request, Page appPage)
        {
            hex = hexColor;
            this.request = request;
            this.appPage = appPage;
            title = request.PaidServiceText;
            isBonusVisible = false;
            InitializeComponent();
            Frame.SetAppThemeColor(Frame.BorderColorProperty, Color.FromHex(Settings.MobileSettings.color),
                Color.Transparent);

            var close = new TapGestureRecognizer();
            close.Tapped += async (s, e) => { await PopupNavigation.Instance.PopAsync(); };
            IconViewClose.GestureRecognizers.Add(close);

          
            FormattedString formatted = new FormattedString();
            formatted.Spans.Add(new Span
            {
                Text = $"{AppResources.Total}: ",
                FontSize = 17,
                TextColor = Color.Black
            });
            decimal bonusPaid = getBonusPaid();
            formatted.Spans.Add(new Span
            {
                Text = (request.PaidSumm - bonusPaid).ToString(),
                FontSize = 20,
                TextColor = hex,
                FontAttributes = FontAttributes.Bold
            });
            formatted.Spans.Add(new Span
            {
                Text = $" {AppResources.Currency}",
                FontSize = 15,
                TextColor = Color.FromHex("#777777")
            });
            LabelTotal.FormattedText = formatted;
            if (bonusPaid > 0)
            {
                LabelBonusCount.Text = AppResources.BonusTotal + bonusPaid;
            }
            else
            {
                LabelBonusCount.IsVisible = false;
            }

            BindingContext = this;
        }

        decimal getBonusPaid()
        {
            decimal count = 0;
            foreach (var each in request.ReceiptItems)
            {
                count += each.BonusAmount;
            }

            return count;
        }


        private async void payApp(object sender, EventArgs e)
        {
            if (!request.IsPaidByUser)
            {
                await appPage.Navigation.PushAsync(
                    new PayServicePage("", request.PaidSumm - getBonusPaid(), request.ID));
                await PopupNavigation.Instance.PopAsync();
            }
            else
            {
                await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorPayApp, "OK");
            }
        }

        private void btnCardPay_Clicked(object sender, EventArgs e)
        {
        }

        private void btnCashPay_Clicked(object sender, EventArgs e)
        {
        }
        
    }
}