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
    public partial class BasketPageNew : ContentPage
    {
        
        public BasketPageNew(ShopViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
        async void Back(object sender, EventArgs args)
        {
            await Navigation.PopAsync();
        }

    }
}