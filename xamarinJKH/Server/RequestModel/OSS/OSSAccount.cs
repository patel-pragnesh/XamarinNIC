namespace xamarinJKH.Server.RequestModel
{
    public class OSSAccount
    {
        
        public string Ident { get; set; } // Лицевой счет
        public decimal Area { get; set; } // Площадь
        public decimal PropertyPercent { get; set; } // Доля

        public string PremiseNumber { get; set; } // Номер собственности
        public string CadastralNumber { get; set; } // Кадастровый номер
        public string PropertyForm { get; set; }
        public string Document { get; set; } // Докумен о собственности
    }
}