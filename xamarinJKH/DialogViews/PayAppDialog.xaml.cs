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
            isBonusVisible = Settings.MobileSettings.useBonusSystem;
            InitializeComponent();
            setBonus();
            Frame.SetAppThemeColor(Frame.BorderColorProperty, Color.FromHex(Settings.MobileSettings.color),
                Color.Transparent);

            var close = new TapGestureRecognizer();
            close.Tapped += async (s, e) => { await PopupNavigation.Instance.PopAsync(); };
            IconViewClose.GestureRecognizers.Add(close);

            var openUrl = new TapGestureRecognizer();
            openUrl.Tapped += async (s, e) =>
            {
                try
                {
                    await Launcher.OpenAsync("https://" + Settings.MobileSettings.bonusOfertaFile.Replace("https://", "").Replace("http://", ""));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorAdditionalLink, "OK");
                }
            };
            LabelDoc.GestureRecognizers.Add(openUrl);
            FormattedString formatted = new FormattedString();
            formatted.Spans.Add(new Span
            {
                Text = $"{AppResources.Total}: ",
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
                Text = $" {AppResources.Currency}",
                FontSize = 15,
                TextColor = Color.FromHex("#777777")
            });
            LabelTotal.FormattedText = formatted;
            BindingContext = this;
        }

        async void setBonus()
        {
            RestClientMP server = new RestClientMP();
            Bonus accountBonusBalance = await server.GetAccountBonusBalance(Settings.Person.Accounts[1].ID);
            bonusCount = accountBonusBalance.BonusBalance;
                // <Label.FormattedText>
                // <FormattedString>
                // <Span Text="Зачесть бонусами " TextColor="Gray"  FontSize="12" />
                // <Span Text="{Binding bonusCount}" TextColor ="Gray" FontAttributes="Bold" FontSize="15" />
                // <Span Text=" {x:Static xamarinJkh:AppResources.PayPoints}" TextColor ="Gray"  FontSize="12" />
                // <Span Text="&#10; * не более 20% от суммы" TextColor ="Gray" FontSize="10" />
                //                                 
                // </FormattedString>
                
                FormattedString formattedBonus = new FormattedString();

                formattedBonus.Spans.Add(new Span
                {
                    Text = "Зачесть бонусами ",
                    FontSize = 12,
                    TextColor = Color.Gray,
                });
                formattedBonus.Spans.Add(new Span
                {
                    Text = bonusCount.ToString(),
                    TextColor = Color.Gray,
                    FontAttributes = FontAttributes.Bold,
                    FontSize = 15,
                });
                formattedBonus.Spans.Add(new Span
                {
                    Text = AppResources.PayPoints,
                    TextColor = Color.Gray,
                    FontSize = 12
                });
                formattedBonus.Spans.Add(new Span
                {
                    Text = "\n * не более 20% от суммы",
                    TextColor = Color.Gray,
                    FontSize = 10
                });

                LabelBonus.FormattedText = formattedBonus;
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
                await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorPayApp, "OK");
            }
        }

        private void btnCardPay_Clicked(object sender, EventArgs e)
        {
        }

        private void btnCashPay_Clicked(object sender, EventArgs e)
        {
        }

        private void CheckBox_OnCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            FrameBtnAdd.IsEnabled = CheckBox.IsChecked;
            BtnAdd.IsEnabled = CheckBox.IsChecked;
        }
    }
}