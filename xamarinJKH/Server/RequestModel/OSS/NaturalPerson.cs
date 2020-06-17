namespace xamarinJKH.Server.RequestModel
{
    public class NaturalPerson
    {
        public int ID { get; set; }
        public string Ident { get; set; }
        public string FIO { get; set; } = "";
        public string DateOfBirth { get; set; }
        public string PlaceOfBirth { get; set; } = "";
        public string PassportSerie { get; set; } = "";
        public string PassportNumber { get; set; } = "";
        public string PassportDate { get; set; }
        public string PassportIssuedBy { get; set; } = "";
        public string RegistrationAddress { get; set; }
        public string Error { get; set; }
    }
}