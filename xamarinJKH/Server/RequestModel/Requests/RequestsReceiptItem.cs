using xamarinJKH.ViewModels;

namespace xamarinJKH.Server.RequestModel
{
    public class RequestsReceiptItem:BaseViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public decimal? Price { get; set; }
        decimal _quantity;
        public decimal Quantity
        {
            get => _quantity;
            set
            {
                _quantity = value;
                OnPropertyChanged(nameof(Quantity));
            }
        }
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

        public RequestsReceiptItem Copy()
        {
            return (RequestsReceiptItem)this.MemberwiseClone();
        }
        
        // кол-во бонусов для списания (оплаты)
        public decimal BonusAmount { get; set; }
    }
}