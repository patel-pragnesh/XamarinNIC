﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamarinJKH.InterfacesIntegration;
using xamarinJKH.Main;
using xamarinJKH.Server;
using xamarinJKH.Server.RequestModel;
using xamarinJKH.Tech;
using xamarinJKH.Utils;

using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms.PancakeView;
using xamarinJKH.DialogViews;
using Xamarin.Essentials;
using System.Text.RegularExpressions;
//using dotMorten.Xamarin.Forms;
using Microsoft.AppCenter.Analytics;

namespace xamarinJKH.Apps
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewAppPage : ContentPage
    {
        private RestClientMP _server = new RestClientMP();
        public ObservableCollection<FileData> files { get; set; }
        public List<byte[]> Byteses = new List<byte[]>();
        private AppsPage _appsPage;
        string TAKE_PHOTO = AppResources.AttachmentTakePhoto;
        string TAKE_GALRY = AppResources.AttachmentChoosePhoto;
        string TAKE_FILE = AppResources.AttachmentChooseFile;
        const string CAMERA = "camera";
        const string GALERY = "galery";
        const string FILE = "file";
        public int PikerLsItem = 0;
        public int PikerTypeItem = 0;
        private AddAppModel _appModel;
        private bool isPassAPP = false;
        PassApp _passApp = new PassApp();
        public NewAppPage()
        {
            InitializeComponent();
            Analytics.TrackEvent("Создание заявки");

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    int statusBarHeight = DependencyService.Get<IStatusBar>().GetHeight();
                    break;
                default:
                    break;
            }
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    //BackgroundColor = Color.White;
                    // ImageTop.Margin = new Thickness(0, 0, 0, 0);
                    // StackLayout.Margin = new Thickness(0, 33, 0, 0);
                    // IconViewNameUk.Margin = new Thickness(0, 33, 0, 0);
                    // if (Application.Current.MainPage.Height > 800)
                    // {
                    //     ScrollViewContainer.Margin = new Thickness(0, 0, 0, -180);
                    //     BackStackLayout.Margin = new Thickness(-5, 35, 0, 0);
                    // }
                    break;
                case Device.Android:
                    // ScrollViewContainer.Margin = new Thickness(0, 0, 0, -162);
                    // double or = Math.Round(((double) App.ScreenWidth / (double) App.ScreenHeight), 2);
                    // if (Math.Abs(or - 0.5) < 0.02)
                    // {
                    //     ScrollViewContainer.Margin = new Thickness(0, 0, 0, -115);
                    //     BackStackLayout.Margin = new Thickness(-5, 15, 0, 0);
                    // }

                    break;
                default:
                    break;
            }

            NavigationPage.SetHasNavigationBar(this, false);
            var backClick = new TapGestureRecognizer();
            backClick.Tapped += async (s, e) => {
               ClosePage();
            };
            BackStackLayout.GestureRecognizers.Add(backClick);
            var techSend = new TapGestureRecognizer();
            techSend.Tapped += async (s, e) => {     await PopupNavigation.Instance.PushAsync(new TechDialog());};
            LabelTech.GestureRecognizers.Add(techSend);
            var pickLs = new TapGestureRecognizer();
            pickLs.Tapped += async (s, e) => {  Device.BeginInvokeOnMainThread(() =>
                {
                    PickerLs.Focus();                 
                });
            };
            StackLayoutLs.GestureRecognizers.Add(pickLs); 
            var pickType = new TapGestureRecognizer();
            pickType.Tapped += async (s, e) => {  
                Device.BeginInvokeOnMainThread(() =>
                {
                    PickerType.Focus();
                });
            };
            StackLayoutType.GestureRecognizers.Add(pickType);
            var call = new TapGestureRecognizer();
            call.Tapped += async (s, e) =>
            {
                if (Settings.Person.Phone != null)
                {
                    IPhoneCallTask phoneDialer;
                    phoneDialer = CrossMessaging.Current.PhoneDialer;
                    if (phoneDialer.CanMakePhoneCall && !string.IsNullOrWhiteSpace(Settings.Person.companyPhone)) 
                        phoneDialer.MakePhoneCall(System.Text.RegularExpressions.Regex.Replace(Settings.Person.companyPhone, "[^+0-9]", ""));
                }

            
            };
            var addFile = new TapGestureRecognizer();
            addFile.Tapped += async (s, e) => { AddFile(); };
            StackLayoutAddFile.GestureRecognizers.Add(addFile);
            
            SetText();
            files = new ObservableCollection<FileData>();

