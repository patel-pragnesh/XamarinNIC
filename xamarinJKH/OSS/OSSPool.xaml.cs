using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.Messaging;
using Rg.Plugins.Popup.Services;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.PancakeView;
using Xamarin.Forms.Xaml;
using xamarinJKH.DialogViews;
using xamarinJKH.InterfacesIntegration;
using xamarinJKH.Server;
using xamarinJKH.Server.RequestModel;
using xamarinJKH.Tech;
using xamarinJKH.Utils;

namespace xamarinJKH
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OSSPool : ContentPage
    {
        Color colorFromMobileSettings = Color.FromHex(Settings.MobileSettings.color);

        OSS _oss = null;
        private string fileLink = "";

        private List<Label> indicators = new List<Label>();
        private RestClientMP server = new RestClientMP();
        private int quest = 0;
        private bool isCheched = false;
        private readonly bool _isComplite;
        private List<StackLayout> _contentQuest = new List<StackLayout>();

        public OSSPool(OSS oSS)
        {
            InitializeComponent();

            NavigationPage.SetHasNavigationBar(this, false);
            var techSend = new TapGestureRecognizer();
            techSend.Tapped += async (s, e) => {     await PopupNavigation.Instance.PushAsync(new TechDialog()); };
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

            var backClick = new TapGestureRecognizer();
            backClick.Tapped += async (s, e) => { ClosePage(); };
            BackStackLayout.GestureRecognizers.Add(backClick);

            UkName.Text = Settings.MobileSettings.main_name;
            LabelPhone.Text =  "+" + Settings.Person.companyPhone.Replace("+","");

            
            //FrameBack.BackgroundColor = colorFromMobileSettings;
            FrameBtnNext.BackgroundColor = colorFromMobileSettings; 
            FrameBtnFinish.BackgroundColor = colorFromMobileSettings;

           
            var nextClick = new TapGestureRecognizer();
            nextClick.Tapped += async (s, e) => { NextQuest(); };
            FrameBtnNext.GestureRecognizers.Add(nextClick);
            //var backClickQuest = new TapGestureRecognizer();
            //backClickQuest.Tapped += async (s, e) => { BackQuest(); };
            //FrameBack.GestureRecognizers.Add(backClickQuest);
            var finishClick = new TapGestureRecognizer();
            finishClick.Tapped += async (s, e) => { FinishClick(); };
            FrameBtnFinish.GestureRecognizers.Add(finishClick);

            _oss = oSS;

            
            for (int qNow =0;qNow< _oss.Questions.Count;qNow++)
            {
                if (!string.IsNullOrWhiteSpace(_oss.Questions[qNow].Answer))
                    quest++;
                
            }
              

            setIndicator();
            setQuest(); 
            setQuestVisible();
            ChechQuestions();
            setVisibleButton();

            SetQuestion(quest);
            if (quest > 0)
                visibleIndicator(quest, true);
            if(ProtokolStackL.IsVisible)
            {
                urlProtokol.TextColor = colorFromMobileSettings;
                urlProtokol.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = new Command(async () => await Launcher.OpenAsync(fileLink))
                });
            }

            pdf2.Foreground = colorFromMobileSettings;
            Color hexColor = (Color) Application.Current.Resources["MainColor"];
            IconViewLogin.SetAppThemeColor(IconView.ForegroundProperty, hexColor, Color.White);
            IconViewTech.SetAppThemeColor(IconView.ForegroundProperty, hexColor, Color.White);
            Pancake.SetAppThemeColor(PancakeView.BorderColorProperty, hexColor, Color.Transparent);
            PancakeViewIcon.SetAppThemeColor(PancakeView.BorderColorProperty, hexColor, Color.Transparent);
            FramePool.SetAppThemeColor(Frame.BorderColorProperty, hexColor, Color.White);
            LabelTech.SetAppThemeColor(Label.TextColorProperty, hexColor, Color.White);
            Color unselect = hexColor.AddLuminosity(0.3);
            StackLayoutIndicator.SetAppThemeColor(StackLayout.BackgroundColorProperty, unselect, Color.White);
        }

        

        void setQuestVisible()
        {
            Container.Children.Add(_contentQuest[quest]);
        }
        void setQuest()
        {
            int i = 1;                       
            
            foreach (var each in _oss.Questions)
            {                               
                StackLayout containerPolss = new StackLayout();

                StackLayout radio = new StackLayout();                

                radio.Children.Add(GetButton(AppResources.OSSPersonalFor, false));
                radio.Children.Add(GetButton(AppResources.OSSPersonalAgainst, false));
                radio.Children.Add(GetButton(AppResources.OSSPersonalNeutral, false));

                containerPolss.Children.Add(radio);

                _contentQuest.Add(containerPolss);
                i++;
            }
        }

        RadioButton GetButton(string text, bool isChecked)
        {
            RadioButton radioButton = new RadioButton
            {
                Text = text,
                IsChecked = isChecked,
                BackgroundColor = Color.Transparent
            };

            if (isChecked)
            {
                radioButton.TextColor = colorFromMobileSettings;
            }

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    radioButton.BorderColor = colorFromMobileSettings;
                    break;
                case Device.Android:
                    radioButton.Effects.Add(Effect.Resolve("MyEffects.RadioButtonEffect"));
                    break;
            }

            radioButton.BorderColor = colorFromMobileSettings;
            radioButton.Margin = new Thickness(-5, 0, 0, 0);
            radioButton.CheckedChanged += (sender, e) =>
            {
                isCheched = true;
                if (radioButton.IsChecked)
                {   
                    switch(radioButton.Text)
                    {
                        case "За":
                            _oss.Questions[quest].Answer = "0";
                            break;
                        case "Против":
                            _oss.Questions[quest].Answer = "1";
                            break;
                        case "Воздержался":
                            _oss.Questions[quest].Answer = "2";
                            break;
                    }
                    radioButton.TextColor = colorFromMobileSettings;
                }
                else
                {
                    radioButton.TextColor = Color.Black;
                }

                setVisibleButton();
            };

            return radioButton;
        }



        void SetQuestion(int q)
        {
            
            Device.BeginInvokeOnMainThread(() =>
            {
                FormattedString formattedString = new FormattedString();
                formattedString.Spans.Add(new Span
                {
                    Text = AppResources.Question + (q+1) + "/" + _oss.Questions.Count + ".",
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Color.Black,
                    FontSize = 17
                });
                formattedString.Spans.Add(new Span
                {
                    Text = " " + _oss.Questions[q].QuestionMessage,
                    TextColor = Color.Black,
                    FontSize = 17
                });

                questionLabel.FormattedText = formattedString;
                ProtokolStackL.IsVisible = _oss.Questions[q].HasFile;
                fileLink = _oss.Questions[q].FileLink;

            });
            
            
        }

        private async void NextQuest()
        {
            //отправка результата на сервер
          var result = await server.SaveAnswer(new OSSAnswer() { Answer= _oss.Questions[quest].Answer , QuestionId= _oss.Questions[quest].ID});
            if(!string.IsNullOrWhiteSpace(result.Error))
            {
                Device.BeginInvokeOnMainThread(async () => {
                    await DisplayAlert(AppResources.ErrorTitle, result.Error, "OK");
                    return;
                });                
            }
            else
            {
                quest++;
                Device.BeginInvokeOnMainThread(() => {
                    visibleIndicator(quest, true);
                    Container.Children.Clear();
                    Container.Children.Add(_contentQuest[quest]);

                    SetQuestion(quest);

                    ChechQuestions();
                    isCheched = false;
                    setVisibleButton();
                });
            }
        }


        private async void FinishClick()
        {
            //отравка последнего ответа на сервер
            var result = await server.SaveAnswer(new OSSAnswer() { Answer = _oss.Questions[quest].Answer, QuestionId = _oss.Questions[quest].ID });
            if (!string.IsNullOrWhiteSpace(result.Error))
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await DisplayAlert(AppResources.ErrorTitle, result.Error, "OK");
                    return;
                });
            }
            else
            {
                //отправка результата на сервер
                var resultComplite = await server.CompleteVote(_oss.ID);

                if (resultComplite.Error == null)
                {
                    await DisplayAlert(AppResources.AlertSuccess, AppResources.SuccessOSSPollPass, "OK");
                    await Navigation.PushAsync(new OSSPersonalVotingResult(_oss, true));
                    Navigation.RemovePage(this);

                }
                else
                {
                    await DisplayAlert(AppResources.ErrorTitle, $"{AppResources.ErrorOSSPollPass}\n" + result.Error, "OK");
                }
            }
              
        }

       
        private void ChechQuestions()
        {
            if (quest + 1 == _oss.Questions.Count)
            {
                FrameBtnNext.IsVisible = false;
                FrameBtnFinish.IsVisible = true;               
            }
            else
            {
                FrameBtnNext.IsVisible = true;
                FrameBtnFinish.IsVisible = false;
            }
        }

        void setVisibleButton()
        {
            if (_isComplite)
            {
                FrameBtnNext.IsVisible = true;
            }
            else
            {
                FrameBtnNext.IsVisible = isCheched;
            }

            if (!string.IsNullOrWhiteSpace( _oss.Questions[quest].Answer))
            {
                FrameBtnNext.IsVisible = true;
            }

            if (quest + 1 == _oss.Questions.Count)
            {
                FrameBtnNext.IsVisible = false;
                if (_isComplite)
                {
                    FrameBtnFinish.IsVisible = false;
                }
                else
                {
                    FrameBtnFinish.IsVisible = isCheched;
                }

                if (!string.IsNullOrWhiteSpace(_oss.Questions[quest].Answer))
                {
                    FrameBtnFinish.IsVisible = !_isComplite;
                }
            }
        }



        void setIndicator()
        {
            Grid grid = new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition {Height = new GridLength(1, GridUnitType.Star)},
                }
            };
            int i = 0;
            
            double width = StackLayoutIndicator.WidthRequest / _oss.Questions.Count;
            foreach (var each in _oss.Questions)
            {
                Color backgroundColor = colorFromMobileSettings;
                if (i > 0)
                {
                    backgroundColor = Color.Transparent;
                }

                Label indicator = new Label
                {
                    HeightRequest = 2,
                    WidthRequest = width,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    BackgroundColor = backgroundColor
                };
                indicators.Add(indicator);
                ColumnDefinition columnDefinition = new ColumnDefinition();
                columnDefinition.Width = new GridLength(1, GridUnitType.Star);
                grid.ColumnDefinitions.Add(columnDefinition);
                grid.Children.Add(indicator, i, 0);
                grid.HorizontalOptions = LayoutOptions.FillAndExpand;
                i++;
            }

            StackLayoutIndicator.Children.Add(grid);
        }

        void visibleIndicator(int i, bool isNext)
        {
            if (isNext)
            {
                indicators[i].BackgroundColor = colorFromMobileSettings;
                indicators[i - 1].BackgroundColor = Color.Transparent;
            }
            else
            {
                indicators[i + 1].BackgroundColor = Color.Transparent;
                indicators[i].BackgroundColor = colorFromMobileSettings;
            }
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