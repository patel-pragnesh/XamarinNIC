using System;
using System.Collections.Generic;
using xamarinJKH.Server.RequestModel;

namespace xamarinJKH.Utils.Compatator
{
    public class HistoryPayComparable : IComparer<PaymentInfo>
    {
        public int Compare(PaymentInfo x, PaymentInfo y)
        {
            DateTime one = DateTime.ParseExact(x.Date, "dd.MM.yyyy", System.Globalization.CultureInfo.CurrentCulture);
            DateTime two = DateTime.ParseExact(y.Date, "dd.MM.yyyy", System.Globalization.CultureInfo.CurrentCulture);
            return one.CompareTo(two);
        }
    }
}