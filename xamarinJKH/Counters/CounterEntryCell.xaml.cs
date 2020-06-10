using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace xamarinJKH.Counters
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CounterEntryCell : ContentView
    {
        public static BindableProperty Index = BindableProperty.Create("CellIndex", typeof(string), typeof(string), null);
        public string CellIndex
        {
            set
            {
                SetValue(Index, value);
                SetCorners(int.Parse(value));
            }
            get => (string)GetValue(Index);
        }
        public CounterEntryCell()
        {
            InitializeComponent();
            BindingContext = new CounterCellViewModel();
            
        }

        void SetCorners(int index)
        {
            if (index == 0)
            {
                Pancake.CornerRadius = new CornerRadius(5, 5, 5, 0);
            }

            if (index == 8)
            {
                Pancake.CornerRadius = new CornerRadius(5, 5, 0, 5);
            }
        }

        internal class CounterCellViewModel
        {
            public Color CellColor => Color.FromHex(xamarinJKH.Utils.Settings.MobileSettings.color);
        }
    }

}