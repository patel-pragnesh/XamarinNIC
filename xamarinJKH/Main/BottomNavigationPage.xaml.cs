using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamarinJKH.Utils;

namespace xamarinJKH.Main
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BottomNavigationPage : TabbedPage
    {
        public BottomNavigationPage()
        {

            InitializeComponent ();
            NavigationPage.SetHasNavigationBar(this, false);
            SelectedTabColor = Color.FromHex(Utils.Settings.MobileSettings.color);
            UnselectedTabColor = Color.Gray;
            CheckAccounts();
        }

        public async void CheckAccounts()
        {
            if (Settings.Person.Accounts.Count == 0)
            {
                await AiForms.Dialogs.Dialog.Instance.ShowAsync<xamarinJKH.DialogViews.AddAccountDialogView>();
            }
        }
    }
}