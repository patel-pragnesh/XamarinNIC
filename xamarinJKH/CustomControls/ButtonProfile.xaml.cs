using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace xamarinJKH
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ButtonProfile : Frame
    {
        public Color clr { get; set; }
        public ButtonProfile()
        {
            InitializeComponent();

            clr = (Color)Application.Current.Resources["MainColor"];

            OSAppTheme currentTheme = Application.Current.RequestedTheme;

            if (currentTheme == OSAppTheme.Light || currentTheme == OSAppTheme.Unspecified)
            {
                Device.BeginInvokeOnMainThread(() => { btnSup.BorderColor = clr; });
            }
            else
            {
                Device.BeginInvokeOnMainThread(() => { btnSup.BorderColor = Color.Transparent; });
            }

            MessagingCenter.Subscribe<Object>(this, "ChangeThemeCounter", (sender) =>
            {
                OSAppTheme currentTheme = Application.Current.RequestedTheme;
                var colors = new Dictionary<string, string>();
                if (currentTheme == OSAppTheme.Light || currentTheme == OSAppTheme.Unspecified)
                {
                    colors.Add("#000000", ((Color)Application.Current.Resources["MainColor"]).ToHex());
                    Device.BeginInvokeOnMainThread(() => { btnSup.BorderColor = (Color)Application.Current.Resources["MainColor"]; });
                }
                else
                {
                    colors.Add("#000000", "#FFFFFF");
                    Device.BeginInvokeOnMainThread(() => { btnSup.BorderColor = Color.Transparent;  });

                }

                IconViewProfile.ReplaceStringMap = colors;
            });

            MessagingCenter.Subscribe<Object>(this, "ChangeThemeConst", (sender) =>
            {
                OSAppTheme currentTheme = Application.Current.RequestedTheme;
                var colors = new Dictionary<string, string>();
                if (currentTheme == OSAppTheme.Light || currentTheme == OSAppTheme.Unspecified)
                {
                    colors.Add("#000000", ((Color)Application.Current.Resources["MainColor"]).ToHex());
                    Device.BeginInvokeOnMainThread(() => { btnSup.BorderColor = (Color)Application.Current.Resources["MainColor"]; });
                }
                else
                {
                    colors.Add("#000000", "#FFFFFF");
                    Device.BeginInvokeOnMainThread(() => { btnSup.BorderColor = Color.Transparent; });

                }

                IconViewProfile.ReplaceStringMap = colors;
            });

            this.BindingContext = this;
        }       
    }
}