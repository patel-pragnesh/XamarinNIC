using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PancakeView;
using Xamarin.Forms.Xaml;
using xamarinJKH.Apps;
using xamarinJKH.Server;
using xamarinJKH.Server.RequestModel;
using xamarinJKH.Tech;
using xamarinJKH.Utils;

namespace xamarinJKH.Shop
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PayShopPage : ContentPage
    {
        public Color hex { get; set; }

        public decimal bonusCount { get; set; }
        private RestClientMP _server = new RestClientMP();
        public Dictionary<String, Goods> Goodset { get; set; }
        public List<AccountInfo> AllAcc { get; set; }
        public bool isBonusVisible { get; set; }
        public AccountInfo SelectedAcc { get; set; }
        public AdditionalService _Additional { get; set; }
        List<RequestsReceiptItem> ReceiptItems = new List<RequestsReceiptItem>();

        public PayShopPage(Dictionary<string, Goods> goodset, AdditionalService additional)
        {
            Goodset = goodset;
            _Additional = additional;
            GoodsIsVisible = Settings.GoodsIsVisible;
            AllAcc = Settings.Person.Accounts;
            SelectedAcc = Settings.Person.Accounts[0];
            isBonusVisible = Settings.MobileSettings.useBonusSystem;
            InitializeComponent();
            var openUrl = new TapGestureRecognizer();
            openUrl.Tapped += async (s, e) =>
            {
                try
                {
                    await Launcher.OpenAsync("https://" + Settings.MobileSettings.bonusOfertaFile
                        .Replace("https://", "").Replace("http://", ""));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorAdditionalLink, "OK");
                }
            };
            LabelDoc.GestureRecognizers.Add(openUrl);
            var techSend = new TapGestureRecognizer();
            techSend.Tapped += async (s, e) => { await Navigation.PushAsync(new TechSendPage()); };
            LabelTech.GestureRecognizers.Add(techSend);

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    break;
                case Device.Android:
                default:
                    break;
            }

            try
            {
                Settings.AppPAge.Remove(this);
                Settings.AppPAge.Add(this);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            NavigationPage.SetHasNavigationBar(this, false);
            var backClick = new TapGestureRecognizer();
            backClick.Tapped += async (s, e) => { _ = await Navigation.PopAsync(); };
            BackStackLayout.GestureRecognizers.Add(backClick);

            hex = Color.FromHex(Settings.MobileSettings.color);
            Color hexColor = (Color) Application.Current.Resources["MainColor"];
            IconViewTech.SetAppThemeColor(IconView.ForegroundProperty, hexColor, Color.White);
            LabelTech.SetAppThemeColor(Label.TextColorProperty, hexColor, Color.White);
            GoodsLayot.SetAppThemeColor(Frame.BorderColorProperty, hexColor, Color.White);
            BindingContext = this;
            SetPriceAndWeight();
        }

        string getBuscketStr()
        {
            ReceiptItems.Clear();
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"{AppResources.YourOrder}\n");
            int i = 0;
            foreach (var each in Goodset)
            {
                Goods value = each.Value;
                if (value.ColBusket != 0)
                {
                    decimal bonus = 0;
                    if (isBonusVisible && CheckBoxBonus.IsChecked)
                    {
                        decimal percent = getPercent() / 100;

                        bonus = value.priceBusket * percent;

                        if ((bonusCount - bonus) > 0)
                        {
                            bonusCount -= bonus;
                        }
                        else
                        {
                            bonus = 0;
                        }
                    }

                    ReceiptItems.Add(new RequestsReceiptItem()
                    {
                        Name = value.Name,
                        Price = value.Price,
                        Quantity = value.ColBusket,
                        Amount = value.priceBusket,
                        BonusAmount = bonus,
                        ID = value.ID
                    });
                    
                    
                    
                    stringBuilder.Append(i + 1)
                        .Append(") ").Append(value.Name).Append(AppResources.Amount)
                        .Append(value.ColBusket).Append(AppResources.Amount2).Append(AppResources.PriceLowerCase)
                        .Append(value.priceBusket).Append(AppResources.Currency)
                        .Append("\n");
                    i++;
                }
            }

            stringBuilder.Append($"{AppResources.TotalPrice}: ").Append(LabelPriceBuscket.Text)
                .Append($"{AppResources.Currency}\n").Append(LabelWeightBuscket.Text).Append(" г.");
            // stringBuilder.Append("\nБезналичный расчет.");

            return stringBuilder.ToString();
        }

        decimal getPercent()
        {
            foreach (var eRate in _Additional.BonusDiscountRates)
            {
                if (eRate.Ident.Equals(SelectedAcc.Ident))
                {
                    return eRate.Rate;
                }
            }

            return 0;
        }

        public bool GoodsIsVisible { get; set; }

        decimal SetPriceAndWeight()
        {
            decimal sumBasket = 0;
            decimal sumWeightBasket = 0;
            sumBasket = Goodset.Sum(_ => _.Value.priceBusket);
            sumWeightBasket = Goodset.Sum(_ => _.Value.weightBusket);

            LabelPriceBuscket.Text = Convert.ToString(sumBasket);
            LabelWeightBuscket.Text = Convert.ToString(sumWeightBasket);

            return sumBasket;
        }

        private async void BtnCheckOut_Clicked(object sender, EventArgs e)
        {
            if (!LabelPriceBuscket.Text.Equals("0"))
            {
                try
                {
                    progress.IsVisible = true;
                    BtnCheckOut.IsEnabled = false;
                    if (Settings.Person.Accounts.Count > 0)
                    {
                        IDResult result = await _server.newAppPay(SelectedAcc.Ident,
                            _Additional.id_RequestType.ToString(), getBuscketStr(), true,
                            SetPriceAndWeight(),
                            "Покупка в магазине " + _Additional.ShopName, ReceiptItems, _Additional.ShopID);

                        if (result.Error == null)
                        {
                            RequestsUpdate requestsUpdate =
                                await _server.GetRequestsUpdates(Settings.UpdateKey, result.ID.ToString());
                            if (requestsUpdate.Error == null)
                            {
                                Settings.UpdateKey = requestsUpdate.NewUpdateKey;
                            }

                            await DisplayAlert(AppResources.AlertSuccess, AppResources.PayShopSuccess, "OK");
                            // foreach (var ePage in Settings.AppPAge)
                            // {
                            //     Navigation.RemovePage(ePage);
                            // }

                            RequestInfo requestInfo = new RequestInfo();
                            requestInfo.ID = result.ID;
                            await Navigation.PushAsync(new AppPage(requestInfo, true));
                        }
                        else
                        {
                            await DisplayAlert(AppResources.ErrorTitle, result.Error, "OK");
                        }
                    }
                    else
                    {
                        await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorAddIdent, "OK");
                    }

                    progress.IsVisible = false;
                    BtnCheckOut.IsEnabled = true;
                }
                catch (Exception ex)
                {
                    await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorUnknown, "OK");
                    BtnCheckOut.IsEnabled = true;
                }
            }
            else
            {
                await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorBasketEmpty, "OK");
            }
        }

        private void btnCashPay_Clicked(object sender, EventArgs e)
        {
            PaymentDescription0.Text = AppResources.PayText0;
            PaymentDescription1.Text = AppResources.PayText1;
            btnCardPay.TextColor = Color.Gray;
            frameBtnCardPay.BorderColor = Color.Gray;

            btnCashPay.TextColor = hex;
            frameBtnCashPay.BorderColor = hex;
        }

        private void btnCardPay_Clicked(object sender, EventArgs e)
        {
            PaymentDescription0.Text = AppResources.PayText2;

            //под замену на текст для карты. запросить текст для карты.
            PaymentDescription1.Text = AppResources.PayText3;
            btnCardPay.TextColor = hex;
            frameBtnCardPay.BorderColor = hex;

            btnCashPay.TextColor = Color.Gray;
            frameBtnCashPay.BorderColor = Color.Gray;
        }

        private void CheckBox_OnCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            FrameBtn.IsEnabled = CheckBox.IsChecked;
            BtnCheckOut.IsEnabled = CheckBox.IsChecked;
        }

        async void setBonus()
        {
            RestClientMP server = new RestClientMP();
            Bonus accountBonusBalance = await server.GetAccountBonusBalance(SelectedAcc.ID);
            bonusCount = accountBonusBalance.BonusBalance;


            FormattedString formattedBonus = new FormattedString();

            formattedBonus.Spans.Add(new Span
            {
                Text = AppResources.AddBonus,
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
                Text = "\n" + AppResources.NoMore + getPercent() + "% " + AppResources.AtSumm,
                TextColor = Color.Gray,
                FontSize = 10
            });

            LabelBonus.FormattedText = formattedBonus;
        }

        private void PickerLs_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            setBonus();
        }
    }
}