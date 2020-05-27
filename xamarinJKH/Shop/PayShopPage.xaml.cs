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
            var payGoods = new TapGestureRecognizer();
            payGoods.Tapped += async (s, e) => { PayGoods(); };
            StackLayouPay.GestureRecognizers.Add(payGoods);
            hex = Color.FromHex(Settings.MobileSettings.color);
            SetText();
            BindingContext = this;
            SetPriceAndWeight();
        }

        async void PayGoods()
        {
            progress.IsVisible = true;
            StackLayouPay.IsVisible = false;
            if (Settings.Person.Accounts.Count > 0)
            {
                IDResult result = await _server.newApp(Settings.Person.Accounts[0].Ident,
                    _Additional.id_RequestType.ToString(), getBuscketStr());

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

            progress.IsVisible = false;
            StackLayouPay.IsVisible = true;
        }

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

        void SetText()
        {
            UkName.Text = Settings.MobileSettings.main_name;
            LabelPhone.Text = "+" + Settings.Person.Phone;
        }

        void SetPriceAndWeight()
        {
            decimal sumBasket = 0;
            decimal sumWeightBasket = 0;
            foreach (var each in Goodset)
            {
                sumBasket += each.Value.priceBusket;
                sumWeightBasket += each.Value.weightBusket;
            }

            LabelPriceBuscket.Text = sumBasket.ToString() + " \u20BD";
            LabelWeightBuscket.Text = (sumWeightBasket / 1000).ToString() + "кг.";
        }
    }
}