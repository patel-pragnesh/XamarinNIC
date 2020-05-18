using System.Collections.Generic;

namespace xamarinJKH.Server.RequestModel
{
    public class RequestList
    {
        public List<RequestInfo> Requests { get; set; }
        public string UpdateKey { get; set; }
        public string Error { get; set; }
    }
    
    public class RequestInfo
    {
        public int ID { get; set; }
        public string RequestNumber { get; set; }
        public string Added { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public int StatusID { get; set; }
        public bool IsClosed { get; set; }
        public bool IsPerformed { get; set; }
    }

    public class RequestContent : RequestInfo
    {
        public string Phone { get; set; }
        public string Address { get; set; }
        public int TypeID { get; set; }
        public string TypeName { get; set; }

        public List<RequestMessage> Messages { get; set; }
        public List<RequestFile> Files { get; set; }
        public string Error { get; set; }
    }

    public class RequestMessage
    {
        public int ID { get; set; }
        public string Added { get; set; }
        public string AuthorName { get; set; }
        public string Text { get; set; }
        public int FileID { get; set; }
        // сообщение создано текущим пользователем
        public bool IsSelf { get; set; }
    }

    public class RequestFile
    {
        public int ID { get; set; }
        public string Added { get; set; }
        public string AuthorName { get; set; }
        public string Name { get; set; }
        // файл загружен текущим пользователем
        public bool IsSelf { get; set; }
        public int FileSize { get; set; }
    }

}