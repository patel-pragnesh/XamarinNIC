using System;
using Xamarin.Forms;
using xamarinJKH.Utils;

namespace xamarinJKH.Apps
{
    public class AppCell : ViewCell
    {
        private Label numberAndDate = new Label();
        private Label LabelDate = new Label();
        private IconView ImageStatus = new IconView();
        private Label LabelStatus = new Label();
        private Label LabelText = new Label();

        public AppCell()
        {
            Frame frame = new Frame();
            frame.HorizontalOptions = LayoutOptions.FillAndExpand;
            frame.VerticalOptions = LayoutOptions.Start;
            frame.BackgroundColor = Color.White;
            frame.Margin = new Thickness(10, 0, 10, 14);
            frame.Padding = new Thickness(25, 15, 25, 20);
            frame.CornerRadius = 40;

            StackLayout container = new StackLayout();
            container.Orientation = StackOrientation.Horizontal;

            StackLayout containerData = new StackLayout();
            containerData.HorizontalOptions = LayoutOptions.FillAndExpand;


            IconView arrow = new IconView();
            arrow.Source = "ic_arrow_forward";
            Color hex = Color.FromHex(Settings.MobileSettings.color);
            arrow.Foreground = hex;
            arrow.HeightRequest = 25;
            arrow.WidthRequest = 25;
            arrow.VerticalOptions = LayoutOptions.CenterAndExpand;
            arrow.HorizontalOptions = LayoutOptions.End;

            numberAndDate.TextColor = Color.Black;
            numberAndDate.FontSize = 12;
            numberAndDate.VerticalOptions = LayoutOptions.StartAndExpand;
            numberAndDate.HorizontalOptions = LayoutOptions.StartAndExpand;
            
            LabelDate.TextColor = Color.Black;
            LabelDate.FontSize = 15;
            LabelDate.Margin = new Thickness(0, -5,0,0);

            StackLayout status = new StackLayout();
            status.Orientation = StackOrientation.Horizontal;
            status.HorizontalOptions = LayoutOptions.FillAndExpand;
            ImageStatus.Foreground = hex;
            ImageStatus.Source = "ic_status_new";
            ImageStatus.HeightRequest = 15;
            ImageStatus.VerticalOptions = LayoutOptions.End;
            ImageStatus.WidthRequest = 15;
            ImageStatus.Margin = new Thickness(0, 0, 0, 0);

            LabelStatus.TextColor = Color.Black;
            LabelStatus.FontSize = 15;
            LabelStatus.VerticalTextAlignment = TextAlignment.Center;
            LabelStatus.VerticalOptions = LayoutOptions.Center;


            // status.Children.Add(ImageStatus);
            // status.Children.Add(LabelStatus);

            LabelText.TextColor = Color.Black;
            LabelText.HorizontalOptions = LayoutOptions.Start;
            LabelText.FontSize = 15;
            LabelText.HorizontalTextAlignment = TextAlignment.Start;
            LabelText.VerticalOptions = LayoutOptions.Start;
            LabelText.FontAttributes = FontAttributes.Bold;

            Grid grid = new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                }
            };
            
            // status.Children.Add(numberAndDate);
            // status.Children.Add(LabelText);

            grid.Children.Add(numberAndDate, 0, 0);
            
            StackLayout stackLayoutStatus = new StackLayout();
            stackLayoutStatus.Orientation = StackOrientation.Horizontal;
            // stackLayoutStatus.Spacing = 0;

            stackLayoutStatus.Children.Add(ImageStatus);
            stackLayoutStatus.Children.Add(LabelStatus);

            containerData.Children.Add(grid);
            containerData.Children.Add(LabelDate);
            containerData.Children.Add(stackLayoutStatus);


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
                    Text = "№" + Number + " ",
                    TextColor = Color.Black,
                    FontSize = 13
                });
                formatted.Spans.Add(new Span
                {
                    Text = "• " + TextApp.Trim(),
                    TextColor = Color.Black,
                    FontAttributes = FontAttributes.Bold,
                    FontSize = 17
                });
                numberAndDate.FormattedText = formatted;
                LabelStatus.Text = Status;
                // LabelText.Text = "• " + TextApp;

                LabelDate.Text = DateApp;

                if (Status.ToString().Contains("выполнена") || Status.ToString().Contains("закрыл"))
                {
                    ImageStatus.Source = "ic_status_done";
                }
                else if (Status.ToString().Contains("новая"))
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