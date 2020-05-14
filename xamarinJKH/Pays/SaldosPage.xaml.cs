using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamarinJKH.Server.RequestModel;
using xamarinJKH.Utils;

namespace xamarinJKH.Pays
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SaldosPage : ContentPage
    {
        public List<BillInfo> BillInfos { get; set; }
        public SaldosPage(List<AccountAccountingInfo> infos)
        {
            SetBills(infos);
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            var backClick = new TapGestureRecognizer();
            backClick.Tapped += async (s, e) => { _ = await Navigation.PopAsync(); };
            BackStackLayout.GestureRecognizers.Add(backClick);
            SetText();
            this.BindingContext = this;
            
        }

        void SetBills(List<AccountAccountingInfo> infos)
        {
            BillInfos = new List<BillInfo>();
            foreach (var each in infos)
            {
                foreach (var VARIABLE in each.Bills)
                {
                    BillInfos.Add(VARIABLE);
                }
                
            }
        }
        
        void SetText()
        {
            UkName.Text = Settings.MobileSettings.main_name;
            LabelPhone.Text = "+" + Settings.Person.Phone;
        }

        private void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            
        }
    }
}