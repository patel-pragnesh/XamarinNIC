using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
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

        public static BindableProperty DarkProperty =
            BindableProperty.Create("DarkImage", typeof(string), typeof(string));

        public static BindableProperty LightProperty =
            BindableProperty.Create("LightImage", typeof(string), typeof(string));

        public static BindableProperty GoBackProperty =
            BindableProperty.Create("BackClick", typeof(Command), typeof(Command));

        public string Title
        {
            get => (string) GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public string DarkImage
        {
            get => (string) GetValue(DarkProperty);
            set
            {
                SetValue(DarkProperty, value);
                ImageTop.SetOnAppTheme(Image.SourceProperty, LightImage, value);
            }
        }

        public string LightImage
        {
            get => (string) GetValue(LightProperty);
            set
            {
                SetValue(LightProperty, value);
                ImageTop.SetOnAppTheme(Image.SourceProperty, value, DarkImage);
            }
        }


        public Command BackClick
        {
            get => (Command) GetValue(GoBackProperty);
            set => SetValue(GoBackProperty, value);
        }

        public Command Update { get; set; }

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
            // MessagingCenter.Send<HeaderViewStack>(this, "GoBack");
            if (BackClick != null)
                BackClick.Execute(null);
        }
    }
}