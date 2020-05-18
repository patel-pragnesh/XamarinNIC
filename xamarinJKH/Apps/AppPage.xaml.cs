using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamarinJKH.Server;
using xamarinJKH.Server.RequestModel;

namespace xamarinJKH.Apps
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AppPage : ContentPage
    {
        private RequestInfo _requestInfo;
        private RequestContent request;
        private RestClientMP _server = new RestClientMP();
        
        public List<RequestMessage>  messages { get; set; }
        public AppPage(RequestInfo requestInfo)
        {
            _requestInfo = requestInfo;
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            var backClick = new TapGestureRecognizer();
            backClick.Tapped += async (s, e) => { _ = await Navigation.PopAsync(); };
            BackStackLayout.GestureRecognizers.Add(backClick);
            setText();
            getMessage();
        }

        private void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            
        }

        async void getMessage()
        {
            request  = await _server.GetRequestsDetailList(_requestInfo.ID.ToString());
            if (request.Error == null)
            {
                messages = request.Messages;
                this.BindingContext = this;
            }
            else
            {
                await DisplayAlert("Ошибка", "Не удалось получить информацию о начислениях", "OK");
            }
        }
        
        void setText()
        {
            LabelNumber.Text = "№ " + _requestInfo.RequestNumber;
        }
    }
}