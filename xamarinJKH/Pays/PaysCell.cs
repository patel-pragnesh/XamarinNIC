using System;
using System.ComponentModel;
using Xamarin.Forms;
using xamarinJKH.Utils;

namespace xamarinJKH.Main
{
    public class PaysCell : ViewCell
    {
        private Label ident = new Label();
        private Label adress = new Label();
        private StackLayout dell = new StackLayout();
        private Label sumPayDate = new Label();
        private Label sumPay = new Label();

        public PaysCell()
        {
            Frame frame = new Frame();
            frame.HorizontalOptions = LayoutOptions.FillAndExpand;
            frame.VerticalOptions = LayoutOptions.Start;
            frame.BackgroundColor = Color.White;
            frame.Margin = new Thickness(0, 0, 0, 10);
            frame.Padding = new Thickness(15, 15, 15, 15);
            frame.CornerRadius = 30;

            StackLayout container = new StackLayout();
            container.Orientation = StackOrientation.Vertical;

            StackLayout dateIdent = new StackLayout();
            dateIdent.Orientation = StackOrientation.Horizontal;

            StackLayout identAdress = new StackLayout();
            identAdress.HorizontalOptions = LayoutOptions.StartAndExpand;
            ident.FontSize = 15;
            ident.TextColor = Color.Black;
            adress.FontSize = 15;
            adress.TextColor = Color.Gray;

            identAdress.Children.Add(ident);
            identAdress.Children.Add(adress);


            IconView x = new IconView();
            x.Source = "ic_close";
            x.Foreground = Color.FromHex(Settings.MobileSettings.color);
            x.HeightRequest = 10;
            x.WidthRequest = 10;

            Label close = new Label();
            close.Text = "Удалить";
            close.TextColor = Color.FromHex(Settings.MobileSettings.color);
            close.FontSize = 15;
            close.TextDecorations = TextDecorations.Underline;
            dell.Orientation = StackOrientation.Horizontal;
            dell.HorizontalOptions = LayoutOptions.EndAndExpand;
            dell.VerticalOptions = LayoutOptions.Center;
            dell.MinimumWidthRequest = 80;

            dell.Children.Add(x);
            dell.Children.Add(close);

            dateIdent.Children.Add(identAdress);
            dateIdent.Children.Add(dell);

            Label separator = new Label();

            separator.HeightRequest = 1;
            separator.BackgroundColor = Color.Gray;
            separator.Margin = new Thickness(0, 5, 0, 5);
            container.Children.Add(dateIdent);
            container.Children.Add(separator);

            StackLayout sums = new StackLayout();
            sums.Orientation = StackOrientation.Horizontal;
            sums.Margin = new Thickness(30, 0, 30, 5);

            sumPayDate.Text = string.Format("Сумма к оплате{0} на 31.05.2020", Environment.NewLine);
            sumPayDate.FontSize = 15;
            sumPayDate.TextColor = Color.Gray;
            sumPayDate.HorizontalTextAlignment = TextAlignment.End;
            sumPayDate.Margin = new Thickness(0, 0, 15, 0);

            sumPay.Text = "4593.01 руб";
            sumPay.TextColor = Color.FromHex(Settings.MobileSettings.color);
            sumPay.FontSize = 25;
            sumPay.VerticalOptions = LayoutOptions.Center;

            sums.Children.Add(sumPayDate);
            sums.Children.Add(sumPay);

            container.Children.Add(sums);

            Frame frameBtn = new Frame();
            frameBtn.HorizontalOptions = LayoutOptions.FillAndExpand;
            frameBtn.VerticalOptions = LayoutOptions.Start;
            frameBtn.BackgroundColor = Color.FromHex(Settings.MobileSettings.color);
            frameBtn.CornerRadius = 15;

            StackLayout containerBtn = new StackLayout();
            containerBtn.Orientation = StackOrientation.Horizontal;
            containerBtn.HorizontalOptions = LayoutOptions.CenterAndExpand;

            IconView image = new IconView();
            image.Source = "ic_pays";
            image.Foreground = Color.White;
            image.Margin = new Thickness(-45, 0, 0, 0);
            image.HeightRequest = 15;

            Label btn = new Label();
            btn.Margin = new Thickness(-15, 0, 0, 0);
            btn.TextColor = Color.White;
            btn.FontAttributes = FontAttributes.Bold;
            btn.FontSize = 15;
            btn.Text = "Оплатить";

            containerBtn.Children.Add(image);
            containerBtn.Children.Add(btn);

            frameBtn.Content = containerBtn;

            container.Children.Add(frameBtn);

            Label payPeriod = new Label();

            FormattedString formatted = new FormattedString();

            formatted.Spans.Add(new Span
            {
                Text = "Платеж обрабатывается",
                FontSize = 15
            });
            formatted.Spans.Add(new Span
            {
                Text = " 2-3 ",
                FontSize = 15,
                FontAttributes = FontAttributes.Bold
            });
            formatted.Spans.Add(new Span
            {
                Text = "рабочих дня",
                FontSize = 15
            });

            payPeriod.FormattedText = formatted;
            payPeriod.TextColor = Color.FromHex(Settings.MobileSettings.color);
            payPeriod.FontSize = 15;
            payPeriod.HorizontalOptions = LayoutOptions.CenterAndExpand;

            container.Children.Add(payPeriod);

            frame.Content = container;

            View = frame;
        }

        public static readonly BindableProperty IdentProperty =
            BindableProperty.Create("Ident", typeof(string), typeof(PaysCell), "");

        public static readonly BindableProperty AdressIdentProperty =
            BindableProperty.Create("AdressIdent", typeof(string), typeof(PaysCell), "");

        public static readonly BindableProperty DateIdentProperty =
            BindableProperty.Create("DateIdent", typeof(string), typeof(PaysCell), "");

        public static readonly BindableProperty SumPayProperty =
            BindableProperty.Create("SumPay", typeof(string), typeof(PaysCell), "");

        public string Ident
        {
            get { return (string) GetValue(IdentProperty); }
            set { SetValue(IdentProperty, value); }
        }

        public string AdressIdent
        {
            get { return (string) GetValue(AdressIdentProperty); }
            set { SetValue(AdressIdentProperty, value); }
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
                    Text = "Л/сч: ",
                    TextColor = Color.Black,
                    FontSize = 15
                });
                formattedIdent.Spans.Add(new Span
                {
                    Text = "№ " + Ident,
                    TextColor = Color.Black,
                    FontAttributes = FontAttributes.Bold,
                    FontSize = 20
                });
                ident.FormattedText = formattedIdent;
                FormattedString formattedPayDate = new FormattedString();

                formattedPayDate.Spans.Add(new Span
                {
                    Text = "Сумма к оплате\n",
                    TextColor = Color.Gray,
                    FontSize = 20
                });
                formattedPayDate.Spans.Add(new Span
                {
                    Text = "на " + DateIdent,
                    TextColor = Color.Black,
                    FontSize = 20
                });
                adress.Text = AdressIdent;
                sumPayDate.FormattedText = formattedPayDate;
                FormattedString formattedPay = new FormattedString();
                formattedPay.Spans.Add(new Span
                {
                    Text = SumPay,
                    TextColor = Color.FromHex(Settings.MobileSettings.color),
                    FontSize = 30
                });
                formattedPay.Spans.Add(new Span
                {
                    Text = " руб.",
                    TextColor = Color.Gray,
                    FontSize = 15
                });
                sumPay.FormattedText = formattedPay;
            }
        }
    }
}