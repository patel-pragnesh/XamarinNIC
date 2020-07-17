namespace xamarinJKH.Server.RequestModel
{
    public class OSSBase
    {
        public int ID { get; set; }
        public string DateStart { get; set; }
        public string DateEnd { get; set; }
        public string Status { get; set; }
        public string MeetingTitle { get; set; }
        public string Form { get; set; }
        public string HouseAddress { get; set; }

        public bool? IsStarted { get; set; }
        public bool IsComplete { get; set; }
        public string InitiatorNames { get; set; }
    }
}