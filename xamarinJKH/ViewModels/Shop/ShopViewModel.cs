using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using xamarinJKH.Server.RequestModel;

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
                    foreach (var good in AllGoods)
                    {
                        if (good.Categories.Contains(value))
                        {
                            Device.BeginInvokeOnMainThread(() => Goods.Add(good));
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

        int totalPrice;
        public int TotalPrice
        {
            get => totalPrice;
            set
            {
                totalPrice = value;
                OnPropertyChanged(nameof(TotalPrice));
            }
        }

        int totalWeight;
        public int TotalWeight
        {
            get => totalWeight;
            set
            {
                totalWeight = value;
                OnPropertyChanged(nameof(TotalWeight));
            }
        }
        public ShopViewModel(AdditionalService service)
        {
            ShopName = service.ShopName;
            Goods = new ObservableCollection<Goods>();
            Categories = new ObservableCollection<string>();
            var categories = new List<string>();
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
                });
            });
            Increase = new Command<Goods>(item =>
            {
                item.ColBusket++;
            });

            Decrease = new Command<Goods>(item =>
            {
                if (item.ColBusket > 0)
                {
                    item.ColBusket--;
                }
            });

            Sort = new Command(() =>
            {
                
            });
        }
    }
}
