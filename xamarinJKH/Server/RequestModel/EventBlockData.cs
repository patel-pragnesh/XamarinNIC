using System.Collections.Generic;

namespace xamarinJKH.Server.RequestModel
{
    public class EventBlockData
    {
        // Опросы
        public List<PollInfo> Polls { get; set; }
        // Объявления
        public List<AnnouncementInfo> Announcements { get; set; }
        // Новости
        public List<NewsInfo> News { get; set; }
        // Доп. услуги
        public List<AdditionalService> AdditionalServices { get; set; }
        public string Error { get; set; }
    }

    public class PollInfo
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public bool IsReaded { get; set; }
        public List<QuestionInfo> Questions { get; set; }
    }

    public class QuestionInfo
    {
        public int ID { get; set; }
        public string Text { get; set; }
        public List<AnswerInfo> Answers { get; set; }
    }

    public class AnswerInfo
    {
        public string Text { get; set; }
        public int ID { get; set; }
        public bool IsUserAnswer { get; set; }
    }

    public class AnnouncementInfo
    {
        public int ID { get; set; }
        public string Created { get; set; }
        public string Text { get; set; }
        public int QuestionGroupID { get; set; }
        public int AdditionalServiceId { get; set; }
        public bool IsReaded { get; set; }
        public string Header { get; set; }
    }

    public class NewsInfo
    {
        public int ID { get; set; }
        public string Header { get; set; }
        public string ShortContent { get; set; }
        public string Created { get; set; }
        public bool HasImage { get; set; }
    }
    public class NewsInfoFull
    {
        public int ID { get; set; }
        public string Created { get; set; }
        public string Header { get; set; }
        public string Text { get; set; }
        public bool HasImage { get; set; }
        public string Error { get; set; }
    }

    public class AdditionalService
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public bool HasLogo { get; set; }
        public string Phone { get; set; }
        public string Group { get; set; }
        public int? id_RequestType { get; set; }
        public int? id_Account { get; set; }
        public bool CanBeOrdered { get; set; }
        public string ShowInAdBlock { get; set; }
        public string ShopName { get; set; }
        public int? ShopID { get; set; }
    }

}