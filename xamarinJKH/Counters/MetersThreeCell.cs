using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Globalization;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using xamarinJKH.CustomRenderers;
using xamarinJKH.DialogViews;
using xamarinJKH.Utils;
using xamarinJKH.Server.RequestModel;

namespace xamarinJKH.Main
{
    public class MetersThreeCell : ViewCell
    {
        private Image img = new Image();
        private IconView imgEdit = new IconView();
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
        StackLayout containerBtn = new StackLayout();
        private Label canCount = new Label();
        Frame frameBtn = new Frame();

        StackLayout count1Stack = new StackLayout();
        StackLayout count2Stack = new StackLayout();
        StackLayout count3Stack = new StackLayout();

        public MetersThreeCell()
        {
            MaterialFrame frame = new MaterialFrame();
            frame.SetAppThemeColor(Frame.BorderColorProperty, Color.FromHex(Settings.MobileSettings.color), Color.White);
            frame.Elevation = 20;
            frame.HorizontalOptions = LayoutOptions.FillAndExpand;
            frame.VerticalOptions = LayoutOptions.Start;
            frame.BackgroundColor = Color.White;
            frame.Margin = new Thickness(10, 0, 10, 10);
            frame.Padding = new Thickness(15, 15, 15, 15);
            frame.CornerRadius = 30;

            StackLayout container = new StackLayout();
            container.Orientation = StackOrientation.Vertical;

            StackLayout header = new StackLayout();
            header.Orientation = StackOrientation.Horizontal;
            header.HorizontalOptions = LayoutOptions.Center;

            resource.FontSize = 15;
            resource.TextColor = Color.Black;
            resource.HorizontalTextAlignment = TextAlignment.Center;

            img.WidthRequest = 25;
            imgEdit.WidthRequest = 20;
            imgEdit.HeightRequest = 20;
            imgEdit.Source = "edit";
            imgEdit.Foreground = Color.FromHex(Settings.MobileSettings.color);

            header.Children.Add(img);
            header.Children.Add(resource);
#if DEBUG
            header.Children.Add(imgEdit);
#endif

            StackLayout addressStack = new StackLayout();
            addressStack.Orientation = StackOrientation.Horizontal;
            addressStack.HorizontalOptions = LayoutOptions.FillAndExpand;
            
            Grid grid = new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) },
                }
            };
            
            
            Label adressLbl = new Label();
            adressLbl.Text = $"{AppResources.Adress}:";
            adressLbl.FontSize = 15;
            adressLbl.TextColor = Color.FromHex("#A2A2A2");
            adressLbl.HorizontalTextAlignment = TextAlignment.Start;
            adressLbl.HorizontalOptions = LayoutOptions.Fill;
            adress.FontSize = 15;
            adress.TextColor = Color.Black;
            adress.HorizontalTextAlignment = TextAlignment.End;
            adress.HorizontalOptions = LayoutOptions.Fill;
            adress.MaxLines = 3;
            // addressStack.Children.Add(adressLbl);
            // addressStack.Children.Add(new Label
            // {
            //     HeightRequest = 1,
            //     HorizontalOptions = LayoutOptions.FillAndExpand,
            //     BackgroundColor = Color.LightGray,
            //     Margin = new Thickness(0, 5, 0, 0),
            //     VerticalOptions = LayoutOptions.Center
            // });
            // addressStack.Children.Add(adress);
            
            grid.Children.Add(adressLbl, 0, 0);
            grid.Children.Add(new Label
            {
                HeightRequest = 1,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.LightGray,
                Margin = new Thickness(0, 5, 0, 0),
                VerticalOptions = LayoutOptions.Center
            }, 1, 0);
            grid.Children.Add(adress, 2, 0);

            container.Children.Add(header);
            container.Children.Add(grid);

            StackLayout numberStack = new StackLayout();
            numberStack.Orientation = StackOrientation.Horizontal;
            numberStack.HorizontalOptions = LayoutOptions.FillAndExpand;
            Label numberLbl = new Label();
            numberLbl.Text = AppResources.FacNum;
            numberLbl.FontSize = 12;
            numberLbl.TextColor = Color.FromHex("#A2A2A2");
            numberLbl.HorizontalTextAlignment = TextAlignment.Start;
            numberLbl.HorizontalOptions = LayoutOptions.Start;
            numberLbl.MaxLines = 1;
            // numberLbl.WidthRequest = 100;
            number.FontSize = 12;
            number.HorizontalOptions = LayoutOptions.End;
            number.TextColor = Color.Black;
            number.HorizontalTextAlignment = TextAlignment.End;
            number.MaxLines = 1;

            Label linesNumb = new Label();
            linesNumb.HeightRequest = 1;
            linesNumb.BackgroundColor = Color.LightGray;
            linesNumb.Margin = new Thickness(0, 2, 0, 0);
            linesNumb.VerticalOptions = LayoutOptions.Center;
            linesNumb.HorizontalOptions = LayoutOptions.FillAndExpand;

            numberStack.Children.Add(numberLbl);
            numberStack.Children.Add(linesNumb);
            numberStack.Children.Add(number);
            container.Children.Add(numberStack);

            StackLayout checkupDateStack = new StackLayout();
            checkupDateStack.Orientation = StackOrientation.Horizontal;
            checkupDateStack.HorizontalOptions = LayoutOptions.FillAndExpand;
            checkupDateStack.Margin = new Thickness(0, -7, 0, 0);
            Label checkupDateLbl = new Label();
            checkupDateLbl.Text = AppResources.LastCheck;
            checkupDateLbl.FontSize = 12;
            checkupDateLbl.TextColor = Color.FromHex("#A2A2A2");
            checkupDateLbl.HorizontalTextAlignment = TextAlignment.Start;
            checkupDateLbl.HorizontalOptions = LayoutOptions.Start;
            checkupDateLbl.MaxLines = 1;
            // checkupDateLbl.WidthRequest = 150;
            checkup_date.FontSize = 12;
            checkup_date.TextColor = Color.Black;
            checkup_date.HorizontalTextAlignment = TextAlignment.End;
            checkup_date.HorizontalOptions = LayoutOptions.End;
            checkup_date.MaxLines = 1;

            Label linesPover = new Label();
            linesPover.HeightRequest = 1;
            linesPover.BackgroundColor = Color.LightGray;
            ;
            linesPover.VerticalOptions = LayoutOptions.Center;
            linesPover.Margin = new Thickness(0, 2, 0, 0);
            linesPover.HorizontalOptions = LayoutOptions.FillAndExpand;

            checkupDateStack.Children.Add(checkupDateLbl);
            checkupDateStack.Children.Add(linesPover);
            checkupDateStack.Children.Add(checkup_date);
            container.Children.Add(checkupDateStack);

            StackLayout recheckStack = new StackLayout();
            recheckStack.Orientation = StackOrientation.Horizontal;
            recheckStack.HorizontalOptions = LayoutOptions.FillAndExpand;
            recheckStack.Margin = new Thickness(0, -7, 0, 0);
            Label recheckLbl = new Label();
            recheckLbl.Text = AppResources.CheckInterval;
            recheckLbl.FontSize = 12;
            recheckLbl.TextColor = Color.FromHex("#A2A2A2");
            recheckLbl.HorizontalTextAlignment = TextAlignment.Start;
            recheckLbl.HorizontalOptions = LayoutOptions.Start;
            recheckLbl.MaxLines = 1;
            // recheckLbl.WidthRequest = 150;
            recheckup.FontSize = 12;
            recheckup.TextColor = Color.Black;
            recheckup.HorizontalTextAlignment = TextAlignment.End;
            recheckup.HorizontalOptions = LayoutOptions.End;
            recheckup.MaxLines = 1;

            Label linesInterv = new Label();
            linesInterv.HeightRequest = 1;
            linesInterv.BackgroundColor = Color.LightGray;
            ;
            linesInterv.VerticalOptions = LayoutOptions.Center;
            linesInterv.Margin = new Thickness(0, 2, 0, 0);
            linesInterv.HorizontalOptions = LayoutOptions.FillAndExpand;

            recheckStack.Children.Add(recheckLbl);
            recheckStack.Children.Add(linesInterv);
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


            count1Stack.Orientation = StackOrientation.Horizontal;
            count1Stack.HorizontalOptions = LayoutOptions.FillAndExpand;
            counterDate1.FontSize = 15;
            counterDate1.TextColor = Color.FromHex("#A2A2A2");
            counterDate1.HorizontalTextAlignment = TextAlignment.Start;
            counterDate1.HorizontalOptions = LayoutOptions.Start;
            counterDate1.MaxLines = 1;
            // counterDate1.WidthRequest = 150;
            count1.FontSize = 15;
            count1.TextColor = Color.Black;
            count1.HorizontalTextAlignment = TextAlignment.End;
            count1.HorizontalOptions = LayoutOptions.End;
            count1.VerticalOptions = LayoutOptions.Start;
            count1.MaxLines = 1;

            Label lines = new Label();
            lines.HeightRequest = 1;
            lines.BackgroundColor = Color.LightGray;
            ;
            lines.VerticalOptions = LayoutOptions.Center;
            lines.HorizontalOptions = LayoutOptions.FillAndExpand;
            count1Stack.Children.Add(counterDate1);
            count1Stack.Children.Add(lines);
            count1Stack.Children.Add(count1);
            container.Children.Add(count1Stack);

            count2Stack.Orientation = StackOrientation.Horizontal;
            count2Stack.HorizontalOptions = LayoutOptions.FillAndExpand;
            counterDate2.FontSize = 15;
            counterDate2.TextColor = Color.FromHex("#A2A2A2");
            counterDate2.HorizontalTextAlignment = TextAlignment.Start;
            counterDate2.HorizontalOptions = LayoutOptions.Start;
            counterDate2.MaxLines = 1;
            // counterDate2.WidthRequest = 150;
            count2.FontSize = 15;
            count2.TextColor = Color.Black;
            count2.HorizontalTextAlignment = TextAlignment.End;
            count2.HorizontalOptions = LayoutOptions.End;
            count2.VerticalOptions = LayoutOptions.Center;
            count2.MaxLines = 1;

            Label lines2 = new Label();
            lines2.HeightRequest = 1;
            lines2.BackgroundColor = Color.LightGray;
            ;
            lines2.VerticalOptions = LayoutOptions.Center;
            lines2.HorizontalOptions = LayoutOptions.FillAndExpand;

            count2Stack.Children.Add(counterDate2);
            count2Stack.Children.Add(lines2);
            count2Stack.Children.Add(count2);
            container.Children.Add(count2Stack);

            count3Stack.Orientation = StackOrientation.Horizontal;
            count3Stack.HorizontalOptions = LayoutOptions.FillAndExpand;
            counterDate3.FontSize = 15;
            counterDate3.TextColor = Color.FromHex("#A2A2A2");
            counterDate3.HorizontalTextAlignment = TextAlignment.Start;
            counterDate3.HorizontalOptions = LayoutOptions.Start;
            counterDate3.MaxLines = 1;
            // counterDate3.WidthRequest = 150;
            count3.FontSize = 15;
            count3.TextColor = Color.Black;
            count3.HorizontalTextAlignment = TextAlignment.End;
            count3.HorizontalOptions = LayoutOptions.End;
            count3.VerticalOptions = LayoutOptions.Center;
            count3.MaxLines = 1;

            Label lines3 = new Label();
            lines3.HeightRequest = 1;
            lines3.BackgroundColor = Color.LightGray;
            ;
            lines3.VerticalOptions = LayoutOptions.Center;
            lines3.HorizontalOptions = LayoutOptions.FillAndExpand;

            count3Stack.Children.Add(counterDate3);
            count3Stack.Children.Add(lines3);
            count3Stack.Children.Add(count3);
            container.Children.Add(count3Stack);

            frameBtn.HorizontalOptions = LayoutOptions.FillAndExpand;
            frameBtn.VerticalOptions = LayoutOptions.Start;
            frameBtn.BackgroundColor = Color.FromHex(Settings.MobileSettings.color);
            frameBtn.CornerRadius = 10;
            frameBtn.Margin = new Thickness(0, 10, 0, 0);
            frameBtn.Padding = 12;

            containerBtn.Orientation = StackOrientation.Horizontal;
            containerBtn.HorizontalOptions = LayoutOptions.CenterAndExpand;

            Label btn = new Label();
            btn.Margin = new Thickness(0, 0, 0, 0);
            btn.TextColor = Color.White;
            btn.FontAttributes = FontAttributes.Bold;
            btn.VerticalTextAlignment = TextAlignment.Center;
            btn.FontSize = 15;
            btn.Text = AppResources.PassPenance;
            containerBtn.Children.Add(new Image()
            {
                Source = "ic_counter",
                HeightRequest = 20
            });
            containerBtn.Children.Add(btn);

            frameBtn.Content = containerBtn;

            container.Children.Add(frameBtn);

            canCount.Text = "Возможность передавать показания доступна с 15 по 25 число текущего месяца!";
            canCount.FontSize = 12;
            canCount.TextDecorations = TextDecorations.Underline;
            canCount.FontAttributes = FontAttributes.Bold;
            canCount.TextColor = Color.Black;
            canCount.HorizontalTextAlignment = TextAlignment.End;
            canCount.HorizontalOptions = LayoutOptions.CenterAndExpand;
            canCount.HorizontalTextAlignment = TextAlignment.Center;

            container.Children.Add(canCount);
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
            BindableProperty.Create("Values", typeof(List<MeterValueInfo>), typeof(MetersThreeCell),
                new List<MeterValueInfo>());

        public static readonly BindableProperty DecimalPointProperty =
            BindableProperty.Create("DecimalPoint", typeof(int), typeof(MetersThreeCell), 3);

        public List<MeterValueInfo> Values
        {
            get { return (List<MeterValueInfo>) GetValue(ValuesProperty); }
            set { SetValue(ValuesProperty, value); }
        }
        public int DecimalPoint
        {
            get => Convert.ToInt32(GetValue(DecimalPointProperty));
            set => SetValue(DecimalPointProperty, value);
        }

        public string Resource
        {
            get
            {
                return ((string) GetValue(ResourceProperty)).ToLower().Contains("водоснабжение")
                    ? $"{(string) GetValue(ResourceProperty)}, м3"
                    : $"{(string) GetValue(ResourceProperty)}, кВт";
            }
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

        void SetEditButton()
        {
            frameBtn.IsVisible = false;
            var stack = (View as Frame).Content as StackLayout;
            stack.Children.RemoveAt(stack.Children.Count - 2);
            stack.Children.RemoveAt(stack.Children.Count - 1);

            stack.Children.Add(new Label()
            {
                Text = $"{AppResources.PenencePassed} {Values[0].Period}",
                FontSize = 14,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center
            });

            int currDay = DateTime.Now.Day;
            if ((Settings.Person.Accounts[0].MetersStartDay <= currDay &&
                 Settings.Person.Accounts[0].MetersEndDay >= currDay) ||
                (Settings.Person.Accounts[0].MetersStartDay == 0 &&
                 Settings.Person.Accounts[0].MetersEndDay == 0))
            {
                var editLabel = new Label()
                {
                    Text = AppResources.ChangePenance,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Color.FromHex(Settings.MobileSettings.color),
                    VerticalTextAlignment = TextAlignment.Center,
                    HorizontalTextAlignment = TextAlignment.Center
                };
                stack.Children.Add(editLabel);
            }    
                
        }
        string GetFormat()
        {
            var dec = DecimalPoint;
            switch (dec)
            {
                case 0: return "{0}";
                case 1: return "{0.0}";
                case 2: return "{0.00}";
                case 3: return "{0.000}";
                
            }
            return "{0.000}";
        }

        protected override async void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (BindingContext != null)
            {
                var editName = new TapGestureRecognizer();
                editName.Tapped += async (s, e) =>
                {
                   await  PopupNavigation.Instance.PushAsync(
                       new EditCounterNameDialog(Color.FromHex(Settings.MobileSettings.color), UniqueNum));
                };
                if (imgEdit.GestureRecognizers.Count > 0)
                {
                    imgEdit.GestureRecognizers[0] = editName;
                }
                else
                {
                    imgEdit.GestureRecognizers.Add(editName);
                }
                
                
                FormattedString formattedResource = new FormattedString();
                formattedResource.Spans.Add(new Span
                {
                    Text = Resource,
                    TextColor = Color.Black,
                    FontAttributes = FontAttributes.Bold,
                    FontSize = 18
                });

                resource.FormattedText = formattedResource;
                adress.Text = Address;
                number.Text = UniqueNum;
                checkup_date.Text = CheckupDate;
                recheckup.Text = RecheckInterval + " лет";
                GetFormat();
                if (Values.Count == 1)
                {
                    counterDate1.Text = Values[0].Period;
                    count1.Text =
                        String.Format(GetFormat(), Values[0].Value); //.ToString(CultureInfo.InvariantCulture);
                    count2Stack.IsVisible = count3Stack.IsVisible = false;
                }
                else if (Values.Count == 2)
                {
                    counterDate1.Text = Values[0].Period;
                    count1.Text =
                        String.Format(GetFormat(), Values[0].Value); //.ToString(CultureInfo.InvariantCulture);
                    counterDate2.Text = Values[1].Period;
                    count2.Text =
                        String.Format(GetFormat(), Values[1].Value); //.ToString(CultureInfo.InvariantCulture);
                    count3Stack.IsVisible = false;
                }
                else if (Values.Count == 3)
                {
                    counterDate1.Text = Values[0].Period;
                    count1.Text =
                        String.Format(GetFormat(), Values[0].Value); //.ToString(CultureInfo.InvariantCulture);
                    counterDate2.Text = Values[1].Period;
                    count2.Text =
                        String.Format(GetFormat(), Values[1].Value); //.ToString(CultureInfo.InvariantCulture);
                    counterDate3.Text = Values[2].Period;
                    count3.Text =
                        String.Format(GetFormat(), Values[2].Value); //.ToString(CultureInfo.InvariantCulture);
                }
                else if (Values.Count == 0)
                {
                    count1Stack.IsVisible = count2Stack.IsVisible = count3Stack.IsVisible = false;
                }


                if (Values.Count > 0 && int.Parse(Values[0].Period.Split('.')[1]) == DateTime.Now.Month)
                {
                    SetEditButton();
                }

                if (Resource.ToLower().Contains("холодное") || Resource.ToLower().Contains("хвс"))
                {
                    img.Source = ImageSource.FromFile("ic_cold_water");
                }
                else if (Resource.ToLower().Contains("горячее") || Resource.ToLower().Contains("гвс") || Resource.ToLower().Contains("подог") || Resource.ToLower().Contains("отопл"))
                {
                    img.Source = ImageSource.FromFile("ic_heat_water");
                }
                else if (Resource.ToLower().Contains("эле"))
                {
                    img.Source = ImageSource.FromFile("ic_electr");
                }
                else
                {
                    img.Source = ImageSource.FromFile("ic_cold_water");
                }

                int currDay = DateTime.Now.Day;
                // currDay = 16;
                frameBtn.IsVisible = true;
                canCount.IsVisible = false;
                if (Settings.Person.Accounts.Count > 0)
                {
                    FormattedString formattedDate = new FormattedString();
                    formattedDate.Spans.Add(new Span
                    {
                        Text = "Возможность передавать показания доступна ",
                        TextColor = Color.FromHex(Settings.MobileSettings.color),
                        FontAttributes = FontAttributes.None,
                        FontSize = 12
                    });
                    if (Settings.Person.Accounts[0].MetersStartDay != null &&
                        Settings.Person.Accounts[0].MetersEndDay != null)
                    {
                        if (Settings.Person.Accounts[0].MetersStartDay != 0 &&
                            Settings.Person.Accounts[0].MetersEndDay != 0)
                        {
                            formattedDate.Spans.Add(new Span
                            {
                                Text = "c " + Settings.Person.Accounts[0].MetersStartDay + " по " +
                                       Settings.Person.Accounts[0].MetersEndDay + " числа ",
                                TextColor = Color.FromHex(Settings.MobileSettings.color),
                                FontAttributes = FontAttributes.Bold,
                                FontSize = 12
                            });
                            formattedDate.Spans.Add(new Span
                            {
                                Text = "текущего месяца!",
                                TextColor = Color.FromHex(Settings.MobileSettings.color),
                                FontAttributes = FontAttributes.None,
                                FontSize = 12
                            });
                        }
                        else
                        {
                            formattedDate.Spans.Add(new Span
                            {
                                Text = "в текущем месяце!",
                                TextColor = Color.FromHex(Settings.MobileSettings.color),
                                FontAttributes = FontAttributes.Bold,
                                FontSize = 12
                            });
                        }
                    }
                    else
                    {
                        formattedDate.Spans.Add(new Span
                        {
                            Text = "в текущем месяце!",
                            TextColor = Color.FromHex(Settings.MobileSettings.color),
                            FontAttributes = FontAttributes.Bold,
                            FontSize = 12
                        });
                    }

                    canCount.FormattedText = formattedDate;
                    if ((Settings.Person.Accounts[0].MetersStartDay <= currDay &&
                         Settings.Person.Accounts[0].MetersEndDay >= currDay) ||
                        (Settings.Person.Accounts[0].MetersStartDay == 0 &&
                         Settings.Person.Accounts[0].MetersEndDay == 0))
                    {
                        frameBtn.IsVisible = true;
                        canCount.IsVisible = false;
                    }
                    else
                    {
                        frameBtn.IsVisible = false;
                        canCount.IsVisible = true;
                    }
                }
            }
        }
    }
}