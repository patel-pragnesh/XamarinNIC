using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamarinJKH.ViewModels.DialogViewModels;

namespace xamarinJKH.DialogViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BonusHistoryDialogView : AiForms.Dialogs.Abstractions.DialogView
    {
        BonusHistoryViewModel viewModel { get; set; }
        public BonusHistoryDialogView()
        {
            InitializeComponent();
            BindingContext = viewModel = new BonusHistoryViewModel();
            viewModel.LoadHistory.Execute(null);
        }

        
    }
}