using System.Collections.Generic;

namespace xamarinJKH.Server.RequestModel
{
    public class PollingResult
    {
        public int PollId { get; set; }
        public string ExtraInfo { get; set; }
        public List<PollAnswer> Answers { get; set; }

    }
    public class PollAnswer
    {
        public int QuestionId { get; set; }
        public int AnswerId { get; set; }
    }

}