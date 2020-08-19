using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

using Xamarin.Forms;
using xamarinJKH.Server.RequestModel;

using System.Linq;
using AiForms.Dialogs.Abstractions;

namespace xamarinJKH.ViewModels.DialogViewModels
{
    public class ShopListViewModel:BaseViewModel
    {
        public ObservableCollection<Goods> Items { get; set; }
        public List<Goods> SelectedItems { get; set; }
        public Command LoadItems { get; set; }
        public Command AddItems { get; set; }
        public Command Increase { get; set; }
        public Command Decrease { get; set; }
        readonly IDialogNotifier Dialog;
        public ShopListViewModel(IDialogNotifier dialog)
        {
            Items = new ObservableCollection<Goods>();
            Dialog = dialog;
            SelectedItems = new List<Goods>();
            LoadItems = new Command(async () =>
            {
                
                var items = await Server.GetShopGoods(1);
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
                SelectedItems.AddRange(Items.Where(x => x.ColBusket > 0));
                Dialog.Cancel();
                MessagingCenter.Send<Object, List<Goods>>(this, "AddItems", SelectedItems);
            });

            Increase = new Command<Goods>(item => item.ColBusket++);
            Decrease = new Command<Goods>(item => item.ColBusket--);
        }
    }
}
