using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace xamarinJKH.AppsConst
{
    public class OptionModel
    {
        public string Name { get; set; }
        public string Image { get; set; }
        public bool IsVisible { get; set; }
        public Command Command { get; set; }
    }
}
