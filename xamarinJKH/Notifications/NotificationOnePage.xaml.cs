using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamarinJKH.Additional;
using xamarinJKH.Server;
using xamarinJKH.Server.RequestModel;
using xamarinJKH.Utils;

namespace xamarinJKH.Notifications
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NotificationOnePage : ContentPage
    {
        private AnnouncementInfo _announcementInfo;
        private AdditionalService _additional;
        private PollInfo _polls;
        private RestClientMP _server = new RestClientMP();

        public NotificationOnePage(AnnouncementInfo announcementInfo)
        {
            _announcementInfo = announcementInfo;
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            var backClick = new TapGestureRecognizer();
            backClick.Tapped += async (s, e) => { _ = await Navigation.PopAsync(); };
            BackStackLayout.GestureRecognizers.Add(backClick);
            SetText();
        }

        async void SetText()
        {
            UkName.Text = Settings.MobileSettings.main_name;
            LabelPhone.Text = "+" + Settings.Person.Phone;
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
        }

        private async void ButtonClick(object sender, EventArgs e)
        {
            await DisplayAlert("Пройти опрос?", _polls.Name, "OK");
            ;
        }
    }
}