using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamarinJKH.ViewModels.Shop;

namespace xamarinJKH.Shop
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ShopPageNew : ContentPage
    {
        ShopViewModel viewModel { get; set; }
        public ShopPageNew(xamarinJKH.Server.RequestModel.AdditionalService select)
        {
            InitializeComponent();
            BindingContext = viewModel = new ShopViewModel(select, this.Navigation);
            viewModel.LoadGoods.Execute(null);
        }

        async void Back(object sender, EventArgs args)
        {
            await Navigation.PopAsync();
        }

        private void CollectionView_Scrolled(object sender, ItemsViewScrolledEventArgs e)
        {
            var index = e.FirstVisibleItemIndex;
            var delta = e.HorizontalDelta;
            if (index >= 0)
            {
                if (index + 1 <= viewModel.Categories.Count - 1 && e.HorizontalOffset < 160)
                {
                    viewModel.SelectedCategory = viewModel.Categories[index];
                }
                else
                {
                    viewModel.SelectedCategory = viewModel.Categories[e.LastVisibleItemIndex];
                }
            }
            if (viewModel.SelectedCategory == " ")
            {
                var id = viewModel.Categories.ToList().IndexOf(" ");
                (sender as CollectionView).ScrollTo(id - 1);// viewModel.Categories[id - 1];
            }
        }
    }
}