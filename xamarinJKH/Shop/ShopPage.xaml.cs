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
using Xamarin.Forms.Internals;
using Xamarin.Forms.PancakeView;
using Xamarin.Forms.Xaml;
using xamarinJKH.Server;
using xamarinJKH.Server.RequestModel;
using xamarinJKH.Tech;
using xamarinJKH.Utils;

namespace xamarinJKH.Shop
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ShopPage : ContentPage
    {
        private RestClientMP server = new RestClientMP();

        //private List<ScrollView> pages = new List<ScrollView>();
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
                    // BackgroundColor = Color.White;
                    // ImageTop.Margin = new Thickness(0, 0, 0, 0);
                    // StackLayout.Margin = new Thickness(0, 20, 0, 0);
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
            var techSend = new TapGestureRecognizer();
            techSend.Tapped += async (s, e) => {     await Navigation.PushAsync(new TechSendPage()); };
            LabelTech.GestureRecognizers.Add(techSend);
            
            NavigationPage.SetHasNavigationBar(this, false);
            var backClick = new TapGestureRecognizer();
            backClick.Tapped += async (s, e) =>
            {
                MessagingCenter.Send<Object>(this, "LoadGoods");
                _ = await Navigation.PopAsync();
            };
            BackStackLayout.GestureRecognizers.Add(backClick);
            hex = Color.FromHex(Settings.MobileSettings.color);
            //var basketPage = new TapGestureRecognizer();
            //basketPage.Tapped += async (s, e) =>
            //{
            //    if (Goodset.Count > 0)
            //    {
            //        await Navigation.PushAsync(new BasketPage(Goodset, this, _Additional));
            //    }
            //    else
            //    {
            //        await DisplayAlert(AppResources.ErrorTitle, "Корзина пуста", "OK");
            //    }
            //};
            //StackLayoutBasket.GestureRecognizers.Add(basketPage);

            SetText();
            GetGoods();
            MessagingCenter.Subscribe<Object, Dictionary<string, Goods>>(this, "UpdateGoods", (sender, goods) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    this.Goodset = goods;
                    SetPriceAndWeight();
                    updateGoods(position);
                });
                
            });
        }

        protected override bool OnBackButtonPressed()
        {
            MessagingCenter.Send<Object>(this, "LoadGoods");
            return base.OnBackButtonPressed();
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
                await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorInfoProducts, "OK");
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

            bool isSecondElement = true;
            //категории в меню со скроллом
            foreach (var c in CategoriesGoods)
            {
                var b = new Button();
                if (c.Key == CategoriesGoods.Keys.First())
                {
                    b = GetLabelForCategory(c.Key, true);
                    //prevButton = b;
                }
                else
                {
                    b = GetLabelForCategory(c.Key, false); 
                    if(isSecondElement)
                    {
                        isSecondElement = false;
                        //nextButton = b;
                    }
                }

                GoodsCategories.Children.Add(b);
                
            }

            Label foo = new Label() { Text = "masssssssssssssssssssssssssssssssssssssssssssssiveStringgggggggggggggggggggg",
                TextColor = Color.Transparent,
                FontSize = 14 };
            GoodsCategories.Children.Add(foo);


            if (CategoriesGoods.Keys.FirstOrDefault() != null)
            {
                currentDisplayCategoryKey = CategoriesGoods.Keys.FirstOrDefault();
                setList(currentDisplayCategoryKey); 
            }

            setKeys();
        }



        Button GetLabelForCategory(string c, bool needUnderLine)
        {
            var b = new Button() { BackgroundColor = Color.Transparent };
            b.Text = c;
            b.FontSize = 20;
            b.BorderWidth = 0;
            b.SetAppThemeColor(Button.TextColorProperty, Color.Black, Color.White);
            //StackLayout labelStack = new StackLayout() { Margin = new Thickness(10, 10, 10, 10), BackgroundColor = Color.Transparent };

            //var cat = new Label();
            //cat.FontSize = 20;
            //cat.TextColor = Color.White;
            //cat.Text = c;
            if (needUnderLine)
            {
                //cat.TextDecorations = TextDecorations.Underline;
                //cat.TextColor = Color.FromHex(Settings.MobileSettings.color);
                b.TextColor = colorFromMobileSettings;// Color.FromHex(Settings.MobileSettings.color);
                prevCategoryTapped = c;
            }
            else
                b.SetAppThemeColor(Button.TextColorProperty, Color.Black, Color.White);

            b.Clicked += B_Clicked;
            //TapGestureRecognizer tap = new TapGestureRecognizer();
            //tap.Tapped += Tap_Tapped;
            //labelStack.GestureRecognizers.Add(tap);
            //labelStack.Children.Add(cat);
            return b /*labelStack*/;
        }


        private void B_Clicked(object sender, EventArgs e)
        {
            var b = (Button)sender;            
            
            var catName = b.Text;

            if (prevCategoryTapped == catName)
                return;
            if (!string.IsNullOrWhiteSpace(prevCategoryTapped))
            {
                var sp = GoodsCategories.Children.FirstOrDefault(_ => _.GetType() == typeof(Button) && ((Button)_).Text == prevCategoryTapped);
                if (sp != null)
                {
                    var bPrev = (Button)sp;
                    bPrev.SetAppThemeColor(Button.TextColorProperty, Color.Black, Color.White);
                }
            }
            b.TextColor = colorFromMobileSettings;
            //замена категории
            prevCategoryTapped = catName;                       

            currentDisplayCategoryKey = catName;

            setList(catName);
        }

        
        static string prevCategoryTapped;

        void setKeys()
        {
            try
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
            catch(Exception e)
            {

            }            
        }

        const string GOODS_IMAGE_URI = RestClientMP.SERVER_ADDR +"/public/GoodsImage/";
        const string SortByPrice = "Сортировать по цене";
        
        Color colorFromMobileSettings = Color.FromHex(Settings.MobileSettings.color);


        async void setList(string i)
        {

            Label text = new Label();
            text.FontSize = 16;
            text.TextColor = Color.Black;
            text.Text = AppResources.Catalog;
            text.FontAttributes = FontAttributes.Bold;

            #region price
            StackLayout SortByPriceLayout = new StackLayout() { Orientation=StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.EndAndExpand };

            IconView iconViewArrow = new IconView();
            iconViewArrow.Source = goodsListOrderedByPriceGrowing? "ic_arrow_down": "ic_arrow_up";
            iconViewArrow.Foreground = colorFromMobileSettings;
            iconViewArrow.HeightRequest = 15;
            iconViewArrow.WidthRequest = 15;
            iconViewArrow.VerticalOptions = LayoutOptions.Center;
            SortByPriceLayout.Children.Add(iconViewArrow);


            Label sortByPrice = new Label();
            sortByPrice.Margin = new Thickness(-5, 0, 0, 0);
            sortByPrice.Text = SortByPrice;
            sortByPrice.TextColor = colorFromMobileSettings;
            sortByPrice.FontSize = 12;
            sortByPrice.VerticalOptions = LayoutOptions.Center;
            SortByPriceLayout.Children.Add(sortByPrice);

            TapGestureRecognizer tgr = new TapGestureRecognizer();
            tgr.Tapped += Tgr_Tapped;
            SortByPriceLayout.GestureRecognizers.Add(tgr);
            #endregion

            StackLayout head = new StackLayout() { Orientation = StackOrientation.Horizontal, Margin = new Thickness(10, 10, 10, 10), HorizontalOptions = LayoutOptions.FillAndExpand};
            head.Children.Add(text);
            head.Children.Add(SortByPriceLayout);

            StackLayout rootStackLayout = new StackLayout();
            rootStackLayout.VerticalOptions = LayoutOptions.FillAndExpand;
            var goods = goodsListOrderedByPriceGrowing?  CategoriesGoods[i].OrderBy(_ => _.Price).ToList() : CategoriesGoods[i].OrderByDescending(_ => _.Price).ToList();            

            var stackLayout = GetOrderedGoodsList(goods);

            rootStackLayout.Children.Add(head);
            ScrollView scrollView = new ScrollView();
            scrollView.VerticalScrollBarVisibility = ScrollBarVisibility.Never;
            scrollView.Content = stackLayout;
            scrollView.VerticalOptions = LayoutOptions.FillAndExpand;
            rootStackLayout.Children.Add(scrollView);

            GoodsLayot.Content = rootStackLayout;
        }

        bool goodsListOrderedByPriceGrowing = true;

        StackLayout GetOrderedGoodsList(List<Goods> goods)
        {
            StackLayout stackLayout = new StackLayout();
            stackLayout.BackgroundColor = Color.White;

            foreach (var each in goods)
            {
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
                    imageGoods.Source = new UriImageSource
                    {
                         CachingEnabled = true,
                         CacheValidity = new TimeSpan(2,0,0,0),
                         Uri = new Uri(GOODS_IMAGE_URI + each.ID.ToString())
                    };
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
                imgFrame.HasShadow = false;

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

                StackLayout stackLayoutAddAndMin = new StackLayout();// { Margin = new Thickness(0, 5, 0, 0) };
                stackLayoutAddAndMin.Orientation = StackOrientation.Horizontal;

                var icViewPlusMinusSize = 22;

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
                        labelCount.TextColor = Color.Black;

                        each.priceBusket = (decimal)(each.Price * each.ColBusket);
                        each.weightBusket = (decimal)(each.Weight * each.ColBusket);

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
                        await DisplayAlert(AppResources.ErrorTitle, $"{AppResources.BasketProduct} {AppResources.BasketProductNoLeft}", "OK");
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
                        await DisplayAlert(AppResources.ErrorTitle, $"{AppResources.BasketProduct} {AppResources.BasketProductNotInBasket}", "OK");
                    }

                    SetPriceAndWeight();
                };
                iconViewMinus.GestureRecognizers.Add(dellItem);

                stackLayoutAddAndMin.HorizontalOptions = LayoutOptions.Center;
                stackLayoutAddAndMin.Children.Add(iconViewMinus);
                stackLayoutAddAndMin.Children.Add(labelCount);
                stackLayoutAddAndMin.Children.Add(iconViewPlus);

                StackLayout l1 = new StackLayout() { Orientation = StackOrientation.Vertical };

                l1.Children.Add(stackLayoutAddAndMin);

                var b1 = new BoxView() { Color = colorFromMobileSettings, HeightRequest = 1, Margin = new Thickness(0, -5, 0, 0) };
                l1.Children.Add(b1);

                stackLayoutprice.Children.Add(LabelPrice);
                stackLayoutprice.Children.Add(l1);

                list.Children.Add(imgFrame);
                list.Children.Add(LabelName);
                list.Children.Add(stackLayoutprice);
                
                stackLayout.Children.Add(list);

                //разделитель
                BoxView b = new BoxView() { Color = Color.FromHex("#e7e7e7"), HeightRequest = 1, Margin = new Thickness(10, 0) };
                stackLayout.Children.Add(b);
            }
            return stackLayout;
        }

        string currentDisplayCategoryKey = "";

        private void Tgr_Tapped(object sender, EventArgs e)
        {            
            goodsListOrderedByPriceGrowing = !goodsListOrderedByPriceGrowing;
            
            setList(currentDisplayCategoryKey);
        }

        //async void SetCarusel()
        //{
        //    int i = 0;
        //    var itemTemplate = new DataTemplate(() =>
        //    {
        //        Label text = new Label();
        //        text.FontSize = 20;
        //        text.TextColor = Color.Black;
        //        text.SetBinding(Label.TextProperty, "Name");

        //        StackLayout rootStackLayout = new StackLayout();

        //        StackLayout stackLayout = new StackLayout();
        //        //Frame stackLayoutFrame = new Frame();
        //        //stackLayoutFrame.CornerRadius = 10;
        //        PancakeView pancake = new PancakeView();
        //        pancake.CornerRadius = new CornerRadius(30, 30, 0, 0);
        //        pancake.BackgroundColor = Color.White;

        //        stackLayout.BackgroundColor = Color.White;

        //        foreach (var each in Keys[i].Goodses)
        //        {
        //            Frame frame = new Frame();
        //            frame.HorizontalOptions = LayoutOptions.FillAndExpand;
        //            frame.VerticalOptions = LayoutOptions.Start;
        //            frame.BackgroundColor = Color.White;
        //            frame.Margin = new Thickness(0, 0, 0, 10);
        //            frame.Padding = new Thickness(15, 15, 15, 15);
        //            //frame.CornerRadius = 10;

        //            StackLayout list = new StackLayout();
        //            list.Orientation = StackOrientation.Horizontal;

        //            Image imageGoods = new Image();
        //            imageGoods.HeightRequest = 50;
        //            imageGoods.WidthRequest = 50;
        //            //imageGoods.Source = ImageSource.FromFile("ic_no_photo");
        //            if (each.HasImage)
        //            {
        //                imageGoods.Source =
        //                    ImageSource.FromUri(new Uri(GOODS_IMAGE_URI +
        //                                                each.ID.ToString()));
        //            }
        //            else
        //            {
        //                imageGoods.Source = ImageSource.FromFile("ic_no_photo");
        //            }


        //            Label LabelName = new Label();
        //            LabelName.TextColor = Color.Black;
        //            LabelName.FontSize = 15;
        //            LabelName.Text = each.Name;
        //            LabelName.Margin = new Thickness(10, 0, 0, 0);
        //            LabelName.HorizontalOptions = LayoutOptions.FillAndExpand;
        //            LabelName.VerticalTextAlignment = TextAlignment.Start;

        //            StackLayout stackLayoutprice = new StackLayout();

        //            Label LabelPrice = new Label();
        //            LabelPrice.TextColor = Color.Black;
        //            LabelPrice.FontSize = 15;
        //            LabelPrice.Text = each.Price.ToString() + " \u20BD";
        //            LabelPrice.HorizontalOptions = LayoutOptions.EndAndExpand;
        //            LabelPrice.VerticalTextAlignment = TextAlignment.Start;

        //            Label labelCount = new Label();
        //            labelCount.TextColor = Color.Gray;
        //            labelCount.FontSize = 15;
        //            labelCount.HorizontalOptions = LayoutOptions.Center;
        //            labelCount.Text = each.ColBusket.ToString() + " " + each.Units;

        //            StackLayout stackLayoutAddAndMin = new StackLayout();
        //            stackLayoutAddAndMin.Orientation = StackOrientation.Horizontal;

        //            IconView iconViewPlus = new IconView();
        //            iconViewPlus.Source = "ic_plus";
        //            iconViewPlus.Foreground = Color.FromHex(Settings.MobileSettings.color);
        //            iconViewPlus.HeightRequest = 15;
        //            iconViewPlus.WidthRequest = 15;


        //            var addItem = new TapGestureRecognizer();
        //            addItem.Tapped += async (s, e) =>
        //            {
        //                if (each.Rest != 0)
        //                {
        //                    each.Rest--;
        //                    each.ColBusket++;
        //                    labelCount.Text = each.ColBusket.ToString() + " " + each.Units;
        //                    each.priceBusket = (decimal) (each.Price * each.ColBusket);
        //                    each.weightBusket = (decimal) (each.Weight * each.ColBusket);

        //                    Price += each.priceBusket;
        //                    Weight += each.weightBusket;

        //                    try
        //                    {
        //                        Goodset.Add(each.ID.ToString(), each);
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        Goodset[each.ID.ToString()] = each;
        //                    }

        //                    isUpdate = true;
        //                    // await DisplayAlert("Добавление", "Товар: " + each.Name + " добавили в корзину", "OK");
        //                }
        //                else
        //                {
        //                    await DisplayAlert(AppResources.ErrorTitle, "Товар: " + each.Name + " закончился", "OK");
        //                }

        //                SetPriceAndWeight();
        //            };
        //            iconViewPlus.GestureRecognizers.Add(addItem);

        //            IconView iconViewMinus = new IconView();
        //            iconViewMinus.Source = "ic_minus";
        //            iconViewMinus.Foreground = Color.FromHex(Settings.MobileSettings.color);
        //            iconViewMinus.HeightRequest = 15;
        //            iconViewMinus.WidthRequest = 15;


        //            var dellItem = new TapGestureRecognizer();
        //            dellItem.Tapped += async (s, e) =>
        //            {
        //                if (each.ColBusket != 0)
        //                {
        //                    each.Rest++;
        //                    each.ColBusket--;
        //                    labelCount.Text = each.ColBusket.ToString() + " " + each.Units;
        //                    each.priceBusket = (decimal) (each.Price * each.ColBusket);
        //                    each.weightBusket = (decimal) (each.Weight * each.ColBusket);

        //                    Price += each.priceBusket;
        //                    Weight += each.weightBusket;

        //                    if (each.ColBusket > 0)
        //                    {
        //                        Goodset[each.ID.ToString()] = each;
        //                    }
        //                    else
        //                    {
        //                        Goodset.Remove(each.ID.ToString());
        //                    }

        //                    isUpdate = true;
        //                    // await DisplayAlert("Добавление", "Товар: " + each.Name + " убрали из корзины", "OK");
        //                }
        //                else
        //                {
        //                    await DisplayAlert(AppResources.ErrorTitle, "Товара: " + each.Name + " в корзине больше нет", "OK");
        //                }

        //                SetPriceAndWeight();
        //            };
        //            iconViewMinus.GestureRecognizers.Add(dellItem);

        //            stackLayoutAddAndMin.HorizontalOptions = LayoutOptions.Center;
        //            stackLayoutAddAndMin.Children.Add(iconViewMinus);
        //            stackLayoutAddAndMin.Children.Add(iconViewPlus);

        //            stackLayoutprice.Children.Add(LabelPrice);
        //            stackLayoutprice.Children.Add(labelCount);
        //            stackLayoutprice.Children.Add(stackLayoutAddAndMin);

        //            list.Children.Add(imageGoods);
        //            list.Children.Add(LabelName);
        //            list.Children.Add(stackLayoutprice);
        //            frame.Content = list;
        //            stackLayout.Children.Add(frame);
        //        }

        //        i++;

        //        //stackLayoutFrame.Content = stackLayout;

        //        rootStackLayout.Children.Add(text);
        //        ScrollView scrollView = new ScrollView();
        //        scrollView.VerticalScrollBarVisibility = ScrollBarVisibility.Never;
        //        scrollView.Content = stackLayout;
        //        //scrollView.Content = stackLayoutFrame;
        //        rootStackLayout.Children.Add(scrollView);
        //        pages.Add(scrollView);
        //        pancake.Content = rootStackLayout;
        //        return pancake;
        //    });
        //    //CarouselViewShop.ItemTemplate = itemTemplate;
        //}

        public void updateGoods(int i)
        {
            //Frame content = new Frame() ;
            //content.BackgroundColor = Color.White;
            //content.CornerRadius = 30;

            StackLayout stackLayout = new StackLayout();
            GoodsLayot.Content = null;
            SetCategories();
            return;

            foreach (var each in Keys[i].Goodses)
            {
                Frame frame = new Frame();
                frame.HorizontalOptions = LayoutOptions.FillAndExpand;
                frame.VerticalOptions = LayoutOptions.Start;
                frame.BackgroundColor = Color.White;
                frame.Margin = new Thickness(0, 0, 0, 10);
                frame.Padding = new Thickness(15, 15, 15, 15);
                //frame.CornerRadius = 10;

                StackLayout list = new StackLayout();
                list.Orientation = StackOrientation.Horizontal;

                Image imageGoods = new Image();
                imageGoods.HeightRequest = 50;
                imageGoods.WidthRequest = 50;
                imageGoods.Source = ImageSource.FromFile("ic_no_photo");
                if (each.HasImage)
                {
                    imageGoods.Source =
                        ImageSource.FromUri(new Uri(GOODS_IMAGE_URI +
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
                        await DisplayAlert(AppResources.ErrorTitle, "Товар: " + each.Name + " закончился", "OK");
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
                        await DisplayAlert(AppResources.ErrorTitle, "Товара: " + each.Name + " в корзине больше нет", "OK");
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
                //pages[i].Content = stackLayout;
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

            FormattedString formattedStringPriceBasket = new FormattedString();
            formattedStringPriceBasket.Spans.Add(new Span
            {
                Text = sumBasket.ToString(),
                FontSize = 18,
                FontAttributes=FontAttributes.Bold,
                TextColor=hex
            });
            formattedStringPriceBasket.Spans.Add(new Span
            {
                Text = $" {AppResources.Currency}",
                TextColor = Color.FromHex("#8a8a8a")  ,
                FontSize=12
            }) ;
            LabelPriceBuscket.FormattedText = formattedStringPriceBasket;

            FormattedString formattedStringWeightBasket = new FormattedString();
            formattedStringWeightBasket.Spans.Add(new Span
            {
                Text = sumWeightBasket.ToString(),
                FontSize = 18,
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.Black
            });
            formattedStringWeightBasket.Spans.Add(new Span
            {
                Text = " г",
                TextColor = Color.FromHex("#8a8a8a"),
                FontSize = 12
            });
            LabelWeightBuscket.FormattedText = formattedStringWeightBasket;

        }

        void SetText()
        {
            //ShopName.Text = Settings.MobileSettings.main_name;
            //LabelPhone.Text = "+" + Settings.Person.Phone;
            Color hexColor = (Color) Application.Current.Resources["MainColor"];
            IconViewTech.SetAppThemeColor(IconView.ForegroundProperty, hexColor, Color.White);
            LabelTech.SetAppThemeColor(Label.TextColorProperty, hexColor, Color.White);
            PancakeViewKind.SetAppThemeColor(PancakeView.BorderColorProperty, hexColor, Color.Transparent);
            GoodsLayot.SetAppThemeColor(PancakeView.BorderColorProperty, hexColor, Color.White);
            PancakeBot.SetAppThemeColor(PancakeView.BorderColorProperty, hexColor, Color.Transparent);
        }

        public class KeysModel
        {
            public string Name { get; set; }
            public List<Goods> Goodses { get; set; }
        }

        private void OnPositionChanged(object sender, PositionChangedEventArgs e)
        {
            //position = CarouselViewShop.Position;
            //if (isUpdate)
            //{
            //    updateGoods(CarouselViewShop.Position);
            //    isUpdate = false;
            //}
        }

        private async void BtnCheckOut_Clicked(object sender, EventArgs e)
        {
            if (Goodset.Count > 0)
            {
                await Navigation.PushAsync(new BasketPage(Goodset, this, _Additional));
            }
            else
            {
                await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorBasketEmpty, "OK");
            }
        }

        

        static double scrollXPosition=0;
        private void ScrollView_Scrolled(object sender, ScrolledEventArgs e)
        {
            if(scrollXPosition+50<e.ScrollX)
            {
                var current =  GoodsCategories.Children.FirstOrDefault(_ => _.GetType() == typeof(Button) && ((Button)_).Text == currentDisplayCategoryKey);
                var next = GoodsCategories.Children.FirstOrDefault(_ => _.X > current.X && _.GetType() == typeof(Button));
                if(next!=null)
                {
                    ((ScrollView)sender).ScrollToAsync(next, ScrollToPosition.Center, true);
                    B_Clicked((Button)next, new EventArgs());
                    scrollXPosition = e.ScrollX;
                }        
            }
            if(scrollXPosition !=0 && scrollXPosition - 50 > e.ScrollX)
            {
                var current = GoodsCategories.Children.FirstOrDefault(_ => _.GetType() == typeof(Button) && ((Button)_).Text == currentDisplayCategoryKey);
                var next = GoodsCategories.Children.FirstOrDefault(_ => _.X < current.X && _.GetType() == typeof(Button));
                if (next != null)
                {
                    ((ScrollView)sender).ScrollToAsync(next, ScrollToPosition.Center, true);
                    B_Clicked((Button)next, new EventArgs());
                    scrollXPosition = e.ScrollX;
                }
            }
            var lastBtn = GoodsCategories.Children.Last(_ => _.GetType() == typeof(Button));
            if (e.ScrollX > lastBtn.X)
                ((ScrollView)sender).ScrollToAsync(lastBtn, ScrollToPosition.Center, false) ;

        }
    }
}