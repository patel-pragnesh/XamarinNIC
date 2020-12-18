using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace xamarinJKH.AppsConst
{
    public class OptionModel:xamarinJKH.ViewModels.BaseViewModel
    {
        public string Name { get; set; }
        public string Image { get; set; }
        public bool IsVisible { get; set; }
        public Command Command { get; set; }
        Dictionary<string, string> replace;
        public Dictionary<string, string> ReplaceMap
        {
            get => replace;
            set
            {
                replace = value;
                OnPropertyChanged("ReplaceMap");
            }
        }
    }
}
