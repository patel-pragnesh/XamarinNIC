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