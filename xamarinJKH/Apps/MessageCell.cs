using System;
using Xamarin.Forms;

namespace xamarinJKH.Apps
{
    public class MessageCell : ViewCell
    {
        private StackLayout ConteinerA = new StackLayout();
        private Image ImagePersonA = new Image();
        private Label LabelNameA = new Label();
        private Label LabeltimeA = new Label();
        private Label LabelTextA = new Label();
        private Label LabelDateA = new Label();

        private StackLayout Container = new StackLayout();
        private Image ImagePerson = new Image();
        private Label LabelName = new Label();
        private Label Labeltime = new Label();
        private Label LabelText = new Label();
        private Label LabelDate = new Label();

        public MessageCell()
        {
            Frame frame = new Frame();
            frame.HorizontalOptions = LayoutOptions.Start;
            frame.VerticalOptions = LayoutOptions.End;
            frame.BackgroundColor = Color.White;
            frame.Margin = new Thickness(0, 0, 0, 0);
            frame.Padding = 5;
            frame.CornerRadius = 30;

            ImagePerson.Source = ImageSource.FromFile("ic_not_author");
            ImagePerson.HeightRequest = 15;
            ImagePerson.VerticalOptions = LayoutOptions.Start;
            frame.Content = ImagePerson;

            Frame frameA = new Frame();
            frameA.HorizontalOptions = LayoutOptions.Start;
            frameA.VerticalOptions = LayoutOptions.End;
            frameA.BackgroundColor = Color.White;
            frameA.Margin = new Thickness(0, 0, 0, 0);
            frameA.Padding = 5;
            frameA.CornerRadius = 30;

            ImagePersonA.Source = ImageSource.FromFile("ic_author");
            ImagePersonA.HeightRequest = 15;
            ImagePersonA.VerticalOptions = LayoutOptions.Start;
            frameA.Content = ImagePersonA;


            StackLayout containerDate = new StackLayout();
            StackLayout containerFioTime = new StackLayout();

            LabelName.TextColor = Color.Gray;
            LabelName.FontSize = 15;
            LabelName.HorizontalOptions = LayoutOptions.Start;

            Labeltime.TextColor = Color.Gray;
            Labeltime.FontSize = 15;
            Labeltime.WidthRequest = 100;
            Labeltime.HorizontalTextAlignment = TextAlignment.End;
            Labeltime.VerticalOptions = LayoutOptions.End;
            LabeltimeA.VerticalOptions = LayoutOptions.End;
            LabeltimeA.HorizontalTextAlignment = TextAlignment.Start;
            LabeltimeA.WidthRequest = 100;
            Labeltime.HorizontalOptions = LayoutOptions.EndAndExpand;

            containerFioTime.Orientation = StackOrientation.Horizontal;
            containerFioTime.Children.Add(frame);
            containerFioTime.Children.Add(LabelName);
            containerFioTime.Children.Add(Labeltime);

            Frame frameText = new Frame();
            frameText.HorizontalOptions = LayoutOptions.FillAndExpand;
            frameText.VerticalOptions = LayoutOptions.StartAndExpand;
            frameText.BackgroundColor = Color.FromHex("#E2E2E2");
            frameText.Margin = new Thickness(0, 0, 0, 10);
            frameText.Padding = new Thickness(15, 15, 15, 15);
            frameText.CornerRadius = 10;

            LabelText.TextColor = Color.Black;
            LabelText.FontSize = 15;
            LabelText.HorizontalTextAlignment = TextAlignment.Start;
            LabelTextA.HorizontalTextAlignment = TextAlignment.Start;
            LabelText.HorizontalOptions = LayoutOptions.Start;

            frameText.Content = LabelText;

            Frame frameDate = new Frame();
            frameDate.HorizontalOptions = LayoutOptions.Center;
            frameDate.VerticalOptions = LayoutOptions.Start;
            frameDate.BackgroundColor = Color.FromHex("#E2E2E2");
            frameDate.Margin = new Thickness(0, 0, 0, 10);
            frameDate.Padding = new Thickness(5, 5, 5, 5);
            frameDate.CornerRadius = 15;

            LabelDate.FontSize = 15;
            LabelDate.TextColor = Color.FromHex("#777777");

            frameDate.Content = LabelDate;
            LabelText.HorizontalOptions = LayoutOptions.Center;

            containerDate.Children.Add(frameDate);
            containerDate.Children.Add(containerFioTime);
            containerDate.Children.Add(frameText);

            Container.Orientation = StackOrientation.Horizontal;

            Container.Children.Add(containerDate);

            StackLayout containerDateA = new StackLayout();
            StackLayout containerFioTimeA = new StackLayout();

            LabelNameA.TextColor = Color.Gray;
            LabelNameA.FontSize = 15;
            LabelNameA.HorizontalTextAlignment = TextAlignment.End;
            LabelNameA.HorizontalOptions = LayoutOptions.Start;

            LabeltimeA.TextColor = Color.Gray;
            LabeltimeA.FontSize = 15;
            LabeltimeA.HorizontalOptions = LayoutOptions.End;

            containerFioTimeA.Orientation = StackOrientation.Horizontal;

            containerFioTimeA.Children.Add(LabeltimeA);
            containerFioTimeA.Children.Add(LabelNameA);
            containerFioTimeA.Children.Add(frameA);


            Frame frameTextA = new Frame();
            frameTextA.HorizontalOptions = LayoutOptions.FillAndExpand;
            frameTextA.VerticalOptions = LayoutOptions.StartAndExpand;
            frameTextA.BackgroundColor = Color.FromHex("#E2E2E2");
            frameTextA.Margin = new Thickness(0, 0, 0, 10);
            frameTextA.Padding = new Thickness(15, 15, 15, 15);
            frameTextA.CornerRadius = 10;

            LabelTextA.TextColor = Color.Black;
            LabelTextA.FontSize = 15;
            LabelTextA.HorizontalOptions = LayoutOptions.End;

            frameTextA.Content = LabelTextA;

            Frame frameDateA = new Frame();
            frameDateA.HorizontalOptions = LayoutOptions.Center;
            frameDateA.VerticalOptions = LayoutOptions.Start;
            frameDateA.BackgroundColor = Color.FromHex("#E2E2E2");
            frameDateA.Margin = new Thickness(0, 0, 0, 10);
            frameDateA.Padding = 5;
            frameDateA.CornerRadius = 30;

            LabelDateA.FontSize = 15;
            LabelDateA.TextColor = Color.FromHex("#777777");

            frameDateA.Content = LabelDateA;
            LabelTextA.HorizontalOptions = LayoutOptions.Center;

            containerDateA.Children.Add(frameDateA);
            containerDateA.Children.Add(containerFioTimeA);
            containerDateA.Children.Add(frameTextA);

            ConteinerA.Orientation = StackOrientation.Horizontal;
            ConteinerA.Children.Add(containerDateA);

            StackLayout stackLayout = new StackLayout();

            stackLayout.Children.Add(Container);
            stackLayout.Children.Add(ConteinerA);

            View = stackLayout;
        }

