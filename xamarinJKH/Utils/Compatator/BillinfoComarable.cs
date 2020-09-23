using System;
using System.Collections.Generic;
using System.Globalization;
using xamarinJKH.Server.RequestModel;

namespace xamarinJKH.Utils.Compatator
{
    public class BillinfoComarable : IComparer<BillInfo>
    {
        public int Compare(BillInfo x, BillInfo y)
        {          

            DateTime one = new DateTime();
            DateTime two = new DateTime();
            var d1 = x.Period.Replace(" г.", "");
            var d2 = y.Period.Replace(" г.", "");
            var oneOk = DateTime.TryParseExact(d1, "MMMM yyyy", CultureInfo.CreateSpecificCulture("ru-RU"), DateTimeStyles.None, out one);
            var twoOk = DateTime.TryParseExact(d2, "MMMM yyyy", CultureInfo.CreateSpecificCulture("ru-RU"), DateTimeStyles.None, out two);
                       
            if (!oneOk)
                return -1;
            if (!twoOk)
                return 1;
           
            var r = one.CompareTo(two);
            return r;
        }
    }
    
    
}