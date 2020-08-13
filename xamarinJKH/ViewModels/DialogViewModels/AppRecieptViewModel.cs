using Xamarin.Forms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using xamarinJKH.Server.RequestModel;

namespace xamarinJKH.ViewModels.DialogViewModels
{
    public class AppRecieptViewModel:BaseViewModel
    {
        public ObservableCollection<RequestsReceiptItem> ReceiptItems { get; set; }
        
        public AppRecieptViewModel(List<RequestsReceiptItem> items)
        {
            ReceiptItems = new ObservableCollection<RequestsReceiptItem>();
            foreach (var item in items)
            {
                Device.BeginInvokeOnMainThread(() => ReceiptItems.Add(item));
            }
        }
    }
}
