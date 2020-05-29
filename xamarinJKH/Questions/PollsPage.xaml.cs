using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamarinJKH.Server.RequestModel;
using xamarinJKH.Utils;

namespace xamarinJKH.Questions
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PollsPage : ContentPage
    {
        private PollInfo _pollInfo;
        private List<StackLayout> _contentQuest = new List<StackLayout>();
        private int quest = 0;
        private bool isCheched = false;

        public PollsPage(PollInfo pollInfo)
        {
            _pollInfo = pollInfo;
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
                default:
                    break;
            }
            NavigationPage.SetHasNavigationBar(this, false);
            var backClick = new TapGestureRecognizer();
            backClick.Tapped += async (s, e) => { _ = await Navigation.PopAsync(); };
            BackStackLayout.GestureRecognizers.Add(backClick);
            SetText();
            setQuest();
            setQuestVisible();
            ChechQuestions();
            setVisibleButton();
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

            FrameBack.BackgroundColor = Color.FromHex(Settings.MobileSettings.color);
            FrameBtnNext.BackgroundColor = Color.FromHex(Settings.MobileSettings.color);
            FrameBtnFinish.BackgroundColor = Color.FromHex(Settings.MobileSettings.color);
        }

        void setQuest()
        {
            int i = 1;
            foreach (var each in _pollInfo.Questions)
            {
                StackLayout containerPolss = new StackLayout();
                Label questions = new Label();
                FormattedString formattedString = new FormattedString();
                formattedString.Spans.Add(new Span
                {
                    Text = "Вопрос " + i + "/" + _pollInfo.Questions.Count + ".",
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Color.Black,
                    FontSize = 20
                });
                formattedString.Spans.Add(new Span
                {
                    Text = " " + each.Text,
                    TextColor = Color.Black,
                    FontSize = 20
                });

                questions.FormattedText = formattedString;
                containerPolss.Children.Add(questions);
                StackLayout radio = new StackLayout();
                foreach (var jAnswer in each.Answers)
                {
                    RadioButton radioButton = new RadioButton
                    {
                        Text = jAnswer.Text,
                        BackgroundColor = Color.Transparent,
                    };
                    radioButton.BorderColor = Color.Red;
                    radioButton.CheckedChanged += (sender, e) =>
                    {
                        isCheched = true;
                        setVisibleButton();
                    };
                    radio.Children.Add(radioButton);
                }

                containerPolss.Children.Add(radio);

                _contentQuest.Add(containerPolss);
                i++;
            }
        }

        void setVisibleButton()
        {
            FrameBtnNext.IsVisible = isCheched;
            if (quest + 1 == _pollInfo.Questions.Count)
            {
                FrameBtnNext.IsVisible = false;
                FrameBtnFinish.IsVisible = isCheched;
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
            quest--;
            Container.Children.Clear();
            Container.Children.Add(_contentQuest[quest]);
            ChechQuestions();
        }

        private void ButtonClickNext(object sender, EventArgs e)
        {
            quest++;
            Container.Children.Clear();
            Container.Children.Add(_contentQuest[quest]);
            ChechQuestions();
            isCheched = false;
            setVisibleButton();
        }

        private void ButtonClickFinish(object sender, EventArgs e)
        {
        }
    }
}