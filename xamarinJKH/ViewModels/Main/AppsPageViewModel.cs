using System;
using System.Collections.Generic;
using System.Text;

using Xamarin.Forms;
using xamarinJKH.Server.RequestModel;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using xamarinJKH.Utils;
using System.Linq;

namespace xamarinJKH.ViewModels.Main
{
    public class AppsPageViewModel : BaseViewModel
    {
        ObservableCollection<RequestInfo> _requests;
        public ObservableCollection<RequestInfo> Requests
        {
            get => _requests;
            set
            {
                if (value != null) _requests = value;
                OnPropertyChanged("Requests");
            }
        }
        public List<RequestInfo> AllRequests { get; set; }
        public Command LoadRequests { get; set; }
        public Command UpdateRequests { get; set; }
        public Color hex { get; set; } = Color.FromHex(Settings.MobileSettings.color);
        bool _showClosed;
        public bool ShowClosed
        {
            get => _showClosed;
            set
            {
                _showClosed = value;
                if (_showClosed)
                {
                    if (AllRequests != null)
                    {
                        Requests = new ObservableCollection<RequestInfo>();
                        foreach (var App in AllRequests.Where(x => x.IsClosed).ToList())
                        {
                            Device.BeginInvokeOnMainThread(() => Requests.Add(App));
                        }
                    }
                }
                else
                {
                    if (AllRequests != null)
                    {
                        Requests = new ObservableCollection<RequestInfo>();
                        foreach (var App in AllRequests.Where(x => !x.IsClosed).ToList())
                        {
                            Device.BeginInvokeOnMainThread(() => Requests.Add(App));
                        }
                    }
                }
                OnPropertyChanged("ShowClosed");
            }
        }
        public AppsPageViewModel()
        {
            Requests = new ObservableCollection<RequestInfo>();
            LoadRequests = new Command(async () =>
            {
                var response = await Server.GetRequestsList();
                AllRequests = new List<RequestInfo>();
                if (response.Error != null)
                {
                    ShowError(response.Error);
                    return;
                }
                else
                {
                    if (Settings.UpdateKey != response.UpdateKey)
                        Settings.UpdateKey = response.UpdateKey;
                    if (response.Requests != null)
                    {
                        AllRequests.AddRange(response.Requests);
                        if (Requests == null) Requests = new ObservableCollection<RequestInfo>();
                        Requests.Clear();
                        foreach (var App in AllRequests.Where(x => x.IsClosed == ShowClosed))
                        {
                            Device.BeginInvokeOnMainThread(() => Requests.Add(App));
                        }
                    }
                }
            });
        }

        public async Task UpdateTask()
        {
            var response = await Server.GetRequestsList();
            if (response.Error == null)
            {
                if (AllRequests != null)
                {
                    var ids = AllRequests.Select(x => x.ID);
                    var newRequests = response.Requests.Where(x => !ids.Contains(x.ID)).ToList();
                    foreach (var newApp in newRequests)
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            AllRequests.Insert(0, newApp);
                            Requests.Insert(0, newApp);
                        });
                    }
                }

            }
        }
    }
}
