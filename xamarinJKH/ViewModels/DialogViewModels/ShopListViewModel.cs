using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

using Xamarin.Forms;
using xamarinJKH.Server.RequestModel;

namespace xamarinJKH.ViewModels.DialogViewModels
{
    public class ShopListViewModel:BaseViewModel
    {
        public ObservableCollection<Goods> Items { get; set; }
        public List<Goods> SelectedItems { get; set; }
        public Command LoadItems { get; set; }
        public Command AddItems { get; set; }
        public ShopListViewModel()
        {
            Items = new ObservableCollection<Goods>();
            SelectedItems = new List<Goods>();
            LoadItems = new Command(async () =>
            {
                var items = await Server.GetShopGoods();
                if (items.Error == null)
                {
                    foreach (var item in items.Data)
                    {
                        Items.Add(item);
                    }
                }
            });

            AddItems = new Command(async () =>
            {
                MessagingCenter.Send<Object, List<Goods>>(this, "AddItems", SelectedItems);
            });
        }
    }
}
