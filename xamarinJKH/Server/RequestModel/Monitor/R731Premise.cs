using System.Collections.Generic;

namespace xamarinJKH.Server.RequestModel
{
    public class R731Premise
    {
        public int ID { get; set; }
        public string Number { get; set; }
        public string UniqueNum { get; set; }
        public string Entrance { get; set; }
        public int? EntranceID { get; set; }
        public bool IsNonresidential { get; set; }

        public Account Account { get; set; }
        List<R731Room> Rooms { get; set; }
    }
}