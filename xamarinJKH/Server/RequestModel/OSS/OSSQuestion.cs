using System.Collections.Generic;
using System.Linq;

namespace xamarinJKH.Server.RequestModel
{
    public class OSSQuestion
    {
        public int ID { get; set; }
        public string Number { get; set; }// Номер вопроса
        public string Text { get; set; }// краткий Текст вопрсоа
        public string Answer { get; set; }// Ответ
        public string QuestionMessage { get; set; } // Полный текст вопроса
        public bool DoNotVoite { get; set; }
        public List<OSSAnswerStats> AnswersStats { get; set; }

        public int CountWhyVoiteYes { get; set; } // Кол-во ответов за
        public int CountWhyVoiteNo { get; set; } // Кол-во ответов против
        public int CountWhyVoiteUnknow { get; set; } // Кол-во ответов воздержался

        public int AnswerTotalCount
        {
            get
            {
                if (AnswersStats == null)
                    return 0;
                return AnswersStats.FirstOrDefault(x => x.Name == "Всего").Count;
            }
        }

        public int Answer0Count
        {
            get
            {
                if (AnswersStats == null)
                    return 0;
                return AnswersStats.FirstOrDefault(x => x.Name == "За").Count;
            }
        }
        public decimal Answer0Percentage
        {
            get
            {
                if (AnswersStats == null)
                    return 0;
                return AnswersStats.FirstOrDefault(x => x.Name == "За").Percentage;
            }
        }

        public int Answer1Count
        {
            get
            {
                if (AnswersStats == null)
                    return 0;
                return AnswersStats.FirstOrDefault(x => x.Name == "Против").Count;
            }
        }
        public decimal Answer1Percentage
        {
            get
            {
                if (AnswersStats == null)
                    return 0;
                return AnswersStats.FirstOrDefault(x => x.Name == "Против").Percentage;
            }
        }

        public int Answer2Count
        {
            get
            {
                if (AnswersStats == null)
                    return 0;
                return AnswersStats.FirstOrDefault(x => x.Name == "Воздержались").Count;
            }
        }
        public decimal Answer2Percentage
        {
            get
            {
                if (AnswersStats == null)
                    return 0;
                return AnswersStats.FirstOrDefault(x => x.Name == "Воздержались").Percentage;
            }
        }

        public string AnswerText
        {
            get
            {
                switch (Answer)
                {
                    case "0":
                        return "За";
                    case "1":
                        return "Против";
                    case "2":
                        return "Воздержалcя";
                }
                return "-";
            }
        }

        public string DecisionText
        {
            get
            {
                if (AnswersStats == null)
                    return "решение не принято";
                return AnswersStats.Where(y => y.Name != "Всего").OrderByDescending(x => x.Count).First().Name;
            }
        }
    }
}