#if DEBUG
            _appModel = new AddAppModel()
            {
                AllAcc = Settings.Person.Accounts,
                AllType = Settings.TypeApp,
                AllKindPass = new List<string> { AppResources.PassMan, AppResources.PassMotorcycle,
                    AppResources.PassCar, AppResources.PassGazele, AppResources.PassCargo },
                AllBrand = new List<string>() {"Suzuki", "Kavasaki", "Lada", "Opel", "Volkswagen", "Запорожец" }, /*Settings.BrandCar,*/
                hex = (Color)Application.Current.Resources["MainColor"],
                SelectedAcc = Settings.Person.Accounts[0],
                SelectedType = Settings.TypeApp[0],
                Files = files
            };
#else
_appModel = new AddAppModel()
            {
                AllAcc = Settings.Person.Accounts,
                AllType = Settings.TypeApp,
                AllKindPass = new List<string>{AppResources.PassMan, AppResources.PassMotorcycle,
                    AppResources.PassCar, AppResources.PassGazele, AppResources.PassCargo},
                AllBrand = Settings.BrandCar,
                hex = (Color)Application.Current.Resources["MainColor"],
                SelectedAcc = Settings.Person.Accounts[0],
                SelectedType = Settings.TypeApp[0],
                Files = files
            };
#endif


            BindingContext = _appModel;
            ListViewFiles.Effects.Add(Effect.Resolve("MyEffects.ListViewHighlightEffect"));

            var passTypeEntryTGR = new TapGestureRecognizer();
            passTypeEntryTGR.Tapped += async (s, e) =>
            {
                if (!PassTypesList.IsVisible)
                {
                    PassTypesList.IsVisible = true;
                }
            };
            PassType.GestureRecognizers.Add(call);
            
        }

        private void SearchBar_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            PassTypesList.IsVisible = true;
            PassTypesList.BeginRefresh();

            try
            {
                var dataEmpty = _appModel.AllKindPass.Where(i => i.ToLower().Contains(e.NewTextValue.ToLower()));
                
                SetVisibleLayout(e.NewTextValue);

                if (string.IsNullOrWhiteSpace(e.NewTextValue))
                {
                    //PassTypesList.IsVisible = false;
                    PassTypesList.ItemsSource = _appModel.AllKindPass;
                }
                else if (dataEmpty.Max().Length == 0)
                    PassTypesList.IsVisible = false;
                else
                    PassTypesList.ItemsSource = _appModel.AllKindPass.Where(i => i.ToLower().Contains(e.NewTextValue.ToLower()));
            }
            catch (Exception ex)
            {
                PassTypesList.IsVisible = false;
            }
            PassTypesList.EndRefresh();

        }

        private void TSBrand_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            TSBrandList.IsVisible = true;
            TSBrandList.BeginRefresh();

            try
            {
                var dataEmpty = _appModel.AllBrand.Where(i => i.ToLower().Contains(e.NewTextValue.ToLower()));
                
                if (string.IsNullOrWhiteSpace(e.NewTextValue))
                {
                    //TSBrandList.IsVisible = false;
                    TSBrandList.ItemsSource = _appModel.AllBrand;
                }
                else if (dataEmpty.Max().Length == 0)
                    TSBrandList.IsVisible = false;
                else
                    TSBrandList.ItemsSource = _appModel.AllBrand.Where(i => i.ToLower().Contains(e.NewTextValue.ToLower()));
            }
            catch (Exception ex)
            {
                TSBrandList.IsVisible = false;
            }
            TSBrandList.EndRefresh();
        }

        void SetVisibleLayout(string listsd)
        {
            if (listsd != null)
            {
                if (!_appModel.AllKindPass.Exists(i => i.ToLower() == listsd.ToLower()))
                {
                    LayoutPeshehod.IsVisible = false;
                    LayoutAvto.IsVisible = false;
                    return;
                }
                    _passApp.idType = _appModel.AllKindPass.IndexOf(listsd) + 1;
                // User selected an item from the suggestion list, take an action on it here.
                if (listsd.Equals(AppResources.PassMan))
                {
                    LayoutPeshehod.IsVisible = true;
                    LayoutAvto.IsVisible = false;

                }
                else
                {
                    LayoutPeshehod.IsVisible = false;
                    LayoutAvto.IsVisible = true;
                }
            }
            else
            {
                // User hit Enter from the search box. Use args.QueryText to determine what to do.
                _passApp.idType = 0;
            }
        }

        private void ListView_OnItemTapped(Object sender, ItemTappedEventArgs e)
        {
            String listsd = e.Item as string;
            PassType.Text = listsd;
            PassTypesList.IsVisible = false;
            ((ListView)sender).SelectedItem = null;            
            SetVisibleLayout(listsd);
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            await Task.Delay(TimeSpan.FromSeconds(1));           
        }

        private async void AddFile()
        {

            if (Device.RuntimePlatform == "Android")
            {
                try
                {
                    var camera_perm = await Plugin.Permissions.CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
                    var storage_perm = await Plugin.Permissions.CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);
                    if (camera_perm != PermissionStatus.Granted || storage_perm != PermissionStatus.Granted)
                    {
                        var status = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Camera, Permission.Storage);
                        if (status[Permission.Camera] == PermissionStatus.Denied && status[Permission.Storage] == PermissionStatus.Denied)
                        {
                            return;
                        }
                    }
                }
                catch
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        var result = await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorNoPermissions, "OK", AppResources.Cancel);
                        if (result)
                            Plugin.Permissions.CrossPermissions.Current.OpenAppSettings();

                    });
                    return;
                }
            }
            var action = await DisplayActionSheet(AppResources.AttachmentTitle, AppResources.Cancel, null,
                TAKE_PHOTO,
                TAKE_GALRY, TAKE_FILE);
            try
            {
                        if (action == TAKE_PHOTO)
                        {
                            await getCameraFile();
                            return;
                        }
                        if (action == TAKE_GALRY)
                        {
                            await GetGalaryFile();
                            return;
                        }
                        if (action == TAKE_FILE)
                        {
                            await PickAndShowFile(null);
                            return;
                        }
            }
            catch (Exception ex)
            {
                
            }
        }
        async void ClosePage()
        {
            try
            {
                await Navigation.PopAsync();
               
            }
            catch
            {
                await Navigation.PopModalAsync();
            }
        }
        private async void PickImage_Clicked(object sender, EventArgs args)
        {
            string[] fileTypes = null;

            if (Device.RuntimePlatform == Device.Android)
            {
                fileTypes = new string[] {"image/png", "image/jpeg"};
            }

            if (Device.RuntimePlatform == Device.iOS)
            {
                fileTypes = new string[] {"public.image"}; // same as iOS constant UTType.Image
            }

            if (Device.RuntimePlatform == Device.UWP)
            {
                fileTypes = new string[] {".jpg", ".png"};
            }

            if (Device.RuntimePlatform == Device.WPF)
            {
                fileTypes = new string[] {"JPEG files (*.jpg)|*.jpg", "PNG files (*.png)|*.png"};
            }

            await PickAndShowFile(fileTypes);
        }

        private async Task PickAndShowFile(string[] fileTypes)
        {
            try
            {
                FileData pickedFile = await CrossFilePicker.Current.PickFile(fileTypes);

                if (pickedFile != null)
                {
                    // UkName.Text = pickedFile.FileName;
                    // LabelPhone.Text = pickedFile.FilePath;
                    if (pickedFile.DataArray.Length > 10000000)
                    {
                        await DisplayAlert(AppResources.ErrorTitle,AppResources.FileTooBig, "OK");
                        return;
                    }

                    files.Add(pickedFile);
                    Byteses.Add(pickedFile.DataArray);
                    ListViewFiles.IsVisible = true;
                    if (ListViewFiles.HeightRequest < 120)
                        ListViewFiles.HeightRequest += 30;
                    // setBinding();
                    _appModel.Files = files;
                    ListViewFiles.ItemsSource = _appModel.Files;
                    // if (pickedFile.FileName.EndsWith("jpg", StringComparison.OrdinalIgnoreCase)
                    //     || pickedFile.FileName.EndsWith("png", StringComparison.OrdinalIgnoreCase))
                    // {
                    //     IconViewNameUk.Source = ImageSource.FromStream(() => pickedFile.GetStream());
                    //     IconViewNameUk.IsVisible = true;
                    // }
                    // else
                    // {
                    //     IconViewNameUk.IsVisible = false;
                    // }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert(AppResources.ErrorTitle, ex.ToString(), "OK");
            }
        }

        async Task getCameraFile()
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsTakePhotoSupported || !CrossMedia.Current.IsCameraAvailable)
            {
                await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorCameraNotAvailable, "OK");

                return;
            }

            MediaFile file = await CrossMedia.Current.TakePhotoAsync(
                new StoreCameraMediaOptions
                {
                    SaveToAlbum = true,
                    Directory = "Demo"
                });

            if (file == null)
                return;
            FileData fileData = new FileData( file.Path,getFileName(file.Path), () => file.GetStream() );
            Byteses.Add(StreamToByteArray(file.GetStream()));
            files.Add(fileData);
            ListViewFiles.IsVisible = true;
            if (ListViewFiles.HeightRequest < 120)
                ListViewFiles.HeightRequest += 30;
            // setBinding();
            _appModel.Files = files;
            ListViewFiles.ItemsSource = _appModel.Files;
        }
        
        async Task GetGalaryFile()
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorGalleryNotAvailable, "OK");

                return;
            }

            var file = await CrossMedia.Current.PickPhotoAsync();
            if (file == null)
                return;
            FileData fileData = new FileData( file.Path,getFileName(file.Path), () => file.GetStream() );
            Byteses.Add(StreamToByteArray(file.GetStream()));
            files.Add(fileData);
            ListViewFiles.IsVisible = true;
            if (ListViewFiles.HeightRequest < 120)
                ListViewFiles.HeightRequest += 30;
            // setBinding();
            _appModel.Files = files;
            ListViewFiles.ItemsSource = _appModel.Files;
            // PickerLs.SelectedIndex = PikerLsItem;
            // PickerType.SelectedIndex = PikerTypeItem;
        }
        
        public async Task startLoadFile(string metod)
        {
            // Loading settings
            Configurations.LoadingConfig = new LoadingConfig
            {
                IndicatorColor = (Color)Application.Current.Resources["MainColor"],
                OverlayColor = Color.Black,
                Opacity = 0.8,
                DefaultMessage = AppResources.LoadingFile,
            };

            await Loading.Instance.StartAsync(async progress =>
            {
                switch (metod)
                {
                    case CAMERA:
                        // await getCameraFile();
                        break;
                    case GALERY:
                        // await GetGalaryFile();
                        break;
                    case FILE:
                        // await PickAndShowFile(null);
                        break;
                }
            });
        }

        public static byte[] StreamToByteArray(Stream stream)
        {
            if (stream is MemoryStream)
            {
                return ((MemoryStream) stream).ToArray();
            }
            else
            {
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

        void SetText()
        {
            UkName.Text = Settings.MobileSettings.main_name;
           
            Color hexColor = (Color) Application.Current.Resources["MainColor"];
            IconViewTech.SetAppThemeColor(IconView.ForegroundProperty, hexColor, Color.White);
            LabelTech.SetAppThemeColor(Label.TextColorProperty, hexColor, Color.White);
            FrameTop.SetAppThemeColor(Frame.BorderColorProperty, hexColor, Color.White);
        }

        void setBinding()
        {
            PikerLsItem = PickerLs.SelectedIndex;
            PikerTypeItem = PickerType.SelectedIndex;
            try
            {
                BindingContext = null;
                BindingContext = new AddAppModel()
                {
                    AllAcc = Settings.Person.Accounts,
                    AllType = Settings.TypeApp,
                    hex = (Color)Application.Current.Resources["MainColor"],
                    SelectedAcc = Settings.Person.Accounts[PikerLsItem],
                    SelectedType = Settings.TypeApp[PikerTypeItem],
                    Files = files
                };
            }
            catch (Exception ex)
            {
                // ignored
            }
        }

        private void picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            // try
            // {
            //     var identLength = Settings.Person.Accounts[PickerLs.SelectedIndex].Ident.Length;
            //     if (identLength < 6)
            //     {
            //         PickerLs.WidthRequest = identLength * 9;
            //     }
            // }
            // catch (Exception ex)
            // {
            //     // ignored
            // }
        }

        public class AddAppModel
        {
            public List<AccountInfo> AllAcc { get; set; }
            public List<NamedValue> AllType { get; set; }
            public List<string> AllBrand { get; set; }
            public List<string> AllKindPass { get; set; }
            public AccountInfo SelectedAcc { get; set; }
            public NamedValue SelectedType { get; set; }

            public ObservableCollection<FileData> Files { get; set; }
            public Color hex { get; set; }
        }

        private async void addApp(object sender, EventArgs e)
        {
            string text = EntryMess.Text;
            FrameBtnAdd.IsVisible = false;
            progress.IsVisible = true;
          
            if (GetEnabledAdd(text))
            {
                try
                {
                    if (Xamarin.Essentials.Connectivity.NetworkAccess != Xamarin.Essentials.NetworkAccess.Internet)
                    {
                        Device.BeginInvokeOnMainThread(async () => await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorNoInternet, "OK"));
                        return;
                    }
                    string ident = Settings.Person.Accounts[PickerLs.SelectedIndex].Ident;
                    string typeId = Settings.TypeApp[PickerType.SelectedIndex].ID;
                    IDResult result = new IDResult();
                    if (isPassAPP)
                    {
                        result = await _server.newAppPass(ident, typeId, text,_passApp.idType, _passApp.Fio,
                            _passApp.SeriaNumber, _passApp.CarBrand, _passApp.CarNumber);

                    }
                    else
                    {
                        result = await _server.newApp(ident, typeId, text);

                    }
                    var update = await _server.GetRequestsUpdates(Settings.UpdateKey, result.ID.ToString());
                    Settings.UpdateKey = update.NewUpdateKey;
                    
                    
                    if (result.Error == null)
                    {
                        sendFiles(result.ID.ToString());
                        await DisplayAlert(AppResources.AlertSuccess, AppResources.AppCreated, "OK");
                       ClosePage();
                    }
                    else
                    {
                        await DisplayAlert(AppResources.ErrorTitle, result.Error, "OK");
                    }
                    MessagingCenter.Send<Object>(this, "UpdateIdent");
                    MessagingCenter.Send<Object>(this, "UpdateEvents");
                    MessagingCenter.Send<Object>(this, "AutoUpdate");
                }
                catch (Exception ex)
                {
                    // ignored
                }
            }

            FrameBtnAdd.IsVisible = true;
            progress.IsVisible = false;
        }

        bool GetEnabledAdd(string text)
        {
            if (isPassAPP)
            {

                if (_passApp.idType != 0)
                {
                    
                    if (_passApp.idType == 1)
                    {
                        _passApp.Fio = EntryFIO.Text;
                        _passApp.SeriaNumber = EntryPassport.Text;
                        _passApp.CarBrand = null;
                        _passApp.CarNumber = null;
                        if (string.IsNullOrWhiteSpace(_passApp.Fio))
                        {
                            DisplayAlert(AppResources.ErrorTitle, AppResources.EnterFIO, "OK");
                            return false;
                        }

                        // if (string.IsNullOrWhiteSpace(_passApp.SeriaNumber))
                        // {
                        //     DisplayAlert(AppResources.ErrorTitle, AppResources.EnterSeriaAndNumber, "OK");
                        //     return false;
                        // }
                        //
                        // if (_passApp.SeriaNumber.Length != 11)
                        // {
                        //     DisplayAlert(AppResources.ErrorTitle,
                        //         AppResources.EnterSeriaAndNumber + " " + AppResources.MaskNumberPassport + ":\n" +
                        //         "1234 567890", "OK");
                        //     return false;
                        // }

                        return true;

                    }
                    else
                    {
                        _passApp.Fio = null;
                        _passApp.SeriaNumber = null;
                        _passApp.CarNumber = EntryNumber.Text;
                        if (string.IsNullOrWhiteSpace(_passApp.CarBrand))
                        {
                            DisplayAlert(AppResources.ErrorTitle, AppResources.EnterCarBrand, "OK");
                            return false;
                        }

                        if (string.IsNullOrWhiteSpace(_passApp.CarNumber))
                        {
                            DisplayAlert(AppResources.ErrorTitle, AppResources.EnterStateNumber, "OK");
                            return false;
                        }
                        else
                        {
                            if (!CheckBoxInNumber.IsChecked)
                            {
                                Regex regexNumberAvto = new Regex(@"^[А-Я]{1}[0-9]{3}[А-Я]{2}[0-9]{2,3}$");
                                Regex regexNumberAvto2 = new Regex(@"[А-Я]{2}[0-9]{3}[0-9]{2,3}$");

                                if (regexNumberAvto.IsMatch(_passApp.CarNumber.Replace(" ","")) ||
                                    regexNumberAvto2.IsMatch(_passApp.CarNumber.Replace(" ","")))
                                {
                                    return true;
                                }
                                else
                                {
                                    DisplayAlert(AppResources.ErrorTitle,
                                        AppResources.EnterStateNumber + " " + AppResources.MaskNumberCar + ":\n" +
                                        "А 234 АА 12, А 234 АА 123, АА 234 22, АА 234 123", "OK");
                                }
                            }
                            else
                            {
                                return true;
                            }
                        }
                    }
                }
                else
                {
                    DisplayAlert(AppResources.ErrorTitle, AppResources.EnterTypePass, "OK");
                }
                
                return false;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(text))
                {
                    DisplayAlert(AppResources.ErrorTitle, AppResources.AppErrorFill, "OK");
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        
        private async void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            FileData select = e.Item as FileData;
            bool answer = await DisplayAlert(AppResources.Delete, AppResources.DeleteFile, AppResources.Yes, AppResources.No);
            if (answer)
            {
                int indexOf = files.IndexOf(@select);
                Byteses.RemoveAt(indexOf);
                files.RemoveAt(indexOf);
                _appModel.Files = files;
                ListViewFiles.ItemsSource = _appModel.Files;
                ListViewFiles.HeightRequest -= 30;
                if (files.Count == 0)
                {
                    ListViewFiles.IsVisible = false;
                }
            }
        }


        async void sendFiles(string id)
        {
            int i = 0; 
            if (Xamarin.Essentials.Connectivity.NetworkAccess != Xamarin.Essentials.NetworkAccess.Internet)
            {
                Device.BeginInvokeOnMainThread(async () => await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorNoInternet, "OK"));
                return;
            }
            foreach (var each in files)
            {
                CommonResult commonResult = await _server.AddFileApps(id, each.FileName, Byteses[i],
                    each.FilePath);
                i++;
            }
        }

        void SetPassApp()
        {
            FrameEntryMess.IsVisible = false;
            LayoutPassApp.IsVisible = true;
        }
        
        void SetDefaultApp()
        {
            FrameEntryMess.IsVisible = true;
            LayoutPassApp.IsVisible = false;
        }
        
        private void pickerType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_appModel.SelectedType.Name.Contains("пропуск"))
            {
                SetPassApp();
                EntryMess.Text = AppResources.NamePassApp;
                isPassAPP = true;
            }
            else
            {
                SetDefaultApp();
                EntryMess.Text = "";
                isPassAPP = false;
            }
        }

        //private void AutoSuggestBox_TextChanged(object sender, AutoSuggestBoxTextChangedEventArgs e)
        //{
        //    // Only get results when it was a user typing, 
        //    // otherwise assume the value got filled in by TextMemberPath 
        //    // or the handler for SuggestionChosen.
        //    if (e.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
        //    {
        //        IEnumerable<string> itemsSource =
        //            from i in _appModel.AllKindPass where i.ToLower().Contains(AutoSuggestBox.Text) select i;
        //        List<string> source = new List<string>(itemsSource);
        //        AutoSuggestBox.ItemsSource = source;
        //    }
        //}

        //private void AutoSuggestBox_QuerySubmitted(object sender, AutoSuggestBoxQuerySubmittedEventArgs e)
        //{
        //}

        //private void AutoSuggestBox_SuggestionChosen(object sender, AutoSuggestBoxSuggestionChosenEventArgs e)
        //{
        //    var eSelectedItem = (string) e.SelectedItem;
        //    if (eSelectedItem != null)
        //    {
        //        _passApp.idType = _appModel.AllKindPass.IndexOf(eSelectedItem) + 1;
        //        // User selected an item from the suggestion list, take an action on it here.
        //        if (eSelectedItem.Equals(AppResources.PassMan))
        //        {
        //            LayoutPeshehod.IsVisible = true;
        //            LayoutAvto.IsVisible = false;
                    
        //        }
        //        else
        //        {
        //            LayoutPeshehod.IsVisible = false;
        //            LayoutAvto.IsVisible = true;
        //        }
        //    }
        //    else
        //    {
        //        // User hit Enter from the search box. Use args.QueryText to determine what to do.
        //        _passApp.idType = 0;
        //    }
        //}
        

       

        private async void EntryNumber_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            string entryNumberText = EntryNumber.Text;
            EntryNumber.Text = entryNumberText.ToUpper();
            Regex regexNumberAvto = new Regex(@"^[А-Я]{1}[0-9]{3}[А-Я]{2}[0-9]{2,3}$");
            Regex regexNumberAvto2 = new Regex(@"[А-Я]{2}[0-9]{3}[0-9]{2,3}$");
            string result = entryNumberText;
            if (!CheckBoxInNumber.IsChecked)
            {
                if (regexNumberAvto.IsMatch(entryNumberText))
                {
                    result = entryNumberText.Insert(1, " ").Insert(5, " ").Insert(8, " ");
                    EntryNumber.Text = result;
                    EntryNumber.MaxLength = 12;

                }
                else if (regexNumberAvto2.IsMatch(entryNumberText))
                {
                    result = entryNumberText.Insert(2, " ").Insert(6, " ");
                    EntryNumber.Text = result;
                    EntryNumber.MaxLength = 10;
                }
            }
        }
        

        private async void EntryNumber_OnFocusChangeRequested(object sender, FocusRequestArgs e)
        {
            if (!e.Focus)
            {
               
            }
        }

       

        //private void AutoSuggestBoxBrand_OnTextChanged(object sender, AutoSuggestBoxTextChangedEventArgs e)
        //{
        //    if (e.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
        //    {
        //        IEnumerable<string> itemsSource =
        //            from i in _appModel.AllBrand where i.ToLower().Contains(AutoSuggestBoxBrand.Text) select i;
        //        List<string> source = new List<string>(itemsSource);
        //        AutoSuggestBoxBrand.ItemsSource = source;
        //    }
        //}

        class PassApp
        {
            public int idType { get; set; } = 0;
            public string CarBrand { get; set;}
            public string CarNumber { get; set;}
            public string Fio { get; set; }
            public string SeriaNumber { get; set; }
        }

        //private void AutoSuggestBoxBrand_OnSuggestionChosen(object sender, AutoSuggestBoxSuggestionChosenEventArgs e)
        //{
        //    var eSelectedItem = (string) e.SelectedItem;
        //    if (eSelectedItem != null)
        //    {
        //        _passApp.CarBrand = eSelectedItem;
        //    }
        //    else
        //    {
        //        _passApp.CarBrand = "";
        //    }
        //}

        private void TSBrandList_OnItemTapped(Object sender, ItemTappedEventArgs e)
        {
            String listsd = e.Item as string;
            TSBrand.Text = listsd;
            TSBrandList.IsVisible = false;
            ((ListView)sender).SelectedItem = null;
            _passApp.CarBrand = listsd;
        }

        private void EntryPassport_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () => { 
            if (EntryPassport.Text.Contains(","))
            {
                EntryPassport.Text = EntryPassport.Text.Replace(",", "");
            }
            });
    }

        private void PassType_Unfocused(object sender, FocusEventArgs e)
        {
            PassTypesList.IsVisible = false;
        }

        private void PassType_Focused(object sender, FocusEventArgs e)
        {
            PassTypesList.IsVisible = true;
        }
                
        private void TSBrand_Unfocused(object sender, FocusEventArgs e)
        {
            TSBrandList.IsVisible = false;
        }

        private void TSBrand_Focused(object sender, FocusEventArgs e)
        {
            TSBrandList.IsVisible = true;
        }
    }
}