using System;
using System.Collections.Generic;

namespace xamarinJKH.Server.RequestModel
{
    public class OSS
    {
        public int ID { get; set; }
        public string MeetingTitle { get; set; } // Заголовок собрания
        public string DateStart { get; set; } // дата начала собрания
        public string DateEnd { get; set; } // дата окончания собрания
        public string DateRealPart { get; set; } // дата начала очного собрания

        public string ResultsReleaseDate { get; set; } // Дата когда будут доступны итоги голосования

        public string DateRegistrationRealPart { get; set; } // Дата ренгистрации для очного голосования
        public string RealPartPlace { get; set; } // Место прохождения очного голосования
        public string PlaceForViewingDocuments { get; set; }

        public string PlaceOfReceiptSolutions { get; set; } // Место подсчета результатов
        public string DateEndReceiptSolutions { get; set; }// Окончание очного голосования

        public string HouseAddress { get; set; } // Адресс дома
        public string Author { get; set; } // Инициатор собрания
        public string Form { get; set; } // Форма голосования (очная/заочная и т.д)
        public string Type { get; set; } // Тип собрания (внеочередное и т.д)
        public string MeetingView { get; set; } // Вид собрания

        public string Comment { get; set; } // Коммент 
        public decimal AreaResidential { get; set; } // Общая площадь помещений:
        public decimal AreaNonresidential { get; set; } // Площадь нежилая
        public bool IsComplete { get; set; } // Собрание завершено 
        public decimal VoitingArea { get; set; } // Общая площадь помещений:
        public decimal ComplateArea { get; set; } // Общая доля проголосовавших
        public decimal ComplateAreaPercents
        {
            get
            {                
                if (VoitingArea == 0) return 0;
                return Math.Round(ComplateArea * 100 / VoitingArea, 2);
            }
        }


        // Администратор собрания
        public string AdminstratorName { get; set; }
        public string AdminstratorDocNumber { get; set; }
        public string AdminstratorAddress { get; set; }
        public string AdminstratorPhone { get; set; }
        public string AdminstratorEmail { get; set; }
        public string AdminstratorPostAddress { get; set; }
        public string AdminstratorSite { get; set; }

        public bool AdministratorIsFL { get; set; } // Физ лицо 
        public bool AdministratorIsUL { get; set; } // Юр Лицо


        public List<OSSQuestion> Questions { get; set; } // Вопросы
        public List<OSSAccount> Accounts { get; set; }// Лс
        public List<OSSInitiator> Initiators { get; internal set; }// Инициаторы
        public string WebSiteForScanDocView { get; internal set; }// Электронные образцы
        public bool InitiatorsIsCompany { get; internal set; } // Инициатор Ук или Собственник
        public int TotalAccounts { get; internal set; } // Кол-во аккаунтов доступных для голосования
        public int ComplateAccoounts { get; internal set; } // Всего проголосовавших собственников 
        public string Error { get; set; }
                
        public bool HasProtocolFile { get; set; } // файл протокола загружен
        
        public string ProtocolFileLink { get; set; } // ccылка на файл протокола
    }
}