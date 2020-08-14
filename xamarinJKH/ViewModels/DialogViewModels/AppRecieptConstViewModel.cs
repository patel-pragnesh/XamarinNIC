using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using xamarinJKH.Server.RequestModel;

namespace xamarinJKH.ViewModels.DialogViewModels
{
    public class AppRecieptConstViewModel:BaseViewModel
    {
        public ObservableCollection<RequestsReceiptItem> ReceiptItems { get; set; }
        public Command Delete { get; set; }
        public Command Edit { get; set; }
        public Command Update { get; set; }
        public int ID { get; set; }

        public AppRecieptConstViewModel(List<RequestsReceiptItem> items, int id)
        {
            ReceiptItems = new ObservableCollection<RequestsReceiptItem>();
            ID = id;
            if (items != null)
            foreach (var item in items)
            {
                Device.BeginInvokeOnMainThread(() => ReceiptItems.Add(item));
            }

            Delete = new Command<RequestsReceiptItem>(item =>
            {
                if (item != null)
                {
                    Device.BeginInvokeOnMainThread(() => ReceiptItems.Remove(item));
                }
            });

            Edit = new Command<RequestsReceiptItem>(item => 
            {
            });

            Update = new Command(async () =>
            {
                var response = await Server.UpdateReceipt(new RequestsReceiptItemsList(this.ID, ReceiptItems));
                
            });

            MessagingCenter.Subscribe<Object, List<Goods>>(this, "AddItems", (sender, args) =>
            {
                if (args != null)
                {
                    foreach (var item in args)
                    {
                        var receipt_item = new RequestsReceiptItem { Name = item.Name, Price = item.Price, Amount = 1 };
                        var existing = ReceiptItems.First(x => x.Name == item.Name);
                        if (existing == null)
                            Device.BeginInvokeOnMainThread(() => ReceiptItems.Add(receipt_item));
                        else
                            existing.Amount++;
                    }
                }
            });
        }
    }
}
