using Xamarin.Forms;
using xamarinJKH.Utils;

namespace xamarinJKH.Apps
{
    public class FileCell : ViewCell
    {
        private Label LabelName = new Label();
        IconView IconViewDell = new IconView();

        public FileCell()
        {
            var hex = Color.FromHex(Settings.MobileSettings.color);

            Frame frame = new Frame();
            frame.HorizontalOptions = LayoutOptions.FillAndExpand;
            frame.VerticalOptions = LayoutOptions.Start;
            frame.BackgroundColor = Color.Transparent;
            frame.BorderColor = hex;
            frame.Margin = new Thickness(0, 0, 0, 5);
            frame.Padding = new Thickness(10, 10, 10, 10);
            frame.CornerRadius = 30;
            
            StackLayout container = new StackLayout();
            container.Orientation = StackOrientation.Horizontal;
            
            LabelName.FontSize = 13;
            LabelName.TextColor = Color.Black;
            LabelName.HorizontalOptions = LayoutOptions.FillAndExpand;
            LabelName.VerticalOptions = LayoutOptions.Center;

            IconViewDell.Source = "ic_close";
            IconViewDell.Foreground = hex;
            IconViewDell.HorizontalOptions = LayoutOptions.End;
            IconViewDell.HeightRequest = 15;
            IconViewDell.WidthRequest = 15;
            
            container.Children.Add(LabelName);
            container.Children.Add(IconViewDell);

            frame.Content = container;

            View = frame;
        }
        
        public static readonly BindableProperty FileNameProperty =
            BindableProperty.Create("FileName", typeof(string), typeof(FileCell), "");

        public string FileName
        {
            get { return (string) GetValue(FileNameProperty); }
            set { SetValue(FileNameProperty, value); }
        }

        protected override async void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (BindingContext != null)
            {
                LabelName.Text = FileName;
            }
        }
    }
}