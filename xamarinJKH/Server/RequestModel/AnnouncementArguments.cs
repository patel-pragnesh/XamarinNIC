using System;
using System.Collections.Generic;

namespace xamarinJKH.Server.RequestModel
{
    public class AnnouncementArguments
    {
        public int ID { get; set; }

// отображать на главной
        public Nullable<bool> ShowOnMainPage { get; set; }

// Заголовок (обязательное)
        public string Header { get; set; }

// Текст (обязательное)
        public string Text { get; set; }

// Район
        public Nullable<int> id_homegroup { get; set; }

// Активно с
        public string ActiveFrom { get; set; }

// Активно до
        public string ActiveTo { get; set; }

// Для л/сч с долгом больше
        public Nullable<decimal> ForAccountsWithDebtOver { get; set; }

// Для л/сч с номером
        public string Ident { get; set; }

// ID опроса
        public Nullable<int> id_QuestionGroup { get; set; }

// ID код доп
        public Nullable<int> id_AdditionalService { get; set; }

// ОС (Android, iOS)
        public string OS { get; set; }

// Список домов для отбора
        public List<int> Houses { get; set; }
    }
}