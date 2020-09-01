using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Rg.Plugins.Popup.Services;
using xamarinJKH.DialogViews;

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
            BindingContext = this;
        }

        async void Tech(object sender, EventArgs args)
        {
            await PopupNavigation.Instance.PushAsync(new TechDialog());
        }

        void Back(object sender, EventArgs args)
        {
            MessagingCenter.Send<HeaderViewStack>(this, "GoBack");
        }
    }
}