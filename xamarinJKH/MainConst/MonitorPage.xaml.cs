using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamarinJKH.Utils;

namespace xamarinJKH.MainConst
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MonitorPage : ContentPage
    {
        public Color hex { get; set; }
        public int fontSize { get; set; }
        public int fontSize2 { get; set; }
        public int fontSize3 { get; set; }
        public int StarSize { get; set; }

        public MonitorPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            hex = Color.FromHex(Settings.MobileSettings.color);
            fontSize = 15;
            fontSize2 = 20;
            fontSize3 = 13;
            StarSize = 33;
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    BackgroundColor = Color.White;
                    ImageFon.Margin = new Thickness(0, 0, 0, 0);
                    StackLayout.Margin = new Thickness(0, 33, 0, 0);
                    IconViewNameUk.Margin = new Thickness(0, 33, 0, 0);
                    break;
                case Device.Android:
                    double or = Math.Round(((double) App.ScreenWidth / (double) App.ScreenHeight), 2);
                    if (Math.Abs(or - 0.5) < 0.02)
                    {
                        fontSize = 11;
                        StarSize = 25;
                        fontSize2 = 15;
                        fontSize3 = 12;
                        // ScrollViewContainer.Margin = new Thickness(10, -135, 10, 0);
                        BackStackLayout.Margin = new Thickness(5, 25, 0, 0);
                        IconViewNameUk.Margin = new Thickness(-3, -10, 0, 0);
                        RelativeLayoutTop.Margin = new Thickness(0,0,0,-130);
                        IconViewNotComplite.Margin = 0;
                        IconViewNotComplite.HeightRequest = 8;
                        IconViewNotComplite.WidthRequest = 8;
                        IconViewPr.HeightRequest = 8;
                        IconViewPr.WidthRequest = 8;
                        IconViewPr.Margin = 0;
                    }

                    break;
                default:
                    break;
            }

            SetText();
            BindingContext = this;
        }

        void SetText()
        {
            UkName.Text = Settings.MobileSettings.main_name;
            FormattedString formatted = new FormattedString();
            formatted.Spans.Add(new Span
            {
                Text = Settings.Person.FIO + ", ",
                TextColor = Color.White,
                FontAttributes = FontAttributes.Bold,
                FontSize = 15
            });
            formatted.Spans.Add(new Span
            {
                Text = "добрый день!",
                TextColor = Color.White,
                FontSize = 15
            });
            LabelPhone.FormattedText = formatted;
        }
    }
}