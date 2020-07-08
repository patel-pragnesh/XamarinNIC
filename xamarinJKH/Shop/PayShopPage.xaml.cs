using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
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
        private RestClientMP _server = new RestClientMP();
        public Dictionary<String, Goods> Goodset { get; set; }
        public AdditionalService _Additional { get; set; }

        public PayShopPage(Dictionary<string, Goods> goodset, AdditionalService additional)
        {
            Goodset = goodset;
            _Additional = additional;
            InitializeComponent();
            var techSend = new TapGestureRecognizer();
            techSend.Tapped += async (s, e) => {     await Navigation.PushAsync(new TechSendPage()); };
            LabelTech.GestureRecognizers.Add(techSend);
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    BackgroundColor = Color.White;
                    ImageTop.Margin = new Thickness(0, 0, 0, 0);
                    StackLayout.Margin = new Thickness(0, 33, 0, 0);
                    //IconViewNameUk.Margin = new Thickness(0, 33, 0, 0);
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
            BindingContext = this;
            SetPriceAndWeight();
        }

        //async void PayGoods()
        //{
        //    try
        //    {
        //        progress.IsVisible = true;
        //        BtnCheckOut.IsEnabled = false;
        //        if (Settings.Person.Accounts.Count > 0)
        //        {
        //            IDResult result = await _server.newAppPay(Settings.Person.Accounts[0].Ident,
        //                _Additional.id_RequestType.ToString(), getBuscketStr(), true, SetPriceAndWeight(), "Покупка в магазине " + _Additional.ShopName);

        //            if (result.Error == null)
        //            {
        //                RequestsUpdate requestsUpdate =
        //                    await _server.GetRequestsUpdates(Settings.UpdateKey, result.ID.ToString());
        //                if (requestsUpdate.Error == null)
        //                {
        //                    Settings.UpdateKey = requestsUpdate.NewUpdateKey;
        //                }

        //                await DisplayAlert("Успешно", "Заказ успешно оформлен", "OK");
        //                // foreach (var ePage in Settings.AppPAge)
        //                // {
        //                //     Navigation.RemovePage(ePage);
        //                // }

        //                RequestInfo requestInfo = new RequestInfo();
        //                requestInfo.ID = result.ID;
        //                await Navigation.PushAsync(new AppPage(requestInfo, true));

        //            }
        //            else
        //            {
        //                await DisplayAlert("Ошибка", result.Error, "OK");
        //            }
        //        }
        //        else
        //        {
        //            await DisplayAlert("Ошибка", "Подключите лицевой счет", "OK");
        //        }

        //        progress.IsVisible = false;
        //        BtnCheckOut.IsEnabled = true;

        //    }
        //    catch (Exception ex)
        //    {                
        //        await DisplayAlert("Ошибка", "Во время выполнения проищошла ошибка", "OK");
        //        BtnCheckOut.IsEnabled = true;
        //    }
        //}

        string getBuscketStr()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Ваш заказ:\n");
            int i = 0;
            foreach (var each in Goodset)
            {
                Goods value = each.Value;
                if (value.ColBusket != 0)
                {
                    stringBuilder.Append(i + 1)
                        .Append(") ").Append(value.Name).Append(" кол-во: ")
                        .Append(value.ColBusket).Append(" цена: ").Append(value.priceBusket)
                        .Append("\n");
                    i++;
                }
            }

            stringBuilder.Append("Итого: ").Append(LabelPriceBuscket.Text.Replace("\u20BD", "Р")).Append(" ").Append(LabelWeightBuscket.Text);
            stringBuilder.Append("\nБезналичный расчет.");

            return stringBuilder.ToString();
        }        

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
                        IDResult result = await _server.newAppPay(Settings.Person.Accounts[0].Ident,
                            _Additional.id_RequestType.ToString(), getBuscketStr(), true, SetPriceAndWeight(), "Покупка в магазине " + _Additional.ShopName);

                        if (result.Error == null)
                        {
                            RequestsUpdate requestsUpdate =
                                await _server.GetRequestsUpdates(Settings.UpdateKey, result.ID.ToString());
                            if (requestsUpdate.Error == null)
                            {
                                Settings.UpdateKey = requestsUpdate.NewUpdateKey;
                            }

                            await DisplayAlert("Успешно", "Заказ успешно оформлен", "OK");
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
                            await DisplayAlert("Ошибка", result.Error, "OK");
                        }
                    }
                    else
                    {
                        await DisplayAlert("Ошибка", "Подключите лицевой счет", "OK");
                    }

                    progress.IsVisible = false;
                    BtnCheckOut.IsEnabled = true;

                }
                catch (Exception ex)
                {
                    await DisplayAlert("Ошибка", "Во время выполнения проищошла ошибка", "OK");
                    BtnCheckOut.IsEnabled = true;
                }
            }
            else
            {
                await DisplayAlert("Ошибка", "Корзина пуста", "OK");
            }
        }

        private void btnCashPay_Clicked(object sender, EventArgs e)
        {
            PaymentDescription0.Text = "Оплата наличными при получении";
            PaymentDescription1.Text = "В момент получения товара передайте курьеру код подтверждения и деньги";
            btnCardPay.TextColor = Color.Gray;
            frameBtnCardPay.BorderColor = Color.Gray;

            btnCashPay.TextColor = hex;
            frameBtnCashPay.BorderColor = hex;
        }

        private void btnCardPay_Clicked(object sender, EventArgs e)
        {
            PaymentDescription0.Text = "Оплата картой при получении";

            //под замену на текст для карты. запросить текст для карты.
            PaymentDescription1.Text = "В момент получения товара передайте курьеру код подтверждения и деньги";
            btnCardPay.TextColor = hex;
            frameBtnCardPay.BorderColor = hex;

            btnCashPay.TextColor = Color.Gray;
            frameBtnCashPay.BorderColor = Color.Gray;

        }
    }
}