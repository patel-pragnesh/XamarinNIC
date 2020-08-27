﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamarinJKH.Utils;
using Rg.Plugins.Popup.Services;
using Plugin.Messaging;
using xamarinJKH.DialogViews;

namespace xamarinJKH
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HeaderView : ContentView
    {
        public static BindableProperty HeaderPicture = BindableProperty.Create("HeaderPicture", typeof(string), typeof(string));
        public static BindableProperty ShowBackButton = BindableProperty.Create("ShowBackButton", typeof(bool), typeof(bool));
        public static BindableProperty TitleProperty = BindableProperty.Create("TitleProperty", typeof(string), typeof(string));
        public string Picture
        {
            get => (string)GetValue(HeaderPicture) + (Application.Current.UserAppTheme == OSAppTheme.Dark || Application.Current.UserAppTheme == OSAppTheme.Unspecified ? string.Empty : "_light");
            set
            {
                SetValue(HeaderPicture, value);
                OnPropertyChanged("Picture");
            }
        }

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set
            {
                SetValue(TitleProperty, value);
                OnPropertyChanged("Title");
            }
        }

        public bool ShowBack
        {
            get => (bool)GetValue(ShowBackButton);
            set
            {
                SetValue(ShowBackButton, value);
                OnPropertyChanged("ShowBack");
            }
        }

        public string Phone
        {
            get => Settings.Person.companyPhone.Replace("+", "");
        }

        public string Company
        {
            get => Settings.MobileSettings.main_name;
        }

        public HeaderView()
        {
            InitializeComponent();
            BindingContext = this;

            MessagingCenter.Subscribe<Object>(this, "ChangeTheme", (sender) =>
            {
                if (Application.Current.UserAppTheme == OSAppTheme.Dark || Application.Current.UserAppTheme == OSAppTheme.Unspecified)
                {
                    Picture = Picture.Replace("_light", "");
                }
                else
                {
                    Picture = Picture.Replace("_light", "");
                    Picture += "_light";
                }
            });
        }

        protected async void GoBack(object sender, EventArgs args)
        {
            var page = this.Parent.Parent;
            await (page as ContentPage).Navigation.PopAsync();
        }

        private void Call(object sender, EventArgs args)
        {
            if (Settings.Person.Phone != null)
            {
                IPhoneCallTask phoneDialer;
                phoneDialer = CrossMessaging.Current.PhoneDialer;
                if (phoneDialer.CanMakePhoneCall)
                    phoneDialer.MakePhoneCall(Settings.Person.companyPhone);
            }
        }

        private async void Tech(object sender, EventArgs args)
        {
            await PopupNavigation.Instance.PushAsync(new TechDialog());
        }
    }
}