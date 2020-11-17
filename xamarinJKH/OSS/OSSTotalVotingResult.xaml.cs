using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AppCenter.Analytics;
using Plugin.Messaging;
using Rg.Plugins.Popup.Services;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PancakeView;
using Xamarin.Forms.Xaml;
using xamarinJKH.DialogViews;
using xamarinJKH.InterfacesIntegration;
using xamarinJKH.Server.RequestModel;
using xamarinJKH.Tech;
using xamarinJKH.Utils;

namespace xamarinJKH
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OSSTotalVotingResult : ContentPage
    {
        public OSSTotalVotingResult(OSS oSS)
        {
            InitializeComponent();
            Analytics.TrackEvent("Общие результаты голосования ОСС");
            NavigationPage.SetHasNavigationBar(this, false);
            var techSend = new TapGestureRecognizer();
            techSend.Tapped += async (s, e) => {    await PopupNavigation.Instance.PushAsync(new TechDialog()); };
            LabelTech.GestureRecognizers.Add(techSend);
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
           


            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    int statusBarHeight = DependencyService.Get<IStatusBar>().GetHeight();
                    //BackgroundColor = Color.White;
                    break;
                default:
                    break;
            }

            var dH = Xamarin.Essentials.DeviceDisplay.MainDisplayInfo.Height;
            if (dH < 1400)
            {
                titleLabel.FontSize = 18;
            }

            var backClick = new TapGestureRecognizer();
            backClick.Tapped += async (s, e) => { ClosePage(); };
            BackStackLayout.GestureRecognizers.Add(backClick);

            SetDecorations();
            
            SetData(oSS);
            Color hexColor = (Color) Application.Current.Resources["MainColor"];
            IconViewTech.SetAppThemeColor(IconView.ForegroundProperty, hexColor, Color.White);
            PancakeBot.SetAppThemeColor(PancakeView.BorderColorProperty, hexColor, Color.Transparent);
            FrameResult.SetAppThemeColor(Frame.BorderColorProperty, hexColor, Color.White);
            LabelTech.SetAppThemeColor(Label.TextColorProperty, hexColor, Color.White);
        }

        Color colorFromMobileSettings = (Color)Application.Current.Resources["MainColor"];

        void SetDecorations()
        {
            UkName.Text = Settings.MobileSettings.main_name;
           
            Btn.BackgroundColor = colorFromMobileSettings;

            StackLayout statusNameIcon = new StackLayout() { Orientation = StackOrientation.Vertical, HorizontalOptions = LayoutOptions.FillAndExpand };

            Label statusName = new Label()
            {
                Text = $"{AppResources.OSSInfoStatus} ",
                FontAttributes = FontAttributes.Bold,
                FontSize = 14,
                TextColor = Color.White,
                HorizontalOptions = LayoutOptions.Start
            };
            statusName.SetAppThemeColor(Label.TextColorProperty, Color.Black, Color.White);
            statusNameIcon.Children.Add(statusName);

            StackLayout coloredStatus = new StackLayout() { HorizontalOptions = LayoutOptions.Start, Orientation = StackOrientation.Horizontal };
            IconView iconViewStatusNameIcon = new IconView();

            Color ColorStatusTextString;

            iconViewStatusNameIcon.Source = "ic_ossEnd";
            iconViewStatusNameIcon.Foreground = Color.FromHex("#2f99ac");
            ColorStatusTextString = Color.FromHex("#2f99ac");
            string textStatius = AppResources.OSSVotingResult;

            iconViewStatusNameIcon.HeightRequest = 15;
            iconViewStatusNameIcon.WidthRequest = 15;
            iconViewStatusNameIcon.VerticalOptions = LayoutOptions.Center;
            iconViewStatusNameIcon.HorizontalOptions = LayoutOptions.Start;

            coloredStatus.Children.Add(iconViewStatusNameIcon);

            Label statSting = new Label() { Text = textStatius, FontSize = 12, TextColor = ColorStatusTextString, HorizontalOptions = LayoutOptions.Start };
            coloredStatus.Children.Add(statSting);

            statusNameIcon.Children.Add(coloredStatus);

            status.Children.Add(statusNameIcon);

            pdf1.Source= "ic_export_pdf";
            pdf1.Foreground= colorFromMobileSettings;
            pdf2.Source = "ic_export_pdf";
            pdf2.Foreground = colorFromMobileSettings;

        }

        private void SetData(OSS oSS)
        {
            //проголосовано * из *
            spanAnswersCnt.TextColor = colorFromMobileSettings;
            int answerTotalCount = 0;

            int answerYesCount = 0;
            int answerNoCount = 0;
            int answerAbstainedCount = 0;

            foreach (var q in oSS.Questions)
            {
                answerTotalCount += q.AnswerTotalCount;
                answerYesCount += q.CountWhyVoiteYes;
                answerNoCount += q.CountWhyVoiteNo;
                answerAbstainedCount += q.CountWhyVoiteUnknow;
            }

            var cntVotes = $"{AppResources.OSSPersonalFor} " + answerTotalCount + "/" + (oSS.TotalAccounts*oSS.Questions.Count).ToString() + ".";
            spanAnswersCnt.Text = cntVotes;

            lCntYes.Text = answerYesCount.ToString();
            cntYes.Foreground = colorFromMobileSettings;
            lCntNo.Text = answerNoCount.ToString();
            cntNo.Foreground = colorFromMobileSettings;

            lCntAbstained.Text = answerAbstainedCount.ToString();
            cntAbstained.Foreground = colorFromMobileSettings;


            delimColored.BackgroundColor = colorFromMobileSettings;

            TotalArea.Text = " " + oSS.VoitingArea.ToString() + $" {AppResources.OSSInfoMeasurmentArea} = 100%";
            Area.Text = " " + oSS.ComplateArea.ToString() + $" {AppResources.OSSInfoMeasurmentArea} = " + oSS.ComplateAreaPercents+ "%";

            //ссылки на документы  - Макс по идее должен сказать откуда взять.
            //urlBlank.TextColor = colorFromMobileSettings;
            //urlBlank.GestureRecognizers.Add(new TapGestureRecognizer
            //{
            //    Command = new Command(async () => await Launcher.OpenAsync(oSS.AdminstratorSite))
            //});

            ProtokolStackL.IsVisible = oSS.HasProtocolFile;
            if(ProtokolStackL.IsVisible)
            {
                urlProtokol.TextColor = colorFromMobileSettings;
                urlProtokol.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = new Command(async () => await Launcher.OpenAsync(oSS.ProtocolFileLink))
                });
            }
            

        }

        private async void Btn_Clicked(object sender, EventArgs e)
        {
            PopUntilDestination(typeof(OSSMain));
        }

        void PopUntilDestination(Type DestinationPage)
        {
            int LeastFoundIndex = 0;
            int PagesToRemove = 0;

            for (int index = Navigation.NavigationStack.Count - 2; index > 0; index--)
            {
                if (Navigation.NavigationStack[index].GetType().Equals(DestinationPage))
                {
                    break;
                }
                else
                {
                    LeastFoundIndex = index;
                    PagesToRemove++;
                }
            }

            for (int index = 0; index < PagesToRemove; index++)
            {
                Navigation.RemovePage(Navigation.NavigationStack[LeastFoundIndex]);
            }

            ClosePage();
        }
        public bool IsRefreshing {get; set; }
        async void ClosePage()
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

    }
}