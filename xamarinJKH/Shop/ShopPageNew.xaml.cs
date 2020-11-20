using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AppCenter.Analytics;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamarinJKH.DialogViews;
using xamarinJKH.InterfacesIntegration;
using xamarinJKH.ViewModels.Shop;
using Microsoft.AppCenter.Crashes;
using xamarinJKH.Tech;

namespace xamarinJKH.Shop
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ShopPageNew : ContentPage
    {
        ShopViewModel viewModel { get; set; }
        public ShopPageNew(xamarinJKH.Server.RequestModel.AdditionalService select)
        {
            InitializeComponent();
            Analytics.TrackEvent("Магазин " + select.Name);

            if(Device.RuntimePlatform==Device.iOS)
            {
                int statusBarHeight = DependencyService.Get<IStatusBar>().GetHeight();
                Pancake.Padding = new Thickness(0, statusBarHeight, 0, 0);
            }

            var techSend = new TapGestureRecognizer();
            techSend.Tapped += async (s, e) => { await Navigation.PushAsync(new AppPage());};
            LabelTech.GestureRecognizers.Add(techSend);

            BindingContext = viewModel = new ShopViewModel(select, this.Navigation);
            viewModel.LoadGoods.Execute(null);

        }

        async void Back(object sender, EventArgs args)
        {
            try
            {
                _ = await Navigation.PopAsync();
            }
            catch { }
        }

        private void CollectionView_Scrolled(object sender, ItemsViewScrolledEventArgs e)
        {
            try
            {
                var index = e.FirstVisibleItemIndex;
                var delta = e.HorizontalDelta;
                if (index >= 0)
                {
                    Analytics.TrackEvent($"index={index}, viewModel.Categories.Count={viewModel.Categories.Count}, e.HorizontalOffset={e.HorizontalOffset}  ");
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
                    Analytics.TrackEvent($"Выбрана ' '(пустая) категория, id={id}");
                    if (id > 0)
                        (sender as CollectionView).ScrollTo(id - 1);// viewModel.Categories[id - 1];
                }
            }
            catch(Exception ex)
            {
                Crashes.TrackError(ex);
            }
            
        }
    }
}