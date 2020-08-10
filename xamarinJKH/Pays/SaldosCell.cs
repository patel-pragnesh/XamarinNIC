using System;
using System.Globalization;
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
            container.Margin = new Thickness(0, 0, 0, 10);
            container.Spacing = 0;
            identDate.HorizontalOptions = LayoutOptions.FillAndExpand;
            sum.HorizontalOptions = LayoutOptions.End;
            sum.HorizontalTextAlignment = TextAlignment.End;
            file.HorizontalOptions = LayoutOptions.End;
            file.Foreground = Color.FromHex(Settings.MobileSettings.color);
            file.HeightRequest = 35;
            file.WidthRequest = 25;
            file.Margin = new Thickness(10, 0, 0, 0);
            file.VerticalOptions = LayoutOptions.End;
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
                var fs = 15;
                if (Xamarin.Essentials.DeviceDisplay.MainDisplayInfo.Width < 700)
                    fs = 12;

                FormattedString formattedIdent = new FormattedString();
                DateIdent = FirstLetterToUpper(DateIdent);

                DateTime dtView;
                string spanTextField;
                //var dateToView1 = DateTime.ParseExact(DateIdent.Replace("г.", " ").Trim(), "MMMM yyyy", new CultureInfo("ru-RU"));
                var dateCorrect = DateTime.TryParseExact(DateIdent.Replace("г.", " ").Trim(), "MMMM yyyy", new CultureInfo("ru-RU"), DateTimeStyles.None , out dtView);
                if (dateCorrect)
                {
                    spanTextField = dtView.ToString("MMMM yyyy") + (CultureInfo.CurrentCulture.Name.Contains("en") ? string.Empty : " г.");
                }
                else
                    spanTextField = DateIdent;
                //.ToString("MMMM yyyy");
                //+ (CultureInfo.CurrentCulture.Name.Contains("en") ? string.Empty : " г.").ToString();

                formattedIdent.Spans.Add(new Span
                {
                    //Text = DateTime.ParseExact(DateIdent.Replace("г."," ").Trim(), "MMMM yyyy", new CultureInfo("ru-RU")).ToString("MMMM yyyy") + 
                    //        (CultureInfo.CurrentCulture.Name.Contains("en") ? string.Empty : " г.").ToString(),
                    Text=spanTextField,
                    TextColor = Color.Black,
                    FontSize = fs
                });
                formattedIdent.Spans.Add(new Span
                {
                    Text = $"\n{AppResources.Acc} ",
                    TextColor = Color.Gray,
                    FontSize = fs
                });
                formattedIdent.Spans.Add(new Span
                {
                    Text = " " + Ident,
                    TextColor = Color.Black,
                    FontAttributes = FontAttributes.Bold,
                    FontSize = fs
                });
                identDate.FormattedText = formattedIdent;

                formattedIdent = new FormattedString();
                double sum2 = Double.Parse(SumPay);
                formattedIdent.Spans.Add(new Span
                {
                    Text = $"{sum2:0.00}".Replace(',','.'),
                    TextColor = Color.Black,
                    FontAttributes = FontAttributes.Bold,
                    FontSize = fs
                });
                formattedIdent.Spans.Add(new Span
                {
                    Text = $"\n{AppResources.Currency}",
                    TextColor = Color.Gray,
                    FontSize = fs-2
                });

                sum.FormattedText = formattedIdent;
                if (!HasImage)
                {
                    file.IsVisible = false;
                }
            }
        }

        public static string FirstLetterToUpper(string str)
        {
            if (str.Length > 0)
            {
                return Char.ToUpper(str[0]) + str.Substring(1);
            }

            return "";
        }
    }
}