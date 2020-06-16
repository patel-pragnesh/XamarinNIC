using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace xamarinJKH
{
    public class BoxForCounter:BoxView
    {
        public Color CellColor => Color.FromHex(xamarinJKH.Utils.Settings.MobileSettings.color);

        public BoxForCounter()
        {
            Color = CellColor;  
            Margin = new Thickness(0);  
            Grid.SetRow(this,1);
        }
    }
}
