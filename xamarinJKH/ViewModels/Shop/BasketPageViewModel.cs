using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using xamarinJKH.Server.RequestModel;
using System.Text;

namespace xamarinJKH.ViewModels.Shop
{
    public class BasketPageViewModel:BaseViewModel
    {
        public ObservableCollection<Goods> BasketItems { get; set; }
        public BasketPageViewModel(ObservableCollection<Goods> Items)
        {
            BasketItems = Items;
        }
    }
}
