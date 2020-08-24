using System.Collections.Generic;

namespace xamarinJKH.Server.RequestModel
{
    public class MeterInfo
    {
        // Номер лицевого счета
        public string Ident { get; set; }
        // ID прибора
        public int ID { get; set; }
        // Заводской номер
        public string FactoryNumber { get; set; }
        public string CustomName  { get; set; }
        // Дата последней поверки
        public string LastCheckupDate { get; set; }
        // Межповерочный интервал
        public int RecheckInterval { get; set; }
        // Ресурс
        public string Resource { get; set; }
        // Адрес
        public string Address { get; set; }
        // Уникальный номер
        public string UniqueNum { get; set; }
        // Количество знаков после запятой
        public int NumberOfDecimalPlaces { get; set; }
        // Показания
        public List<MeterValueInfo> Values { get; set; }
        // отключен
        public bool IsDisabled { get; set; }
    }

    public class MeterValueInfo
    {
        // Дата
        public string Period { get; set; }
        // Значение (Т1 для 2х 3х тарифныз)
        public decimal Value { get; set; }
        // Значение Т2
        public decimal? ValueT2 { get; set; }
        // Значение Т3
        public decimal? ValueT3 { get; set; }
    }
}