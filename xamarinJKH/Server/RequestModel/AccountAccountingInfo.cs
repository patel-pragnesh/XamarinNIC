using System.Collections.Generic;
using Xamarin.Forms;

namespace xamarinJKH.Server.RequestModel
{
    public class AccountAccountingInfo
    {
        public string Ident { get; set; }
        public decimal Sum { get; set; }
        public decimal SumFine { get; set; }
        public string Address { get; set; }
        public string INN { get; set; }
        public string DebtActualDate { get; set; }
        public decimal InsuranceSum { get; set; }
        public int? HouseId { get; set; }
        public bool DontShowInsurance { get; set; }

        public List<BillInfo> Bills { get; set; }
        public List<MobilePayment> MobilePayments { get; set; }
        public List<PaymentInfo> Payments { get; set; }
        public string Error { get; set; }
    }
    
    public class BillInfo 
    {
        public int ID { get; set; }
        public string Ident { get; set; }
        public string Period { get; set; }
        public bool HasFile { get; set; }
        public decimal Total { get; set; }
        
    }
    
    

    public class MobilePayment
    {
        public int ID { get; set; }
        public string Ident { get; set; }
        public string ID_Pay { get; set; }
        public string Date { get; set; }
        public decimal Sum { get; set; }
        public string Status { get; set; }
        public string Desc { get; set; }
        public string Email { get; set; }
    }

    public class PaymentInfo
    {
        public string Ident { get; set; }
        public string Date { get; set; }
        public string Period { get; set; }
        public decimal Sum { get; set; }
    }

    public class AccountingInfoModel
    {
        public List<AccountAccountingInfo> AllAcc { get; set; }
        public AccountAccountingInfo SelectedAcc { get; set; }
        public Color hex { get; set; }
        
    }
    
}