﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using AiForms.Dialogs;
using AiForms.Dialogs.Abstractions;
using Microsoft.AppCenter.Analytics;
using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamarinJKH.Apps;
using xamarinJKH.DialogViews;
using xamarinJKH.InterfacesIntegration;
using xamarinJKH.Server;
using xamarinJKH.Server.RequestModel;
using xamarinJKH.Utils;

namespace xamarinJKH.Tech
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AppPage : ContentPage
    {
        private RequestInfo _requestInfo;
        private RequestContent request;

        private RequestList _requestList;
        private RestClientMP _server = new RestClientMP();
        private bool _isRefreshing = false;
        string TAKE_PHOTO = AppResources.AttachmentTakePhoto;
        string TAKE_GALRY = AppResources.AttachmentChoosePhoto;
        string TAKE_FILE = AppResources.AttachmentChooseFile;
        const string CAMERA = "camera";
        const string GALERY = "galery";
        const string FILE = "file";

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

        public bool closeMessages { get; set; } = !Settings.MobileSettings.disableCommentingRequests;
        CancellationTokenSource TokenSource { get; set; }
        CancellationToken Token { get; set; }
        bool PermissionAsked;

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            TokenSource = new CancellationTokenSource();
            Token = TokenSource.Token;

            var UpdateTask = new Task(async () =>
            {
                try
                {
                    while (!Token.IsCancellationRequested)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(2));
                        var update = await _server.GetRequestsDetailListTech(Settings.Person.Phone, GetLastIdMessage());
                        if (update.Error == null)
                        {
                            //Settings.DateUniq = "";

                            foreach (var each in update.Messages)
                            {
                                if (!messages.Contains(each))
                                    //Device.BeginInvokeOnMainThread(() => messages.Add(each));
                                    Device.BeginInvokeOnMainThread(async () =>
                                    {
                                        addAppMessage(each,
                                            messages.Count > 1 ? messages[messages.Count - 2].AuthorName : null);
                                        var lastChild = baseForApp.Children.LastOrDefault();

                                        if (lastChild != null)
                                        {
                                        //    Device.BeginInvokeOnMainThread(async () =>
                                        //await scrollFroAppMessages.ScrollToAsync(lastChild.X, lastChild.Y + 30, true)
                                        //);
                                            await scrollFroAppMessages.ScrollToAsync(lastChild.X, lastChild.Y + 30, false);
                                            //await scrollFroAppMessages.ScrollToAsync(lastChild, ScrollToPosition.End,
                                            //      false);
                                        }
                                    });
                                request.Messages.Add(each);
                            }

                            //Device.BeginInvokeOnMainThread(() => additionalList.ScrollTo(messages[messages.Count - 1], 0, true));
                        }
                    }
                }
                catch (Exception e)
                {
                }
            }, Token);
            try
            {
                UpdateTask.Start();
            }
            catch
            {
            }

            await Task.Delay(TimeSpan.FromSeconds(1));
            if (Device.RuntimePlatform == "Android")
            {
                return;
                try
                {
                    if (!PermissionAsked)
                    {
                        var camera_perm =
                            await Plugin.Permissions.CrossPermissions.Current.CheckPermissionStatusAsync(Permission
                                .Camera);
                        if (camera_perm != PermissionStatus.Granted)
                        {
                            await CrossPermissions.Current.RequestPermissionsAsync(Permission.Camera,
                                Permission.Storage);
                        }
                    }
                }
                catch
                {
                }
                finally
                {
                    PermissionAsked = true;
                }
            }
        }

        public string DateUniq = "";
        //public static string DateUniqeServ = "";


        protected override void OnDisappearing()
        {
            try
            {
                TokenSource.Cancel();
                TokenSource.Dispose();
            }
            catch
            {
            }

            MessagingCenter.Send<Object>(this, "AutoUpdate");
            base.OnDisappearing();
        }

        private async Task RefreshData()
        {
            if (Xamarin.Essentials.Connectivity.NetworkAccess != Xamarin.Essentials.NetworkAccess.Internet)
            {
                Device.BeginInvokeOnMainThread(async () =>
                    await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorNoInternet, "OK"));
                return;
            }

            var update =
                await _server.GetRequestsDetailListTech(Settings.Person.Phone, GetLastIdMessage());
            if (update.Error == null)
            {
                //Settings.DateUniq = "";

                foreach (var each in update.Messages)
                {
                    if (!messages.Contains(each))
                    {
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            addAppMessage(each, messages.Count > 1 ? messages[messages.Count - 2].AuthorName : null);
                            var lastChild = baseForApp.Children.LastOrDefault();

                            //var y = lastChild.Y- scrollFroAppMessages.Y;
                            //var x = lastChild.X;
                            //Device.BeginInvokeOnMainThread(async () => await scrollFroAppMessages.ScrollToAsync(x, y + 30, true));
                            if (lastChild != null)
                                Device.BeginInvokeOnMainThread(async () =>
                                    await scrollFroAppMessages.ScrollToAsync(lastChild, ScrollToPosition.End, true));
                        });
                        messages.Add(each);
                        request.Messages.Add(each);
                    }
                }

                // additionalList.ScrollTo(messages[messages.Count - 1], 0, true);
                var lastChild = baseForApp.Children.LastOrDefault();

                //var y = lastChild.Y- scrollFroAppMessages.Y;
                //var x = lastChild.X;
                //Device.BeginInvokeOnMainThread(async () => await scrollFroAppMessages.ScrollToAsync(x, y + 30, true));
                Device.BeginInvokeOnMainThread(async () =>
                {
                    if (lastChild != null)
                        await scrollFroAppMessages.ScrollToAsync(lastChild, ScrollToPosition.End, true);
                });
                //Device.BeginInvokeOnMainThread(async () => await MethodWithDelayAsync(500));
                //await MethodWithDelayAsync(500);

                //await scrollFroAppMessages.ScrollToAsync(lastChild.X, lastChild.Y + 30, true);
            }
            else
            {
                await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorComments, "OK");
            }
        }

        string GetLastIdMessage()
        {
            String lastId = null;
            try
            {
                lastId = request.Messages.LastOrDefault()?.ID.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return lastId;
        }

        //public System.Collections.ObjectModel.ObservableCollection<RequestMessage> messages { get; set; }
        List<RequestMessage> messages = new List<RequestMessage>();

        public Color hex { get; set; }
        public bool isPayd { get; set; }

        public bool close = false;

        public AppPage()
        {
            InitializeComponent();
            Analytics.TrackEvent("Диалог с техподдержкой ");
            try
            {
                _speechRecongnitionInstance = DependencyService.Get<ISpeechToText>();
            }
            catch (Exception ex)
            {
#if DEBUG
                //ошибку выводить в сообщение для дебага
                EntryMess.Text = ex.Message;
#endif
                throw ex;
            }

            MessagingCenter.Subscribe<ISpeechToText, string>(this, "STT",
                (sender, args) => { SpeechToTextFinalResultRecieved(args); });

            MessagingCenter.Subscribe<ISpeechToText>(this, "Final", (sender) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    new PopupPage();
                    IconViewMic.ReplaceStringMap = new Dictionary<string, string> {{"#000000", hex.ToHex()}};
                });
            });

            MessagingCenter.Subscribe<IMessageSender, string>(this, "STT",
                (sender, args) => { SpeechToTextFinalResultRecieved(args); });


            messages =
                new List<RequestMessage>(); // System.Collections.ObjectModel.ObservableCollection<RequestMessage>();

            hex = (Color) Application.Current.Resources["MainColor"];
            getMessage2();
            this.BindingContext = this;

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    int statusBarHeight = DependencyService.Get<IStatusBar>().GetHeight();
                    //Pancake22.HeightRequest = statusBarHeight;
                    //Pancake22.IsVisible = true;
                    if (DeviceDisplay.MainDisplayInfo.Width < 700)
                        mainStack.Padding = new Thickness(0, statusBarHeight * 2, 0, 0);
                    else
                        mainStack.Padding = new Thickness(0, statusBarHeight, 0, 0);

                    break;
                case Device.Android:
                    double or = Math.Round(((double) App.ScreenWidth / (double) App.ScreenHeight), 2);
                    if (Math.Abs(or - 0.5) < 0.02)
                    {
                        //ScrollViewContainer.Margin = new Thickness(0, 0, 0, -150);
                    }

                    break;
                default:
                    break;
            }

            NavigationPage.SetHasNavigationBar(this, false);
            var backClick = new TapGestureRecognizer();
            backClick.Tapped += async (s, e) =>
            {
               
                    Settings.isSelf = null;
                    Settings.DateUniq = "";
                    try
                    {
                        _ = await Navigation.PopAsync();
                    }
                    catch
                    {
                        _ = await Navigation.PopModalAsync();
                    }
            };
            BackStackLayout.GestureRecognizers.Add(backClick);
            var sendMess = new TapGestureRecognizer();
            sendMess.Tapped += async (s, e) => { sendMessage(); };
            IconViewSend.GestureRecognizers.Add(sendMess);
            var recordmic = new TapGestureRecognizer();
            recordmic.Tapped += async (s, e) => { RecordMic(); };
            IconViewMic.GestureRecognizers.Add(recordmic);
            var addFile = new TapGestureRecognizer();
            addFile.Tapped += async (s, e) => { addFileApp(); };
            IconViewAddFile.GestureRecognizers.Add(addFile);

            setText();
            //additionalList.Effects.Add(Effect.Resolve("MyEffects.ListViewHighlightEffect"));
        }


        private async void RecordMic()
        {
            try
            {
                _speechRecongnitionInstance.StartSpeechToText();
            }
            catch (Exception ex)
            {
#if DEBUG
                EntryMess.Text = ex.Message;
#endif
            }

            if (Device.RuntimePlatform == Device.iOS)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    IconViewMic.ReplaceStringMap = new Dictionary<string, string> {{"#000000", "#A2A2A2"}};
                });
            }

            //var result = await CrossSpeechToText.StartVoiceInput(AppResources.VoiceInput);
            //EntryMess.Text +=  " " + result;
        }

        private void SpeechToTextFinalResultRecieved(string args)
        {
            if (!string.IsNullOrWhiteSpace(args))
                EntryMess.Text += " " + args;
        }

        private ISpeechToText _speechRecongnitionInstance;

        protected override bool OnBackButtonPressed()
        {
            if (close)
            {
                MessagingCenter.Send<Object>(this, "LoadGoods");
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


        //private async void OnItemTapped(object sender, ItemTappedEventArgs e)
        //{
        //    RequestMessage select = e.Item as RequestMessage;
        //    if (@select != null && @select.FileID != -1)
        //    {
        //        string fileName = FileName(@select.Text);
        //        if (await DependencyService.Get<IFileWorker>().ExistsAsync(fileName))
        //        {
        //            await Launcher.OpenAsync(new OpenFileRequest
        //            {
        //                File = new ReadOnlyFile(DependencyService.Get<IFileWorker>().GetFilePath(fileName))
        //            });
        //        }
        //        else
        //        {
        //            await Settings.StartProgressBar("Загрузка", 0.8);
        //            byte[] memoryStream = await _server.GetFileAPP(select.FileID.ToString());
        //            if (memoryStream != null)
        //            {
        //                await DependencyService.Get<IFileWorker>().SaveTextAsync(fileName, memoryStream);
        //                Loading.Instance.Hide();
        //                await Launcher.OpenAsync(new OpenFileRequest
        //                {
        //                    File = new ReadOnlyFile(DependencyService.Get<IFileWorker>().GetFilePath(fileName))
        //                });
        //            }
        //            else
        //            {
        //                await DisplayAlert("Ошибка", "Не удалось скачать файл", "OK");
        //            }
        //        }
        //    }
        //}

        //string FileName(string text)
        //{
        //    return text
        //        .Replace("Отправлен новый файл: ", "")
        //        .Replace("\"", "")
        //        .Replace("\"", "");
        //}

        async void addFileApp()
        {
            // getCameraFile();
            // // GetGalaryFile();
            //
            // // PickAndShowFile(null);
            MediaFile file = null;
            if (Device.RuntimePlatform == "Android")
            {
                try
                {
                    var camera_perm =
                        await Plugin.Permissions.CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
                    var storage_perm =
                        await Plugin.Permissions.CrossPermissions.Current
                            .CheckPermissionStatusAsync(Permission.Storage);
                    if (camera_perm != PermissionStatus.Granted || storage_perm != PermissionStatus.Granted)
                    {
                        var status =
                            await CrossPermissions.Current.RequestPermissionsAsync(Permission.Camera,
                                Permission.Storage);
                        if (status[Permission.Camera] == PermissionStatus.Denied &&
                            status[Permission.Storage] == PermissionStatus.Denied)
                        {
                            return;
                        }
                    }
                }
                catch (Exception e)
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        var result = await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorNoPermissions, "OK",
                            AppResources.Cancel);
                        if (result)
                            Plugin.Permissions.CrossPermissions.Current.OpenAppSettings();
                    });
                    return;
                }
            }

            var action = await DisplayActionSheet(AppResources.AttachmentTitle, AppResources.Cancel, null,
                TAKE_PHOTO,
                TAKE_GALRY, TAKE_FILE);
            if (action == TAKE_PHOTO)
            {
                if (!CrossMedia.Current.IsTakePhotoSupported || !CrossMedia.Current.IsCameraAvailable)
                {
                    await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorCameraNotAvailable, "OK");

                    return;
                }

                try
                {
                    file = await CrossMedia.Current.TakePhotoAsync(
                        new StoreCameraMediaOptions
                        {
                            SaveToAlbum = false,
                            Directory = "Demo"
                        });
                    if (file != null)
                        await startLoadFile(CAMERA, file);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                return;
            }

            if (action == TAKE_GALRY)
            {
                if (!CrossMedia.Current.IsPickPhotoSupported)
                {
                    await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorGalleryNotAvailable, "OK");

                    return;
                }

                try
                {
                    file = await CrossMedia.Current.PickPhotoAsync();
                    if (file == null)
                        return;
                    await startLoadFile(GALERY, file);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                return;
            }

            if (action == TAKE_FILE)
                await startLoadFile(FILE, null);

            Loading.Instance.Hide();
        }

        async Task getCameraFile(MediaFile file)
        {
            if (file == null)
                return;
            CommonResult commonResult = await _server.AddFileAppsTech(Settings.Person.Phone,
                getFileName(file.Path), StreamToByteArray(file.GetStream()),
                file.Path);
            if (commonResult == null)
            {
                await ShowToast(AppResources.SuccessFileSent);
                await RefreshData();
            }
        }

        public async Task startLoadFile(string metod, MediaFile file)
        {
            // Loading settings
            Configurations.LoadingConfig = new LoadingConfig
            {
                IndicatorColor = hex,
                OverlayColor = Color.Black,
                Opacity = 0.8,
                DefaultMessage = AppResources.FileSending,
            };

            await Loading.Instance.StartAsync(async progress =>
            {
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
            });
        }

        async Task GetGalaryFile(MediaFile file)
        {
            CommonResult commonResult = await _server.AddFileAppsTech(Settings.Person.Phone,
                getFileName(file.Path), StreamToByteArray(file.GetStream()),
                file.Path);
            if (commonResult == null)
            {
                await ShowToast(AppResources.SuccessFileSent);
                await RefreshData();
            }
        }

        public static byte[] StreamToByteArray(Stream stream)
        {
            if (stream is MemoryStream)
            {
                return ((MemoryStream) stream).ToArray();
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
                        await DisplayAlert(AppResources.ErrorTitle, AppResources.FileTooBig, "OK");
                        IconViewAddFile.IsVisible = true;
                        progressFile.IsVisible = false;
                        return;
                    }


                    CommonResult commonResult = await _server.AddFileAppsTech(Settings.Person.Phone,
                        pickedFile.FileName, pickedFile.DataArray,
                        pickedFile.FilePath);
                    if (commonResult == null)
                    {
                        await ShowToast(AppResources.SuccessFileSent);
                        await RefreshData();
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert(AppResources.ErrorTitle, ex.ToString(), "OK");
            }

            IconViewAddFile.IsVisible = true;
            progressFile.IsVisible = false;
        }


        async void sendMessage()
        {
            try
            {
                string message = EntryMess.Text;
                //if (string.IsNullOrWhiteSpace(message))
                //{
                //    EntryMess.Text = string.Empty;
                //    return;
                //}
                var anim = new Task(async () =>
                {
                    IconViewSend.Scale = 0.7;
                    await Task.Delay(TimeSpan.FromSeconds(0.5));
                    IconViewSend.Scale = 1;
                });
                anim.Start();
                if (!string.IsNullOrWhiteSpace(message))
                {
                    // progress.IsVisible = true;
                    IconViewSend.IsEnabled = false;
                    IsSucceed result = await _server.AddMessageTech(message, Settings.Person.Phone);
                    if (result.isSucceed)
                    {
                        EntryMess.Text = "";
                        // await ShowToast(AppResources.MessageSent);
                        //await RefreshData();

                        var lastChild = baseForApp.Children.LastOrDefault();
                        if (lastChild != null)
                            Device.BeginInvokeOnMainThread(async () =>
                                await scrollFroAppMessages.ScrollToAsync(lastChild, ScrollToPosition.End, true));
                    }
                }
                else
                {
                    await ShowToast(AppResources.ErrorMessageEmpty);
                }

                // progress.IsVisible = false;
                IconViewSend.IsEnabled = true;
            }
            catch (Exception e)
            {
                await ShowToast(AppResources.MessageNotSent);


                // progress.IsVisible = false;
                IconViewSend.IsEnabled = true;
            }
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

        //async void getMessage()
        //{

        //    request = await _server.GetRequestsDetailList(_requestInfo.ID.ToString());
        //    if (request.Error == null)
        //    {
        //        Settings.DateUniq = "";
        //        foreach (var message in request.Messages)
        //        {
        //            //if (message.IsSelf)
        //            //    message.IsSelf = false;

        //            //if (!message.IsSelf)
        //            message.IsSelf = false;
        //                Device.BeginInvokeOnMainThread(() => messages.Add(message));
        //        }
        //        LabelNumber.Text = "№ " + request.RequestNumber;
        //    }
        //    else
        //    {
        //        await DisplayAlert("Ошибка", "Не удалось получить информацию по комментариям", "OK");
        //    }

        //    await MethodWithDelayAsync(1000);

        //}


        async void getMessage2()
        {
            Analytics.TrackEvent("Запрос сообщений");
            new Task(async () =>
            {
                Configurations.LoadingConfig = new LoadingConfig
                {
                    IndicatorColor = (Color) Application.Current.Resources["MainColor"],
                    OverlayColor = Color.Black,
                    Opacity = 0.8,
                    DefaultMessage = AppResources.Loading,
                };

                Device.BeginInvokeOnMainThread(async () =>
                              await Loading.Instance.StartAsync(async progress =>
                              {
                                  Analytics.TrackEvent("Запрос сообщений " + Settings.Person.Phone);
                                  request = await _server.GetRequestsDetailListTech(Settings.Person.Phone);
                                  if (request.Error == null)
                                  {
                                      Analytics.TrackEvent("Результат запроса " + Newtonsoft.Json.JsonConvert.SerializeObject(request));
                                      Settings.DateUniq = "";
                                      foreach (var message in request.Messages)
                                      {
                                          if (!messages.Contains(message))
                                          {
                                              Device.BeginInvokeOnMainThread(() => addAppMessage(message,
                                                  messages.Count > 1 ? messages[messages.Count - 2].AuthorName : null));
                                              messages.Add(message);
                                          }

                            //messages.Add(message);
                            //Device.BeginInvokeOnMainThread(() => addAppMessage(message));
                        }

                        // LabelNumber.Text = "№ " + request.RequestNumber;
                    }
                                  else
                                  {
                                      await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorComments, "OK");
                                  }
                                  Device.BeginInvokeOnMainThread(async () => await MethodWithDelayAsync(1000));
                              })
                              );
            }).Start();

        }

        void addAppMessage(RequestMessage message, string prevAuthor)
        {
            StackLayout data;
            string newDate;
            if (message.IsSelf)
            {
                data = new MessageCellAuthor(message, this, DateUniq, out newDate, true);
            }
            else
            {
                data = new MessageCellService(message, this, DateUniq, out newDate, prevAuthor, true);
            }

            DateUniq = newDate;

            baseForApp.Children.Add(data);
        }


        public async Task MethodWithDelayAsync(int milliseconds)
        {
            await Task.Delay(milliseconds);

            try
            {
                //additionalList.ScrollTo(messages[messages.Count - 1], 0, true);
                var lastChild = baseForApp.Children.LastOrDefault();
                if (lastChild != null)
                    //await scrollFroAppMessages.ScrollToAsync(lastChild.X, lastChild.Y + 30, true);// (lastChild.X, lastChild.Y + 30, true);
                    await scrollFroAppMessages.ScrollToAsync(lastChild, ScrollToPosition.End, true);
            }
            catch
            {
            }
        }

        async void setText()
        {
            await CrossMedia.Current.Initialize();
            Color hexColor = (Color) Application.Current.Resources["MainColor"];
            FrameMessage.SetAppThemeColor(Frame.BorderColorProperty, hexColor, Color.White);
        }

        private async void ShowInfo()
        {
            if (request != null)
            {
                string Status = request.Status;
                if (!string.IsNullOrEmpty(Status))
                {
                    string Source = Settings.GetStatusIcon(request.StatusID);
                    if (!string.IsNullOrWhiteSpace(request.Phone) && (request.Phone.Contains("+") == false &&
                                                                      request.Phone.Substring(0, 2) == "79"))
                    {
                        request.Phone = "+" + request.Phone;
                    }

                    bool IsPass = request.PassInfo != null;
                    bool isMan = false;
                    if (IsPass)
                    {
                        isMan = request.PassInfo.CategoryId == 1;
                    }

                    try
                    {
                        if (!string.IsNullOrEmpty(Source))
                            await Dialog.Instance.ShowAsync<InfoAppDialog>(new
                            {
                                _Request = request,
                                HexColor = this.hex,
                                SourceApp = Source,
                                isPass = IsPass,
                                isManType = isMan
                            });
                    }
                    catch
                    {
                    }
                }
            }
        }

        public async Task ShowRating()
        {
            await Settings.StartOverlayBackground(hex);
        }

        private async void OpenInfo(object sender, EventArgs e)
        {
            ShowInfo();
        }

        private async void OpenReceipt(object sender, EventArgs args)
        {
            await Dialog.Instance.ShowAsync(
                new AppReceiptDialogWindow(new ViewModels.DialogViewModels.AppRecieptViewModel(request.ReceiptItems)));
        }


        private void EntryMess_TextChanged(object sender, TextChangedEventArgs e)
        {
            var entry = sender as BordlessEditor;
            entry.MinimumHeightRequest = 100;

            if (entry != null)
            {
                if (entry.Height > 121)
                {
                    entry.HeightRequest = 120;
                    entry.AutoSize = EditorAutoSizeOption.Disabled;
                }
                else
                {
                    if (e.OldTextValue != null && e.NewTextValue.Length < e.OldTextValue.Length &&
                        e.NewTextValue.Length < 100)
                    {
                        entry.HeightRequest = -1;
                    }

                    entry.AutoSize = EditorAutoSizeOption.TextChanges;
                }
            }
        }
    }
}