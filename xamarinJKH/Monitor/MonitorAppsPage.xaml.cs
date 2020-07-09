using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamarinJKH.Apps;
using xamarinJKH.AppsConst;
using xamarinJKH.InterfacesIntegration;
using xamarinJKH.Server;
using xamarinJKH.Server.RequestModel;
using xamarinJKH.Tech;
using xamarinJKH.Utils;

namespace xamarinJKH.Monitor
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MonitorAppsPage : ContentPage
    {
        public List<Requests> RequestInfos { get; set; }
        private RequestList _requestList;
        private RestClientMP _server = new RestClientMP();
        private bool _isRefreshing = false;
        public Color hex { get; set; }

        
        
        public MonitorAppsPage(List<Requests> requestInfos)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    int statusBarHeight = DependencyService.Get<IStatusBar>().GetHeight();
                    Pancake.Padding = new Thickness(0, statusBarHeight, 0, 0);
                    break;
                default:
                    break;
            }
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    BackgroundColor = Color.White;
                    // ImageFon.Margin = new Thickness(0, 0, 0, 0);
                    // StackLayout.Margin = new Thickness(0, 33, 0, 0);
                    // IconViewNameUk.Margin = new Thickness(0, 33, 0, 0);
                    break;
                case Device.Android:
                    // double or = Math.Round(((double)App.ScreenWidth / (double)App.ScreenHeight), 2);
                    // if (Math.Abs(or - 0.5) < 0.02)
                    // {
                    //     ScrollViewContainer.Margin = new Thickness(0, 0, 0, -125);
                    //     BackStackLayout.Margin = new Thickness(5, 25, 0, 0);
                    // }

                    break;
                default:
                    break;
            }
            var techSend = new TapGestureRecognizer();
            techSend.Tapped += async (s, e) => {     await Navigation.PushAsync(new TechSendPage()); };
            LabelTech.GestureRecognizers.Add(techSend);
            var backClick = new TapGestureRecognizer();
            backClick.Tapped += async (s, e) => { _ = await Navigation.PopAsync(); };
            BackStackLayout.GestureRecognizers.Add(backClick);
            hex = Color.FromHex(Settings.MobileSettings.color);
            SetText();
            RequestInfos = requestInfos;
            additionalList.BackgroundColor = Color.Transparent;
            additionalList.Effects.Add(Effect.Resolve("MyEffects.ListViewHighlightEffect"));
            BindingContext = this;
        }
        

        void SetText()
        {
            UkName.Text = Settings.MobileSettings.main_name;
            FormattedString formattedName = new FormattedString();
            formattedName.Spans.Add(new Span
            {
                Text = Settings.Person.FIO,
                TextColor = Color.White,
                FontAttributes = FontAttributes.Bold,
                FontSize = 16
            });
            formattedName.Spans.Add(new Span
            {
                Text = ", добрый день!",
                TextColor = Color.White,
                FontAttributes = FontAttributes.None,
                FontSize = 16
            });
            LabelName.FormattedText = formattedName;
         
        }

        
        
        private async void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            Requests select = e.Item as Requests;
            RequestInfo requestInfo = new RequestInfo()
            {
                ID = select.Number,
                RequestNumber  = select.RequestNumber,
                Added = select.Added.ToString(),
                Name = select.Name,
                Status = select.Status,
                StatusID = select.id_Status,
                IsClosed = !select.IsActive,
                IsPerformed = true
            };
            
            await Navigation.PushAsync(new AppConstPage(requestInfo, false));

        }
        
    }
}