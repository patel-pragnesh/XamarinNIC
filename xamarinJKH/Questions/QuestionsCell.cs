using Xamarin.Forms;
using xamarinJKH.CustomRenderers;
using xamarinJKH.Utils;

namespace xamarinJKH.Questions
{
    public class QuestionsCell : ViewCell
    {
        private Label title = new Label();
        private Label date = new Label();
        private Label countQuestTitle = new Label();
        private Label countQuest = new Label();
        private Label countAnsweredTitle = new Label();
        private Label countAnswered = new Label();
        Label btn = new Label();

        public QuestionsCell()
        {
            MaterialFrame frame = new MaterialFrame();
            frame.SetAppThemeColor(Frame.BorderColorProperty, Color.FromHex(Settings.MobileSettings.color), Color.Transparent);
            frame.Elevation = 20;
            frame.HorizontalOptions = LayoutOptions.FillAndExpand;
            frame.VerticalOptions = LayoutOptions.Start;
            frame.BackgroundColor = Color.White;
            frame.Margin = new Thickness(10, 0, 10, 10);
            frame.Padding = new Thickness(18, 18, 18, 18);
            frame.CornerRadius = 35;

            StackLayout container = new StackLayout();
            container.Orientation = StackOrientation.Vertical;

            title.TextColor = Color.Black;
            title.FontAttributes = FontAttributes.Bold;
            title.FontSize = 18;

            container.Children.Add(title);

            date.HorizontalOptions = LayoutOptions.Start;
            date.TextColor = Color.Black;
            date.FontSize = 12;
            date.Margin = new Thickness(0, -5, 0, 0);
            StackLayout containerCount = new StackLayout();
            containerCount.Orientation = StackOrientation.Horizontal;
            StackLayout containerOne = new StackLayout();
            containerOne.Orientation = StackOrientation.Horizontal;
            StackLayout containerTwo = new StackLayout();
            containerTwo.Orientation = StackOrientation.Horizontal;

            countQuestTitle.Text = "Количество вопросов:";
            countQuestTitle.HorizontalOptions = LayoutOptions.Start;
            countQuestTitle.FontSize = 12;
            countAnsweredTitle.VerticalOptions = LayoutOptions.Center;
            countQuestTitle.TextColor = Color.Black;

            countQuest.TextColor = Color.FromHex(Settings.MobileSettings.color);
            countQuest.HorizontalOptions = LayoutOptions.Start;
            countQuest.FontSize = 12;
            countQuest.FontAttributes = FontAttributes.Bold;

            containerOne.Children.Add(countQuestTitle);
            containerOne.Children.Add(countQuest);

            countAnsweredTitle.Text = "Количество отвеченных:";
            countAnsweredTitle.TextColor = Color.Black;
            countAnsweredTitle.HorizontalOptions = LayoutOptions.Start;
            countAnsweredTitle.VerticalOptions = LayoutOptions.Center;
            countAnsweredTitle.FontSize = 12;

            countAnswered.TextColor = Color.FromHex(Settings.MobileSettings.color);
            countAnswered.HorizontalOptions = LayoutOptions.Start;
            countAnswered.FontAttributes = FontAttributes.Bold;
            countAnswered.FontSize = 12;
            containerTwo.Children.Add(countAnsweredTitle);
            containerTwo.Children.Add(countAnswered);
            containerTwo.HorizontalOptions = LayoutOptions.End;

            Grid grid = new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition {Height = new GridLength(1, GridUnitType.Star)},
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition {Width = new GridLength(1, GridUnitType.Star)},
                    new ColumnDefinition {Width = new GridLength(1, GridUnitType.Star)},
                }
            };

            // containerCount.Children.Add(containerOne);
            // containerCount.Children.Add(containerTwo);
            grid.Children.Add(containerOne, 0, 0);
            grid.Children.Add(containerTwo, 1, 0);

            container.Children.Add(date);
            container.Children.Add(grid);

            Frame frameBtn = new Frame();
            frameBtn.HorizontalOptions = LayoutOptions.FillAndExpand;
            frameBtn.VerticalOptions = LayoutOptions.Start;
            frameBtn.BackgroundColor = Color.FromHex(Settings.MobileSettings.color);
            frameBtn.Margin = new Thickness(0, 10, 0, 10);
            frameBtn.Padding = 0;
            frameBtn.CornerRadius = 10;

            StackLayout containerBtn = new StackLayout();
            containerBtn.Spacing = 0;
            containerBtn.Orientation = StackOrientation.Horizontal;
            containerBtn.HorizontalOptions = LayoutOptions.CenterAndExpand;

            IconView image = new IconView();
            image.Source = "ic_questions2";
            image.Foreground = Color.White;
            // image.Margin = new Thickness(-40, 0, 0, 0);
            image.HeightRequest = 25;
            image.WidthRequest = 25;

            // Label btn = new Label();
            // btn.Margin = new Thickness(-15, 0, 0, 0);
            // btn.TextColor = Color.White;
            // btn.FontAttributes = FontAttributes.Bold;
            // btn.Text = "Пройти опрос";

            
            // btn.Margin = new Thickness(-30, 0, 0, 0);
            btn.TextColor = Color.White;
            btn.BackgroundColor = Color.Transparent;
            btn.HorizontalOptions = LayoutOptions.Center;
            btn.Margin = new Thickness(9,13,0,13);
            btn.FontAttributes = FontAttributes.Bold;
            btn.VerticalOptions = LayoutOptions.Center;
            btn.FontSize = 15;
            btn.Text = "Пройти опрос";

            containerBtn.Children.Add(image);
            containerBtn.Children.Add(btn);

            frameBtn.Content = containerBtn;

            container.Children.Add(frameBtn);

            frame.Content = container;

            View = frame;
        }

        public static readonly BindableProperty CountQuestProperty =
            BindableProperty.Create("CountQuest", typeof(string), typeof(QuestionsCell), "");

        public static readonly BindableProperty CountAnswProperty =
            BindableProperty.Create("CountAnsw", typeof(string), typeof(QuestionsCell), "");

        public static readonly BindableProperty DateQuestProperty =
            BindableProperty.Create("DateQuest", typeof(string), typeof(QuestionsCell), "");

        public static readonly BindableProperty TitleQuestProperty =
            BindableProperty.Create("TitleQuest", typeof(string), typeof(QuestionsCell), "");

        public static readonly BindableProperty IsCompleteProperty =
            BindableProperty.Create(" IsComplete", typeof(bool), typeof(QuestionsCell), false);

        public string CountQuest
        {
            get { return (string) GetValue(CountQuestProperty); }
            set { SetValue(CountQuestProperty, value); }
        }

        public string CountAnsw
        {
            get { return (string) GetValue(CountAnswProperty); }
            set { SetValue(CountAnswProperty, value); }
        }

        public string DateQuest
        {
            get { return (string) GetValue(DateQuestProperty); }
            set { SetValue(DateQuestProperty, value); }
        }

        public bool IsComplete
        {
            get { return (bool) GetValue(IsCompleteProperty); }
            set { SetValue(IsCompleteProperty, false); }
        }

        public string TitleQuest
        {
            get { return (string) GetValue(TitleQuestProperty); }
            set { SetValue(TitleQuestProperty, value); }
        }

        protected override async void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (BindingContext != null)
            {
                countAnswered.Text = CountAnsw;
                if (IsComplete)
                {
                    countAnswered.Text = CountQuest;
                    btn.Text = "Посмотреть ответы";
                }
                date.Text = DateQuest;
                title.Text = TitleQuest;
                countQuest.Text = CountQuest;
            }
        }
    }
}