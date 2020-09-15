using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace xamarinJKH.Models
{
    public class AppOption:INotifyPropertyChanged
    {
        string image;
        public string Image
        {
            get => image;
            set
            {
                image = value;
                OnPropertyChanged(nameof(Image));
            }
        }

        string name;
        public string Name
        {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged(Name);
            }
        }

        public Command Command { get; set; }
        bool isPaid;
        public bool IsPaid
        {
            get => isPaid;
            set
            {
                isPaid = value;
                OnPropertyChanged(nameof(IsPaid));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public AppOption()
        {
            IsPaid = true;
        }

    }
}
