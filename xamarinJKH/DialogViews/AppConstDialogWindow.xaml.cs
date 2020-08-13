using AiForms.Dialogs.Abstractions;
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
    public partial class AppConstDialogWindow : DialogView
    {
        AppRecieptConstViewModel viewModel { get; set; }
        public AppConstDialogWindow(AppRecieptConstViewModel vm)
        {
            InitializeComponent();
            BindingContext = viewModel = vm;
            MessagingCenter.Subscribe<AppRecieptConstViewModel, string>(this, "SelectPrice", (sender, args) =>
            {
                var entry = FindByName(args);
                if (entry != null)
                {
                    (entry as BordlessEditor).IsReadOnly = false;
                    (entry as BordlessEditor).Focus();
                }
            });
        }
    }
}