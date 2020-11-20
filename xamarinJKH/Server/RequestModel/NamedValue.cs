using System.Collections.Generic;

namespace xamarinJKH.Server.RequestModel
{
    public class NamedValue
    {
        public int ID { get; set; }
        public string Name { get; set; }
        
        public class AccountingInfoModel
        {
            public List<NamedValue> AllAcc { get; set; }
            public NamedValue SelectedAcc { get; set; }
        }
    }
}