using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.Messaging;
using Rg.Plugins.Popup.Services;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PancakeView;
using Xamarin.Forms.Xaml;
using xamarinJKH.Additional;
using xamarinJKH.DialogViews;
using xamarinJKH.InterfacesIntegration;
using xamarinJKH.Questions;
using xamarinJKH.Server;
using xamarinJKH.Server.RequestModel;
using xamarinJKH.Tech;
using xamarinJKH.Utils;
using FileInfo = xamarinJKH.Server.RequestModel.FileInfo;

namespace xamarinJKH.Notifications
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NotificationOnePage : ContentPage
    {
        private AnnouncementInfo _announcementInfo;
        private AdditionalService _additional;
        private PollInfo _polls;
        private RestClientMP _server = new RestClientMP();
        
        public List<FileInfo> Files { get; set; }

        public NotificationOnePage(AnnouncementInfo announcementInfo)
        {
            
            _announcementInfo = announcementInfo;
            InitializeComponent();
            CollectionViewFiles.ItemsLayout = new GridItemsLayout(2, ItemsLayoutOrientation.Vertical);
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    int statusBarHeight = DependencyService.Get<IStatusBar>().GetHeight();
                    Pancake.Padding = new Thickness(0, statusBarHeight, 0, 0);
                    //BackgroundColor = Color.White;
                    break;
                default:
                    break;
            }

            NavigationPage.SetHasNavigationBar(this, false);
            var techSend = new TapGestureRecognizer();
            techSend.Tapped += async (s, e) => {  await PopupNavigation.Instance.PushAsync(new TechDialog());    };
            LabelTech.GestureRecognizers.Add(techSend);
            var call = new TapGestureRecognizer();
            call.Tapped += async (s, e) =>
            {
                if (Settings.Person.Phone != null)
                {
                    IPhoneCallTask phoneDialer;
                    phoneDialer = CrossMessaging.Current.PhoneDialer;
                    if (phoneDialer.CanMakePhoneCall) 
                        phoneDialer.MakePhoneCall(Settings.Person.companyPhone);
                }

            
            };
            LabelPhone.GestureRecognizers.Add(call);
            var backClick = new TapGestureRecognizer();
            backClick.Tapped += async (s, e) => { close(); };
            BackStackLayout.GestureRecognizers.Add(backClick);
            SetText();
            Files = announcementInfo.Files;
            BindingContext = this;
        }

        async void SetText()
        {
            UkName.Text = Settings.MobileSettings.main_name;
            LabelPhone.Text =  "+" + Settings.Person.companyPhone.Replace("+","");
            LabelTitle.Text = _announcementInfo.Header;
            LabelDate.Text = _announcementInfo.Created;
            LabelText.Text = _announcementInfo.Text;
            FrameBtnQuest.BackgroundColor = Color.FromHex(Settings.MobileSettings.color);
            int announcementInfoAdditionalServiceId = _announcementInfo.AdditionalServiceId;
            if (announcementInfoAdditionalServiceId != -1)
            {
                _additional = Settings.GetAdditionalService(announcementInfoAdditionalServiceId);
                byte[] imageByte = await _server.GetPhotoAdditional(announcementInfoAdditionalServiceId.ToString());
                Stream stream = new MemoryStream(imageByte);
                ImageAdd.Source = ImageSource.FromStream(() => { return stream; });
                ImageAdd.IsVisible = true;

                var openAdditional = new TapGestureRecognizer();
                openAdditional.Tapped += async (s, e) =>
                {
                    await Navigation.PushAsync(new AdditionalOnePage(_additional));
                };
                ImageAdd.GestureRecognizers.Add(openAdditional);
            }

            if (_announcementInfo.QuestionGroupID != -1)
            {
                _polls = Settings.GetPollInfo(_announcementInfo.QuestionGroupID);
                FrameBtnQuest.IsVisible = true;
            }
            
            Color hexColor = (Color) Application.Current.Resources["MainColor"];
            IconViewLogin.SetAppThemeColor(IconView.ForegroundProperty, hexColor, Color.White);
            IconViewTech.SetAppThemeColor(IconView.ForegroundProperty, hexColor, Color.Black);
            Pancake.SetAppThemeColor(PancakeView.BorderColorProperty, hexColor, Color.Transparent);
            PancakeViewIcon.SetAppThemeColor(PancakeView.BorderColorProperty, hexColor, Color.Transparent);
            LabelTech.SetAppThemeColor(Label.TextColorProperty, hexColor, Color.Black);
        }

        async void open(Page page)
        {
            try
            {
                await Navigation.PushAsync(page);
            }
            catch (Exception e)
            {
                await Navigation.PushModalAsync(page);
            }
        }
        async void close()
        {
            try
            {
                await Navigation.PopAsync();
            }
            catch (Exception e)
            {
                await Navigation.PopModalAsync();
            }
        }

        private async void ButtonClick(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PollsPage(_polls, false));
        }

        private async void OnCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FileInfo select = (e.CurrentSelection.FirstOrDefault() as FileInfo);
            try
            {
                if (@select != null) await Launcher.OpenAsync(@select.Link);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorAdditionalLink, "OK");
            }
        }
    }
}