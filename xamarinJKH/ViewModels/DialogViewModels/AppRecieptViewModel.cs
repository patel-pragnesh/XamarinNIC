using Xamarin.Forms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using xamarinJKH.Server.RequestModel;
using System.Linq;

namespace xamarinJKH.ViewModels.DialogViewModels
{
    public class AppRecieptViewModel:BaseViewModel
    {
        public ObservableCollection<RequestsReceiptItem> ReceiptItems { get; set; }
        decimal price;
        public decimal Price
        {
            get => price;
            set
            {
                price = value;
                OnPropertyChanged(nameof(Price));
            }
        }
        public AppRecieptViewModel(List<RequestsReceiptItem> items)
        {
            ReceiptItems = new ObservableCollection<RequestsReceiptItem>();
            foreach (var item in items)
            {
                Device.BeginInvokeOnMainThread(() => ReceiptItems.Add(item));
            }
            var total = items.Select(x => x.Price * x.Amount).Sum();
            Price = Convert.ToDecimal(total);
        }
    }
}
