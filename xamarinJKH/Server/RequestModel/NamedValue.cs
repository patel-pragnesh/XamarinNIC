using System.Collections.Generic;
using xamarinJKH.ViewModels;

namespace xamarinJKH.Server.RequestModel
{
    public class NamedValue:BaseViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        bool selected;
        public bool Selected
        {
            get => selected;
            set
            {
                selected = value;
                OnPropertyChanged("Selected");
            }
        }
        
        public class AccountingInfoModel
        {
            public List<NamedValue> AllAcc { get; set; }
            public NamedValue SelectedAcc { get; set; }
        }
    }
}