using System;
using System.Collections.Generic;
using System.Text;

using Xamarin.Forms;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using xamarinJKH.Server;
using AiForms.Dialogs;

using xamarinJKH.Utils;

namespace xamarinJKH.ViewModels
{
    public class BaseViewModel:INotifyPropertyChanged
    {
        bool _isLoading;
        public bool IsLoading
        {
            set
            {
                _isLoading = value;
                if (_isLoading)
                {
                    Loading.Instance.Show(LoadingMessage);
                }
                else
                {
                    Loading.Instance.Hide();
                }
            }
        }
        public string LoadingMessage { get; set; }
        bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged("IsBusy");
            }
        }

        bool isRefreshing;
        public bool IsRefreshing
        {
            get => isRefreshing;
            set
            {
                isRefreshing = value;
                OnPropertyChanged("IsRefreshing");
            }
        }
        string title;
        public string Title
        {
            get => Settings.MobileSettings.main_name;// title;
            //set
            //{
            //    title = value;
            //    OnPropertyChanged("Title");
            //}
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public RestClientMP Server => DependencyService.Get<RestClientMP>(DependencyFetchTarget.NewInstance);
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void ShowError(string error, string title = "Ошибка")
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await Application.Current.MainPage.DisplayAlert(title, error, "OK");
            });
        }
    }
}
