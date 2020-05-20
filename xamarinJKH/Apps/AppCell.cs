using System;
using Xamarin.Forms;
using xamarinJKH.Utils;

namespace xamarinJKH.Apps
{
    public class AppCell : ViewCell
    {
        private Label numberAndDate = new Label();
        private IconView ImageStatus = new IconView();
        private Label LabelStatus = new Label();
        private Label LabelText = new Label();

        public AppCell()
        {
            Frame frame = new Frame();
            frame.HorizontalOptions = LayoutOptions.FillAndExpand;
            frame.VerticalOptions = LayoutOptions.Start;
            frame.BackgroundColor = Color.White;
            frame.Margin = new Thickness(0, 0, 0, 10);
            frame.Padding = new Thickness(15, 15, 15, 15);
            frame.CornerRadius = 30;

            StackLayout container = new StackLayout();
            container.Orientation = StackOrientation.Horizontal;

            StackLayout containerData = new StackLayout();
            containerData.HorizontalOptions = LayoutOptions.FillAndExpand;


            IconView arrow = new IconView();
            arrow.Source = "ic_arrow_forward";
            Color hex = Color.FromHex(Settings.MobileSettings.color);
            arrow.Foreground = hex;
            arrow.HeightRequest = 20;
            arrow.WidthRequest = 20;
            arrow.VerticalOptions = LayoutOptions.Center;
            arrow.HorizontalOptions = LayoutOptions.End;

            numberAndDate.TextColor = Color.Black;

            StackLayout status = new StackLayout();
            status.Orientation = StackOrientation.Horizontal;
            status.HorizontalOptions = LayoutOptions.FillAndExpand;
            ImageStatus.Foreground = hex;
            ImageStatus.Source = "ic_status_new";
            ImageStatus.HeightRequest = 10;
            ImageStatus.WidthRequest = 10;
            ImageStatus.Margin = new Thickness(-5, 0, 0, 0);

            LabelStatus.TextColor = Color.Black;
            LabelStatus.FontSize = 10;
            LabelStatus.VerticalTextAlignment = TextAlignment.Center;
            LabelStatus.VerticalOptions = LayoutOptions.Center;

            status.Children.Add(numberAndDate);
            status.Children.Add(ImageStatus);
            status.Children.Add(LabelStatus);

            LabelText.TextColor = Color.Black;
            LabelText.HorizontalOptions = LayoutOptions.FillAndExpand;
            LabelText.FontSize = 15;

            containerData.Children.Add(status);
            containerData.Children.Add(LabelText);

            container.Children.Add(containerData);
            container.Children.Add(arrow);

            frame.Content = container;

            View = frame;
        }


        public static readonly BindableProperty NumberProperty =
            BindableProperty.Create("Number", typeof(string), typeof(AppCell), "");

        public static readonly BindableProperty StatusProperty =
            BindableProperty.Create("Status", typeof(string), typeof(AppCell), "");

        public static readonly BindableProperty DateAppProperty =
            BindableProperty.Create("DateApp", typeof(string), typeof(AppCell), "");

        public static readonly BindableProperty TextAppProperty =
            BindableProperty.Create("TextApp", typeof(string), typeof(AppCell), "");

        public string Number
        {
            get { return (string) GetValue(NumberProperty); }
            set { SetValue(NumberProperty, value); }
        }

        public string Status
        {
            get { return (string) GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
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


        protected override async void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (BindingContext != null)
            {
                FormattedString formatted = new FormattedString();
                formatted.Spans.Add(new Span
                {
                    Text = "№" + Number,
                    TextColor = Color.Black,
                    FontAttributes = FontAttributes.Bold,
                    FontSize = 15
                });
                formatted.Spans.Add(new Span
                {
                    Text = " " + DateApp + " ",
                    TextColor = Color.Black,
                    FontSize = 13
                });
                numberAndDate.FormattedText = formatted;
                LabelStatus.Text = Status;
                LabelText.Text = TextApp;
                

                if (Status.ToString().Contains("выполнена") || Status.ToString().Contains("закрыл") )
                {
                    ImageStatus.Source = "ic_status_done";
                }else if (Status.ToString().Contains("новая"))
                {
                    ImageStatus.Source = "ic_status_new";
                }
                else
                {
                    ImageStatus.Source = "ic_status_wait";
                }
            }
        }
    }
}