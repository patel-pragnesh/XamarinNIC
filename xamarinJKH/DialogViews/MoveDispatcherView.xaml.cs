using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AiForms.Dialogs;
using AiForms.Dialogs.Abstractions;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamarinJKH.Server;
using xamarinJKH.Server.RequestModel;

namespace xamarinJKH.DialogViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MoveDispatcherView : Rg.Plugins.Popup.Pages.PopupPage
    {
        private RestClientMP server = new RestClientMP();
        public Color HexColor { get; set; }
        public RequestInfo _Request { get; set; }
        bool IsConst = false;
        public int PikerDispItem = 0;
        List<NamedValue> dispList { get; set; }

        public MoveDispatcherView(Color hexColor, RequestInfo request, bool isConst)
        {
            HexColor = hexColor;
            _Request = request;
            InitializeComponent();
            IsConst = isConst;
            getDispatcherList();
            var close = new TapGestureRecognizer();
            close.Tapped += async (s, e) => { await PopupNavigation.Instance.PopAsync(); };
            IconViewClose.GestureRecognizers.Add(close);
        }

        async void getDispatcherList()
        {
            ItemsList<NamedValue> result = await server.GetDispatcherList();
            dispList = result.Data;
            BindingContext = null;
            BindingContext = new DispListModel()
            {
                AllDisp = dispList,
                hex = HexColor,
                SelectedDisp = dispList[0]
            };
        }

        private async void CloseApp(object sender, EventArgs e)
        {
            await StartProgressBar();
        }

        public async Task ShowToast(string title)
        {
            Toast.Instance.Show<ToastDialog>(new { Title = title, Duration = 1500 });
            // Optionally, view model can be passed to the toast view instance.
        }

        public class DispListModel
        {
            public List<NamedValue> AllDisp { get; set; }
            public NamedValue SelectedDisp { get; set; }
            public Color hex { get; set; }
        }

        public async Task StartProgressBar(string title = "", double opacity = 0.6)
        {
            // Loading settings
            if (string.IsNullOrEmpty(title))
                 title = AppResources.MoveDispatcherStatus;
            Configurations.LoadingConfig = new LoadingConfig
            {
                IndicatorColor = HexColor,
                OverlayColor = Color.Black,
                Opacity = opacity,
                DefaultMessage = title,
            };

            await Loading.Instance.StartAsync(async progress =>
            {
                string dispId = dispList[PickerDisp.SelectedIndex].ID.ToString();
                CommonResult result = await server.ChangeDispatcherConst(_Request.ID.ToString(), dispId);
                if (result.Error == null)
                {
                    await ShowToast(AppResources.MoveDispatcherSuccess);
                    await PopupNavigation.Instance.PopAsync();
                }
                else
                {
                    await ShowToast(result.Error);
                }
            });
        }

        private void pickerDisp_SelectedIndexChanged(object sender, EventArgs e)
        {
            // try
            // {
            //     var identLength = Settings.TypeApp[PickerType.SelectedIndex].Name.Length;
            //     if (identLength < 6)
            //     {
            //         PickerType.WidthRequest = identLength * 10;
            //     }
            // }
            // catch (Exception ex)
            // {
            //     // ignored
            // }
        }
    }
}