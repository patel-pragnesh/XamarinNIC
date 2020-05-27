using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Globalization;
using Xamarin.Forms;
using xamarinJKH.Utils;
using xamarinJKH.Server.RequestModel;

namespace xamarinJKH.Main
{
    public class MetersThreeCell : ViewCell
    {
        private Image img = new Image();
        private Label resource = new Label();
        private Label adress = new Label();
        private Label number = new Label();
        private Label checkup_date = new Label();
        private Label recheckup = new Label();
        private Label tarif1 = new Label();
        private StackLayout tarif1Stack = new StackLayout();
        private Label counterDate1 = new Label();
        private Label count1 = new Label();
        private Label tarif2 = new Label();
        private StackLayout tarif2Stack = new StackLayout();
        private Label counterDate2 = new Label();
        private Label count2 = new Label();
        private Label tarif3 = new Label();
        private StackLayout tarif3Stack = new StackLayout();
        private Label counterDate3 = new Label();
        private Label count3 = new Label();

        public MetersThreeCell()
        {
            Frame frame = new Frame();
            frame.HorizontalOptions = LayoutOptions.FillAndExpand;
            frame.VerticalOptions = LayoutOptions.Start;
            frame.BackgroundColor = Color.White;
            frame.Margin = new Thickness(10, 0, 10, 10);
            frame.Padding = new Thickness(15, 15, 15, 15);
            frame.CornerRadius = 30;

            StackLayout container = new StackLayout();
            container.Orientation = StackOrientation.Vertical;

            resource.FontSize = 15;
            resource.TextColor = Color.Black;
            resource.HorizontalTextAlignment = TextAlignment.Center;
            
            StackLayout addressStack = new StackLayout();
            addressStack.Orientation = StackOrientation.Horizontal;
            addressStack.HorizontalOptions = LayoutOptions.FillAndExpand;
            Label adressLbl = new Label();
            adressLbl.Text = "Адрес: ";
            adressLbl.FontSize = 15;
            adressLbl.TextColor = Color.FromHex("#A2A2A2");
            adressLbl.HorizontalTextAlignment = TextAlignment.Start;
            adressLbl.HorizontalOptions = LayoutOptions.StartAndExpand;
            adressLbl.MaxLines = 1;
            adressLbl.WidthRequest = 100;
            adress.FontSize = 15;
            adress.TextColor = Color.Black;
            adress.HorizontalTextAlignment = TextAlignment.End;
            adress.HorizontalOptions = LayoutOptions.EndAndExpand;
            adress.MaxLines = 3;
            addressStack.Children.Add(adressLbl);
            addressStack.Children.Add(adress);
            container.Children.Add(resource);
            container.Children.Add(addressStack);
            
            StackLayout numberStack = new StackLayout();
            numberStack.Orientation = StackOrientation.Horizontal;
            numberStack.HorizontalOptions = LayoutOptions.FillAndExpand;
            Label numberLbl = new Label();
            numberLbl.HorizontalOptions = LayoutOptions.StartAndExpand;
            numberLbl.Text = "Заводской №: ";
            numberLbl.FontSize = 15;
            numberLbl.TextColor = Color.FromHex("#A2A2A2");
            numberLbl.HorizontalTextAlignment = TextAlignment.Start;
            numberLbl.MaxLines = 1;
            numberLbl.WidthRequest = 100;
            number.FontSize = 15;
            number.HorizontalOptions = LayoutOptions.EndAndExpand;
            number.TextColor = Color.Black;
            number.HorizontalTextAlignment = TextAlignment.End;
            number.MaxLines = 1;
            numberStack.Children.Add(numberLbl);
            numberStack.Children.Add(number);
            container.Children.Add(numberStack);
            
            StackLayout checkupDateStack = new StackLayout();
            checkupDateStack.Orientation = StackOrientation.Horizontal;
            checkupDateStack.HorizontalOptions = LayoutOptions.FillAndExpand;
            Label checkupDateLbl = new Label();
            checkupDateLbl.Text = "Последняя поверка: ";
            checkupDateLbl.FontSize = 15;
            checkupDateLbl.TextColor = Color.FromHex("#A2A2A2");
            checkupDateLbl.HorizontalTextAlignment = TextAlignment.Start;
            checkupDateLbl.HorizontalOptions = LayoutOptions.StartAndExpand;
            checkupDateLbl.MaxLines = 1;
            checkupDateLbl.WidthRequest = 150;
            checkup_date.FontSize = 15;
            checkup_date.TextColor = Color.Black;
            checkup_date.HorizontalTextAlignment = TextAlignment.End;
            checkup_date.HorizontalOptions = LayoutOptions.EndAndExpand;
            checkup_date.MaxLines = 1;
            checkupDateStack.Children.Add(checkupDateLbl);
            checkupDateStack.Children.Add(checkup_date);
            container.Children.Add(checkupDateStack);
            
            StackLayout recheckStack = new StackLayout();
            recheckStack.Orientation = StackOrientation.Horizontal;
            recheckStack.HorizontalOptions = LayoutOptions.FillAndExpand;
            Label recheckLbl = new Label();
            recheckLbl.Text = "Межповерочный интервал: ";
            recheckLbl.FontSize = 15;
            recheckLbl.TextColor = Color.FromHex("#A2A2A2");
            recheckLbl.HorizontalTextAlignment = TextAlignment.Start;
            recheckLbl.HorizontalOptions = LayoutOptions.StartAndExpand;
            recheckLbl.MaxLines = 1;
            recheckLbl.WidthRequest = 150;
            recheckup.FontSize = 15;
            recheckup.TextColor = Color.Black;
            recheckup.HorizontalTextAlignment = TextAlignment.End;
            recheckup.HorizontalOptions = LayoutOptions.EndAndExpand;
            recheckup.MaxLines = 1;
            recheckStack.Children.Add(recheckLbl);
            recheckStack.Children.Add(recheckup);
            container.Children.Add(recheckStack);
            
            Label separator = new Label();

            separator.HeightRequest = 1;
            separator.BackgroundColor = Color.LightGray;
            separator.Margin = new Thickness(0, 5, 0, 5);
            container.Children.Add(separator);
            
            tarif1.FontSize = 15;
            tarif1.TextColor = Color.Red;
            tarif1.HorizontalTextAlignment = TextAlignment.Center;
            
            StackLayout count1Stack = new StackLayout();
            count1Stack.Orientation = StackOrientation.Horizontal;
            count1Stack.HorizontalOptions = LayoutOptions.FillAndExpand;
            counterDate1.FontSize = 15;
            counterDate1.TextColor = Color.FromHex("#A2A2A2");
            counterDate1.HorizontalTextAlignment = TextAlignment.Start;
            counterDate1.HorizontalOptions = LayoutOptions.StartAndExpand;
            counterDate1.MaxLines = 1;
            counterDate1.WidthRequest = 150;
            count1.FontSize = 15;
            count1.TextColor = Color.Black;
            count1.HorizontalTextAlignment = TextAlignment.End;
            count1.HorizontalOptions = LayoutOptions.EndAndExpand;
            count1.MaxLines = 1;
            count1Stack.Children.Add(counterDate1);
            count1Stack.Children.Add(count1);
            container.Children.Add(count1Stack);
            
            StackLayout count2Stack = new StackLayout();
            count2Stack.Orientation = StackOrientation.Horizontal;
            count2Stack.HorizontalOptions = LayoutOptions.FillAndExpand;
            counterDate2.FontSize = 15;
            counterDate2.TextColor = Color.FromHex("#A2A2A2");
            counterDate2.HorizontalTextAlignment = TextAlignment.Start;
            counterDate2.HorizontalOptions = LayoutOptions.StartAndExpand;
            counterDate2.MaxLines = 1;
            counterDate2.WidthRequest = 150;
            count2.FontSize = 15;
            count2.TextColor = Color.Black;
            count2.HorizontalTextAlignment = TextAlignment.End;
            count2.HorizontalOptions = LayoutOptions.EndAndExpand;
            count2.MaxLines = 1;
            count2Stack.Children.Add(counterDate2);
            count2Stack.Children.Add(count2);
            container.Children.Add(count2Stack);
            
            StackLayout count3Stack = new StackLayout();
            count3Stack.Orientation = StackOrientation.Horizontal;
            count3Stack.HorizontalOptions = LayoutOptions.FillAndExpand;
            counterDate3.FontSize = 15;
            counterDate3.TextColor = Color.FromHex("#A2A2A2");
            counterDate3.HorizontalTextAlignment = TextAlignment.Start;
            counterDate3.HorizontalOptions = LayoutOptions.StartAndExpand;
            counterDate3.MaxLines = 1;
            counterDate3.WidthRequest = 150;
            count3.FontSize = 15;
            count3.TextColor = Color.Black;
            count3.HorizontalTextAlignment = TextAlignment.End;
            count3.HorizontalOptions = LayoutOptions.EndAndExpand;
            count3.MaxLines = 1;
            count3Stack.Children.Add(counterDate3);
            count3Stack.Children.Add(count3);
            container.Children.Add(count3Stack);

            Frame frameBtn = new Frame();
            frameBtn.HorizontalOptions = LayoutOptions.FillAndExpand;
            frameBtn.VerticalOptions = LayoutOptions.Start;
            frameBtn.BackgroundColor = Color.FromHex(Settings.MobileSettings.color);
            frameBtn.CornerRadius = 15;

            StackLayout containerBtn = new StackLayout();
            containerBtn.Orientation = StackOrientation.Horizontal;
            containerBtn.HorizontalOptions = LayoutOptions.CenterAndExpand;

            Label btn = new Label();
            btn.Margin = new Thickness(0, 0, 0, 0);
            btn.TextColor = Color.White;
            btn.FontAttributes = FontAttributes.Bold;
            btn.FontSize = 15;
            btn.Text = "Передать показания";
            containerBtn.Children.Add(btn);

            frameBtn.Content = containerBtn;

            container.Children.Add(frameBtn);

            frame.Content = container;

            View = frame;
        }
        
