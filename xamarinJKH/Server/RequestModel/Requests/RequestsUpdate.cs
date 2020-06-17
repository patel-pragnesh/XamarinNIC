using System.Collections.Generic;

namespace xamarinJKH.Server.RequestModel
{
    public class RequestsUpdate
    {
        public string NewUpdateKey { get; set; }
        public List<int> UpdatedRequests { get; set; }
        public RequestContent CurrentRequestUpdates { get; set; }
        public string Error { get; set; }
    }
}