using System;
using System.Collections.Generic;
using System.Text;

namespace xamarinJKH.Server.RequestModel
{
    public class PaidRequestCodeModel
    {
        public string RequestId { get; set; }
        public string Code { get; set; }
    }

    public class PaidRequestResponse
    {
        public bool IsCorrect { get; set; }
    }
}
