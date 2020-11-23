using System;
using System.Collections.Generic;
using System.Text;

using Xamarin.Forms;
using xamarinJKH.Server.RequestModel;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using xamarinJKH.Utils;
using System.Linq;
using System.Runtime.CompilerServices;

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
        //public Color hex { get; set; } = (Color)Application.Current.Resources["MainColor"];
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

        bool empty;
        public bool Empty
        {
            get => empty;
            set
            {
                empty = value;
                OnPropertyChanged("Empty");
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
                    Device.BeginInvokeOnMainThread(() => {
                        if (response.Requests != null)
                        {
                            MessagingCenter.Send<Object, int>(this, "SetRequestsAmount", response.Requests.Where(x => !x.IsReadedByClient).Count());
                            AllRequests.AddRange(response.Requests);
                            if (Requests == null)
                            {
                                Empty = Requests.Count == 0;
                                Requests = new ObservableCollection<RequestInfo>();
                            }
                            Requests.Clear();
                            foreach (var App in AllRequests.Where(x => x.IsClosed == ShowClosed))
                            {
                                Device.BeginInvokeOnMainThread(() => Requests.Add(App));
                            }
                        }

                        MessagingCenter.Subscribe<Object, string>(this, "AddIdent", (sender, args) => LoadRequests.Execute(null));
                        MessagingCenter.Send<Object>(this, "EndRefresh");

                    });
                    //if (response.Requests != null)
                    //{
                    //    MessagingCenter.Send<Object, int>(this, "SetRequestsAmount", response.Requests.Where(x => !x.IsReadedByClient).Count());
                    //    AllRequests.AddRange(response.Requests);
                    //    if (Requests == null)
                    //    {
                    //        Empty = Requests.Count == 0;
                    //        Requests = new ObservableCollection<RequestInfo>();
                    //    }
                    //    Requests.Clear();
                    //    foreach (var App in AllRequests.Where(x => x.IsClosed == ShowClosed))
                    //    {
                    //        Device.BeginInvokeOnMainThread(() => Requests.Add(App));
                    //    }
                    //}
                }

                //MessagingCenter.Subscribe<Object, string>(this, "AddIdent", (sender, args) => LoadRequests.Execute(null));
                //MessagingCenter.Send<Object>(this, "EndRefresh");
            });
        }

        public async Task UpdateTask()
        {
            var response = await Server.GetRequestsList();
            if (response.Error == null)
            {
                MessagingCenter.Send<Object, int>(this, "SetRequestsAmount", response.Requests.Where(x => !x.IsReadedByClient).Count());
                if (AllRequests != null)
                {
                    Empty = Requests.Count == 0;
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
