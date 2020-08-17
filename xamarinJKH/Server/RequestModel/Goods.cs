using System.Collections.Generic;

namespace xamarinJKH.Server.RequestModel
{
    public class Goods:xamarinJKH.ViewModels.BaseViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Units { get; set; }
        public decimal? Rest { get; set; }
        public decimal? Weight { get; set; }
        public bool HasImage { get; set; }
        public decimal? Price { get; set; }
        int _colBasket;
        public int ColBusket
        {
            get => _colBasket;
            set
            {
                if (value >= 0)
                {
                    _colBasket = value;
                    OnPropertyChanged("ColBusket");

                }
            }
        }
        public decimal priceBusket = 0;
        public decimal weightBusket = 0;
        
        public List<string> Categories { get; set; }
        string image;
        public string Image
        {
            get => RestClientMP.SERVER_ADDR + "/public/GoodsImage/" + this.ID;
        }
        

    }
}