using System.Collections.Generic;

namespace xamarinJKH.Server.RequestModel
{
    public class LoginResult
    {
        public string Login { get; set; } = "";
        public bool IsDispatcher { get; set; }
        public bool AccessOSS { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; } = "";
        public string companyPhone { get; set; } = "";
        public string Birthday { get; set; }
        public string FIO { get; set; }
        // токен доступа. 
        //Необходимо передавать как заголовок в методы, требующие авторизации
        public string acx { get; set; } 

        public List<AccountInfo> Accounts { get; set; }
        public string Error { get; set; }
        
        public string Code { get; set; }
        public override string ToString()
        {
            return FIO + " " + Login;
        }
    }

    public class AccountInfo
    {
        public AccountInfo(string ident, int metersStartDay, int metersEndDay, int id, string fio, string address, string company, bool metersAccessFlag)
        {
            Ident = ident.Trim();
            MetersStartDay = metersStartDay;
            MetersEndDay = metersEndDay;
            ID = id;
            FIO = fio;
            Address = address;
            Company = company;
            MetersAccessFlag = metersAccessFlag;
        }

        public AccountInfo()
        {
        }

        public int ID { get; set; }
        public string Ident { get; set; } = "";
        public string FIO { get; set; } = "";
        public string Address { get; set; } = "";
        public string Company { get; set; }
        public int MetersStartDay { get; set; }
        public int MetersEndDay { get; set; }
        public bool MetersAccessFlag { get; set; }

        public override string ToString()
        {
            return Ident;
        }
    }
    
   
    
}