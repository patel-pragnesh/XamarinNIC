using Xamarin.Forms;
using xamarinJKH.Notifications;
using xamarinJKH.Server.RequestModel;
using xamarinJKH.Utils;

namespace xamarinJKH
{
    public class NotificationCell : ViewCell
    {
        private Label title = new Label();
        private Label more = new Label();
        private Label date = new Label();
        private Label text = new Label();

        public NotificationCell()
        {
            Frame frame = new Frame();
            frame.HorizontalOptions = LayoutOptions.FillAndExpand;
            frame.VerticalOptions = LayoutOptions.Start;
            frame.BackgroundColor = Color.White;
            frame.Margin = new Thickness(0, 0, 0, 10);
            frame.Padding = new Thickness(20, 10, 20, 10);
            frame.CornerRadius = 30;
            StackLayout container = new StackLayout();
            container.Orientation = StackOrientation.Vertical;
            StackLayout titleContainer = new StackLayout();
            titleContainer.Orientation = StackOrientation.Horizontal;
            title.HorizontalOptions = LayoutOptions.FillAndExpand;
            title.Text = "Заголовок";
            title.FontAttributes = FontAttributes.Bold;
            title.TextColor = Color.Black;
            more.HorizontalOptions = LayoutOptions.End;
            more.HorizontalTextAlignment = TextAlignment.End;
            more.TextDecorations = TextDecorations.Underline;
            more.Text = "Подробнее >";
            more.FontSize = 11;
            more.TextColor = Color.FromHex(Settings.MobileSettings.color);
            more.WidthRequest = 190;
           
            titleContainer.Children.Add(title);
            titleContainer.Children.Add(more);
            container.Children.Add(titleContainer);
            date.HorizontalOptions = LayoutOptions.Start;
            date.TextColor = Color.Black;
            container.Children.Add(date);
            text.HorizontalOptions = LayoutOptions.FillAndExpand;
            text.VerticalOptions = LayoutOptions.FillAndExpand;
            text.TextColor = Color.Black;
            text.TextType = TextType.Html;
            container.Children.Add(text);
            frame.Content = container;
            View = frame;
        }

        public static readonly BindableProperty TextNotifProperty =
            BindableProperty.Create("TextNotif", typeof(string), typeof(NotificationCell), "");

        public static readonly BindableProperty DateNotifProperty =
            BindableProperty.Create("DateNotif", typeof(string), typeof(NotificationCell), "");

        public static readonly BindableProperty TitleNotifProperty =
            BindableProperty.Create("TitleNotif", typeof(string), typeof(NotificationCell), "");

        public static readonly BindableProperty IdProperty =
            BindableProperty.Create("ID", typeof(string), typeof(NotificationCell), "");

        public string TextNotif
        {
            get { return (string) GetValue(TextNotifProperty); }
            set { SetValue(TextNotifProperty, value); }
        }

        public string DateNotif
        {
            get { return (string) GetValue(DateNotifProperty); }
            set { SetValue(DateNotifProperty, value); }
        }

        public string TitleNotif
        {
            get { return (string) GetValue(TitleNotifProperty); }
            set { SetValue(TitleNotifProperty, value); }
        }

        public string ID
        {
            get { return (string) GetValue(IdProperty); }
            set { SetValue(IdProperty, value); }
        }

        protected override async void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (BindingContext != null)
            {
                text.Text = TextNotif;
                date.Text = DateNotif;
                title.Text = TitleNotif;
            }
        }
    }
}