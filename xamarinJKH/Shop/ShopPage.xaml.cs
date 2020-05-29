using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamarinJKH.Server;
using xamarinJKH.Server.RequestModel;
using xamarinJKH.Utils;

namespace xamarinJKH.Shop
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ShopPage : ContentPage
    {
        private RestClientMP server = new RestClientMP();

        private List<ScrollView> pages = new List<ScrollView>();
        public List<Goods> Goodses { get; set; }
        public Dictionary<string, Goods> Goodset { get; set; }

        public List<KeysModel> Keys { get; set; }

        public Color hex { get; set; }
        public AdditionalService _Additional { get; set; }

        public int position { get; set; }

        private decimal Price = 0;
        private decimal Weight = 0;
        public Dictionary<string, List<Goods>> CategoriesGoods { get; set; }

        public bool isUpdate = false;

        public ShopPage(AdditionalService additional)
        {
            _Additional = additional;
            InitializeComponent();
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    BackgroundColor = Color.White;
                    ImageTop.Margin = new Thickness(0, 0, 0, 0);
                    StackLayout.Margin = new Thickness(0, 33, 0, 0);
                    IconViewNameUk.Margin = new Thickness(0, 33, 0, 0);
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
            var basketPage = new TapGestureRecognizer();
            basketPage.Tapped += async (s, e) =>
            {
                if (Goodset.Count > 0)
                {
                    await Navigation.PushAsync(new BasketPage(Goodset, this, _Additional));
                }
                else
                {
                    await DisplayAlert("Ошибка", "Корзина пуста", "OK");
                }
            };
            StackLayoutBasket.GestureRecognizers.Add(basketPage);

            SetText();
            GetGoods();
        }

        async void GetGoods()
        {
            ItemsList<Goods> itemsGoods = await server.GetShopGoods();
            if (itemsGoods.Error == null)
            {
                Goodses = itemsGoods.Data;
                Goodset = new Dictionary<string, Goods>();
                SetCategories();
            }
            else
            {
                await DisplayAlert("Ошибка", "Не удалось получить информацию о товарах", "OK");
            }
        }


        async void SetCategories()
        {
            CategoriesGoods = new Dictionary<string, List<Goods>>();
            Keys = new List<KeysModel>();
            foreach (var each in Goodses)
            {
                foreach (var str in each.Categories)
                {
                    if (CategoriesGoods.ContainsKey(str))
                    {
                        List<Goods> list = CategoriesGoods[str];
                        if (list != null)
                        {
                            list.Add(each);
                            CategoriesGoods[str] = list;
                        }
                    }
                    else
                    {
                        List<Goods> list = new List<Goods>();
                        list.Add(each);
                        CategoriesGoods.Add(str, list);
                    }
                }
            }

            setKeys();

            CarouselViewShop.SetBinding(ItemsView.ItemsSourceProperty, "Keys");
            SetCarusel();
        }


        void setKeys()
        {
            foreach (var each in CategoriesGoods)
            {
                KeysModel keysModel = new KeysModel();
                keysModel.Name = each.Key;
                keysModel.Goodses = each.Value;
                Keys.Add(keysModel);
            }

            this.BindingContext = this;
        }

        async void SetCarusel()
        {
            int i = 0;
            var itemTemplate = new DataTemplate(() =>
            {
                Label text = new Label();
                text.FontSize = 20;
                text.TextColor = Color.White;
                text.SetBinding(Label.TextProperty, "Name");

                StackLayout rootStackLayout = new StackLayout();

                StackLayout stackLayout = new StackLayout();

                foreach (var each in Keys[i].Goodses)
                {
                    Frame frame = new Frame();
                    frame.HorizontalOptions = LayoutOptions.FillAndExpand;
                    frame.VerticalOptions = LayoutOptions.Start;
                    frame.BackgroundColor = Color.White;
                    frame.Margin = new Thickness(0, 0, 0, 10);
                    frame.Padding = new Thickness(15, 15, 15, 15);
                    frame.CornerRadius = 10;

                    StackLayout list = new StackLayout();
                    list.Orientation = StackOrientation.Horizontal;

                    Image imageGoods = new Image();
                    imageGoods.HeightRequest = 50;
                    imageGoods.WidthRequest = 50;
                    imageGoods.Source = ImageSource.FromFile("ic_no_photo");
                    if (each.HasImage)
                    {
                        imageGoods.Source =
                            ImageSource.FromUri(new Uri("https://api.sm-center.ru/test_erc_udm/public/GoodsImage/" +
                                                        each.ID.ToString()));
                    }


                    Label LabelName = new Label();
                    LabelName.TextColor = Color.Black;
                    LabelName.FontSize = 15;
                    LabelName.Text = each.Name;
                    LabelName.Margin = new Thickness(10, 0, 0, 0);
                    LabelName.HorizontalOptions = LayoutOptions.FillAndExpand;
                    LabelName.VerticalTextAlignment = TextAlignment.Start;

                    StackLayout stackLayoutprice = new StackLayout();

                    Label LabelPrice = new Label();
                    LabelPrice.TextColor = Color.Black;
                    LabelPrice.FontSize = 15;
                    LabelPrice.Text = each.Price.ToString() + " \u20BD";
                    LabelPrice.HorizontalOptions = LayoutOptions.EndAndExpand;
                    LabelPrice.VerticalTextAlignment = TextAlignment.Start;

                    Label labelCount = new Label();
                    labelCount.TextColor = Color.Gray;
                    labelCount.FontSize = 15;
                    labelCount.HorizontalOptions = LayoutOptions.Center;
                    labelCount.Text = each.ColBusket.ToString() + " " + each.Units;

                    StackLayout stackLayoutAddAndMin = new StackLayout();
                    stackLayoutAddAndMin.Orientation = StackOrientation.Horizontal;

                    IconView iconViewPlus = new IconView();
                    iconViewPlus.Source = "ic_plus";
                    iconViewPlus.Foreground = Color.FromHex(Settings.MobileSettings.color);
                    iconViewPlus.HeightRequest = 15;
                    iconViewPlus.WidthRequest = 15;


                    var addItem = new TapGestureRecognizer();
                    addItem.Tapped += async (s, e) =>
                    {
                        if (each.Rest != 0)
                        {
                            each.Rest--;
                            each.ColBusket++;
                            labelCount.Text = each.ColBusket.ToString() + " " + each.Units;
                            each.priceBusket = (decimal) (each.Price * each.ColBusket);
                            each.weightBusket = (decimal) (each.Weight * each.ColBusket);

                            Price += each.priceBusket;
                            Weight += each.weightBusket;

                            try
                            {
                                Goodset.Add(each.ID.ToString(), each);
                            }
                            catch (Exception ex)
                            {
                                Goodset[each.ID.ToString()] = each;
                            }

                            isUpdate = true;
                            // await DisplayAlert("Добавление", "Товар: " + each.Name + " добавили в корзину", "OK");
                        }
                        else
                        {
                            await DisplayAlert("Ошибка", "Товар: " + each.Name + " закончился", "OK");
                        }

                        SetPriceAndWeight();
                    };
                    iconViewPlus.GestureRecognizers.Add(addItem);

                    IconView iconViewMinus = new IconView();
                    iconViewMinus.Source = "ic_minus";
                    iconViewMinus.Foreground = Color.FromHex(Settings.MobileSettings.color);
                    iconViewMinus.HeightRequest = 15;
                    iconViewMinus.WidthRequest = 15;


                    var dellItem = new TapGestureRecognizer();
                    dellItem.Tapped += async (s, e) =>
                    {
                        if (each.ColBusket != 0)
                        {
                            each.Rest++;
                            each.ColBusket--;
                            labelCount.Text = each.ColBusket.ToString() + " " + each.Units;
                            each.priceBusket = (decimal) (each.Price * each.ColBusket);
                            each.weightBusket = (decimal) (each.Weight * each.ColBusket);

                            Price += each.priceBusket;
                            Weight += each.weightBusket;

                            if (each.ColBusket > 0)
                            {
                                Goodset[each.ID.ToString()] = each;
                            }
                            else
                            {
                                Goodset.Remove(each.ID.ToString());
                            }

                            isUpdate = true;
                            // await DisplayAlert("Добавление", "Товар: " + each.Name + " убрали из корзины", "OK");
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
                    stackLayoutAddAndMin.Children.Add(iconViewPlus);

                    stackLayoutprice.Children.Add(LabelPrice);
                    stackLayoutprice.Children.Add(labelCount);
                    stackLayoutprice.Children.Add(stackLayoutAddAndMin);

                    list.Children.Add(imageGoods);
                    list.Children.Add(LabelName);
                    list.Children.Add(stackLayoutprice);
                    frame.Content = list;
                    stackLayout.Children.Add(frame);
                }

                i++;

                rootStackLayout.Children.Add(text);
                ScrollView scrollView = new ScrollView();
                scrollView.VerticalScrollBarVisibility = ScrollBarVisibility.Never;
                scrollView.Content = stackLayout;
                rootStackLayout.Children.Add(scrollView);
                pages.Add(scrollView);
                return rootStackLayout;
            });
            CarouselViewShop.ItemTemplate = itemTemplate;
        }

        public void updateGoods(int i)
        {
            StackLayout stackLayout = new StackLayout();
            foreach (var each in Keys[i].Goodses)
            {
                Frame frame = new Frame();
                frame.HorizontalOptions = LayoutOptions.FillAndExpand;
                frame.VerticalOptions = LayoutOptions.Start;
                frame.BackgroundColor = Color.White;
                frame.Margin = new Thickness(0, 0, 0, 10);
                frame.Padding = new Thickness(15, 15, 15, 15);
                frame.CornerRadius = 10;

                StackLayout list = new StackLayout();
                list.Orientation = StackOrientation.Horizontal;

                Image imageGoods = new Image();
                imageGoods.HeightRequest = 50;
                imageGoods.WidthRequest = 50;
                imageGoods.Source = ImageSource.FromFile("ic_no_photo");
                if (each.HasImage)
                {
                    imageGoods.Source =
                        ImageSource.FromUri(new Uri("https://api.sm-center.ru/test_erc_udm/public/GoodsImage/" +
                                                    each.ID.ToString()));
                }


                Label LabelName = new Label();
                LabelName.TextColor = Color.Black;
                LabelName.FontSize = 15;
                LabelName.Text = each.Name;
                LabelName.Margin = new Thickness(10, 0, 0, 0);
                LabelName.HorizontalOptions = LayoutOptions.FillAndExpand;
                LabelName.VerticalTextAlignment = TextAlignment.Start;

                StackLayout stackLayoutprice = new StackLayout();

                Label LabelPrice = new Label();
                LabelPrice.TextColor = Color.Black;
                LabelPrice.FontSize = 15;
                LabelPrice.Text = each.Price.ToString() + " \u20BD";
                LabelPrice.HorizontalOptions = LayoutOptions.EndAndExpand;
                LabelPrice.VerticalTextAlignment = TextAlignment.Start;

                Label labelCount = new Label();
                labelCount.TextColor = Color.Gray;
                labelCount.FontSize = 15;
                labelCount.HorizontalOptions = LayoutOptions.Center;
                labelCount.Text = each.ColBusket.ToString() + " " + each.Units;
                if (Goodset.ContainsKey(each.ID.ToString()))
                {
                    labelCount.Text = Goodset[each.ID.ToString()].ColBusket + " " + each.Units;
                }

                StackLayout stackLayoutAddAndMin = new StackLayout();
                stackLayoutAddAndMin.Orientation = StackOrientation.Horizontal;

                IconView iconViewPlus = new IconView();
                iconViewPlus.Source = "ic_plus";
                iconViewPlus.Foreground = Color.FromHex(Settings.MobileSettings.color);
                iconViewPlus.HeightRequest = 15;
                iconViewPlus.WidthRequest = 15;


                var addItem = new TapGestureRecognizer();
                addItem.Tapped += async (s, e) =>
                {
                    if (each.Rest != 0)
                    {
                        each.Rest--;
                        each.ColBusket++;
                        labelCount.Text = each.ColBusket.ToString() + " " + each.Units;
                        each.priceBusket = (decimal) (each.Price * each.ColBusket);
                        each.weightBusket = (decimal) (each.Weight * each.ColBusket);

                        Price += each.priceBusket;
                        Weight += each.weightBusket;

                        try
                        {
                            Goodset.Add(each.ID.ToString(), each);
                        }
                        catch (Exception ex)
                        {
                            Goodset[each.ID.ToString()] = each;
                        }

                        isUpdate = true;
                        // await DisplayAlert("Добавление", "Товар: " + each.Name + " добавили в корзину", "OK");
                    }
                    else
                    {
                        await DisplayAlert("Ошибка", "Товар: " + each.Name + " закончился", "OK");
                    }

                    SetPriceAndWeight();
                };
                iconViewPlus.GestureRecognizers.Add(addItem);

                IconView iconViewMinus = new IconView();
                iconViewMinus.Source = "ic_minus";
                iconViewMinus.Foreground = Color.FromHex(Settings.MobileSettings.color);
                iconViewMinus.HeightRequest = 15;
                iconViewMinus.WidthRequest = 15;


                var dellItem = new TapGestureRecognizer();
                dellItem.Tapped += async (s, e) =>
                {
                    if (each.ColBusket != 0)
                    {
                        each.Rest++;
                        each.ColBusket--;
                        labelCount.Text = each.ColBusket.ToString() + " " + each.Units;
                        each.priceBusket = (decimal) (each.Price * each.ColBusket);
                        each.weightBusket = (decimal) (each.Weight * each.ColBusket);

                        Price += each.priceBusket;
                        Weight += each.weightBusket;

                        if (each.ColBusket > 0)
                        {
                            Goodset[each.ID.ToString()] = each;
                        }
                        else
                        {
                            Goodset.Remove(each.ID.ToString());
                        }

                        isUpdate = true;
                        // await DisplayAlert("Добавление", "Товар: " + each.Name + " убрали из корзины", "OK");
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
                stackLayoutAddAndMin.Children.Add(iconViewPlus);

                stackLayoutprice.Children.Add(LabelPrice);
                stackLayoutprice.Children.Add(labelCount);
                stackLayoutprice.Children.Add(stackLayoutAddAndMin);

                list.Children.Add(imageGoods);
                list.Children.Add(LabelName);
                list.Children.Add(stackLayoutprice);
                frame.Content = list;
                stackLayout.Children.Add(frame);
            }

            try
            {
                pages[i].Content = stackLayout;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void SetPriceAndWeight()
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

        void SetText()
        {
            UkName.Text = Settings.MobileSettings.main_name;
            LabelPhone.Text = "+" + Settings.Person.Phone;
        }

        public class KeysModel
        {
            public string Name { get; set; }
            public List<Goods> Goodses { get; set; }
        }

        private void OnPositionChanged(object sender, PositionChangedEventArgs e)
        {
            position = CarouselViewShop.Position;
            if (isUpdate)
            {
                updateGoods(CarouselViewShop.Position);
                isUpdate = false;
            }
        }
    }
}