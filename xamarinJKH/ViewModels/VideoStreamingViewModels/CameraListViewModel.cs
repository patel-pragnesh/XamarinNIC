﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using xamarinJKH.Server.RequestModel;

namespace xamarinJKH.ViewModels.VideoStreamingViewModels
{
    public class CameraListViewModel:BaseViewModel
    {
        public ObservableCollection<CameraModel> Cameras { get; set; }
        public Command LoadCameras { get; set; }
        public CameraListViewModel()
        {
            Cameras = new ObservableCollection<CameraModel>();
            LoadCameras = new Command(() =>
            {
                Task.Run(async () =>
                {
                    var cameras = await Server.GetCameras();
                    if (cameras.Error == null)
                    {
                        foreach (var camera in cameras.Data)
                        {
                            if (!string.IsNullOrEmpty(camera.Url.Trim()))
                                Device.BeginInvokeOnMainThread(() => Cameras.Add(camera));
                        }
                    }
                });
            });
            //Cameras.Add(new CameraModel() { Name = "Test camera", Url = "https://vs.domru.ru/translation?id=331972335&guid=dc3b372c7e421f19192a&mode=hls" });
        }
    }
}
