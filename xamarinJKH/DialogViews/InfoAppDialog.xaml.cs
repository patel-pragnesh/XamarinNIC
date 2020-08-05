using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AiForms.Dialogs.Abstractions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamarinJKH.Server.RequestModel;
using xamarinJKH.Utils;

namespace xamarinJKH.DialogViews
{
    public partial class InfoAppDialog : DialogView
    {
        public double width { get; set; }
        public Color HexColor { get; set; }
        public string SourceApp { get; set; }
        public RequestInfo _Request { get; set; }

        public InfoAppDialog()
        {
            InitializeComponent();
            code.IsVisible = !Settings.Person.IsDispatcher;
            Frame.SetAppThemeColor(Frame.BorderColorProperty, Color.FromHex(Settings.MobileSettings.color), Color.Transparent);
            switch (Device.RuntimePlatform)
            {
                case Device.Android:
                    double or = Math.Round(((double) App.ScreenWidth / (double) App.ScreenHeight), 2);
                    if (Math.Abs(or - 0.5) < 0.02)
                    {
                        Frame.Margin = new Thickness(15,113,15,15); 
                    }else if (Math.Abs(or - 0.55) < 0.02)
                    {
                        Frame.Margin = new Thickness(15,100,15,15); 
                    }
                    break;
            }

            this.BindingContext = this;
        }


        public override void SetUp()
        {
            // called each opening dialog
        }

        public override void TearDown()
        {
        }

        public override void RunPresentationAnimation()
        {
            // define opening animation
        }

        public override void RunDismissalAnimation()
        {
            // define closing animation
        }

        public override void Destroy()
        {
            // define clean up process.
        }

        void Handle_OK_Clicked(object sender, System.EventArgs e)
        {
            // send complete notification to the dialog.
            DialogNotifier.Complete();
        }

        void Handle_Cancel_Clicked(object sender, System.EventArgs e)
        {
            // send cancel notification to the dialog.
            DialogNotifier.Cancel();
        }
    }
}