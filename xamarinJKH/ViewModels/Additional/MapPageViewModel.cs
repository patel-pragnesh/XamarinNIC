using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using xamarinJKH.Server.RequestModel;
using Xamarin.Forms.Maps;

using Plugin.Permissions;
using Plugin.Permissions.Abstractions;

using Xamarin.Essentials;

namespace xamarinJKH.ViewModels.Additional
{
    public class MapPageViewModel : BaseViewModel
    {
        public ObservableCollection<AdditionalService> Pins { get; set; }
        bool showUser;
        public bool ShowUser
        {
            get => showUser;
            set
            {
                showUser = value;
                OnPropertyChanged(nameof(ShowUser));
            }
        }
        public Command LoadPins { get; set; }
        public Command GetPermission { get; set; }
        public MapPageViewModel()
        {
            Pins = new ObservableCollection<AdditionalService>();
            LoadPins = new Command<ICollection<AdditionalService>>(services =>
            {
                Pins.Clear();
                foreach (var service in services)
                {
                    try
                    {
                        Pins.Add(service);
                    }
                    catch { }
                }
            });

            GetPermission = new Command(async () =>
            {
                PermissionStatus location = PermissionStatus.Denied;
                try
                {
                    location = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
                    if (location != PermissionStatus.Granted)
                    {
                        var status = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);
                        if (status[Permission.Location] != PermissionStatus.Granted)
                        {
                            Device.BeginInvokeOnMainThread(async () =>
                            {
                                var result = await Application.Current.MainPage.DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorNoPermissions, "OK", AppResources.Cancel);
                                if (result)
                                    Plugin.Permissions.CrossPermissions.Current.OpenAppSettings();

                            });
                            return;
                        }
                    }
                }
                catch { }
                finally
                {
                    if (location == PermissionStatus.Granted)
                    {
                        var position = await Geolocation.GetLastKnownLocationAsync();

                        if (position != null)
                        {
                            ShowUser = true;
                            MessagingCenter.Send<MapPageViewModel, Position>(this, "FocusMap", new Position(position.Latitude, position.Longitude));
                        }
                    }
                }
            });
        }
    }
}
