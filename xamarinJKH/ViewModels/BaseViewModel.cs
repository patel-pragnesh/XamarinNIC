using System;
using System.Collections.Generic;
using System.Text;

using Xamarin.Forms;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using xamarinJKH.Server;

namespace xamarinJKH.ViewModels
{
    public class BaseViewModel:INotifyPropertyChanged
    {
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
        string title;
        public string Title
        {
            get => xamarinJKH.Utils.Settings.MobileSettings.main_name;// title;
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
