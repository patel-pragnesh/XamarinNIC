using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamarinJKH.Pays;
using xamarinJKH.Server.RequestModel;
using xamarinJKH.Utils;

namespace xamarinJKH.DialogViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PayAppDialog : Rg.Plugins.Popup.Pages.PopupPage
    {
        public Color hex { get; set; }
        public string title { get; set; }
        private RequestContent request;
        private Page appPage;
        public PayAppDialog(Color hexColor,  RequestContent request, Page appPage)
        {
            hex = hexColor;
            this.request = request;
            this.appPage = appPage;
            title = request.PaidServiceText;
            InitializeComponent();
            var close = new TapGestureRecognizer();
            close.Tapped += async (s, e) => { await PopupNavigation.Instance.PopAsync(); };
            IconViewClose.GestureRecognizers.Add(close);
            FormattedString formatted = new FormattedString();
            formatted.Spans.Add(new Span
            {
                Text = "Итого: ",
                FontSize = 17,
                TextColor = Color.Black
            });
            formatted.Spans.Add(new Span
            {
                Text = request.PaidSumm.ToString(),
                FontSize = 20,
                TextColor = Color.FromHex(Settings.MobileSettings.color),
                FontAttributes = FontAttributes.Bold
            });
            formatted.Spans.Add(new Span
            {
                Text = " руб.",
                FontSize = 15,
                TextColor = Color.FromHex("#777777")
            });
            LabelTotal.FormattedText = formatted;
            BindingContext = this;
        }

        private async void payApp(object sender, EventArgs e)
        {
            if (!request.IsPaidByUser)
            {
                await appPage.Navigation.PushAsync(new PayServicePage("", request.PaidSumm, request.ID));
                await PopupNavigation.Instance.PopAsync();
            }
            else
            {
                await DisplayAlert("Ошибка", "Заказ уже оплачен", "OK");
            }
        }
    }
}