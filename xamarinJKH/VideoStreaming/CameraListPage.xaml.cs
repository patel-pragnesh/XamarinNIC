using System;
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
            viewModel = new CameraListViewModel();
            viewModel.GoBack = new Command(async () =>
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
            InitializeComponent();
            Analytics.TrackEvent("Список камер");
           
            BindingContext = viewModel;
            HeaderViewStack.BackClick = viewModel.GoBack;
            HeaderViewStack.DarkImage = viewModel.DarkImage;
            HeaderViewStack.LightImage = viewModel.LightImage;
            viewModel.LoadCameras.Execute(null);
        }

        async void CameraSelect(object sender, SelectionChangedEventArgs args)
        {
            try
            {
                var camera = args.CurrentSelection[0] as CameraModel; 
                if (Navigation.ModalStack.FirstOrDefault(x => x is CameraPage) == null)
                    await Navigation.PushModalAsync(new CameraPage(camera.Url));
                (sender as CollectionView).SelectedItem = null;
            }
            catch
            {
            }
        }
    }
}