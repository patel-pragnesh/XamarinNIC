using System;
using System.Collections.Generic;
using System.Text;

namespace xamarinJKH.Server.RequestModel
{
    public class BonusCashFlow
    {
        // дата/время
        public string Moment { get; set; }
        // сумма бонусов
        public decimal Amount { get; set; }
        // описание
        public string Description { get; set; }
    }
}
