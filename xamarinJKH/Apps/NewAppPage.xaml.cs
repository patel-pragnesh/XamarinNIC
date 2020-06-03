using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamarinJKH.Main;
using xamarinJKH.Server;
using xamarinJKH.Server.RequestModel;
using xamarinJKH.Utils;

namespace xamarinJKH.Apps
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewAppPage : ContentPage
    {
        private RestClientMP _server = new RestClientMP();
        public List<FileData> files { get; set; }
        public List<byte[]> Byteses = new List<byte[]>();
        private AppsPage _appsPage;

        public NewAppPage(AppsPage appsPage)
        {
            _appsPage = appsPage;
            InitializeComponent();
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    BackgroundColor = Color.White;
                    ImageTop.Margin = new Thickness(0, 0, 0, 0);
                    StackLayout.Margin = new Thickness(0, 33, 0, 0);
                    IconViewNameUk.Margin = new Thickness(0, 33, 0, 0);
                    break;
                case Device.Android:
                    double or = Math.Round(((double) App.ScreenWidth / (double) App.ScreenHeight), 2);
                    if (Math.Abs(or - 0.5) < 0.02)
                    {
                        ScrollViewContainer.Margin = new Thickness(0,0,0,-115);
                        BackStackLayout.Margin = new Thickness(-5, 15, 0, 0);
                    }
                    break;
                default:
                    break;
            }
            NavigationPage.SetHasNavigationBar(this, false);
            var backClick = new TapGestureRecognizer();
            backClick.Tapped += async (s, e) => { _ = await Navigation.PopAsync(); };
            BackStackLayout.GestureRecognizers.Add(backClick);
            var addFile = new TapGestureRecognizer();
            addFile.Tapped += async (s, e) => { AddFile(); };
            StackLayoutAddFile.GestureRecognizers.Add(addFile);

            SetText();
            files = new List<FileData>();
            BindingContext = new AddAppModel()
            {
                AllAcc = Settings.Person.Accounts,
                AllType = Settings.TypeApp,
                hex = Color.FromHex(Settings.MobileSettings.color),
                SelectedAcc = Settings.Person.Accounts[0],
                SelectedType = Settings.TypeApp[0],
                Files = files
            };
            ListViewFiles.Effects.Add(Effect.Resolve("MyEffects.ListViewHighlightEffect"));
        }

        private async void AddFile()
        {
            await PickAndShowFile(null);
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
                        await DisplayAlert("Ошибка", "Размер файла превышает 10мб" , "OK");
                        return;
                    }
                    files.Add(pickedFile);
                    Byteses.Add(pickedFile.DataArray);
                    ListViewFiles.IsVisible = true;
                    if (ListViewFiles.HeightRequest < 120)
                        ListViewFiles.HeightRequest += 30;
                    setBinding();
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
                await DisplayAlert("Ошибка", ex.ToString(), "OK");
            }
        }

        void SetText()
        {
            UkName.Text = Settings.MobileSettings.main_name;
            LabelPhone.Text = "+" + Settings.Person.Phone;
        }

        void setBinding()
        {
            BindingContext = null;
            BindingContext = new AddAppModel()
            {
                AllAcc = Settings.Person.Accounts,
                AllType = Settings.TypeApp,
                hex = Color.FromHex(Settings.MobileSettings.color),
                SelectedAcc = Settings.Person.Accounts[0],
                SelectedType = Settings.TypeApp[0],
                Files = files
            };
        }

        private void picker_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        public class AddAppModel
        {
            public List<AccountInfo> AllAcc { get; set; }
            public List<NamedValue> AllType { get; set; }
            public AccountInfo SelectedAcc { get; set; }
            public NamedValue SelectedType { get; set; }

            public List<FileData> Files { get; set; }
            public Color hex { get; set; }
        }

        private async void addApp(object sender, EventArgs e)
        {
            string text = EntryMess.Text;
            FrameBtnAdd.IsVisible = false;
            progress.IsVisible = true;
            if (!text.Equals(""))
            {
                try
                {
                    string ident = Settings.Person.Accounts[PickerLs.SelectedIndex].Ident;
                    string typeId = Settings.TypeApp[PickerLs.SelectedIndex].ID;
                    IDResult result = await _server.newApp(ident, typeId, text);


                    if (result.Error == null)
                    {
                        sendFiles(result.ID.ToString());
                        await DisplayAlert("Успешно", "Заявка успешно создана", "OK");
                        await Navigation.PopAsync();
                    }
                    else
                    {
                        await DisplayAlert("Ошибка", result.Error, "OK");
                    }
                }
                catch (Exception ex)
                {
                    // ignored
                }
            }
            else
            {
                await DisplayAlert("Ошибка", "Заполните описание заявки", "OK");
            }

            FrameBtnAdd.IsVisible = true;
            progress.IsVisible = false;
        }

        private async void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            FileData select = e.Item as FileData;
            bool answer = await DisplayAlert("Удаление", "Удалить файл?", "Да", "Нет");
            if (answer)
            {
                int indexOf = files.IndexOf(@select);
                Byteses.RemoveAt(indexOf);
                files.RemoveAt(indexOf);
                ListViewFiles.HeightRequest -= 30;
                if (files.Count == 0)
                {
                    ListViewFiles.IsVisible = false;
                }

                setBinding();
            }
        }

        async void sendFiles(string id)
        {
            int i = 0;
            foreach (var each in files)
            {
                CommonResult commonResult = await _server.AddFileApps(id, each.FileName, Byteses[i],
                    each.FilePath);
                i++;
            }
        }
        
    }
}