using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Markup;
using Xamarin.Forms.Xaml;
using xamarinJKH.Server.RequestModel;
using xamarinJKH.Utils;

namespace xamarinJKH.Shop
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BasketPage : ContentPage
    {
        public Dictionary<String, Goods> Goodset { get; set; }
        public Color hex { get; set; }

        public string orderNum { get; set; }

        private ShopPage _shopPage;
        public AdditionalService _Additional { get; set; }
        public BasketPage(Dictionary<string, Goods> goodset, ShopPage shopPage, AdditionalService additional)
        {
            //заглушка для номера заказа
            orderNum = " №" + Convert.ToString(1234567890);

            Goodset = goodset;
            _shopPage = shopPage;
            _Additional = additional;
            InitializeComponent();
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    BackgroundColor = Color.White;
                    ImageTop.Margin = new Thickness(0, 0, 0, 0);
                    StackLayout.Margin = new Thickness(0, 33, 0, 0);
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

            backClick.Tapped += async (s, e) =>
            {
                UpdateShop();
                _ = await Navigation.PopAsync();
            };
            
            BackStackLayout.GestureRecognizers.Add(backClick);
            //var PayPageShop = new TapGestureRecognizer();
            //PayPageShop.Tapped += async (s, e) =>
            //{
            //    if (!LabelPriceBuscket.Text.Equals("0 \u20BD"))
            //    {
            //        await Navigation.PushAsync(new PayShopPage(Goodset, _Additional));
            //    }
            //    else
            //    {
            //        await DisplayAlert("Ошибка", "Корзина пуста", "OK");
            //    }
            //};
            //StackLayoutBasket.GestureRecognizers.Add(PayPageShop);
            hex = Color.FromHex(Settings.MobileSettings.color);
            //SetText();
            SetPriceAndWeight();
            //setBasket();
            BindingContext = this;
        }

        private void UpdateShop()
        {
            _shopPage.Goodset = Goodset;
            _shopPage.SetPriceAndWeight();
            _shopPage.isUpdate = true;
            _shopPage.updateGoods(_shopPage.position);
        }

        //void SetText()
        //{
        //    UkName.Text = Settings.MobileSettings.main_name;
        //    LabelPhone.Text = "+" + Settings.Person.Phone;
        //}

        protected override bool OnBackButtonPressed()
        {
            UpdateShop();
            return base.OnBackButtonPressed();
        }

        //void setBasket()
        //{
        //    StackLayout rootStackLayout = new StackLayout();

        //    StackLayout stackLayout = new StackLayout();
        //    foreach (var goods in Goodset)
        //    {
        //        var each = goods.Value;
        //        Frame frame = new Frame();
        //        frame.HorizontalOptions = LayoutOptions.FillAndExpand;
        //        frame.VerticalOptions = LayoutOptions.Start;
        //        frame.BackgroundColor = Color.White;
        //        frame.Margin = new Thickness(10, 10, 10, 0);
        //        frame.Padding = new Thickness(15, 15, 15, 15);
        //        frame.CornerRadius = 10;

        //        StackLayout list = new StackLayout();
        //        list.Orientation = StackOrientation.Horizontal;

        //        Image imageGoods = new Image();
        //        imageGoods.HeightRequest = 50;
        //        imageGoods.WidthRequest = 50;
        //        imageGoods.Source = ImageSource.FromFile("ic_no_photo");
        //        if (each.HasImage)
        //        {
        //            imageGoods.Source =
        //                ImageSource.FromUri(new Uri("https://api.sm-center.ru/test_erc_udm/public/GoodsImage/" +
        //                                            each.ID.ToString()));
        //        }


        //        Label LabelName = new Label();
        //        LabelName.TextColor = Color.Black;
        //        LabelName.FontSize = 15;
        //        LabelName.Text = each.Name;
        //        LabelName.Margin = new Thickness(10, 0, 0, 0);
        //        LabelName.HorizontalOptions = LayoutOptions.FillAndExpand;
        //        LabelName.VerticalTextAlignment = TextAlignment.Start;

        //        StackLayout stackLayoutprice = new StackLayout();

        //        Label LabelPrice = new Label();
        //        LabelPrice.TextColor = Color.Black;
        //        LabelPrice.FontSize = 15;
        //        LabelPrice.Text = each.Price.ToString() + " \u20BD";
        //        LabelPrice.HorizontalOptions = LayoutOptions.EndAndExpand;
        //        LabelPrice.VerticalTextAlignment = TextAlignment.Start;

        //        Label labelCount = new Label();
        //        labelCount.TextColor = Color.Gray;
        //        labelCount.FontSize = 15;
        //        labelCount.HorizontalOptions = LayoutOptions.Center;
        //        labelCount.Text = each.ColBusket.ToString() + " " + each.Units;

        //        StackLayout stackLayoutAddAndMin = new StackLayout();
        //        stackLayoutAddAndMin.Orientation = StackOrientation.Horizontal;

        //        IconView iconViewPlus = new IconView();
        //        iconViewPlus.Source = "ic_plus";
        //        iconViewPlus.Foreground = Color.FromHex(Settings.MobileSettings.color);
        //        iconViewPlus.HeightRequest = 15;
        //        iconViewPlus.WidthRequest = 15;


        //        var addItem = new TapGestureRecognizer();
        //        addItem.Tapped += async (s, e) =>
        //        {
        //            if (each.Rest != 0)
        //            {
        //                each.Rest--;
        //                each.ColBusket++;
        //                labelCount.Text = each.ColBusket.ToString() + " " + each.Units;
        //                each.priceBusket = (decimal) (each.Price * each.ColBusket);
        //                each.weightBusket = (decimal) (each.Weight * each.ColBusket);


        //                try
        //                {
        //                    Goodset.Add(each.ID.ToString(), each);
        //                }
        //                catch (Exception ex)
        //                {
        //                    Goodset[each.ID.ToString()] = each;
        //                }


        //                // await DisplayAlert("Добавление", "Товар: " + each.Name + " добавили в корзину", "OK");
        //            }
        //            else
        //            {
        //                await DisplayAlert("Ошибка", "Товар: " + each.Name + " закончился", "OK");
        //            }

        //            SetPriceAndWeight();
        //        };
        //        iconViewPlus.GestureRecognizers.Add(addItem);

        //        IconView iconViewMinus = new IconView();
        //        iconViewMinus.Source = "ic_minus";
        //        iconViewMinus.Foreground = Color.FromHex(Settings.MobileSettings.color);
        //        iconViewMinus.HeightRequest = 15;
        //        iconViewMinus.WidthRequest = 15;


        //        var dellItem = new TapGestureRecognizer();
        //        dellItem.Tapped += async (s, e) =>
        //        {
        //            if (each.ColBusket != 0)
        //            {
        //                each.Rest++;
        //                each.ColBusket--;
        //                labelCount.Text = each.ColBusket.ToString() + " " + each.Units;
        //                each.priceBusket = (decimal) (each.Price * each.ColBusket);
        //                each.weightBusket = (decimal) (each.Weight * each.ColBusket);


        //                if (each.ColBusket > 0)
        //                {
        //                    Goodset[each.ID.ToString()] = each;
        //                }
        //                else
        //                {
        //                    Goodset.Remove(each.ID.ToString());
        //                }


        //                // await DisplayAlert("Добавление", "Товар: " + each.Name + " убрали из корзины", "OK");
        //            }
        //            else
        //            {
        //                await DisplayAlert("Ошибка", "Товара: " + each.Name + " в корзине больше нет", "OK");
        //            }

        //            SetPriceAndWeight();
        //        };
        //        iconViewMinus.GestureRecognizers.Add(dellItem);

        //        stackLayoutAddAndMin.HorizontalOptions = LayoutOptions.Center;
        //        stackLayoutAddAndMin.Children.Add(iconViewMinus);
        //        stackLayoutAddAndMin.Children.Add(iconViewPlus);

        //        stackLayoutprice.Children.Add(LabelPrice);
        //        stackLayoutprice.Children.Add(labelCount);
        //        stackLayoutprice.Children.Add(stackLayoutAddAndMin);

        //        list.Children.Add(imageGoods);
        //        list.Children.Add(LabelName);
        //        list.Children.Add(stackLayoutprice);
        //        frame.Content = list;
        //        stackLayout.Children.Add(frame);
        //    }


        //    rootStackLayout.Children.Add(stackLayout);

        //    StackLayoutConatiner.Children.Add(rootStackLayout);
        //}

        void SetPriceAndWeight()
        {
            decimal sumBasket = 0;
            decimal sumWeightBasket = 0;
            foreach (var each in Goodset)
            {
                sumBasket += each.Value.priceBusket;
                sumWeightBasket += each.Value.weightBusket;
            }

            LabelPriceBuscket.Text = sumBasket.ToString();
            LabelWeightBuscket.Text = (sumWeightBasket).ToString();

            
        }

        private async void BtnCheckOut_Clicked(object sender, EventArgs e)
        {
            if (!LabelPriceBuscket.Text.Equals("0"))
            {
                await Navigation.PushAsync(new PayShopPage(Goodset, _Additional));
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
            frameBtnCardPay.BorderColor= Color.Gray;

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