using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamarinJKH.Additional;
using xamarinJKH.News;
using xamarinJKH.Questions;
using xamarinJKH.Shop;
using xamarinJKH.Utils;

namespace xamarinJKH.Main
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EventsPage : ContentPage
    {
        public EventsPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    ImageFon.Margin = new Thickness(0, 7, 0, 0);
                    StackLayout.Margin = new Thickness(0, 33, 0, 0);
                    IconViewNameUk.Margin = new Thickness(0, 33, 0, 0);
                    break;
                case Device.Android:
                default:
                    break;
            }
            SetText();
            SetColor();
            SetVisibleControls();
        }

        void SetVisibleControls()
        {
            setVisible(Settings.EventBlockData.News.Count == 0, StartNews, FrameNews);
            setVisible(Settings.EventBlockData.Polls.Count == 0, StartQuestions, FrameQuestions);
            setVisible(Settings.EventBlockData.Announcements.Count == 0, StartNotification, FrameNotification);
            setVisible(Settings.EventBlockData.AdditionalServices.Count == 0, StartOffers, FrameOffers);
            setVisible(false, StartShop, FrameShop);
        }

        void setVisible(bool visible, Action funk, VisualElement frame)
        {
            if (visible)
            {
                frame.IsVisible = false;
            }
            else
            {
                funk();

            }
        }

        private void StartNews()
        {
            var startNews = new TapGestureRecognizer();
            startNews.Tapped += async (s, e) => { await Navigation.PushAsync (new NewsPage()); };
            FrameNews.GestureRecognizers.Add(startNews);
        }

        private void StartQuestions()
        {
            var startQuest = new TapGestureRecognizer();
            startQuest.Tapped += async (s, e) => { await Navigation.PushAsync (new QuestionsPage()); };
            FrameQuestions.GestureRecognizers.Add(startQuest);
        }

        private async void StartOffers()
        {
            var startAdditional = new TapGestureRecognizer();
            startAdditional.Tapped += async (s, e) => { await Navigation.PushAsync (new AdditionalPage()); };
            FrameOffers.GestureRecognizers.Add(startAdditional);
        }

        private void StartNotification()
        {
            var startNotif = new TapGestureRecognizer();
            startNotif.Tapped += async (s, e) => { await Navigation.PushAsync (new NotificationsPage()); };
            FrameNotification.GestureRecognizers.Add(startNotif);
        }
        private void StartShop()
        {
            
        }

        void SetText()
        {
            UkName.Text = Settings.MobileSettings.main_name;
            LabelPhone.Text = "+" + Settings.Person.Phone;
        }

        void SetColor()
        {
            Color hexColor = Color.FromHex(Settings.MobileSettings.color);
            // IconViewLogin.Foreground = hexColor;
            // IconViewTech.Foreground = hexColor;
            IconViewNotification.Foreground = hexColor;
            IconViewShop.Foreground = hexColor;
            IconViewForvardShop.Foreground = hexColor;
            IconViewForvardNotification.Foreground = hexColor;
            IconViewNews.Foreground = hexColor;
            IconViewForvardNews.Foreground = hexColor;
            IconViewQuestions.Foreground = hexColor;
            IconViewForvardQuestions.Foreground = hexColor;
            IconViewOffers.Foreground = hexColor;
            IconViewForvardOffers.Foreground = hexColor;
            // LabelTech.TextColor = hexColor;
        }
    }
}