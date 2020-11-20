using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Rg.Plugins.Popup.Services;
using xamarinJKH.DialogViews;
using xamarinJKH.InterfacesIntegration;
using xamarinJKH.Tech;

namespace xamarinJKH
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HeaderViewStack : ContentView
    {
        public static BindableProperty TitleProperty = BindableProperty.Create("Title", typeof(string), typeof(string));
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }
        public HeaderViewStack()
        {
            InitializeComponent();

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    int statusBarHeight = DependencyService.Get<IStatusBar>().GetHeight();
                    BackStackLayout.Padding = new Thickness(0, statusBarHeight, 0, 0);
                    
                    break;
                default:
                    break;
            }

            BindingContext = this;
        }

        async void Tech(object sender, EventArgs args)
        {
            await Navigation.PushAsync(new AppPage());
        }

        void Back(object sender, EventArgs args)
        {
            MessagingCenter.Send<HeaderViewStack>(this, "GoBack");
        }
    }
}