using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AiForms.Dialogs;
using AiForms.Dialogs.Abstractions;
using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Messaging;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Rg.Plugins.Popup.Services;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamarinJKH.DialogViews;
using xamarinJKH.InterfacesIntegration;
using xamarinJKH.Server;
using xamarinJKH.Server.RequestModel;
using xamarinJKH.Utils;
using PermissionStatus = Plugin.Permissions.Abstractions.PermissionStatus;

namespace xamarinJKH.AppsConst
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AppConstPage : ContentPage
    {
        private RequestInfo _requestInfo;
        private RequestContent request;

        private RequestList _requestList;
        private RestClientMP _server = new RestClientMP();
        private bool _isRefreshing = false;
        const string TAKE_PHOTO = "Сделать фото";
        const string TAKE_GALRY = "Выбрать фото из галереи";
        const string TAKE_FILE = "Выбрать файл";
        const string CAMERA = "camera";
        const string GALERY = "galery";
        const string FILE = "file";
        bool isPaid;
        public bool IsRequestPaid
        {
            get => isPaid;
            set
            {
                isPaid = value;
                OnPropertyChanged(nameof(IsRequestPaid));
            }
        }

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

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            if (Device.RuntimePlatform == "Android")
            {
                var camera_perm = await Plugin.Permissions.CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
                if (camera_perm != PermissionStatus.Granted)
                {
                    await CrossPermissions.Current.RequestPermissionsAsync(Permission.Camera, Permission.Storage);
                }

                var file_perm = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);
                if (file_perm != PermissionStatus.Granted)
                {
                    await CrossPermissions.Current.RequestPermissionsAsync(Permission.Storage);
                }

            }
        }

        private async Task RefreshData()
        {
            RequestsUpdate requestsUpdate =
                await _server.GetRequestsUpdatesConst(Settings.UpdateKey, _requestInfo.ID.ToString());
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
        public bool isNotRead { get; set; }

        public AppConstPage(RequestInfo requestInfo, bool isNotRead = true, bool closeAll = false)
        {
            close = closeAll;
            _requestInfo = requestInfo;
            this.isNotRead = isNotRead;
            InitializeComponent();
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    // ImageTop.Margin = new Thickness(0, 33, 0, 0);
                    // StackLayout.Margin = new Thickness(0, 33, 0, 0);
                    // IconViewNameUk.Margin = new Thickness(0, 33, 0, 0);
                    break;
                case Device.Android:
                    double or = Math.Round(((double)App.ScreenWidth / (double)App.ScreenHeight), 2);
                    if (Math.Abs(or - 0.5) < 0.02)
                    {
                        ScrollViewContainer.Margin = new Thickness(0, 0, 0, -150);
                    }

                    break;
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
                    Settings.isSelf = null;
                    Settings.DateUniq = "";
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
            var showInfo = new TapGestureRecognizer();
            showInfo.Tapped += async (s, e) => { ShowInfo(); };
            StackLayoutInfo.GestureRecognizers.Add(showInfo);
            var acceptAp = new TapGestureRecognizer();
            acceptAp.Tapped += async (s, e) => { acceptApp(); };
            StackLayoutAccept.GestureRecognizers.Add(acceptAp);
            var performAp = new TapGestureRecognizer();
            performAp.Tapped += async (s, e) => { performApp(); };
            StackLayoutExecute.GestureRecognizers.Add(performAp);
            var moveDisp = new TapGestureRecognizer();
            moveDisp.Tapped += async (s, e) =>
            {
                // await ShowRating();
                await PopupNavigation.Instance.PushAsync(new MoveDispatcherView(hex, _requestInfo, true));
                await RefreshData();
            };
            StackLayoutMoveDisp.GestureRecognizers.Add(moveDisp);
            var closeApp = new TapGestureRecognizer();
            closeApp.Tapped += async (s, e) =>
            {
                // await ShowRating();
                await PopupNavigation.Instance.PushAsync(new RatingBarContentView(hex, _requestInfo, true));
                await RefreshData();
            };
            StackLayoutClose.GestureRecognizers.Add(closeApp);
            hex = Color.FromHex(Settings.MobileSettings.color);

            setText();
            getMessage();
            additionalList.Effects.Add(Effect.Resolve("MyEffects.ListViewHighlightEffect"));
            MessagingCenter.Subscribe<Object>(this, "RefreshApp", (sender) => RefreshCommand.Execute(null));
        }

        protected async void SendCode(object sender, EventArgs args)
        {
            await AiForms.Dialogs.Dialog.Instance.ShowAsync(new EnterCodeDialogView(this._requestInfo.ID.ToString()));
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
                Settings.isSelf = null;
                Settings.DateUniq = "";
                return base.OnBackButtonPressed();
            }
        }

        private async void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            RequestMessage select = e.Item as RequestMessage;
            if (@select != null && @select.FileID != -1)
            {
                string fileName = FileName(@select.Text);
                if (await DependencyService.Get<IFileWorker>().ExistsAsync(fileName))
                {
                    await Launcher.OpenAsync(new OpenFileRequest
                    {
                        File = new ReadOnlyFile(DependencyService.Get<IFileWorker>().GetFilePath(fileName))
                    });
                }
                else
                {
                    await Settings.StartProgressBar("Загрузка", 0.8);
                    byte[] memoryStream = await _server.GetFileAPPConst(select.FileID.ToString());
                    if (memoryStream != null)
                    {
                        await DependencyService.Get<IFileWorker>().SaveTextAsync(fileName, memoryStream);
                        Loading.Instance.Hide();
                        await Launcher.OpenAsync(new OpenFileRequest
                        {
                            File = new ReadOnlyFile(DependencyService.Get<IFileWorker>().GetFilePath(fileName))
                        });
                    }
                    else
                    {
                        await DisplayAlert("Ошибка", "Не удалось скачать файл", "OK");
                    }
                }
            }
        }

        string FileName(string text)
        {
            return text
                .Replace("Отправлен новый файл: ", "")
                .Replace("\"", "")
                .Replace("\"", "");
        }

        async void addFileApp()
        {
            // getCameraFile();
            // // GetGalaryFile();
            //
            // // PickAndShowFile(null);
            if (Device.RuntimePlatform == "Android")
            {
                await CrossMedia.Current.Initialize();
                var camera_perm = await Plugin.Permissions.CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
                if (camera_perm != PermissionStatus.Granted)
                {
                    await CrossPermissions.Current.RequestPermissionsAsync(Permission.Camera, Permission.Storage);
                }

                var file_perm = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);
                if (file_perm != PermissionStatus.Granted)
                {
                    await CrossPermissions.Current.RequestPermissionsAsync(Permission.Storage);
                }

            }
            
            MediaFile file = null;
            var action = await DisplayActionSheet("Добавить вложение", "Отмена", null,
                TAKE_PHOTO,
                TAKE_GALRY, TAKE_FILE);
            switch (action)
            {
                case TAKE_PHOTO:
                    if (!CrossMedia.Current.IsTakePhotoSupported || !CrossMedia.Current.IsCameraAvailable)
                    {
                        await DisplayAlert("Ошибка", "Камера не доступна", "OK");

                        return;
                    }

                    try
                    {
                        file = await CrossMedia.Current.TakePhotoAsync(
                            new StoreCameraMediaOptions
                            {
                                SaveToAlbum = false,
                                Directory = "Demo",
                                PhotoSize = PhotoSize.Medium,
                            });
                        
                        if (file != null)
                            await startLoadFile(CAMERA, file);
                    }
                    catch (Exception e)
                    {
                        await DisplayAlert("Ошибка", $"{e.Message}\n{e.StackTrace}", "OK");
                        Console.WriteLine(e);
                    }
                    break;
                case TAKE_GALRY:
                    if (!CrossMedia.Current.IsPickPhotoSupported)
                    {
                        await DisplayAlert("Ошибка", "Галерея не доступна", "OK");

                        return;
                    }

                    try
                    {
                        file = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions { PhotoSize = PhotoSize.Medium });
                        if (file == null)
                            return;
                        await startLoadFile(GALERY, file);
                    }
                    catch (Exception e)
                    {
                        await DisplayAlert("Ошибка", $"{e.Message}\n{e.StackTrace}", "OK");
                        Console.WriteLine(e);
                    }
                   
                    break;
                case TAKE_FILE:
                    await startLoadFile(FILE, null);
                    break;
            }
        }

        async Task getCameraFile(MediaFile file)
        {
           
            if (file == null)
                return;
            CommonResult commonResult = await _server.AddFileAppsConst(_requestInfo.ID.ToString(),
                getFileName(file.Path), StreamToByteArray(file.GetStream()),
                file.Path);
            if (commonResult == null)
            {
                await ShowToast("Файл отправлен");
                await RefreshData();
            }
        }

        MediaFile photo { get; set; }

        public async Task startLoadFile(string metod, MediaFile file)
        {
            try
            {
                // Loading settings
                //Configurations.LoadingConfig = new LoadingConfig
                //{
                //    IndicatorColor = hex,
                //    OverlayColor = Color.Black,
                //    Opacity = 0.8,
                //    DefaultMessage = "Отправка файла",
                //};

                //await Loading.Instance.StartAsync(async progress =>
                //{
                    switch (metod)
                    {
                        case CAMERA:
                            await getCameraFile(file);
                            break;
                        case GALERY:
                            await GetGalaryFile(file);
                            break;
                        case FILE:
                            await PickAndShowFile(null);
                            break;
                    }
                //});
            }
            catch (Exception ex)
            {
                await DisplayAlert(null, $"{ex.Message}\n{ex.StackTrace}", "OK");
                Console.WriteLine(ex.Message);
            }
            
        }

        async Task GetGalaryFile(MediaFile file)
        {
            CommonResult commonResult = await _server.AddFileAppsConst(_requestInfo.ID.ToString(),
                getFileName(file.Path), StreamToByteArray(file.GetStream()),
                file.Path);
            if (commonResult == null)
            {
                await ShowToast("Файл отправлен");
                await RefreshData();
            }
        }

        public static byte[] StreamToByteArray(Stream stream)
        {
            if (stream is MemoryStream)
            {
                return ((MemoryStream)stream).ToArray();
            }
            else
            {
                // Jon Skeet's accepted answer 
                return ReadFully(stream);
            }
        }

        string getFileName(string path)
        {
            try
            {
                string[] fileName = path.Split('/');
                return fileName[fileName.Length - 1];
            }
            catch (Exception ex)
            {
                return "filename";
            }
        }

        public static byte[] ReadFully(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
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
                        IconViewAddFile.IsVisible = true;
                        progressFile.IsVisible = false;
                        return;
                    }


                    CommonResult commonResult = await _server.AddFileAppsConst(_requestInfo.ID.ToString(),
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
                CommonResult result = await _server.AddMessageConst(message, _requestInfo.ID.ToString(), false);
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
            request = await _server.GetRequestsDetailListConst(_requestInfo.ID.ToString());
            if (request.Error == null)
            {
                Settings.DateUniq = "";
                messages = request.Messages;
                LabelNumber.Text = "№ " + request.RequestNumber;
                IsRequestPaid = request.IsPaid;
                this.BindingContext = this;
            }
            else
            {
                await DisplayAlert("Ошибка", "Не удалось получить информацию по комментариям", "OK");
            }

            await MethodWithDelayAsync(1000);
        }

        async void acceptApp()
        {
            progress.IsVisible = true;
            var request = await _server.LockAppConst(_requestInfo.ID.ToString());
            if (request.Error == null)
            {
                await ShowToast("Заявка принята к выполнению");
                await RefreshData();
            }
            else
            {
                await DisplayAlert("Ошибка", "Не удалось принять заявку", "OK");
            }
            progress.IsVisible = false;
        }

        async void performApp()
        {
            progress.IsVisible = true;
            var request = await _server.PerformAppConst(_requestInfo.ID.ToString());
            if (request.Error == null)
            {
                await ShowToast("Заявка выполнена");
                await RefreshData();
            }
            else
            {
                await DisplayAlert("Ошибка", "Не удалось выполнить заявку", "OK");
            }
            progress.IsVisible = false;
        }

        public async Task MethodWithDelayAsync(int milliseconds)
        {
            await Task.Delay(milliseconds);

            additionalList.ScrollTo(messages[messages.Count - 1], 0, true);
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
            
            Color hexColor = (Color) Application.Current.Resources["MainColor"];
            FrameKeys.SetAppThemeColor(Frame.BorderColorProperty, hexColor, Color.FromHex("#B5B5B5"));
            FrameMessage.SetAppThemeColor(Frame.BorderColorProperty, hexColor, Color.White);
        }

        private async void ShowInfo()
        {
            string Status = request.Status;
            string Source = "ic_status_wait";
            if (Status.ToString().Contains("выполнена") || Status.ToString().Contains("закрыл"))
            {
                Source = "ic_status_done";
            }
            else if (Status.ToString().Contains("новая"))
            {
                Source = "ic_status_new";
            }
            if (request.Phone.Contains("+") == false && request.Phone.Substring(0, 2) == "79")
            {
                request.Phone = "+" + request.Phone;
            }
            Call = new Command<string>(async (phone) =>
            {
                if (phone!= null)
                {
                    IPhoneCallTask phoneDialer;
                    phoneDialer = CrossMessaging.Current.PhoneDialer;
                    if (phoneDialer.CanMakePhoneCall) 
                        phoneDialer.MakePhoneCall(phone);
                }
            });
            var ret = await Dialog.Instance.ShowAsync<InfoAppDialog>(new
            {
                _Request = request,
                HexColor = this.hex,
                SourceApp = Source,
                Calling = Call
            });
        }

        public Command<string> Call { get; set; }

        public async Task ShowRating()
        {
            await Settings.StartOverlayBackground(hex);
        }

        private async void OpenInfo(object sender, EventArgs e)
        {
            ShowInfo();
        }

        private async void Transit_OnTapped(object sender, EventArgs e)
        {
            progress.IsVisible = true;
            var request = await _server.SetPaidRequestStatusOnTheWay(_requestInfo.ID.ToString());
            if (request.Error == null)
            {
                await ShowToast(AppResources.TransitOrder);
                await RefreshData();
            }
            else
            {
                await DisplayAlert(AppResources.Order, request.Error, "OK");
            }
            progress.IsVisible = false;
        }
    }
}