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
    public class AppsPageViewModel:BaseViewModel
    {
        public ObservableCollection<RequestInfo> Requests { get; set; }
        public List<RequestInfo> AllRequests { get; set; }
        public Command LoadRequests { get; set; }
        public Command UpdateRequests { get; set; }
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
                        Requests.Clear();
                        foreach (var App in AllRequests.Where(x => x.IsClosed).ToList())
                        {
                            Requests.Add(App);
                        }
                    }
                }
                else
                {
                    if (AllRequests != null)
                    {
                        Requests.Clear();
                        foreach (var App in AllRequests.Where(x => !x.IsClosed).ToList())
                        {
                            Requests.Add(App);
                        }
                    }
                }
            }
        }
        public AppsPageViewModel()
        {
            Requests = new ObservableCollection<RequestInfo>();
            LoadRequests = new Command(() =>
            {
                Task.Run(async () =>
                {
                    var response = await Server.GetRequestsList();
                    AllRequests = new List<RequestInfo>();
                    AllRequests.AddRange(response.Requests);
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
                            foreach (var App in response.Requests)
                            {
                                Device.BeginInvokeOnMainThread(() => Requests.Add(App));
                            }
                        }
                    }
                });
            });
            
        }
    }
}
