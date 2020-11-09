using System;

namespace xamarinJKH.Server.RequestModel
{
    public class ComissionModel
    {
        public Decimal TotalSum { get; set; }
        public Decimal Comission  { get; set; }
        public bool HideComissionInfo   { get; set; }
        public string Error { get; set; }
    }
}