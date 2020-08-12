using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamarinJKH.Server;
using xamarinJKH.Server.RequestModel;

namespace xamarinJKH.DialogViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SetLsConstDialog : Rg.Plugins.Popup.Pages.PopupPage
    {
        private RestClientMP _server  = new RestClientMP();

        public SetLsConstDialog()
        {
            InitializeComponent();
            var setLss = new TapGestureRecognizer();
            setLss.Tapped += async (s, e) =>
            {
                getHouse();
            };
            StackLayoutHouse.GestureRecognizers.Add(setLss);
        }
        
        void setListHouse(ItemsList<HouseProfile> groups, ref string[] param)
        {
            Houses = new Dictionary<string, string>();
            param = new string [groups.Data.Count];
            int i = 0;
            foreach (var each in groups.Data)
            {
                try
                {
                    if (each.Address != null)
                    {
                        Houses.Add(each.Address, each.ID.ToString());
                        param[i] = each.Address;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                i++;
            }
        }
        async Task getHouse()
        {
            ItemsList<HouseProfile> groups = await _server.GetHouse();
            if (groups.Error == null)
            {
                string[] param = null;
                setListHouse(groups, ref param);
                var action = await DisplayActionSheet(AppResources.HomeChoose, AppResources.Cancel, null, param);
                if (action != null && !action.Equals(AppResources.Cancel))
                {
                    LabelHouse.Text = action;
                }
            }
            else
            {
                await DisplayAlert(AppResources.ErrorTitle, groups.Error, "OK");
            }
        }
        public Dictionary<string, string> Houses { get; set; }
    }
}