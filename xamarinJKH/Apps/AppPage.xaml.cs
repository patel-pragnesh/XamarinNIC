using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
using Xamarin.Essentials;
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

        public bool close = false;

        public AppPage(RequestInfo requestInfo, bool closeAll = false)
        {
            close = closeAll;
            _requestInfo = requestInfo;
            InitializeComponent();
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    ImageTop.Margin = new Thickness(0, 33, 0, 0);
                    StackLayout.Margin = new Thickness(0, 33, 0, 0);
                    // IconViewNameUk.Margin = new Thickness(0, 33, 0, 0);
                    break;
                case Device.Android:
                default:
                    break;
            }
            NavigationPage.SetHasNavigationBar(this, false);
            var backClick = new TapGestureRecognizer();
            backClick.Tapped += async (s, e) =>
            {
                if (close)
                {
                    await Navigation.PopToRootAsync();
                }
                else
                {
                    _ = await Navigation.PopAsync();
                }
            };
            BackStackLayout.GestureRecognizers.Add(backClick);

            var sendMess = new TapGestureRecognizer();
            sendMess.Tapped += async (s, e) => { sendMessage(); };
            IconViewSend.GestureRecognizers.Add(sendMess);
            var addFile = new TapGestureRecognizer();
            addFile.Tapped += async (s, e) => { addFileApp(); };
            IconViewAddFile.GestureRecognizers.Add(addFile);
            hex = Color.FromHex(Settings.MobileSettings.color);
            setText();
            getMessage();
        }

        protected override bool OnBackButtonPressed()
        {
            if (close)
            {
                Navigation.PopToRootAsync();
                return true;
            }
            else
            {
                return base.OnBackButtonPressed();
            }
        }

        private async void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
        }

        async void addFileApp()
        {
            PickAndShowFile(null);
        }

        private async Task PickAndShowFile(string[] fileTypes)
        {
            try
            {
                IconViewAddFile.IsVisible = false;
                progressFile.IsVisible = true;
                FileData pickedFile = await CrossFilePicker.Current.PickFile(fileTypes);

                if (pickedFile != null)
                {
                    // UkName.Text = pickedFile.FileName;
                    // LabelPhone.Text = pickedFile.FilePath;
                    if (pickedFile.DataArray.Length > 10000000)
                    {
                        await DisplayAlert("Ошибка", "Размер файла превышает 10мб", "OK");
                        return;
                    }

                    
                  
                    
                    CommonResult commonResult = await _server.AddFileApps(_requestInfo.ID.ToString(),
                        pickedFile.FileName, pickedFile.DataArray,
                        pickedFile.FilePath);
                    if (commonResult == null)
                    {
                        await ShowToast("Файл отправлен");
                        await RefreshData();
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", ex.ToString(), "OK");
            }

            IconViewAddFile.IsVisible = true;
            progressFile.IsVisible = false;
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
                LabelNumber.Text = "№ " + request.RequestNumber;
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
            try
            {
                LabelNumber.Text = "№ " + _requestInfo.RequestNumber;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}