        public static readonly BindableProperty ResourceProperty =
            BindableProperty.Create("Resource", typeof(string), typeof(MetersThreeCell), "");

        public static readonly BindableProperty AddressProperty =
            BindableProperty.Create("Address", typeof(string), typeof(MetersThreeCell), "");

        public static readonly BindableProperty UniqueNumProperty =
            BindableProperty.Create("FactoryNumber", typeof(string), typeof(MetersThreeCell), "");

        public static readonly BindableProperty CheckupDateProperty =
            BindableProperty.Create("LastCheckupDate", typeof(string), typeof(MetersThreeCell), "");
        
        public static readonly BindableProperty RecheckIntervalProperty =
            BindableProperty.Create("RecheckInterval", typeof(string), typeof(MetersThreeCell), "");

        public static readonly BindableProperty ValuesProperty =
            BindableProperty.Create("Values", typeof(List<MeterValueInfo>), typeof(MetersThreeCell), new List<MeterValueInfo>());

        public List<MeterValueInfo> Values
        {
            get { return (List<MeterValueInfo>) GetValue(ValuesProperty); }
            set { SetValue(ValuesProperty, value); }
        }
        
        public string Resource
        {
            get { return (string) GetValue(ResourceProperty); }
            set { SetValue(ResourceProperty, value); }
        }

