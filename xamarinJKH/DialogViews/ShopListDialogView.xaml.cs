using AiForms.Dialogs.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamarinJKH.ViewModels.DialogViewModels;
using xamarinJKH.Server.RequestModel;

namespace xamarinJKH.DialogViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ShopListDialogView : DialogView
    {
        ShopListViewModel viewModel;
        public ShopListDialogView()
        {
            InitializeComponent();
            BindingContext = viewModel = new ShopListViewModel(DialogNotifier);

            viewModel.LoadItems.Execute(null);
        }
    }
}