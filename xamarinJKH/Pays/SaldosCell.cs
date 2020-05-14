using System;
using Xamarin.Forms;
using xamarinJKH.Utils;

namespace xamarinJKH.Pays
{
    public class SaldosCell : ViewCell
    {
        private Label identDate = new Label();
        private Label sum = new Label();
        private IconView file = new IconView();

        public SaldosCell()
        {
            StackLayout container = new StackLayout();
            container.Orientation = StackOrientation.Horizontal;
            container.Margin = new Thickness(0,0,0,10);
            identDate.HorizontalOptions = LayoutOptions.FillAndExpand;
            sum.HorizontalOptions = LayoutOptions.End;
            sum.HorizontalTextAlignment = TextAlignment.End;
            file.HorizontalOptions = LayoutOptions.End;
            file.Foreground = Color.FromHex(Settings.MobileSettings.color);
            file.HeightRequest = 20;
            // file.WidthRequest = 10;
            file.Source = "ic_file";
            container.Children.Add(identDate);
            container.Children.Add(sum);
            container.Children.Add(file);

            View = container;
        }


        public static readonly BindableProperty IdentProperty =
            BindableProperty.Create("Ident", typeof(string), typeof(SaldosCell), "");

        public static readonly BindableProperty HasImageProperty =
            BindableProperty.Create("HasImage", typeof(bool), typeof(SaldosCell), false);

        public static readonly BindableProperty DateIdentProperty =
            BindableProperty.Create("DateIdent", typeof(string), typeof(SaldosCell), "");

        public static readonly BindableProperty SumPayProperty =
            BindableProperty.Create("SumPay", typeof(string), typeof(SaldosCell), "");

        public string Ident
        {
            get { return (string) GetValue(IdentProperty); }
            set { SetValue(IdentProperty, value); }
        }

        public bool HasImage
        {
            get { return (bool) GetValue(HasImageProperty); }
            set { SetValue(HasImageProperty, value); }
        }

        public string DateIdent
        {
            get { return (string) GetValue(DateIdentProperty); }
            set { SetValue(DateIdentProperty, value); }
        }

        public string SumPay
        {
            get { return (string) GetValue(SumPayProperty); }
            set { SetValue(SumPayProperty, value); }
        }


        protected override async void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (BindingContext != null)
            {
                FormattedString formattedIdent = new FormattedString();
                formattedIdent.Spans.Add(new Span
                {
                    Text = DateIdent,
                    TextColor = Color.Black,
                    FontSize = 15
                });
                formattedIdent.Spans.Add(new Span
                {
                    Text = "\nЛ/сч: ",
                    TextColor = Color.Gray,
                    FontSize = 15
                });
                formattedIdent.Spans.Add(new Span
                {
                    Text = " " + Ident,
                    TextColor = Color.Black,
                    FontAttributes = FontAttributes.Bold,
                    FontSize = 15
                });
                identDate.FormattedText = formattedIdent;

                formattedIdent = new FormattedString();
                formattedIdent.Spans.Add(new Span
                {
                    Text = SumPay,
                    TextColor = Color.Black,
                    FontAttributes = FontAttributes.Bold,
                    FontSize = 15
                });
                formattedIdent.Spans.Add(new Span
                {
                    Text = "\nруб.",
                    TextColor = Color.Gray,
                    FontSize = 15
                });

                sum.FormattedText = formattedIdent;
                if (!HasImage)
                {
                    file.IsVisible = false;
                }
            }
        }
    }
}