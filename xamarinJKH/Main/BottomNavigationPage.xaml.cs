using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace xamarinJKH.Main
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BottomNavigationPage : TabbedPage
    {
        public BottomNavigationPage()
        {

            InitializeComponent ();
            SelectedTabColor = Color.FromHex(Utils.Settings.MobileSettings.color);
            UnselectedTabColor = Color.Gray;
           
        }
    }
}