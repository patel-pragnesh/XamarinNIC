using xamarinJKH.ViewModels;

namespace xamarinJKH.Server.RequestModel
{
    public class RequestsReceiptItem:BaseViewModel
    {
        public string Name { get; set; }
        public decimal? Price { get; set; }
        public decimal Quantity { get; set; }
        decimal _amount;
        public decimal Amount
        {
            get => _amount;
            set
            {
                _amount = value;
                OnPropertyChanged(nameof(Amount));
            }
        }
    }
}