using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

using Xamarin.Forms;
using xamarinJKH.Server.RequestModel;

namespace xamarinJKH.ViewModels.VideoStreamingViewModels
{
    public class CameraListViewModel:BaseViewModel
    {
        public ObservableCollection<CameraModel> Cameras { get; set; }
        public CameraListViewModel()
        {
            Cameras = new ObservableCollection<CameraModel>();
            Cameras.Add(new CameraModel() { Name = "Test camera", Link = "https://vs.domru.ru/translation?id=331972335&guid=dc3b372c7e421f19192a&mode=hls" });
        }
    }
}
