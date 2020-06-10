using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace xamarinJKH.Counters
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CounterEntryView : ContentView
    {
        public Entry CounterInput
        {
            get => EntryCount;
        }
        public CounterEntryView()
        {
            InitializeComponent();
        }


    }
}