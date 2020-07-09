using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.Messaging;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamarinJKH.InterfacesIntegration;
using xamarinJKH.Server.RequestModel;
using xamarinJKH.Tech;
using xamarinJKH.Utils;

namespace xamarinJKH
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OSSPersonalVotingResult : ContentPage
    {
        Color colorFromMobileSettings = Color.FromHex(Settings.MobileSettings.color);

        public OSSPersonalVotingResult(OSS oSS, bool userFinishPool=false)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

            if(userFinishPool)
            {
                listNeedUpdate = userFinishPool;
            }
            var techSend = new TapGestureRecognizer();
            techSend.Tapped += async (s, e) => {     await Navigation.PushAsync(new TechSendPage()); };
            LabelTech.GestureRecognizers.Add(techSend);
            var call = new TapGestureRecognizer();
            call.Tapped += async (s, e) =>
            {
                if (Settings.Person.Phone != null)
                {
                    IPhoneCallTask phoneDialer;
                    phoneDialer = CrossMessaging.Current.PhoneDialer;
                    if (phoneDialer.CanMakePhoneCall) 
                        phoneDialer.MakePhoneCall(Settings.Person.Phone);
                }

            
            };
            LabelPhone.GestureRecognizers.Add(call);
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

        }

        void SetDecorations()
        {
            UkName.Text = Settings.MobileSettings.main_name;
            LabelPhone.Text =  "+" + Settings.Person.companyPhone.Replace("+","");
            Btn.BackgroundColor = colorFromMobileSettings;


            
            StackLayout statusNameIcon = new StackLayout() { Orientation = StackOrientation.Vertical, HorizontalOptions = LayoutOptions.FillAndExpand };

            Label statusName = new Label()
            {
                Text = "Статус собрания: ",
                FontAttributes = FontAttributes.Bold,
                FontSize = 14,
                TextColor = Color.White,
                HorizontalOptions = LayoutOptions.Start
            };

            statusNameIcon.Children.Add(statusName);

            StackLayout coloredStatus = new StackLayout() { HorizontalOptions = LayoutOptions.Start, Orientation = StackOrientation.Horizontal };
            IconView iconViewStatusNameIcon = new IconView();

            Color ColorStatusTextString;
            
            iconViewStatusNameIcon.Source = "ic_status_done";
            iconViewStatusNameIcon.Foreground = Color.FromHex("#50ac2f");
            ColorStatusTextString = Color.FromHex("#50ac2f");
            string textStatius = "Ваш голос учтён";

            iconViewStatusNameIcon.HeightRequest = 15;
            iconViewStatusNameIcon.WidthRequest = 15;
            iconViewStatusNameIcon.VerticalOptions = LayoutOptions.Center;
            iconViewStatusNameIcon.HorizontalOptions = LayoutOptions.Start;

            coloredStatus.Children.Add(iconViewStatusNameIcon);

            Label statSting = new Label() { Text = textStatius, FontSize = 12, TextColor = ColorStatusTextString, HorizontalOptions = LayoutOptions.Start };
            coloredStatus.Children.Add(statSting);

            statusNameIcon.Children.Add(coloredStatus);

            status.Children.Add(statusNameIcon);

        }

        private void SetData(OSS oSS)
        {
            //проголосовано * из *
            spanAnswersCnt.TextColor = colorFromMobileSettings;
            var cntVotes = "за "+ oSS.Questions.Where(_ => !string.IsNullOrWhiteSpace(_.Answer)).Count().ToString() + " / " + oSS.Questions.Count.ToString()+".";
            spanAnswersCnt.Text = cntVotes;

            //ответы по штукам
            lCntYes.Text = oSS.Questions.Where(_ => _.Answer == "0").Count().ToString();
            cntYes.Foreground = colorFromMobileSettings;
            lCntNo.Text = oSS.Questions.Where(_ => _.Answer == "1").Count().ToString();
            cntNo.Foreground = colorFromMobileSettings;

            lCntAbstained.Text = oSS.Questions.Where(_ => _.Answer == "2").Count().ToString();
            cntAbstained.Foreground = colorFromMobileSettings;


            delimColored.BackgroundColor = colorFromMobileSettings;

            TotalArea.Text = " "+ oSS.VoitingArea.ToString()+ " м.кв. = 100%";
            Area.Text= " " + oSS.Accounts[0].Area.ToString() + " м.кв. = "+ Math.Round(oSS.Accounts[0].Area/oSS.VoitingArea*100,3) + "%";
            dayEnd.Text = oSS.DateEnd.Split(' ')[0];
            var r1Date = oSS.ResultsReleaseDate.Split(' ')[0];
            var r1Time = oSS.ResultsReleaseDate.Split(' ')[1];
            dayEndPlus.Text = $" заключительный день голосования. Итоги голосования будут доступны {r1Date} в {r1Time}" +
                $" местного времени. Когда результаты голосования будут подсчитаны, Вы получите уведомление в формате Push сообщения и доступ к \"Протоколу ОСС\" с пакетом необходимых документов.";



        }

        bool listNeedUpdate = false;

        private async void Btn_Clicked(object sender, EventArgs e)
        {            
            if (listNeedUpdate)
                await Navigation.PushAsync(new OSSMain());
            else
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