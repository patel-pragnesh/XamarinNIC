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
            setBasket();
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

        Color colorFromMobileSettings = Color.FromHex(Settings.MobileSettings.color);
        const string GOODS_IMAGE_URI = "https://api.sm-center.ru/test_erc_udm/public/GoodsImage/";


        void setBasket()
        {
            StackLayoutConatiner.Children.Clear();

            var icViewPlusMinusSize = 22;

            foreach (var goods in Goodset)
            {
                var each = goods.Value;
                StackLayout list = new StackLayout();
                list.Orientation = StackOrientation.Horizontal;
                list.Padding = new Thickness(10, 10, 10, 10);
                list.HorizontalOptions = LayoutOptions.FillAndExpand;
                list.VerticalOptions = LayoutOptions.Start;

                Image imageGoods = new Image();

                imageGoods.HeightRequest = 35;
                imageGoods.WidthRequest = 35;
                if (each.HasImage)
                {
                    imageGoods.Source =
                        ImageSource.FromUri(new Uri(GOODS_IMAGE_URI +
                                                    each.ID.ToString()));
                }
                else
                {
                    imageGoods.Source = ImageSource.FromFile("ic_no_photo");
                }

                Frame imgFrame = new Frame();
                imgFrame.CornerRadius = 10;
                imgFrame.HeightRequest = 40;
                imgFrame.WidthRequest = 40;
                imgFrame.VerticalOptions = LayoutOptions.Start;
                imgFrame.BackgroundColor = Color.FromHex("#e7e7e7");
                imgFrame.Padding = new Thickness(5, 5, 5, 5);
                imgFrame.Content = imageGoods;


                Label LabelName = new Label();
                LabelName.TextColor = Color.Black;
                LabelName.FontSize = 15;
                LabelName.FontAttributes = FontAttributes.Bold;

                FormattedString LabelNameFormattedString = new FormattedString();

                Span name = new Span();
                name.Text = each.Name;
                name.TextColor = Color.Black;
                name.FontAttributes = FontAttributes.Bold;
                name.FontSize = 15;

                LabelNameFormattedString.Spans.Add(name);

                if (each.Weight != 0)
                {
                    Span weigt = new Span();
                    weigt.Text = "\n" + each.Weight.ToString() + " гр";
                    weigt.TextColor = Color.Gray;
                    weigt.FontSize = 10;
                    LabelNameFormattedString.Spans.Add(weigt);
                }

                LabelName.FormattedText = LabelNameFormattedString;

                LabelName.Margin = new Thickness(10, 0, 0, 0);
                LabelName.HorizontalOptions = LayoutOptions.FillAndExpand;
                LabelName.VerticalTextAlignment = TextAlignment.Start;

                StackLayout stackLayoutprice = new StackLayout();

                Label LabelPrice = new Label();
                LabelPrice.HorizontalOptions = LayoutOptions.EndAndExpand;
                LabelPrice.VerticalTextAlignment = TextAlignment.Start;

                FormattedString LabelPriceFormattedString = new FormattedString();

                Span price = new Span();
                price.Text = each.Price.ToString();
                price.TextColor = Color.Black;
                price.FontAttributes = FontAttributes.Bold;
                price.FontSize = 15;
                LabelPriceFormattedString.Spans.Add(price);

                Span rub = new Span();
                rub.Text = " руб";
                rub.TextColor = Color.Gray;
                rub.FontSize = 10;
                LabelPriceFormattedString.Spans.Add(rub);

                LabelPrice.FormattedText = LabelPriceFormattedString;

                Label labelCount = new Label();
                labelCount.Margin = new Thickness(10, 0);
                labelCount.TextColor = Color.Gray;
                labelCount.FontSize = 15;
                labelCount.HorizontalOptions = LayoutOptions.Center;
                labelCount.Text = each.ColBusket.ToString();

                StackLayout stackLayoutAddAndMin = new StackLayout();
                stackLayoutAddAndMin.Orientation = StackOrientation.Horizontal;

                IconView iconViewPlus = new IconView();
                iconViewPlus.Source = "ic_shop_plus";
                iconViewPlus.Foreground = colorFromMobileSettings;
                iconViewPlus.HeightRequest = icViewPlusMinusSize;
                iconViewPlus.WidthRequest = icViewPlusMinusSize;

                var addItem = new TapGestureRecognizer();
                addItem.Tapped += async (s, e) =>
                {
                    if (each.Rest != 0)
                    {
                        each.Rest--;
                        each.ColBusket++;
                        labelCount.Text = each.ColBusket.ToString();
                        each.priceBusket = (decimal)(each.Price * each.ColBusket);
                        each.weightBusket = (decimal)(each.Weight * each.ColBusket);
                        try
                        {
                            Goodset.Add(each.ID.ToString(), each);
                        }
                        catch (Exception ex)
                        {
                            Goodset[each.ID.ToString()] = each;
                        }
                    }
                    else
                    {
                        await DisplayAlert("Ошибка", "Товар: " + each.Name + " закончился", "OK");
                    }

                    SetPriceAndWeight();
                };
                iconViewPlus.GestureRecognizers.Add(addItem);

                IconView iconViewMinus = new IconView();
                iconViewMinus.Source = "ic_shop_minus";
                iconViewMinus.Foreground = colorFromMobileSettings;
                iconViewMinus.HeightRequest = icViewPlusMinusSize;
                iconViewMinus.WidthRequest = icViewPlusMinusSize;


                var dellItem = new TapGestureRecognizer();
                dellItem.Tapped += async (s, e) =>
                {
                    if (each.ColBusket != 0)
                    {
                        each.Rest++;
                        each.ColBusket--;
                        labelCount.Text = each.ColBusket.ToString();
                        if (each.ColBusket == 0)
                            labelCount.TextColor = Color.Gray;
                        each.priceBusket = (decimal)(each.Price * each.ColBusket);
                        each.weightBusket = (decimal)(each.Weight * each.ColBusket);


                        if (each.ColBusket > 0)
                        {
                            Goodset[each.ID.ToString()] = each;
                        }
                        else
                        {
                            Goodset.Remove(each.ID.ToString());
                        }

                    }
                    else
                    {
                        await DisplayAlert("Ошибка", "Товара: " + each.Name + " в корзине больше нет", "OK");
                    }

                    SetPriceAndWeight();
                };
                iconViewMinus.GestureRecognizers.Add(dellItem);

                stackLayoutAddAndMin.HorizontalOptions = LayoutOptions.Center;
                stackLayoutAddAndMin.Children.Add(iconViewMinus);
                stackLayoutAddAndMin.Children.Add(labelCount);
                stackLayoutAddAndMin.Children.Add(iconViewPlus);

                //stackLayoutprice.Children.Add(LabelPrice);
                //stackLayoutprice.Children.Add(labelCount);
                //stackLayoutprice.Children.Add(stackLayoutAddAndMin);

                //list.Children.Add(imageGoods);
                //list.Children.Add(LabelName);
                //list.Children.Add(stackLayoutprice);

                //stackLayout.Children.Add(list);
                StackLayout l1 = new StackLayout() { Orientation = StackOrientation.Vertical };

                l1.Children.Add(stackLayoutAddAndMin);

                var b1 = new BoxView() { Color = colorFromMobileSettings, HeightRequest = 1, Margin = new Thickness(0, -5, 0, 0) };
                l1.Children.Add(b1);

                stackLayoutprice.Children.Add(LabelPrice);
                stackLayoutprice.Children.Add(l1);

                list.Children.Add(imgFrame);
                list.Children.Add(LabelName);
                list.Children.Add(stackLayoutprice);

                StackLayoutConatiner.Children.Add(list);

                //разделитель
                BoxView b = new BoxView() { Color = Color.FromHex("#e7e7e7"), HeightRequest = 1, Margin = new Thickness(10, 0) };
                StackLayoutConatiner.Children.Add(b);
            }
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
            TotalWeigth.Text = Convert.ToString( sumWeightBasket);
            TotalPrice.Text = Convert.ToString(sumBasket);          
        }


        private async void BtnCheckOut_Clicked(object sender, EventArgs e)
        {
            if (!TotalPrice.Text.Equals("0"))
            {
                await Navigation.PushAsync(new PayShopPage(Goodset, _Additional));
            }
            else
            {
                await DisplayAlert("Ошибка", "Корзина пуста", "OK");
            }
        }

        //private void btnCashPay_Clicked(object sender, EventArgs e)
        //{
        //    PaymentDescription0.Text = "Оплата наличными при получении";
        //    PaymentDescription1.Text = "В момент получения товара передайте курьеру код подтверждения и деньги";
        //    btnCardPay.TextColor = Color.Gray;
        //    frameBtnCardPay.BorderColor= Color.Gray;

        //    btnCashPay.TextColor = hex;
        //    frameBtnCashPay.BorderColor = hex;
        //}

        //private void btnCardPay_Clicked(object sender, EventArgs e)
        //{
        //    PaymentDescription0.Text = "Оплата картой при получении";

        //    //под замену на текст для карты. запросить текст для карты.
        //    PaymentDescription1.Text = "В момент получения товара передайте курьеру код подтверждения и деньги";
        //    btnCardPay.TextColor = hex;
        //    frameBtnCardPay.BorderColor = hex;

        //    btnCashPay.TextColor = Color.Gray;
        //    frameBtnCashPay.BorderColor = Color.Gray;

        //}
    }
}