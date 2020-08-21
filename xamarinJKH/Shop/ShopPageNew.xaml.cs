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
    }
}