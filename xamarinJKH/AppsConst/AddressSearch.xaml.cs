﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamarinJKH.ViewModels.AppsConst;
using dotMorten.Xamarin.Forms;
using Rg.Plugins.Popup.Services;
using xamarinJKH.DialogViews;
using xamarinJKH.Server.RequestModel;
using xamarinJKH.InterfacesIntegration;

namespace xamarinJKH.AppsConst
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddressSearch : ContentPage
    {
        public int Type { get; set; }
        AddressSearchViewModel viewModel { get; set; }
        public AddressSearch(int type, Tuple<NamedValue, NamedValue, NamedValue> selected)
        {
            InitializeComponent();
            this.Type = type;
            BindingContext = viewModel = new AddressSearchViewModel(selected);
            switch (type)
            {
                case 1: StreetStack.IsVisible = false;
                    //HouseStack.IsVisible = false;
                    FlatStack.IsVisible = false;
                    break;
                case 2:
                    //HouseStack.IsVisible = false;
                    FlatStack.IsVisible = false;
                    break;
            }


            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    int statusBarHeight = DependencyService.Get<IStatusBar>().GetHeight();
                    Pancake2.HeightRequest = statusBarHeight;                    
                    break;
                default:
                    break;
            }


        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            viewModel.LoadDistricts.Execute(null);
        }
        private async void GoBack(object sender, EventArgs args)
        {
            await Navigation.PopAsync();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Send<Object, Tuple<NamedValue, NamedValue, NamedValue>>(this, "SetNames", new Tuple<NamedValue, NamedValue, NamedValue>(this.viewModel.DistrictObject, this.viewModel.HouseObject, this.viewModel.FlatObject));
            MessagingCenter.Send<Object, Tuple<int?, int?, int?, string>>(this, "SetTypes", new Tuple<int?, int?, int?, string>(viewModel.DistrictID, viewModel.HouseID, viewModel.PremiseID, viewModel.Street));
        }

        private void District_TextChanged(object sender, dotMorten.Xamarin.Forms.AutoSuggestBoxTextChangedEventArgs e)
        {
            var box = sender as dotMorten.Xamarin.Forms.AutoSuggestBox;
            if (e.Reason == dotMorten.Xamarin.Forms.AutoSuggestionBoxTextChangeReason.UserInput)
            {
                box.ItemsSource =
                    viewModel.Districts.Select(x => x.Name).Where(y => y.ToUpper().Contains(box.Text.ToUpper())).ToList();
            }
        }

        private void District_SuggestionChosen(object sender, dotMorten.Xamarin.Forms.AutoSuggestBoxSuggestionChosenEventArgs e)
        {
            var selected = e.SelectedItem;
            var id = viewModel.Districts.Find(x => x.Name == selected).ID;
            viewModel.DistrictID = Convert.ToInt32(id);
            (sender as AutoSuggestBox).Unfocus();
        }

        private void House_TextChanged(object sender, dotMorten.Xamarin.Forms.AutoSuggestBoxTextChangedEventArgs e)
        {
            var box = sender as dotMorten.Xamarin.Forms.AutoSuggestBox;
            if (viewModel.Houses != null)
                box.ItemsSource =
                    viewModel.Houses.Select(x => x.Address).ToList();
        }
        private void House_Focused(object sender, FocusEventArgs e)
        {
            (sender as dotMorten.Xamarin.Forms.AutoSuggestBox).Text = null;
            (sender as dotMorten.Xamarin.Forms.AutoSuggestBox).Text = " ";
        }

        private void House_SuggestionChosen(object sender, dotMorten.Xamarin.Forms.AutoSuggestBoxSuggestionChosenEventArgs e)
        {
            var selected = e.SelectedItem;
            var id = viewModel.Houses.Find(x => x.Address == selected).ID;
            viewModel.HouseID = Convert.ToInt32(id);
            (sender as AutoSuggestBox).Unfocus();
        }

        private void BordlessEditor_TextChanged(object sender, TextChangedEventArgs e)
        {
            viewModel.Street = e.NewTextValue;
        }

        private void BordlessEditor_Unfocused(object sender, FocusEventArgs e)
        {
            viewModel.LoadHouses.Execute(null);
        }

        private void Entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            var flat = (sender as Entry).Text;
            try
            {
                (sender as Entry).Text = flat.Remove(',').Remove('.');

            }
            catch { }
            if (!string.IsNullOrEmpty(flat))
            {
                try
                {
                    viewModel.PremiseID = Convert.ToInt32(flat);

                }
                catch { }
            }
        }

        private void Street_TextChanged(object sender, TextChangedEventArgs e)
        {
            viewModel.Street = (sender as Entry).Text;
        }

        async void Confirm(object sender, EventArgs args)
        {
            MessagingCenter.Send<Object, Tuple<int?, int?, int?, string>>(this, "SetTypes", new Tuple<int?, int?, int?, string>(viewModel.DistrictID, viewModel.HouseID, viewModel.PremiseID, viewModel.Street));
            await Navigation.PopAsync();
        }

        private async void ChooseDistrict(object sender, EventArgs e)
        {
            await PopupNavigation.PushAsync(new SearchDialogView((int)SearchType.DISTRICT));

        }
        private async void ChooseStreet(object sender, EventArgs e)
        {
            await PopupNavigation.PushAsync(new SearchDialogView((int)SearchType.STREET));
        }

        private async void ChooseHouse(object sender, EventArgs e)
        {
            await PopupNavigation.PushAsync(new SearchDialogView((int)SearchType.HOUSE));
        }

        private async void ChoosePremise(object sender, EventArgs e)
        {
            await PopupNavigation.PushAsync(new SearchDialogView((int)SearchType.FLAT, viewModel.HouseID));
        }
    }
}