using System.Collections.Generic;

namespace xamarinJKH.Server.RequestModel
{
    public class Goods
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Units { get; set; }
        public decimal? Rest { get; set; }
        public decimal? Weight { get; set; }
        public bool HasImage { get; set; }
        public decimal? Price { get; set; }
        public int ColBusket = 0;
        public decimal priceBusket = 0;
        public decimal weightBusket = 0;
        
        public List<string> Categories { get; set; }

    }
}