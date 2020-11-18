﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AppCenter.Analytics;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamarinJKH.Server.RequestModel;
using xamarinJKH.ViewModels.VideoStreamingViewModels;

namespace xamarinJKH.VideoStreaming
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CameraListPage : ContentPage
    {
        CameraListViewModel viewModel;
        public CameraListPage()
        {
            InitializeComponent();
            Analytics.TrackEvent("Список камер");
            BindingContext = viewModel = new CameraListViewModel();

            MessagingCenter.Subscribe<HeaderViewStack>(this, "GoBack", async sender =>
            {
                if (Application.Current.MainPage.Navigation.ModalStack.Count > 1)
                {

                    await Navigation.PopModalAsync();
                }
                else
                {

                    await Navigation.PopAsync();
                }
            });
            viewModel.LoadCameras.Execute(null);
        }

        async void CameraSelect(object sender, SelectionChangedEventArgs args)
        {
            try
            {
                var camera = args.CurrentSelection[0] as CameraModel; 
                if (Navigation.ModalStack.FirstOrDefault(x => x is CameraPage) == null)
                    if (camera != null)
                        await Navigation.PushModalAsync(new CameraPage(camera.Url, camera.Address));
                (sender as CollectionView).SelectedItem = null;
            }
            catch
            {

            }
        }
    }
}