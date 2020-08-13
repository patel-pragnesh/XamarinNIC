using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using xamarinJKH.ViewModels.DialogViewModels;
using AiForms.Dialogs.Abstractions;

namespace xamarinJKH.DialogViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AppReceiptDialogWindow : DialogView
    {
        public AppReceiptDialogWindow(AppRecieptViewModel vm)
        {
            InitializeComponent();

            BindingContext = vm;
        }
    }
}