using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.FilePicker.Abstractions;
using Rg.Plugins.Popup.Services;
using Syncfusion.SfAutoComplete.XForms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamarinJKH.DialogViews;
using xamarinJKH.Server;
using xamarinJKH.Server.RequestModel;
using xamarinJKH.Tech;
using xamarinJKH.Utils;
using SelectionChangedEventArgs = Syncfusion.SfAutoComplete.XForms.SelectionChangedEventArgs;

namespace xamarinJKH.PushNotification
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SendPushPage : ContentPage
    {
        public bool isRefresh { get; set; }
        public ObservableCollection<NamedValue> groups { get; set; }
        public ObservableCollection<HouseProfile> HouseProfiles { get; set; }

        private List<HouseProfile> DefaultHouses = new List<HouseProfile>();
        private List<HouseProfile> SelecttHouses = new List<HouseProfile>();

        public SendPushPage()
        {
            InitializeComponent();
            getGroups();
            NavigationPage.SetHasNavigationBar(this, false);
            UkName.Text = Settings.MobileSettings.main_name;
            var backClick = new TapGestureRecognizer();
            backClick.Tapped += async (s, e) =>
            {
                try
                {
                    _ = await Navigation.PopAsync();
                }
                catch
                {
                }
            };
            var techSend = new TapGestureRecognizer();
            techSend.Tapped += async (s, e) =>
            {
                if (Settings.Person != null && !string.IsNullOrWhiteSpace(Settings.Person.Phone))
                {
                    await Navigation.PushModalAsync(new AppPage());
                }
                else
                {
                    await PopupNavigation.Instance.PushAsync(new EnterPhoneDialog());
                }
            };
            LabelTech.GestureRecognizers.Add(techSend);

            var kind = new TapGestureRecognizer();
            kind.Tapped += async (s, e) =>
            {
                var action = await DisplayActionSheet(AppResources.AreaChoose, AppResources.Cancel, null,
                    AppResources.ToRayon, AppResources.ByHome, AppResources.ByLS, AppResources.ByDuty);
                if (action != null && !action.Equals(AppResources.Cancel))
                {
                    LabelKind.Text = action;

                    if (action.Contains(AppResources.ToRayon))
                    {
                        LayoutRayon.IsVisible = true;
                        LayoutHouses.IsVisible = false;
                        LayoutLs.IsVisible = false;
                        LayoutDebt.IsVisible = false;
                        autoCompleteHouses.Clear();
                        SelecttHouses.Clear();
                        EntryLS.Text = "";
                        EntryDebt.Text = "";
                        autoCompleteHouses.EnableAutoSize = false;
                        Device.BeginInvokeOnMainThread(() => autoComplete.Focus());
                    }
                    else if (action.Contains(AppResources.ByHome))
                    {
                        LayoutRayon.IsVisible = false;
                        LayoutHouses.IsVisible = true;
                        LayoutLs.IsVisible = false;
                        LayoutDebt.IsVisible = false;
                        autoComplete.Clear();
                        _selectedGroupId = -1;
                        EntryLS.Text = "";
                        EntryDebt.Text = "";
                        autoCompleteHouses.EnableAutoSize = true;
                        Device.BeginInvokeOnMainThread(() => autoCompleteHouses.Focus());
                    }else if (action.Contains(AppResources.ByLS))
                    {
                        LayoutLs.IsVisible = true;
                        LayoutRayon.IsVisible = false;
                        LayoutHouses.IsVisible = false;
                        LayoutDebt.IsVisible = false;
                        EntryLS.Focus();
                        autoComplete.Clear();
                        autoCompleteHouses.Clear();
                        SelecttHouses.Clear();
                        _selectedGroupId = -1;
                        autoCompleteHouses.EnableAutoSize = false;
                        EntryDebt.Text = "";
                    }else if (action.Contains(AppResources.ByDuty))
                    {
                        LayoutDebt.IsVisible = true;
                        LayoutLs.IsVisible = false;
                        LayoutRayon.IsVisible = false;
                        LayoutHouses.IsVisible = false;
                        autoComplete.Clear();
                        autoCompleteHouses.Clear();
                        EntryDebt.Focus();
                        SelecttHouses.Clear();
                        autoCompleteHouses.EnableAutoSize = false;
                        _selectedGroupId = -1;
                        EntryLS.Text = "";
                    }
                }
            };
            StackLayoutKind.GestureRecognizers.Add(kind);
            BackStackLayout.GestureRecognizers.Add(backClick);
        }

        Thickness frameMargin = new Thickness();

        private void BordlessEditor_Focused(object sender, FocusEventArgs e)
        {
            if (Xamarin.Essentials.DeviceDisplay.MainDisplayInfo.Width < 800)
            {
                frameMargin = Frame.Margin;
                Device.BeginInvokeOnMainThread(() => { Frame.Margin = new Thickness(15, 0, 15, 15); });
            }
        }

        private RestClientMP _server = new RestClientMP();

        async void getGroups()
        {
            ItemsList<NamedValue> result = await _server.GetHouseGroups();
            ItemsList<HouseProfile> resultHouse = await _server.GetHouse();
            groups = new ObservableCollection<NamedValue>(result.Data);
            HouseProfiles = new ObservableCollection<HouseProfile>(resultHouse.Data);
            DefaultHouses = resultHouse.Data;
            BindingContext = this;
        }

        private void BordlessEditor_Unfocused(object sender, FocusEventArgs e)
        {
            if (Xamarin.Essentials.DeviceDisplay.MainDisplayInfo.Width < 800)
            {
                Device.BeginInvokeOnMainThread(() => { Frame.Margin = frameMargin; });
            }
        }

        private void CheckBox_OnCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
        }

        private int _selectedGroupId = -1;

        private void AutoComplete_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            NamedValue select = e.Value as NamedValue;
            if (@select != null)
            {
                _selectedGroupId = @select.ID;
            }
            else
            {
                _selectedGroupId = -1;
            }
        }

        private void AutoCompleteHouses_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Value != null && e.RemovedItems == null)
            {
                SelecttHouses.Clear();
               
                foreach (HouseProfile each in (IEnumerable) autoCompleteHouses.SelectedItem)
                {
                    SelecttHouses.Add(each);
                }
            }
            else
            {
                HouseProfile rem = e.RemovedItems as HouseProfile;
                if (rem != null)
                {
                    SelecttHouses.Remove(rem);
                }
                else
                {
                    foreach (HouseProfile each in (IEnumerable) e.RemovedItems)
                    {
                        SelecttHouses.Remove(each);
                    }
                }

            }
        }

        private void AutoCompleteHouses_OnFocusChanged(object sender, FocusChangedEventArgs e)
        {
            // autoCompleteHouses.HeightRequest =  GetHeight(e.HasFocus);
        }

        public int GetHeight(bool value)
        {
            if (value)
            {
                return 80;
            }

            return 40;
        }

        private void addApp(object sender, EventArgs e)
        {
            
        }
    }
}