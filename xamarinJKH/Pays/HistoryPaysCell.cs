using Xamarin.Forms;
using xamarinJKH.Utils;

namespace xamarinJKH.Pays
{
    public class HistoryPaysCell : ViewCell
    {
        Label LabelDate = new Label();
        Label LabelPeriod = new Label();
        Label LabelSum = new Label();
        
        public HistoryPaysCell()
        {
            StackLayout container = new StackLayout();
            container.Orientation = StackOrientation.Horizontal;
            container.HorizontalOptions = LayoutOptions.FillAndExpand;
            
            LabelDate.TextColor = Color.Black;
            LabelDate.HorizontalOptions = LayoutOptions.StartAndExpand;
            LabelDate.FontSize = 15;
            
            LabelPeriod.TextColor = Color.Black;
            LabelPeriod.HorizontalOptions = LayoutOptions.CenterAndExpand;
            LabelPeriod.FontSize = 15;
            LabelPeriod.FontAttributes = FontAttributes.Bold;
            LabelPeriod.HorizontalTextAlignment = TextAlignment.Start;
            
            LabelSum.TextColor = Color.Black;
            LabelSum.HorizontalOptions = LayoutOptions.EndAndExpand;
            LabelSum.FontSize = 15;
            
            container.Children.Add(LabelDate);
            container.Children.Add(LabelPeriod);
            container.Children.Add(LabelSum);

            View = container;

        }

        public static readonly BindableProperty PeriodProperty =
            BindableProperty.Create("Period", typeof(string), typeof(HistoryPaysCell), "");
        
        public static readonly BindableProperty DatePayProperty =
            BindableProperty.Create("DatePay", typeof(string), typeof(HistoryPaysCell), "");

        public static readonly BindableProperty SumPayProperty =
            BindableProperty.Create("SumPay", typeof(string), typeof(HistoryPaysCell), "");

        public string Period
        {
            get { return (string) GetValue(PeriodProperty); }
            set { SetValue(PeriodProperty, value); }
        }
        

        public string DatePay
        {
            get { return (string) GetValue(DatePayProperty); }
            set { SetValue(DatePayProperty, value); }
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
                LabelDate.Text = DatePay;
                LabelPeriod.Text = Period;
                FormattedString formattedIdent = new FormattedString();
                formattedIdent.Spans.Add(new Span
                {
                    Text = SumPay,
                    TextColor = Color.FromHex(Settings.MobileSettings.color),
                    FontSize = 15,
                    FontAttributes = FontAttributes.Bold
                });
                formattedIdent.Spans.Add(new Span
                {
                    Text = " руб.",
                    TextColor = Color.Gray,
                    FontSize = 10
                });
                LabelSum.FormattedText = formattedIdent;
            }
        }
    }
}