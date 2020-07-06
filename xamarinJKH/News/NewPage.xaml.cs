using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamarinJKH.Additional;
using xamarinJKH.Server;
using xamarinJKH.Server.RequestModel;
using xamarinJKH.Utils;

namespace xamarinJKH.News
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewPage : ContentPage
    {
        private NewsInfo newsInfo;
        private NewsInfoFull newsInfoFull;
        private RestClientMP _server = new RestClientMP();
       

        public NewPage(NewsInfo newsInfo)
        {
            this.newsInfo = newsInfo;
            InitializeComponent();
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    BackgroundColor = Color.White;
                    BackgroundColor = Color.White;
                    // ImageTop.Margin = new Thickness(0, 0, 0, 0);
                    // StackLayout.Margin = new Thickness(0, 33, 0, 0);
                    // IconViewNameUk.Margin = new Thickness(0, 33, 0, 0);
                    break;
                case Device.Android:
                default:
                    break;
            }
            NavigationPage.SetHasNavigationBar(this, false);
            var backClick = new TapGestureRecognizer();
            backClick.Tapped += async (s, e) => { _ = await Navigation.PopAsync(); };
            BackStackLayout.GestureRecognizers.Add(backClick);
            SetText();
        }

        async void SetText()
        {
            UkName.Text = Settings.MobileSettings.main_name;
            LabelPhone.Text = "+" + Settings.Person.Phone;
            LabelTitle.Text = newsInfo.Header;
            LabelDate.Text = newsInfo.Created;
            newsInfoFull = await _server.GetNewsFull(newsInfo.ID.ToString());
            LabelText.Text = newsInfoFull.Text;

            if (newsInfoFull.HasImage)
            {
                MemoryStream stream = await _server.GetNewsImage(newsInfoFull.ID.ToString());
                ImageAdd.Source = ImageSource.FromStream(() => { return stream; });
                ImageAdd.IsVisible = true;
            }
        }
    }
}