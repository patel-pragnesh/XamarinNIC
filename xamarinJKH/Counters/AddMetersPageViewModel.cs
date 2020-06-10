using System;
using System.Collections.Generic;
using System.Text;

using xamarinJKH.ViewModels;
using Xamarin.Forms;

namespace xamarinJKH.Counters
{
    public class AddMetersPageViewModel:BaseViewModel
    {
        decimal _prevCount;
        public decimal PrevCounter
        {
            get => _prevCount;
            set
            {
                _prevCount = value;
                OnPropertyChanged("PrevCount");
            }
        }

        public AddMetersPageViewModel(decimal value)
        {
            PrevCounter = value;
        }
    }
}
