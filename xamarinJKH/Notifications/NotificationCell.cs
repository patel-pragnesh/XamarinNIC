using Xamarin.Essentials;
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
            frame.Margin = new Thickness(10, 0, 10, 10);
            frame.Padding = new Thickness(20, 10, 20, 10);
            frame.CornerRadius = 30;
            StackLayout container = new StackLayout();
            container.Orientation = StackOrientation.Vertical;
            StackLayout titleContainer = new StackLayout();
            titleContainer.Orientation = StackOrientation.Horizontal;
            titleContainer.HorizontalOptions = LayoutOptions.FillAndExpand;
            title.Text = "Заголовок";
            title.HorizontalTextAlignment = TextAlignment.Start;
            title.HorizontalOptions = LayoutOptions.StartAndExpand;
            title.FontAttributes = FontAttributes.Bold;
            title.FontSize = 13;
            title.TextColor = Color.Black;
            more.Text = "Подробнее >";
            more.FontSize = 12;
            more.TextDecorations = TextDecorations.Underline;
            more.TextColor = Color.FromHex(Settings.MobileSettings.color);
            more.HorizontalTextAlignment = TextAlignment.End;
            more.HorizontalOptions = LayoutOptions.EndAndExpand;
            more.MaxLines = 1;
            more.MinimumWidthRequest = 80;
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
            View.BackgroundColor = Color.White;
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