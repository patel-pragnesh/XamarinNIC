using AiForms.Dialogs.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamarinJKH.ViewModels.DialogViewModels;
using System.Text.RegularExpressions;
using Xamarin.Forms.Internals;
using System.Diagnostics.Tracing;
using AiForms.Dialogs;

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
        }

        decimal currentVal;
        protected void SelectAllText(object sender, FocusEventArgs args)
        {
            decimal.TryParse((sender as Entry).Text.Replace(AppResources.Currency, " ").Trim(), out currentVal);
        }

        void SetEditor(object sender, EventArgs args)
        {
            var button = sender as ImageButton;
            var cell = button.Parent;
            var editor = (cell as Grid).Children[2] as Entry;
            editor.IsReadOnly = false;
            editor.Focus();
        }

        protected void SetPrice(object sender, FocusEventArgs args)
        {
            var entry = (sender as Entry);
            try
            {
                decimal price = 0;
                if (entry.Text != null)
                {
                    var value = decimal.TryParse(entry.Text.Replace(AppResources.Currency, " ").Trim(), out price);
                    if (price > 0)
                    {
                        entry.Text = String.Format("{0:0.00} ", price) + AppResources.Currency;
                        var item = viewModel.ReceiptItems.First(x => x.Name == entry.ClassId);
                        if (item != null)
                        {
                            item.Price = price;
                        }
                    }
                    else
                    {
                        price = this.currentVal;
                        entry.Text = String.Format("{0:0.00} ", price) + AppResources.Currency;
                    }
                }
                else
                {
                    price = this.currentVal;
                    entry.Text = String.Format("{0:0.00} ", price) + AppResources.Currency;
                }
            }
            catch
            {

            }
            finally
            {
                if (entry != null)
                {
                    entry.Unfocus();
                }
            }
            
        }
            
        async void AddGoods(object sender, EventArgs args)
        {
            await Dialog.Instance.ShowAsync(new ShopListDialogView());
        }

    }
}