using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamarinJKH.Server.RequestModel;
using xamarinJKH.Utils;

namespace xamarinJKH
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OSSPersonalVotingResult : ContentPage
    {
        Color colorFromMobileSettings = Color.FromHex(Settings.MobileSettings.color);

        public OSSPersonalVotingResult(OSS oSS)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    BackgroundColor = Color.White;
                    BackgroundColor = Color.White;
                    ImageTop.Margin = new Thickness(0, 0, 0, 0);
                    StackLayout.Margin = new Thickness(0, 33, 0, 0);
                    IconViewNameUk.Margin = new Thickness(0, 33, 0, 0);
                    RelativeLayoutTop.Margin = new Thickness(0, 0, 0, 0);
                    if (App.ScreenHeight <= 667)//iPhone6
                    {
                        RelativeLayoutTop.Margin = new Thickness(0, 0, 0, -110);
                    }
                    else if (App.ScreenHeight <= 736)//iPhone8Plus Height=736
                    {
                        RelativeLayoutTop.Margin = new Thickness(0, 0, 0, -145);
                    }
                    else
                    {
                        RelativeLayoutTop.Margin = new Thickness(0, 0, 0, -145);
                    }


                    break;
                case Device.Android:
                    RelativeLayoutTop.Margin = new Thickness(0, 0, 0, -135);
                    double or = Math.Round(((double)App.ScreenWidth / (double)App.ScreenHeight), 2);
                    if (Math.Abs(or - 0.5) < 0.02)
                    {
                        RelativeLayoutTop.Margin = new Thickness(0, 0, 0, -90);
                    }
                    //else
                    //{
                    //    ossContent.Margin = new Thickness(20, 30, 20, 0);
                    //}

                    break;
                default:
                    break;
            }

            var backClick = new TapGestureRecognizer();
            backClick.Tapped += async (s, e) => { _ = await Navigation.PopAsync(); };
            BackStackLayout.GestureRecognizers.Add(backClick);

            SetDecorations();



            SetData(oSS);

        }

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

            //отвепты по штукам
            lCntYes.Text = oSS.Questions.Where(_ => _.Answer == "0").Count().ToString();
            cntYes.BackgroundColor = colorFromMobileSettings;
            lCntNo.Text = oSS.Questions.Where(_ => _.Answer == "1").Count().ToString();
            cntNo.BackgroundColor = colorFromMobileSettings;

            lCntAbstained.Text = oSS.Questions.Where(_ => _.Answer == "2").Count().ToString();
            cntAbstained.BackgroundColor = colorFromMobileSettings;


            delimColored.BackgroundColor = colorFromMobileSettings;

            TotalArea.Text = " "+ oSS.VoitingArea.ToString()+ " м.кв. = 100%";
            Area.Text= " " + oSS.Accounts[0].Area.ToString() + " м.кв. = "+ Math.Round(oSS.Accounts[0].Area/oSS.VoitingArea*100,3) + "%";
            dayEnd.Text = oSS.DateEnd.Split(' ')[0];
            var r1Date = oSS.ResultsReleaseDate.Split(' ')[0];
            var r1Time = oSS.ResultsReleaseDate.Split(' ')[1];
            dayEndPlus.Text = $" заключительный день голосования. Итоги голосования будут доступны {r1Date} в {r1Time}" +
                $" местного времени. Когда результаты голосования будут подсчитаны, Вы получите уведомление в формате Push сообщения и доступ к \"Протоколу ОСС\" с пакетом необходимых документов.";



        }


        private async void Btn_Clicked(object sender, EventArgs e)
        {
            //for (var counter = 1; counter < 2; counter++)
            //{
            //Navigation.RemovePage(Navigation.NavigationStack[Navigation.NavigationStack.Count - 1]);
            //}
            PopUntilDestination(typeof(OSSMain));
            //await Navigation.PopAsync();
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

            Navigation.PopAsync();
        }
    }
}