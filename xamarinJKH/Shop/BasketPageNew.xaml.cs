﻿using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AppCenter.Analytics;
using Xamarin.Forms;
using Xamarin.Forms.PancakeView;
using Xamarin.Forms.Xaml;
using xamarinJKH.DialogViews;
using xamarinJKH.InterfacesIntegration;
using xamarinJKH.Tech;
using xamarinJKH.Utils;
using xamarinJKH.ViewModels.Shop;
using xamarinJKH.Main;

namespace xamarinJKH.Shop
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BasketPageNew : ContentPage
    {
        private Color hex;
        
        public BasketPageNew(ShopViewModel vm)
        {
            InitializeComponent();
            Analytics.TrackEvent("Корзина магазина");

            if (Device.RuntimePlatform == Device.iOS)
            {
                int statusBarHeight = DependencyService.Get<IStatusBar>().GetHeight();
                Pancake.Padding = new Thickness(0, statusBarHeight, 0, 0);
            }
            
            //BorderColor = "{AppThemeBinding Light={x:DynamicResource MainColor}, Dark=#e7e7e7}"
            hex = (Color)Application.Current.Resources["MainColor"];
            Color hexColor = (Color)Application.Current.Resources["MainColor"];
            GoodsLayot.SetAppThemeColor(PancakeView.BorderColorProperty, hexColor, Color.White);
            PancakeBot.SetAppThemeColor(PancakeView.BorderColorProperty, hexColor, Color.Transparent);
            var profile = new TapGestureRecognizer();
            profile.Tapped += async (s, e) =>
            {
                if (Navigation.NavigationStack.FirstOrDefault(x => x is ProfilePage) == null)
                    await Navigation.PushAsync(new ProfilePage());
            };
            IconViewProfile.GestureRecognizers.Add(profile);

            var techSend = new TapGestureRecognizer();
            techSend.Tapped += async (s, e) => { await Navigation.PushAsync(new AppPage()); };
            LabelTech.GestureRecognizers.Add(techSend);


            BindingContext = vm;
        }
        async void Back(object sender, EventArgs args)
        {
            try
            {
                (this.BindingContext as ShopViewModel).Sort.Execute(null);
                _ = await Navigation.PopAsync();
            }
            catch { }
        }

    }
}