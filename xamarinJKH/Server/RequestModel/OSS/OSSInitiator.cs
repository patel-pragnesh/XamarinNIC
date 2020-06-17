namespace xamarinJKH.Server.RequestModel
{
    public class OSSInitiator
    {
        public string Ident { get; set; }
        public string FIO { get; set; }

        public string PremiseNumber { get; set; }
        public string CadastralNumber { get; set; }
        public decimal PropertyPercent { get; set; }
        public string Document { get; set; }

        public decimal Area { get; set; }
    }
}