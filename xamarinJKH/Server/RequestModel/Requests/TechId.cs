namespace xamarinJKH.Server.RequestModel
{
    public class TechId
    {
        // {"isSucceed":true,"requestId":"150133"}
        
        private bool isSucceed { get; set; }
        public int requestId { get; set; }
        public string Error { get; set; }
    }
}