using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamarinJKH.InterfacesIntegration;
using xamarinJKH.Server;
using xamarinJKH.Server.RequestModel;
using xamarinJKH.Utils;

namespace xamarinJKH.Apps
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AppPage : ContentPage
    {
        private RequestInfo _requestInfo;
        private RequestContent request;

        private RequestList _requestList;
        private RestClientMP _server = new RestClientMP();
        private bool _isRefreshing = false;

        public bool IsRefreshing
        {
            get { return _isRefreshing; }
            set
            {
                _isRefreshing = value;
                OnPropertyChanged(nameof(IsRefreshing));
            }
        }

        public ICommand RefreshCommand
        {
            get
            {
                return new Command(async () =>
                {
                    IsRefreshing = true;

                    await RefreshData();

                    IsRefreshing = false;
                });
            }
        }

        private async Task RefreshData()
        {
            RequestsUpdate requestsUpdate =
                await _server.GetRequestsUpdates(Settings.UpdateKey, _requestInfo.ID.ToString());
            if (requestsUpdate.Error == null)
            {
                Settings.UpdateKey = requestsUpdate.NewUpdateKey;
                if (requestsUpdate.CurrentRequestUpdates != null)
                {
                    Settings.DateUniq = "";
                    request = requestsUpdate.CurrentRequestUpdates;
                    foreach (var each in requestsUpdate.CurrentRequestUpdates.Messages)
                    {
                        messages.Add(each);
                    }

                    additionalList.ItemsSource = null;
                    additionalList.ItemsSource = messages;
                    additionalList.ScrollTo(messages[messages.Count - 1], 0, true);
                }
            }
            else
            {
                await DisplayAlert("Ошибка", "Не удалось получить информацию по комментариям", "OK");
            }

            additionalList.ScrollTo(messages[messages.Count - 1], 0, true);
        }

        public List<RequestMessage> messages { get; set; }
        public Color hex { get; set; }

        public AppPage(RequestInfo requestInfo)
        {
            _requestInfo = requestInfo;
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            var backClick = new TapGestureRecognizer();
            backClick.Tapped += async (s, e) => { _ = await Navigation.PopAsync(); };
            BackStackLayout.GestureRecognizers.Add(backClick);

            var sendMess = new TapGestureRecognizer();
            sendMess.Tapped += async (s, e) => { sendMessage(); };
            IconViewSend.GestureRecognizers.Add(sendMess);
            hex = Color.FromHex(Settings.MobileSettings.color);
            setText();
            getMessage();
        }

        private async void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
        }

        async void sendMessage()
        {
            string message = EntryMess.Text;
            if (!message.Equals(""))
            {
                progress.IsVisible = true;
                IconViewSend.IsVisible = false;
                CommonResult result = await _server.AddMessage(message, _requestInfo.ID.ToString());
                if (result.Error == null)
                {
                    EntryMess.Text = "";
                    await ShowToast("Сообщение отправлено");
                    await RefreshData();
                }
            }
            else
            {
                await ShowToast("Введите текст сообщения");
            }

            progress.IsVisible = false;
            IconViewSend.IsVisible = true;
        }

        private async Task ShowToast(string text)
        {
            if (Device.RuntimePlatform == Device.iOS)
            {
                await DisplayAlert("", text, "OK");
            }
            else
            {
                DependencyService.Get<IMessage>().ShortAlert(text);
            }
        }

        async void getMessage()
        {
            request = await _server.GetRequestsDetailList(_requestInfo.ID.ToString());
            if (request.Error == null)
            {
                Settings.DateUniq = "";
                messages = request.Messages;
                this.BindingContext = this;
                additionalList.ScrollTo(messages[messages.Count - 1], 0, true);
            }
            else
            {
                await DisplayAlert("Ошибка", "Не удалось получить информацию по комментариям", "OK");
            }
        }

        void setText()
        {
            LabelNumber.Text = "№ " + _requestInfo.RequestNumber;
        }
    }
}