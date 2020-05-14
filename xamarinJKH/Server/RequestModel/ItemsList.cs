using System.Collections.Generic;

namespace xamarinJKH.Server.RequestModel
{
    public class ItemsList<T>
    {
        public List<T> Data { get; set; }
        public string Error { get; set; }
    }
}