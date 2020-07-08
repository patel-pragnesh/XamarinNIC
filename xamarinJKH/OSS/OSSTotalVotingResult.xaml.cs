using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
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
            NavigationPage.SetHasNavigationBar(this, false);
            var techSend = new TapGestureRecognizer();
            techSend.Tapped += async (s, e) => {     await Navigation.PushAsync(new TechSendPage()); };
            LabelTech.GestureRecognizers.Add(techSend);
            //switch (Device.RuntimePlatform)
            //{
            //    case Device.iOS:
            //        BackgroundColor = Color.White;
            //        BackgroundColor = Color.White;
            //        ImageTop.Margin = new Thickness(0, 0, 0, 0);
            //        StackLayout.Margin = new Thickness(0, 33, 0, 0);
            //        IconViewNameUk.Margin = new Thickness(0, 33, 0, 0);
            //        RelativeLayoutTop.Margin = new Thickness(0, 0, 0, 0);
            //        if (App.ScreenHeight <= 667)//iPhone6
            //        {
            //            RelativeLayoutTop.Margin = new Thickness(0, 0, 0, -110);
            //        }
            //        else if (App.ScreenHeight <= 736)//iPhone8Plus Height=736
            //        {
            //            RelativeLayoutTop.Margin = new Thickness(0, 0, 0, -145);
            //        }
            //        else
            //        {
            //            RelativeLayoutTop.Margin = new Thickness(0, 0, 0, -145);
            //        }


            //        break;
            //    case Device.Android:
            //        RelativeLayoutTop.Margin = new Thickness(0, 0, 0, -135);
            //        double or = Math.Round(((double)App.ScreenWidth / (double)App.ScreenHeight), 2);
            //        if (Math.Abs(or - 0.5) < 0.02)
            //        {
            //            RelativeLayoutTop.Margin = new Thickness(0, 0, 0, -90);
            //        }
            //        //else
            //        //{
            //        //    ossContent.Margin = new Thickness(20, 30, 20, 0);
            //        //}

            //        break;
            //    default:
            //        break;
            //}
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    BackgroundColor = Color.White;
                    if (Application.Current.MainPage.Height < 800)
                    {
                        ScrollViewContainer.Margin = new Thickness(10, -180, 10, 0);
                    }
                    else
                    {
                        ScrollViewContainer.Margin = new Thickness(10, -185, 10, 0);
                    }
                    break;
                case Device.Android:
                    break;
                default:
                    break;
            }
            var backClick = new TapGestureRecognizer();
            backClick.Tapped += async (s, e) => { ClosePage(); };
            BackStackLayout.GestureRecognizers.Add(backClick);

            SetDecorations();
            
            SetData(oSS);
        }

        Color colorFromMobileSettings = Color.FromHex(Settings.MobileSettings.color);

        void SetDecorations()
        {
            UkName.Text = Settings.MobileSettings.main_name;
            LabelPhone.Text = "+" + Settings.Person.Phone;
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

            iconViewStatusNameIcon.Source = "ic_ossEnd";
            iconViewStatusNameIcon.Foreground = Color.FromHex("#2f99ac");
            ColorStatusTextString = Color.FromHex("#2f99ac");
            string textStatius = "Итоги голосования";

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

            var cntVotes = "за " + answerTotalCount + "/" + (oSS.TotalAccounts*oSS.Questions.Count).ToString() + ".";
            spanAnswersCnt.Text = cntVotes;

            lCntYes.Text = answerYesCount.ToString();
            cntYes.Foreground = colorFromMobileSettings;
            lCntNo.Text = answerNoCount.ToString();
            cntNo.Foreground = colorFromMobileSettings;

            lCntAbstained.Text = answerAbstainedCount.ToString();
            cntAbstained.Foreground = colorFromMobileSettings;


            delimColored.BackgroundColor = colorFromMobileSettings;

            TotalArea.Text = " " + oSS.VoitingArea.ToString() + " м.кв. = 100%";
            Area.Text = " " + oSS.ComplateArea.ToString() + " м.кв. = " + oSS.ComplateAreaPercents+ "%";

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