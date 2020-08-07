using System;
using Xamarin.Forms;
using xamarinJKH.CustomRenderers;
using xamarinJKH.Utils;

namespace xamarinJKH.AppsConst
{
    public class AppConstCell : ViewCell
    {
        private Label numberAndDate = new Label();
        private Label LabelDate = new Label();
        private IconView ImageStatus = new IconView();
        private Label LabelStatus = new Label();
        private Label LabelText = new Label();

        public AppConstCell()
        {
            MaterialFrame frame = new MaterialFrame();
            frame.SetAppThemeColor(Frame.BorderColorProperty, Color.FromHex(Settings.MobileSettings.color), Color.White);
            frame.Elevation = 20;
            frame.HorizontalOptions = LayoutOptions.FillAndExpand;
            frame.VerticalOptions = LayoutOptions.Start;
            frame.BackgroundColor = Color.White;
            frame.Margin = new Thickness(10, 0, 10, 14);
            frame.Padding = new Thickness(25, 15, 25, 20);
            frame.CornerRadius = 40;

            StackLayout container = new StackLayout();
            container.Orientation = StackOrientation.Horizontal;

            StackLayout containerData = new StackLayout();
            containerData.Orientation = StackOrientation.Vertical;
            containerData.HorizontalOptions = LayoutOptions.FillAndExpand;


            IconView arrow = new IconView();
            arrow.Source = "ic_arrow_forward";
            Color hex = Color.FromHex(Settings.MobileSettings.color);
            arrow.Foreground = hex;
            arrow.HeightRequest = 25;
            arrow.WidthRequest = 25;
            arrow.VerticalOptions = LayoutOptions.CenterAndExpand;
            arrow.HorizontalOptions = LayoutOptions.End;

            //LabelDate.TextColor = Color.Black;
            //LabelDate.FontSize = 15;
            //LabelDate.Margin = new Thickness(0, -5, 0, 0);

            numberAndDate.TextColor = Color.Black;
            numberAndDate.FontSize = 12;
            numberAndDate.VerticalOptions = LayoutOptions.CenterAndExpand;
            numberAndDate.HorizontalOptions = LayoutOptions.Fill;

            ImageStatus.Foreground = hex;
            ImageStatus.Source = "ic_status_new";
            ImageStatus.HeightRequest = 15;
            ImageStatus.VerticalOptions = LayoutOptions.Center;
            ImageStatus.WidthRequest = 15;
            ImageStatus.Margin = new Thickness(0, 0, 0, 0);
            ImageStatus.HorizontalOptions = LayoutOptions.End;

            LabelStatus.TextColor = Color.Black;
            LabelStatus.FontSize = 13;
            LabelStatus.VerticalTextAlignment = TextAlignment.Center;
            LabelStatus.VerticalOptions = LayoutOptions.Center;
            //LabelStatus.LineBreakMode = LineBreakMode.WordWrap;
            LabelStatus.HorizontalOptions = LayoutOptions.End;


            // status.Children.Add(ImageStatus);
            // status.Children.Add(LabelStatus);

            LabelText.TextColor = Color.Black;
            LabelText.HorizontalOptions = LayoutOptions.Start;
            LabelText.FontSize = 15;
            LabelText.HorizontalTextAlignment = TextAlignment.Start;
            LabelText.VerticalOptions = LayoutOptions.Start;
            LabelText.MaxLines = 1;

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

            //grid.Children.Add(numberAndDate, 0, 0);

            StackLayout stackLayoutStatus = new StackLayout();
            stackLayoutStatus.Orientation = StackOrientation.Horizontal;
            stackLayoutStatus.HorizontalOptions = LayoutOptions.Fill;
            
            // stackLayoutStatus.Spacing = 0;
            stackLayoutStatus.Children.Add(numberAndDate);
            stackLayoutStatus.Children.Add(ImageStatus);
            stackLayoutStatus.Children.Add(LabelStatus);

            StackLayout stackLayoutText = new StackLayout();
            stackLayoutText.Orientation = StackOrientation.Horizontal;
            stackLayoutText.HorizontalOptions = LayoutOptions.FillAndExpand;
            stackLayoutText.Children.Add(LabelText);
            //containerData.Children.Add(grid);
            containerData.Children.Add(stackLayoutStatus);
            containerData.Children.Add(stackLayoutText);


            container.Children.Add(containerData);
            container.Children.Add(arrow);

            frame.Content = container;


            View = frame;
        }


        public static readonly BindableProperty NumberProperty =
            BindableProperty.Create("Number", typeof(string), typeof(AppConstCell), "");

        public static readonly BindableProperty StatusProperty =
            BindableProperty.Create("Status", typeof(string), typeof(AppConstCell), "");

        public static readonly BindableProperty DateAppProperty =
            BindableProperty.Create("DateApp", typeof(string), typeof(AppConstCell), "");

        public static readonly BindableProperty TextAppProperty =
            BindableProperty.Create("TextApp", typeof(string), typeof(AppConstCell), "");

        public string Number
        {
            get { return (string)GetValue(NumberProperty); }
            set { SetValue(NumberProperty, value); }
        }

        public string Status
        {
            get { return (string)GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }

        public string DateApp
        {
            get { return (string)GetValue(DateAppProperty); }
            set { SetValue(DateAppProperty, value); }
        }

        public string TextApp
        {
            get { return (string)GetValue(TextAppProperty); }
            set { SetValue(TextAppProperty, value); }
        }


        protected override async void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (BindingContext != null)
            {
                var fNum = 17;
                var fdt = 13;
                if (Xamarin.Essentials.DeviceDisplay.MainDisplayInfo.Width < 700)
                {
                    fNum = 11;
                     fdt = 9;
                    if (Status.Length > 17)
                    {
                        ImageStatus.HorizontalOptions = LayoutOptions.End;
                        LabelStatus.HorizontalOptions = LayoutOptions.End;
                        var w = 700;
                        LabelStatus.WidthRequest = w;
                    }
                }
                else
                if (Xamarin.Essentials.DeviceDisplay.MainDisplayInfo.Width < 800)
                {
                    fNum = 13;
                    fdt = 11;
                    
                    if (Status.Length > 17)
                    {
                        ImageStatus.HorizontalOptions = LayoutOptions.End;
                        LabelStatus.HorizontalOptions = LayoutOptions.End;
                        var w = 100;
                        LabelStatus.WidthRequest = w;
                    }
                }
                FormattedString formatted = new FormattedString();
                formatted.Spans.Add(new Span
                {
                    Text = "№" + Number + " ",
                    TextColor = Color.Black,
                    FontAttributes = FontAttributes.Bold,
                    FontSize = fNum
                });
                formatted.Spans.Add(new Span
                {
                    Text = "• " + DateApp + " •",
                    TextColor = Color.Black,
                    FontSize = fdt
                });
                numberAndDate.FormattedText = formatted;
                numberAndDate.MaxLines = 1;
                LabelStatus.Text = Status;
                LabelStatus.FontSize = fdt;
                

                // LabelText.Text = "• " + TextApp;

                LabelText.Text = TextApp;

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