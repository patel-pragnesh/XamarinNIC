using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Forms;
using xamarinJKH.Server.RequestModel;

namespace xamarinJKH.ViewModels.DialogViewModels
{
    public class BonusHistoryViewModel:BaseViewModel
    {
        public ObservableCollection<BonusCashFlow> History { get; set; }
        public Command LoadHistory { get; set; }
    }
}
