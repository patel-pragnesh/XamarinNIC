using System.Collections.Generic;

namespace xamarinJKH.Server.RequestModel
{
    public class PeriodStats
    {
        public int RequestsCount { get; set; }
        public List<Requests> UnperformedRequestsList { get; set; }
        public List<Requests> OverdueRequestsList { get; set; }
        public int ClosedRequestsWithMark1Count { get; set; }
        public List<Requests> ClosedRequestsWithMark1List { get; set; }
        public int ClosedRequestsWithMark2Count { get; set; }
        public List<Requests> ClosedRequestsWithMark2List { get; set; }
        public int ClosedRequestsWithMark3Count { get; set; }
        public List<Requests> ClosedRequestsWithMark3List { get; set; }
        public int ClosedRequestsWithMark4Count { get; set; }
        public List<Requests> ClosedRequestsWithMark4List { get; set; }
        public int ClosedRequestsWithMark5Count { get; set; }
        public List<Requests> ClosedRequestsWithMark5List { get; set; }
    }
}