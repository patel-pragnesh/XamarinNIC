using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            BindingContext = viewModel = new CameraListViewModel();

            MessagingCenter.Subscribe<HeaderViewStack>(this, "GoBack", sender =>
            {
                if (Application.Current.MainPage.Navigation.ModalStack.Count > 1)
                {

                    Navigation.PopModalAsync();
                }
                else
                {

                    Navigation.PopAsync();
                }
            });
            viewModel.LoadCameras.Execute(null);
        }

        async void CameraSelect(object sender, SelectionChangedEventArgs args)
        {
            try
            {
                var camera = args.CurrentSelection[0] as CameraModel;
                await Navigation.PushModalAsync(new CameraPage(camera.Url));
                (sender as CollectionView).SelectedItem = null;
            }
            catch
            {

            }
        }
    }
}