        public static readonly BindableProperty NameProperty =
            BindableProperty.Create("Name", typeof(string), typeof(MessageCell), "");

        public static readonly BindableProperty TimeProperty =
            BindableProperty.Create("Time", typeof(string), typeof(MessageCell), "");

        public static readonly BindableProperty DateAppProperty =
            BindableProperty.Create("DateApp", typeof(string), typeof(MessageCell), "");

        public static readonly BindableProperty TextAppProperty =
            BindableProperty.Create("TextApp", typeof(string), typeof(MessageCell), "");

        public static readonly BindableProperty IsSelfProperty =
            BindableProperty.Create("IsSelf", typeof(bool), typeof(MessageCell), false);

        public string Name
        {
            get { return (string) GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        public string Time
        {
            get { return (string) GetValue(TimeProperty); }
            set { SetValue(TimeProperty, value); }
        }

        public string DateApp
        {
            get { return (string) GetValue(DateAppProperty); }
            set { SetValue(DateAppProperty, value); }
        }

        public string TextApp
        {
            get { return (string) GetValue(TextAppProperty); }
            set { SetValue(TextAppProperty, value); }
        }

        public bool IsSelf
        {
            get { return (bool) GetValue(IsSelfProperty); }
            set { SetValue(IsSelfProperty, value); }
        }


        protected override async void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (BindingContext != null)
            {
                if (IsSelf)
                {
                    ConteinerA.IsVisible = true;
                    Container.IsVisible = false;
                }
                else
                {
                    ConteinerA.IsVisible = false;
                    Container.IsVisible = true;
                }

                var strings = DateApp.Split(' ');
                LabelDate.Text = LabelDateA.Text = strings[0];
                LabelName.Text = LabelNameA.Text = Name;
                LabelText.Text = LabelTextA.Text = TextApp;
                Labeltime.Text = LabeltimeA.Text = strings[1].Substring(0,5);
            }
        }
    }
}