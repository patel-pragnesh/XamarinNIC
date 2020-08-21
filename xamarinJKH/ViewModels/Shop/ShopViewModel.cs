using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using xamarinJKH.Server.RequestModel;
using xamarinJKH.Shop;

namespace xamarinJKH.ViewModels.Shop
{
    public class ShopViewModel:BaseViewModel
    {
        public ObservableCollection<Goods> Goods { get; set; }
        public ObservableCollection<string> Categories { get; set; }
        public List<Goods> AllGoods { get; set; }
        public Command LoadGoods { get; set; }
        string shopName;
        public string ShopName
        {
            get => shopName;
            set
            {
                shopName = value;
                OnPropertyChanged(nameof(ShopName));
            }
        }
        string selected;
        public string SelectedCategory
        {
            get => selected;
            set
            {
                if (selected != value)
                {
                    Goods.Clear();
                    if (!Asending)
                    {
                        foreach (var good in AllGoods.OrderBy(_ => _.Price).ToList())
                        {
                            if (good.Categories.Contains(value))
                            {
                                Device.BeginInvokeOnMainThread(() => Goods.Add(good));
                            }
                        }
                    }
                    else
                    {
                        foreach (var good in AllGoods.OrderByDescending(_ => _.Price).ToList())
                        {
                            if (good.Categories.Contains(value))
                            {
                                Device.BeginInvokeOnMainThread(() => Goods.Add(good));
                            }
                        }
                    }
                }
                selected = value;
                OnPropertyChanged(nameof(SelectedCategory));
            }
        }
        public Command Increase { get; set; }
        public Command Decrease { get; set; }
        public Command Sort { get; set; }
        bool asending;
        public bool Asending
        {
            get => asending;
            set
            {
                asending = value;
                OnPropertyChanged(nameof(Asending));
            }
        }

        decimal? totalPrice;
        public decimal? TotalPrice
        {
            get => totalPrice;
            set
            {
                totalPrice = value;
                OnPropertyChanged(nameof(TotalPrice));
            }
        }

        decimal? totalWeight;
        public decimal? TotalWeight
        {
            get => totalWeight;
            set
            {
                totalWeight = value;
                OnPropertyChanged(nameof(TotalWeight));
            }
        }

        readonly INavigation Navigation;
        public Command GoToBasket { get; set; }
        public Command GoToPay { get; set; }
        public AdditionalService Service { get; set; }
        public ShopViewModel(AdditionalService service, INavigation navigation)
        {
            this.Navigation = navigation;
            Service = service;
            ShopName = service.ShopName;
            Goods = new ObservableCollection<Goods>();
            Categories = new ObservableCollection<string>();
            var categories = new List<string>();
            TotalPrice = 0;
            TotalWeight = 0;
            LoadGoods = new Command(() =>
            {
                Task.Run(async () =>
                {
                    var goods = await Server.GetShopGoods(service.ShopID);
                    if (goods.Error == null)
                    {
                        AllGoods = new List<Goods>();
                        AllGoods.AddRange(goods.Data);

                        foreach (var good in goods.Data)
                        {
                            foreach (var category in good.Categories)
                            {
                                if (!categories.Contains(category.Trim()))
                                {
                                    categories.Add(category);
                                }
                            }
                        }

                        foreach (var good in AllGoods)
                        {
                            if (good.Categories.Contains(categories[0]))
                            {
                                Device.BeginInvokeOnMainThread(() => Goods.Add(good));
                            }
                        }
                    }
                }).ContinueWith((result) =>
                {
                    Categories.Clear();
                    foreach (var category in categories)
                    {
                        Device.BeginInvokeOnMainThread(() => Categories.Add(category));
                    }
                    Device.BeginInvokeOnMainThread(() => SelectedCategory = Categories[0]);
                });
            });
            Increase = new Command<Goods>(item =>
            {
                item.ColBusket++;
                TotalPrice = Goods.Select(x => x.Price * x.ColBusket).Sum();
                TotalWeight = Goods.Sum(x => x.Weight * x.ColBusket);
            });

            Decrease = new Command<Goods>(item =>
            {
                if (item.ColBusket > 0)
                {
                    item.ColBusket--;
                }
                TotalPrice = Goods.Select(x => x.Price * x.ColBusket).Sum();
                TotalWeight = Goods.Sum(x => x.Weight * x.ColBusket);
            });

            Sort = new Command(() =>
            {
                Goods.Clear();
                if (Asending)
                {
                    foreach (var good in AllGoods.OrderBy(_ => _.Price).ToList())
                    {
                        if (good.Categories.Contains(SelectedCategory))
                        {
                            Device.BeginInvokeOnMainThread(() => Goods.Add(good));
                        }
                    }
                }
                else
                {
                    foreach (var good in AllGoods.OrderByDescending(_ => _.Price).ToList())
                    {
                        if (good.Categories.Contains(SelectedCategory))
                        {
                            Device.BeginInvokeOnMainThread(() => Goods.Add(good));
                        }
                    }
                }
                Asending = !Asending;
            });

            GoToBasket = new Command(async () =>
            {
                if (this.TotalPrice > 0)
                    await Navigation.PushAsync(new BasketPageNew(this));
                else
                    this.ShowError(AppResources.ErrorBasketEmpty);
            });

            GoToPay = new Command(async () =>
            {
                var goodset = new Dictionary<string, Goods>();
                foreach (var item in AllGoods)
                {
                    if (item.ColBusket > 0)
                    {
                        goodset.Add(item.ID.ToString(), item);
                    }
                }

                if (goodset.Count > 0)
                {
                    await Navigation.PushAsync(new PayShopPage(goodset, Service));
                }
                else
                {
                    this.ShowError(AppResources.ErrorBasketEmpty);
                }
            });
        }
    }
}
