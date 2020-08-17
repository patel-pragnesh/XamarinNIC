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
            BindingContext = viewModel = new ShopListViewModel();

            viewModel.LoadItems.Execute(null);
        }

        void AddItems(object sender, EventArgs args)
        {
            DialogNotifier.Cancel();
        }

        private void collection_items_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            viewModel.SelectedItems.Clear();
            if (e.CurrentSelection != null)
            {
                foreach (var good in e.CurrentSelection)
                {
                    viewModel.SelectedItems.Add(good as Goods);
                }

            }
        }
    }
}