        public string Address
        {
            get { return (string) GetValue(AddressProperty); }
            set { SetValue(AddressProperty, value); }
        }

        public string UniqueNum
        {
            get { return (string) GetValue(UniqueNumProperty); }
            set { SetValue(UniqueNumProperty, value); }
        }

        public string CheckupDate
        {
            get { return (string) GetValue(CheckupDateProperty); }
            set { SetValue(CheckupDateProperty, value); }
        }

        public string RecheckInterval
        {
            get { return (string) GetValue(RecheckIntervalProperty); }
            set { SetValue(RecheckIntervalProperty, value); }
        }


        protected override async void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (BindingContext != null)
            {
                FormattedString formattedResource = new FormattedString();
                formattedResource.Spans.Add(new Span
                {
                    Text = Resource,
                    TextColor = Color.Black,
                    FontAttributes = FontAttributes.Bold,
                    FontSize = 20
                });
                resource.FormattedText = formattedResource;
                adress.Text = Address;
                number.Text = UniqueNum;
                checkup_date.Text = CheckupDate;
                recheckup.Text = RecheckInterval + " лет";
                if (Values.Count == 1)
                {
                    counterDate1.Text = Values[0].Period;
                    count1.Text = Values[0].Value.ToString(CultureInfo.InvariantCulture);
                }else if (Values.Count == 2)
                {
                    counterDate1.Text = Values[0].Period;
                    count1.Text = Values[0].Value.ToString(CultureInfo.InvariantCulture);
                    counterDate2.Text = Values[1].Period;
                    count2.Text = Values[1].Value.ToString(CultureInfo.InvariantCulture);
                }else if (Values.Count == 3)
                {
                    counterDate1.Text = Values[0].Period;
                    count1.Text = Values[0].Value.ToString(CultureInfo.InvariantCulture);
                    counterDate2.Text = Values[1].Period;
                    count2.Text = Values[1].Value.ToString(CultureInfo.InvariantCulture);
                    counterDate3.Text = Values[2].Period;
                    count3.Text = Values[2].Value.ToString(CultureInfo.InvariantCulture);
                }
            }
        }
    }
}