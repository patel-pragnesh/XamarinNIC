using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
                if (item != null)
                {
                    MessagingCenter.Send<AppRecieptConstViewModel, string>(this, "SelectPrice", item.Name);
                }
            });
        }
    }
}
