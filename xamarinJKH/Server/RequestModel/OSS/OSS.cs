using System;
using System.Collections.Generic;

namespace xamarinJKH.Server.RequestModel
{
    public class OSS
    {
        public int ID { get; set; }
        public string MeetingTitle { get; set; }
        public string DateStart { get; set; }
        public string DateEnd { get; set; }
        public string DateRealPart { get; set; }

        public string ResultsReleaseDate { get; set; }

        public string DateRegistrationRealPart { get; set; }
        public string RealPartPlace { get; set; }
        public string PlaceForViewingDocuments { get; set; }

        public string PlaceOfReceiptSolutions { get; set; }
        public string DateEndReceiptSolutions { get; set; }

        public string HouseAddress { get; set; }
        public string Author { get; set; }
        public string Form { get; set; }
        public string Type { get; set; }
        public string MeetingView { get; set; }

        public string Comment { get; set; }
        public decimal AreaResidential { get; set; }
        public decimal AreaNonresidential { get; set; }
        public bool IsComplete { get; set; }
        public decimal VoitingArea { get; set; }
        public decimal ComplateArea { get; set; }
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

        public bool AdministratorIsFL { get; set; }
        public bool AdministratorIsUL { get; set; }


        public List<OSSQuestion> Questions { get; set; }
        public List<OSSAccount> Accounts { get; set; }
        public List<OSSInitiator> Initiators { get; internal set; }
        public string WebSiteForScanDocView { get; internal set; }
        public bool InitiatorsIsCompany { get; internal set; }
        public int TotalAccounts { get; internal set; }
        public int ComplateAccoounts { get; internal set; }
        public string Error { get; set; }
    }
}