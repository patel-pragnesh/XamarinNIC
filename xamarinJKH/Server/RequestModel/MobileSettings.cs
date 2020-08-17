using System.Collections.Generic;

namespace xamarinJKH.Server.RequestModel
{
    public class MobileSettings
    {
        public string showAds { get; set; }
        public int adsType { get; set; }
        public string adsCodeIOS { get; set; }
        public string adsCodeAndroid { get; set; }
        public string bonusOfertaFile { get; set; }
        public bool enableOSS { get; set; }    
        
        public bool chooseIdentByHouse { get; set; }    
        public bool useBonusSystem { get; set; }    
        public bool useDispatcherAuth { get; set; }
        public string color { get; set; }
        
        public double servicePercent { get; set; }
        public bool DontShowDebt { get; set; }
        public bool registerWithoutSMS { get; set; }
        public bool сheckCrashSystem { get; set; }
        public string main_name { get; set; }
        
        public List<MobileMenu> menu { get; set; }
        public string Error { get; set; }
        public string appLinkIOS { get; set; }
        public string appLinkAndroid { get; set; }
    }

    public class MobileMenu
    {
        public int id { get; set; }
        public string name_app { get; set; }
        public string simple_name { get; set; }
        public int visible { get; set; }
    }
}