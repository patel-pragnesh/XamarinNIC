using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AiForms.Dialogs;
using AiForms.Dialogs.Abstractions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamarinJKH.Utils;

namespace xamarinJKH.DialogViews
{
    public partial class RatingBarView : DialogView
    {
        public Color HexColor { get; set; }

        public RatingBarView()
        {
            InitializeComponent();
            
            var close = new TapGestureRecognizer();
            close.Tapped += async (s, e) =>
            {
                DialogNotifier.Cancel();
            };
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    RatingBar.Margin = new Thickness(0, 0, 0, 0);
                    RatingBar.FillColor = Color.FromHex(Settings.MobileSettings.color);
                    RatingBar.HeightRequest = 35;
                    break;
                case Device.Android:
                    break;
                default:
                    break;
            }
            IconViewClose.GestureRecognizers.Add(close);
        }
        
        
        private void CloseApp(object sender, EventArgs e)
        {
        }

        public override void SetUp()
        {
        }
        
        public override void TearDown()
        {
            Loading.Instance.Hide();
        }
        
        
    }
}