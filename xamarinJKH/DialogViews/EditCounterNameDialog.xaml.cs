using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace xamarinJKH.DialogViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditCounterNameDialog :Rg.Plugins.Popup.Pages.PopupPage
    {
        public Color hex { get; set; }
        public string UniqueNum { get; set; }
        
        public EditCounterNameDialog(Color hexColor, string uniqName)
        {
            hex = hexColor;
            UniqueNum = uniqName;
            InitializeComponent();
            var close = new TapGestureRecognizer();
            close.Tapped += async (s, e) => { await PopupNavigation.Instance.PopAsync(); };
            IconViewClose.GestureRecognizers.Add(close);
            BindingContext = this;
        }
        
    }
}