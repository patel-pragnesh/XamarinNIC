using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamarinJKH.Server;
using xamarinJKH.Server.RequestModel;
using xamarinJKH.Utils;

namespace xamarinJKH.Questions
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PollsPage : ContentPage
    {
        private PollInfo _pollInfo;
        private readonly bool _isComplite;
        private List<StackLayout> _contentQuest = new List<StackLayout>();
        private RestClientMP server = new RestClientMP();
        private int quest = 0;
        private bool isCheched = false;
        private List<Label> indicators = new List<Label>();

        private PollingResult _pollingResult = new PollingResult();

        public PollsPage(PollInfo pollInfo, bool isComplite)
        {
            _pollInfo = pollInfo;
            _isComplite = isComplite;
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
                        LabelPrev.FontSize = LabelFinish.FontSize = LabelNext.FontSize = 13;
                    }
                    break;
                default:
                    break;
            }

            NavigationPage.SetHasNavigationBar(this, false);
            var backClick = new TapGestureRecognizer();
            backClick.Tapped += async (s, e) => { _ = await Navigation.PopAsync(); };
            BackStackLayout.GestureRecognizers.Add(backClick);
            var nextClick = new TapGestureRecognizer();
            nextClick.Tapped += async (s, e) => { NextQuest(); };
            FrameBtnNext.GestureRecognizers.Add(nextClick);
            var backClickQuest = new TapGestureRecognizer();
            backClickQuest.Tapped += async (s, e) => { BackQuest(); };
            FrameBack.GestureRecognizers.Add(backClickQuest);
            var finishClick = new TapGestureRecognizer();
            finishClick.Tapped += async (s, e) => { FinishClick(); };
            FrameBtnFinish.GestureRecognizers.Add(finishClick);
            SetText();
            setQuest();
            setQuestVisible();
            ChechQuestions();
            setVisibleButton();
            setIndicator();
            if (_isComplite)
            {
                Container.IsEnabled = false;
            }
        }

        private async void FinishClick()
        {
            CommonResult result = await server.SaveResultPolls(_pollingResult);
            if (result.Error == null)
            {
                await DisplayAlert("Успешно", "Ответы успешно переданы", "OK");
                _ = await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Ошибка", "Не удалось передать ответы\n" + result.Error, "OK");
            }
        }

        private void ChechQuestions()
        {
            if (quest + 1 == _pollInfo.Questions.Count)
            {
                FrameBtnNext.IsVisible = false;
                FrameBtnFinish.IsVisible = true;
                if (quest > 0)
                {
                    FrameBack.IsVisible = true;
                }
                else
                {
                    FrameBack.IsVisible = false;
                }
            }
            else
            {
                FrameBtnNext.IsVisible = true;
                FrameBtnFinish.IsVisible = false;

                if (quest > 0)
                {
                    FrameBack.IsVisible = true;
                }
                else
                {
                    FrameBack.IsVisible = false;
                }
            }
        }

        void SetText()
        {
            UkName.Text = Settings.MobileSettings.main_name;
            LabelPhone.Text = "+" + Settings.Person.Phone;
            LabelTitle.Text = _pollInfo.Name;

            FrameBack.BackgroundColor = Color.FromHex("#4A4A4A");
            FrameBtnNext.BackgroundColor = Color.FromHex(Settings.MobileSettings.color);
            FrameBtnFinish.BackgroundColor = Color.FromHex(Settings.MobileSettings.color);
        }

        void setQuest()
        {
            int i = 1;
            _pollingResult.Answers = new List<PollAnswer>();
            _pollingResult.PollId = _pollInfo.ID;
            _pollingResult.ExtraInfo = "";
            foreach (var each in _pollInfo.Questions)
            {
                PollAnswer pollAnswer = new PollAnswer()
                {
                    QuestionId = each.ID,
                    AnswerId = -1
                };
                _pollingResult.Answers.Add(pollAnswer);
                StackLayout containerPolss = new StackLayout();
                Label questions = new Label();
                FormattedString formattedString = new FormattedString();
                formattedString.Spans.Add(new Span
                {
                    Text = "Вопрос " + i + "/" + _pollInfo.Questions.Count + ".",
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Color.Black,
                    FontSize = 17
                });
                formattedString.Spans.Add(new Span
                {
                    Text = " " + each.Text,
                    TextColor = Color.Black,
                    FontSize = 17
                });

                questions.FormattedText = formattedString;
                containerPolss.Children.Add(questions);
                StackLayout radio = new StackLayout();
                foreach (var jAnswer in each.Answers)
                {
                    RadioButton radioButton = new RadioButton
                    {
                        Text = jAnswer.Text,
                        IsChecked = jAnswer.IsUserAnswer,
                        BackgroundColor = Color.Transparent
                    };
                    isCheched = jAnswer.IsUserAnswer;
                    if (jAnswer.IsUserAnswer)
                    {
                        radioButton.TextColor = Color.FromHex(Settings.MobileSettings.color);
                    }

                    switch (Device.RuntimePlatform)
                    {
                        case Device.iOS:
                            radioButton.BorderColor = Color.FromHex(Settings.MobileSettings.color);
                            break;
                        case Device.Android:
                            radioButton.Effects.Add(Effect.Resolve("MyEffects.RadioButtonEffect"));
                            break;
                    }

                    radioButton.BorderColor = Color.Red;
                    radioButton.Margin = new Thickness(-5, 0, 0, 0);
                    radioButton.CheckedChanged += (sender, e) =>
                    {
                        isCheched = true;
                        if (radioButton.IsChecked)
                        {
                            pollAnswer.AnswerId = jAnswer.ID;
                            radioButton.TextColor = Color.FromHex(Settings.MobileSettings.color);
                        }
                        else
                        {
                            radioButton.TextColor = Color.Black;
                        }

                        setVisibleButton();
                    };

                    radio.Children.Add(radioButton);
                }

                containerPolss.Children.Add(radio);

                _contentQuest.Add(containerPolss);
                i++;
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
            double width = StackLayoutIndicator.WidthRequest / _pollInfo.Questions.Count;
            foreach (var each in _pollInfo.Questions)
            {
                Color backgroundColor = Color.FromHex(Settings.MobileSettings.color);
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

            if (_pollingResult.Answers[quest].AnswerId != -1)
            {
                FrameBtnNext.IsVisible = true;
            }

            if (quest + 1 == _pollInfo.Questions.Count)
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

                if (_pollingResult.Answers[quest].AnswerId != -1)
                {
                    FrameBtnFinish.IsVisible = !_isComplite;
                }

                if (quest > 0)
                {
                    FrameBack.IsVisible = true;
                }
                else
                {
                    FrameBack.IsVisible = false;
                }
            }
        }

        void setQuestVisible()
        {
            Container.Children.Add(_contentQuest[quest]);
        }

        private void ButtonClickBack(object sender, EventArgs e)
        {
            BackQuest();
        }

        private void BackQuest()
        {
            quest--;
            visibleIndicator(quest, false);
            Container.Children.Clear();
            Container.Children.Add(_contentQuest[quest]);
            ChechQuestions();
            FrameBtnNext.IsVisible = true;
        }

        private void ButtonClickNext(object sender, EventArgs e)
        {
            NextQuest();
        }

        void visibleIndicator(int i, bool isNext)
        {
            if (isNext)
            {
                indicators[i].BackgroundColor = Color.FromHex(Settings.MobileSettings.color);
                indicators[i - 1].BackgroundColor = Color.Transparent;
            }
            else
            {
                indicators[i + 1].BackgroundColor = Color.Transparent;
                indicators[i].BackgroundColor = Color.FromHex(Settings.MobileSettings.color);
            }
        }

        private void NextQuest()
        {
            quest++;
            visibleIndicator(quest, true);
            Container.Children.Clear();
            Container.Children.Add(_contentQuest[quest]);
            ChechQuestions();
            isCheched = false;
            setVisibleButton();
        }

        private async void ButtonClickFinish(object sender, EventArgs e)
        {
        }
    }
}