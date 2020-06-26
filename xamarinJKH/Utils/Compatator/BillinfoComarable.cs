using System;
using System.Collections.Generic;
using xamarinJKH.Server.RequestModel;

namespace xamarinJKH.Utils.Compatator
{
    public class BillinfoComarable : IComparer<BillInfo>
    {
        public int Compare(BillInfo x, BillInfo y)
        {
            DateTime one = DateTime.ParseExact(x.Period.Replace(" г.", ""), "MMMM yyyy", System.Globalization.CultureInfo.CurrentCulture);
            DateTime two = DateTime.ParseExact(y.Period.Replace(" г.", ""), "MMMM yyyy", System.Globalization.CultureInfo.CurrentCulture);
            return one.CompareTo(two);
        }
    }
    
    
}