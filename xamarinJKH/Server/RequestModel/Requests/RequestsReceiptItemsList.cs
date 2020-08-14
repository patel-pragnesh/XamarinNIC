using System;
using System.Collections.Generic;
using System.Text;

namespace xamarinJKH.Server.RequestModel
{
    public class RequestsReceiptItemsList
    {
        public int RequestId { get; set; }
        public List<RequestsReceiptItem> ReceiptItems { get; set; }

        public RequestsReceiptItemsList(int id, IEnumerable<RequestsReceiptItem> items)
        {
            RequestId = id;
            ReceiptItems = new List<RequestsReceiptItem>();
            ReceiptItems.AddRange(items);
        }
    }
}
