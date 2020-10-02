using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AiForms.Dialogs;
using AiForms.Dialogs.Abstractions;
using Rg.Plugins.Popup.Services;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PancakeView;
using Xamarin.Forms.Xaml;
using xamarinJKH.CustomRenderers;
using xamarinJKH.DialogViews;
using xamarinJKH.InterfacesIntegration;
using xamarinJKH.Monitor;
using xamarinJKH.Server;
using xamarinJKH.Server.RequestModel;
using xamarinJKH.Tech;
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
        private List<string> period = new List<string>() {AppResources.TodayPeriod, AppResources.WeekPeriod, AppResources.MonthPeriod};
        private Dictionary<string, string> HousesGroup = new Dictionary<string, string>();
        private Dictionary<string, string> Houses = new Dictionary<string, string>();
        private Thickness IconViewNotComplite;
        private Thickness IconViewPrMargin;
        private double IconViewNotCompliteHeightRequest = 11;
        private double IconViewPrHeightRequest = 11;
        private string street = "";

        public MonitorPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            hex = (Color)Application.Current.Resources["MainColor"];
            fontSize = 13;
            fontSize2 = 20;
            fontSize3 = 13;
            StarSize = 33;
            IconViewNotComplite = new Thickness(0, 5, 0, 0);
            IconViewPrMargin = new Thickness(0, 5, 0, 0);
            
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    int statusBarHeight = DependencyService.Get<IStatusBar>().GetHeight();
                    Pancake.Padding = new Thickness(0, statusBarHeight, 0, 0);
                    break;
                default:
                    break;
            }

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    //BackgroundColor = Color.White;
                    // ImageFon.Margin = new Thickness(0, 0, 0, 0);
                    // StackLayout.Margin = new Thickness(0, 33, 0, 0);
                    // IconViewNameUk.Margin = new Thickness(0, 33, 0, 0);
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
                        // BackStackLayout.Margin = new Thickness(5, 25, 0, 0);
                        // IconViewNameUk.Margin = new Thickness(-3, -10, 0, 0);
                        // RelativeLayoutTop.Margin = new Thickness(0, 0, 0, -130);
                        IconViewNotComplite = 0;
                        IconViewNotCompliteHeightRequest = 8;
                        IconViewPrHeightRequest = 8;
                        IconViewPrMargin = 0;
                    }

                    break;
                default:
                    break;
            }
            var techSend = new TapGestureRecognizer();
            techSend.Tapped += async (s, e) => {     await PopupNavigation.Instance.PushAsync(new TechDialog()); };
            LabelTech.GestureRecognizers.Add(techSend);
            var addClick = new TapGestureRecognizer();
            addClick.Tapped += async (s, e) => { await StartStatistick(); };
            StackLayoutGroup.GestureRecognizers.Add(addClick);
            var addClickHome = new TapGestureRecognizer();
            addClickHome.Tapped += async (s, e) => { await StartStatistick(false); };
            StackLayoutHouse.GestureRecognizers.Add(addClickHome);

            var colapse = new TapGestureRecognizer();
            colapse.Tapped += async (s, e) =>
            {
                LayoutGrid.IsVisible = !LayoutGrid.IsVisible;
                if (LayoutGrid.IsVisible)
                {
                    IconViewArrow.Source = "ic_arrow_up_monitorpng";
                    MaterialFrameNotDoingContainer.Padding = new Thickness(0, 0, 0, 25);
                    colapseAll(AppResources.FailedRequests);
                }
                else
                {
                    IconViewArrow.Source = "ic_arrow_down_monitor";
                    MaterialFrameNotDoingContainer.Padding = 0;
                }
            };

            MaterialFrameNotDoing.GestureRecognizers.Add(colapse);

            _visibleModels.Add(AppResources.FailedRequests, new VisibleModel()
            {
                //IconView = IconViewArrow,
                _materialFrame = MaterialFrameNotDoingContainer,
                _grid = LayoutGrid
            });
            SetText();
            ChangeTheme = new Command(async () =>
            {
                SetAdminName();
            });
            MessagingCenter.Subscribe<Object>(this, "ChangeAdminMonitor", (sender) => ChangeTheme.Execute(null));
            BindingContext = this;
        }


        public RestClientMP _server = new RestClientMP();

        async Task getMonitorStandart(int id, int houseID = -1)
        {
            if (Xamarin.Essentials.Connectivity.NetworkAccess != Xamarin.Essentials.NetworkAccess.Internet)
            {
                Device.BeginInvokeOnMainThread(async () => await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorNoInternet, "OK"));
                return;
            }
            ItemsList<RequestStats> result = await _server.RequestStats(id, houseID);
            if (result.Error == null)
            {
                if (result.Data.Count > 0)
                    setMonitoring(result.Data[0]);
            }
            else
            {
                await DisplayAlert(AppResources.ErrorTitle, result.Error, "OK");
            }
        }

        void setMonitoring(RequestStats result)
        {
            List<PeriodStats> periodStatses = new List<PeriodStats>();
            periodStatses.Add(result.Today);
            periodStatses.Add(result.Week);
            periodStatses.Add(result.Month);
            setNotDoingApps(result.TotalUnperformedRequestsList);
            int i = 0;
            foreach (var each in periodStatses)
            {
                MaterialFrame container = new MaterialFrame();
                container.SetAppThemeColor(Frame.BorderColorProperty, (Color)Application.Current.Resources["MainColor"], Color.White);
                container.Elevation = 20;
                container.Margin = new Thickness(20, 0, 20, 10);
                container.CornerRadius = 35;
                container.BackgroundColor = Color.White;
                container.Padding = new Thickness(0, 0, 0, 25);

                StackLayout stackLayoutFrame = new StackLayout();
                stackLayoutFrame.Spacing = 0;

                container.Content = stackLayoutFrame;

                MaterialFrame materialFrameTop = new MaterialFrame();
                materialFrameTop.SetAppThemeColor(Frame.BorderColorProperty, (Color)Application.Current.Resources["MainColor"], Color.White);
                materialFrameTop.Elevation = 20;
                materialFrameTop.CornerRadius = 35;
                materialFrameTop.BackgroundColor = Color.White;
                materialFrameTop.Padding = new Thickness(0, 25, 0, 25);

                stackLayoutFrame.Children.Add(materialFrameTop);

                StackLayout stackLayoutTop = new StackLayout();
                stackLayoutTop.Orientation = StackOrientation.Horizontal;

                materialFrameTop.Content = stackLayoutTop;

                IconView iconViewTop = new IconView()
                {
                    Source = "ic_calendar",
                    Foreground = hex,
                    HeightRequest = 30,
                    Margin = new Thickness(-10, 0, 0, 0),
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Center
                };

                Label labelTitleTop = new Label()
                {
                    Text = period[i],
                    FontSize = 15,
                    TextColor = Color.Black,
                    FontAttributes = FontAttributes.Bold,
                    Margin = new Thickness(-20, 0, 0, 0),
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.Center
                };

                IconView iconViewArrow = new IconView()
                {
                    Source = "ic_arrow_up_monitorpng",
                    HeightRequest = 25,
                    WidthRequest = 25,
                    Foreground = hex,
                    Margin = new Thickness(0, 0, 15, 0),
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.Center
                };


                stackLayoutTop.Children.Add(iconViewTop);
                stackLayoutTop.Children.Add(labelTitleTop);
                stackLayoutTop.Children.Add(iconViewArrow);

                StackLayout stackLayoutBot = new StackLayout()
                {
                    Margin = new Thickness(20, 0, 20, 0),
                    Spacing = 0
                };

                var colapse = new TapGestureRecognizer();
                colapse.Tapped += async (s, e) =>
                {
                    stackLayoutBot.IsVisible = !stackLayoutBot.IsVisible;
                    if (stackLayoutBot.IsVisible)
                    {
                        iconViewArrow.Source = "ic_arrow_up_monitorpng";
                        container.Padding = new Thickness(0, 0, 0, 25);
                        colapseAll(labelTitleTop.Text);
                    }
                    else
                    {
                        iconViewArrow.Source = "ic_arrow_down_monitor";
                        container.Padding = 0;
                    }
                };

                if (i > 0)
                {
                    stackLayoutBot.IsVisible = false;
                    iconViewArrow.Source = "ic_arrow_down_monitor";
                    container.Padding = 0;
                }

                if (!_visibleModels.ContainsKey(period[i]))
                {
                    _visibleModels.Add(period[i], new VisibleModel()
                    {
                        IconView = iconViewArrow,
                        _materialFrame = container,
                        _grid = stackLayoutBot
                    });
                }
                else
                {
                    _visibleModels[period[i]] = new VisibleModel()
                    {
                        //IconView = IconViewArrow,
                        _materialFrame = container,
                        _grid = stackLayoutBot
                    };
                }

                materialFrameTop.GestureRecognizers.Add(colapse);

                stackLayoutFrame.Children.Add(stackLayoutBot);

                FormattedString formatted = new FormattedString();
                formatted.Spans.Add(new Span
                {
                    Text = $"{AppResources.RequestsReceived} ",
                    TextColor = Color.Black
                });
                formatted.Spans.Add(new Span
                {
                    Text = each.RequestsCount.ToString(),
                    TextColor = hex,
                    FontAttributes = FontAttributes.Bold
                });

                Label labelCountApps = new Label()
                {
                    FontSize = 15,
                    Margin = new Thickness(0, 10, 0, 10),
                    TextColor = Color.Black,
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.Center,
                    FormattedText = formatted
                };

                Grid grid = new Grid
                {
                    RowSpacing = 0,
                    RowDefinitions =
                    {
                        new RowDefinition {Height = new GridLength(1, GridUnitType.Star)},
                    },
                    ColumnDefinitions =
                    {
                        new ColumnDefinition {Width = new GridLength(1, GridUnitType.Star)},
                        new ColumnDefinition {Width = new GridLength(1, GridUnitType.Absolute)},
                        new ColumnDefinition {Width = new GridLength(1, GridUnitType.Star)},
                    }
                };

                StackLayout stackLayoutGridOne = new StackLayout();

                StackLayout stackLayoutNotDoing = new StackLayout()
                {
                    Orientation = StackOrientation.Horizontal
                };

                IconView iconViewDis = new IconView()
                {
                    Source = "ic_dislike",
                    HeightRequest = fontSize2,
                    WidthRequest = fontSize2,
                    Foreground = hex,
                    HorizontalOptions = LayoutOptions.Center
                };

                Label labelNotDoing = new Label()
                {
                    Text = $"{AppResources.FailedReq}:",
                    FontSize = fontSize3,
                    TextColor = Color.Black,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Start
                };

                StackLayout stackLayoutnotDoingCount = new StackLayout()
                {
                    Spacing = 0
                };

                StackLayout stackLayoutNotDoingContent = new StackLayout()
                {
                    Orientation = StackOrientation.Horizontal,
                    Spacing = 0
                };

                Label labelCountNotDoing = new Label()
                {
                    Text = each.UnperformedRequestsList.Count.ToString(),
                    TextColor = hex,
                    FontSize = fontSize,
                    FontAttributes = FontAttributes.Bold
                };

                var forwardAppsNot = new TapGestureRecognizer();
                forwardAppsNot.Tapped += async (s, e) =>
                {
                    if (each.UnperformedRequestsList.Count > 0)
                    {
                        await Navigation.PushAsync(new MonitorAppsPage(each.UnperformedRequestsList));
                    }
                };

                stackLayoutNotDoingContent.GestureRecognizers.Add(forwardAppsNot);


                IconView iconViewArrowForward = new IconView()
                {
                    Source = "ic_arrow_forward",
                    HeightRequest = IconViewNotCompliteHeightRequest,
                    WidthRequest = IconViewNotCompliteHeightRequest,
                    Margin = IconViewNotComplite,
                    VerticalOptions = LayoutOptions.Center,
                    Foreground = hex,
                    HorizontalOptions = LayoutOptions.Center
                };

                stackLayoutNotDoingContent.Children.Add(labelCountNotDoing);
                stackLayoutNotDoingContent.Children.Add(iconViewArrowForward);

                Label labelSeparatorNotDoing = new Label()
                {
                    HeightRequest = 1,
                    BackgroundColor = hex,
                    HorizontalOptions = LayoutOptions.Fill
                };

                stackLayoutnotDoingCount.Children.Add(stackLayoutNotDoingContent);
                stackLayoutnotDoingCount.Children.Add(labelSeparatorNotDoing);


                stackLayoutNotDoing.Children.Add(iconViewDis);
                stackLayoutNotDoing.Children.Add(labelNotDoing);
                stackLayoutNotDoing.Children.Add(stackLayoutnotDoingCount);

                stackLayoutGridOne.Children.Add(stackLayoutNotDoing);

                Label separatorVertical = new Label()
                {
                    WidthRequest = 1,
                    BackgroundColor = Color.FromHex("#878787"),
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand
                };

                StackLayout stackLayoutUnperformed = new StackLayout()
                {
                    Orientation = StackOrientation.Horizontal,
                    VerticalOptions = LayoutOptions.Start
                };

                IconView iconViewUnperformed = new IconView()
                {
                    Source = "ic_time",
                    HeightRequest = fontSize2,
                    WidthRequest = fontSize2,
                    Foreground = hex,
                    HorizontalOptions = LayoutOptions.Center
                };
                Label labelUnperformed = new Label()
                {
                    Text = AppResources.Overdue,
                    FontSize = fontSize3,
                    TextColor = Color.Black,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Start
                };

                StackLayout stackLayoutUnperformedCount = new StackLayout()
                {
                    Spacing = 0,
                    VerticalOptions = LayoutOptions.Center
                };

                StackLayout stackLayoutUnperformedContent = new StackLayout()
                {
                    Orientation = StackOrientation.Horizontal,
                    Spacing = 0
                };

                Label labelCountUnperformed = new Label()
                {
                    Text = each.OverdueRequestsList.Count.ToString(),
                    TextColor = hex,
                    FontSize = fontSize,
                    FontAttributes = FontAttributes.Bold
                };
                IconView iconViewUnperformedArrowForward = new IconView()
                {
                    Source = "ic_arrow_forward",
                    HeightRequest = IconViewPrHeightRequest,
                    WidthRequest = IconViewPrHeightRequest,
                    Margin = IconViewPrMargin,
                    VerticalOptions = LayoutOptions.Start,
                    Foreground = hex,
                    HorizontalOptions = LayoutOptions.Center
                };

                var forwardAppsUnper = new TapGestureRecognizer();
                forwardAppsUnper.Tapped += async (s, e) =>
                {
                    if (each.OverdueRequestsList.Count > 0)
                        await Navigation.PushAsync(new MonitorAppsPage(each.OverdueRequestsList));
                };

                stackLayoutUnperformedCount.GestureRecognizers.Add(forwardAppsUnper);

                stackLayoutUnperformedContent.Children.Add(labelCountUnperformed);
                stackLayoutUnperformedContent.Children.Add(iconViewUnperformedArrowForward);

                Label labelSeparatorUnperformed = new Label()
                {
                    HeightRequest = 1,
                    BackgroundColor = hex,
                    HorizontalOptions = LayoutOptions.Fill
                };

                stackLayoutUnperformedCount.Children.Add(stackLayoutUnperformedContent);
                stackLayoutUnperformedCount.Children.Add(labelSeparatorUnperformed);

                stackLayoutUnperformed.Children.Add(iconViewUnperformed);
                stackLayoutUnperformed.Children.Add(labelUnperformed);
                stackLayoutUnperformed.Children.Add(stackLayoutUnperformedCount);

                grid.Children.Add(stackLayoutGridOne, 0, 0);
                grid.Children.Add(separatorVertical, 1, 0);
                grid.Children.Add(stackLayoutUnperformed, 2, 0);

                setCustumerXaml(ref grid, each);

                Label separatorHorizontal = new Label()
                {
                    HeightRequest = 1,
                    Margin = new Thickness(0, 10, 0, 0),
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    BackgroundColor = Color.FromHex("#878787")
                };

                StackLayout stackLayoutStar = new StackLayout()
                {
                    Orientation = StackOrientation.Horizontal
                };

                StackLayout stackLayoutPoint = new StackLayout()
                {
                    HorizontalOptions = LayoutOptions.StartAndExpand,
                    VerticalOptions = LayoutOptions.Center,
                    Orientation = StackOrientation.Vertical
                };

                Label labelTitlePoint = new Label()
                {
                    Text = $"{AppResources.Marks}:",
                    FontSize = fontSize,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Color.Black
                };
                stackLayoutPoint.Children.Add(labelTitlePoint);

                StackLayout stackLayoutStarItems = new StackLayout()
                {
                    Spacing = 5,
                    HorizontalOptions = LayoutOptions.EndAndExpand,
                    VerticalOptions = LayoutOptions.Center,
                    Orientation = StackOrientation.Horizontal
                };

                int[] points = new[]
                {
                    each.ClosedRequestsWithMark1Count,
                    each.ClosedRequestsWithMark2Count,
                    each.ClosedRequestsWithMark3Count,
                    each.ClosedRequestsWithMark4Count,
                    each.ClosedRequestsWithMark5Count
                };

                for (int j = 0; j < points.Length; j++)
                {
                    StackLayout stackLayoutStarItem = new StackLayout()
                    {
                        Spacing = -10
                    };
                    var forwardStar = new TapGestureRecognizer();
                    List<Requests> requests = getRequestsStar(each, j);
                    forwardStar.Tapped += async (s, e) =>
                    {
                        if (requests.Count > 0)
                        {
                            await Navigation.PushAsync(new MonitorAppsPage(requests));
                        }
                    };

                    stackLayoutStarItem.GestureRecognizers.Add(forwardStar);
                    Frame frameStarContainer = new Frame()
                    {
                        Padding = 0,
                        CornerRadius = 0,
                        HasShadow = false,
                        BackgroundColor = Color.Transparent,
                        IsClippedToBounds = true
                    };

                    Grid gridContentStar = new Grid()
                    {
                        HeightRequest = 50,
                        IsClippedToBounds = true
                    };

                    IconView iconViewStar = new IconView()
                    {
                        Source = "ic_star",
                        HeightRequest = StarSize,
                        WidthRequest = StarSize,
                        Foreground = hex,
                        VerticalOptions = LayoutOptions.Center
                    };

                    Frame frameContentStar = new Frame()
                    {
                        Margin = 0,
                        Padding = 0,
                        CornerRadius = 5,
                        BackgroundColor = Color.Transparent,
                        HasShadow = false,
                        IsClippedToBounds = true
                    };

                    Label labelTextStar = new Label()
                    {
                        Text = points[j].ToString(),
                        FontSize = fontSize,
                        HorizontalOptions = LayoutOptions.Center,
                        Margin = 0,
                        VerticalOptions = LayoutOptions.Center,
                        TextColor = Color.White
                    };

                    frameContentStar.Content = labelTextStar;

                    gridContentStar.Children.Add(iconViewStar);
                    gridContentStar.Children.Add(frameContentStar);

                    frameStarContainer.Content = gridContentStar;

                    Label labelNumberPoints = new Label()
                    {
                        Text = (j + 1).ToString(),
                        FontSize = fontSize,
                        TextColor = Color.FromHex("#878787"),
                        HorizontalOptions = LayoutOptions.Center
                    };

                    stackLayoutStarItem.Children.Add(frameStarContainer);
                    stackLayoutStarItem.Children.Add(labelNumberPoints);


                    stackLayoutStarItems.Children.Add(stackLayoutStarItem);
                }


                stackLayoutStar.Children.Add(stackLayoutPoint);
                stackLayoutStar.Children.Add(stackLayoutStarItems);

                stackLayoutBot.Children.Add(labelCountApps);
                stackLayoutBot.Children.Add(grid);
                stackLayoutBot.Children.Add(separatorHorizontal);
                stackLayoutBot.Children.Add(stackLayoutStar);


                i++;
                LayoutContent.Children.Add(container);
            }
        }

        List<Requests> getRequestsStar(PeriodStats stats, int i)
        {
            switch (i)
            {
                case 0: return stats.ClosedRequestsWithMark1List;
                case 1: return stats.ClosedRequestsWithMark2List;
                case 2: return stats.ClosedRequestsWithMark3List;
                case 3: return stats.ClosedRequestsWithMark4List;
                case 4: return stats.ClosedRequestsWithMark5List;
                default: return new List<Requests>();
            }
        }

        public async Task StartStatistick(bool isGroup = true)
        {
            // Loading settings
            Configurations.LoadingConfig = new LoadingConfig
            {
                IndicatorColor = hex,
                OverlayColor = Color.Black,
                Opacity = 0.8,
                DefaultMessage = AppResources.MonitorStats,
            };

            await Loading.Instance.StartAsync(async progress =>
            {
                // some heavy process.
                if (isGroup)
                    await getHouseGroups();
                else
                {
                    await getHouse();
                }
            });
        }

        void colapseAll(string name)
        {
            foreach (var each in _visibleModels)
            {
                if (!each.Key.Equals(name) && each.Value._grid.IsVisible)
                {
                    each.Value._grid.IsVisible = false;
                    each.Value._materialFrame.Padding = 0;
                    each.Value.IconView.Source = "ic_arrow_down_monitor";
                }
            }
        }

        async Task getHouseGroups()
        {
            if (Xamarin.Essentials.Connectivity.NetworkAccess != Xamarin.Essentials.NetworkAccess.Internet)
            {
                Device.BeginInvokeOnMainThread(async () => await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorNoInternet, "OK"));
                return;
            }
            ItemsList<NamedValue> groups = await _server.GetHouseGroups();
            if (groups.Error == null)
            {
                string[] param = null;
                setListGroups(groups, ref param);
                var action = await DisplayActionSheet(AppResources.AreaChoose, AppResources.Cancel, null, param);
                if (action != null && !action.Equals(AppResources.Cancel))
                {
                    LayoutContent.Children.Clear();
                    MaterialFrameNotDoingContainer.IsVisible = false;
                    LabelGroup.Text = action;
                    street = action;
                    await getMonitorStandart(Int32.Parse(HousesGroup[action]));
                }
            }
            else
            {
                await DisplayAlert(AppResources.ErrorTitle, groups.Error, "OK");
            }
        }

        async Task getHouse()
        {
            if (Xamarin.Essentials.Connectivity.NetworkAccess != Xamarin.Essentials.NetworkAccess.Internet)
            {
                Device.BeginInvokeOnMainThread(async () => await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorNoInternet, "OK"));
                return;
            }
            ItemsList<HouseProfile> groups = await _server.GetHouse();
            if (groups.Error == null)
            {
                string[] param = null;
                setListHouse(groups, ref param);
                var action = await DisplayActionSheet(AppResources.HomeChoose, AppResources.Cancel, null, param);
                if (action != null && !action.Equals(AppResources.Cancel))
                {
                    LayoutContent.Children.Clear();
                    MaterialFrameNotDoingContainer.IsVisible = false;
                    LabelHouse.Text = action;
                    await getMonitorStandart(-1, Int32.Parse(Houses[action]));
                }
            }
            else
            {
                await DisplayAlert(AppResources.ErrorTitle, groups.Error, "OK");
            }
        }

        void setListGroups(ItemsList<NamedValue> groups, ref string[] param)
        {
            HousesGroup = new Dictionary<string, string>();
            param = new string [groups.Data.Count];
            int i = 0;
            foreach (var each in groups.Data)
            {
                HousesGroup.Add(each.Name, each.ID);
                param[i] = each.Name;
                i++;
            }
        }

        void setListHouse(ItemsList<HouseProfile> groups, ref string[] param)
        {
            Houses = new Dictionary<string, string>();
            param = new string [groups.Data.Count];
            int i = 0;
            foreach (var each in groups.Data)
            {
                try
                {
                    if (each.Address != null)
                    {
                        Houses.Add(each.Address, each.ID.ToString());
                        param[i] = each.Address;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                i++;
            }
        }

        void setCustumerXaml(ref Grid grid, PeriodStats periodStats)
        {
            Dictionary<string, int> UnperformedMap = setPerformer(periodStats.UnperformedRequestsList);
            Dictionary<string, int> Overdue = setPerformer(periodStats.OverdueRequestsList);

            int max = Math.Max(UnperformedMap.Count, Overdue.Count);
            for (int i = 0; i < max; i++)
            {
                RowDefinition rowDefinition = new RowDefinition()
                {
                    Height = new GridLength(1, GridUnitType.Star)
                };
                grid.RowDefinitions.Add(rowDefinition);
                Label labelSeparatorUnperformed = new Label()
                {
                    MinimumWidthRequest = 1,
                    BackgroundColor = Color.FromHex("#878787"),
                    VerticalOptions = LayoutOptions.Fill
                };
                grid.Children.Add(labelSeparatorUnperformed, 1, i + 1);
            }

            int j;
            VisiblePerformers(grid, UnperformedMap, 0, periodStats.UnperformedRequestsList);
            VisiblePerformers(grid, Overdue, 2, periodStats.OverdueRequestsList);
        }

        private void VisiblePerformers(Grid grid, Dictionary<string, int> UnperformedMap, int position,
            List<Requests> requestses, bool space = true)
        {
            int j = space ? 1 : 0;

            foreach (var each in UnperformedMap)
            {
                StackLayout stackLayoutNotDoing = new StackLayout()
                {
                    Orientation = StackOrientation.Horizontal
                };

                if (j == 1)
                {
                    stackLayoutNotDoing.Margin = new Thickness(0, 5, 0, 0);
                }


                Label labelNotDoing = new Label()
                {
                    Text = each.Key.Split()[0],
                    FontSize = fontSize3,
                    TextColor = Color.Black,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Fill
                };

                StackLayout stackLayoutnotDoingCount = new StackLayout()
                {
                    Spacing = 0,
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.Center
                };

                StackLayout stackLayoutNotDoingContent = new StackLayout()
                {
                    Orientation = StackOrientation.Horizontal,
                    Spacing = 0
                };

                Label labelCountNotDoing = new Label()
                {
                    Text = each.Value.ToString(),
                    TextColor = hex,
                    FontSize = fontSize,
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.FillAndExpand
                };
                IconView iconViewArrowForward = new IconView()
                {
                    Source = "ic_arrow_forward",
                    HeightRequest = IconViewNotCompliteHeightRequest,
                    WidthRequest = IconViewNotCompliteHeightRequest,
                    Margin = IconViewNotComplite,
                    VerticalOptions = LayoutOptions.Center,
                    Foreground = hex,
                    HorizontalOptions = LayoutOptions.Center
                };

                var forwardAppsNot = new TapGestureRecognizer();
                List<Requests> requests = getRequests(each.Key, requestses);
                forwardAppsNot.Tapped += async (s, e) =>
                {
                    if (requests.Count > 0)
                    {
                        await Navigation.PushAsync(new MonitorAppsPage(requests));
                    }
                };

                stackLayoutNotDoingContent.GestureRecognizers.Add(forwardAppsNot);

                stackLayoutNotDoingContent.Children.Add(labelCountNotDoing);
                stackLayoutNotDoingContent.Children.Add(iconViewArrowForward);

                Label labelSeparatorNotDoing = new Label()
                {
                    HeightRequest = 1,
                    BackgroundColor = hex,
                    HorizontalOptions = LayoutOptions.Fill
                };

                stackLayoutnotDoingCount.Children.Add(stackLayoutNotDoingContent);
                stackLayoutnotDoingCount.Children.Add(labelSeparatorNotDoing);


                stackLayoutNotDoing.Children.Add(labelNotDoing);
                stackLayoutNotDoing.Children.Add(stackLayoutnotDoingCount);

                grid.Children.Add(stackLayoutNotDoing, position, j);
                j++;
            }
        }

        List<Requests> getRequests(string name, List<Requests> result)
        {
            List<Requests> requestses = new List<Requests>();
            foreach (var each in result)
            {
                var eachPerformer = each.Performer;
                if (eachPerformer == null || eachPerformer.Equals("") || eachPerformer.Equals("-"))
                {
                    if (name.Equals("Сотрудник"))
                    {
                        requestses.Add(each);
                    }
                }
                else if (eachPerformer.Equals(name))
                {
                    requestses.Add(each);
                }
            }

            return requestses;
        }


        void setNotDoingApps(List<Requests> requestses)
        {
            MaterialFrameNotDoingContainer.IsVisible = true;
            LayoutGrid.Children.Clear();
            Grid grid = new Grid
            {
                RowSpacing = 0,
                ColumnDefinitions =
                {
                    new ColumnDefinition {Width = new GridLength(1, GridUnitType.Star)},
                    new ColumnDefinition {Width = new GridLength(1, GridUnitType.Absolute)},
                    new ColumnDefinition {Width = new GridLength(1, GridUnitType.Star)},
                }
            };

            LayoutGrid.Children.Add(grid);
            FormattedString formatted = new FormattedString();
            IconViewArrow.Source = "ic_arrow_down_monitor";
            MaterialFrameNotDoingContainer.Padding = 0;
            LayoutGrid.IsVisible = false;
            formatted.Spans.Add(new Span
            {
                Text = $"{AppResources.FailedRequests}: ",
                TextColor = Color.Black
            });
            formatted.Spans.Add(new Span
            {
                Text = requestses.Count.ToString(),
                TextColor = hex,
                FontAttributes = FontAttributes.Bold
            });
            LabelNotDoingCount.FormattedText = formatted;

            Dictionary<string, int> dictionary = setPerformer(requestses);
            Dictionary<string, int> dictionaryFirst = new Dictionary<string, int>();
            Dictionary<string, int> dictionarySecond = new Dictionary<string, int>();
            int half = dictionary.Count / 2;

            int i = 0;
            foreach (var each in dictionary)
            {
                if (i <= half)
                    dictionaryFirst.Add(each.Key, each.Value);
                else
                    dictionarySecond.Add(each.Key, each.Value);
                ;
                i++;
            }


            int max = Math.Max(dictionaryFirst.Count, dictionarySecond.Count);
            for (int j = 0; j < max; j++)
            {
                RowDefinition rowDefinition = new RowDefinition()
                {
                    Height = new GridLength(1, GridUnitType.Star)
                };
                grid.RowDefinitions.Add(rowDefinition);
                Label labelSeparatorUnperformed = new Label()
                {
                    MinimumWidthRequest = 1,
                    BackgroundColor = Color.FromHex("#878787"),
                    VerticalOptions = LayoutOptions.Fill
                };
                grid.Children.Add(labelSeparatorUnperformed, 1, j);
            }


            VisiblePerformers(grid, dictionaryFirst, 0, requestses, false);
            VisiblePerformers(grid, dictionarySecond, 2, requestses, false);
        }


        Dictionary<string, int> setPerformer(List<Requests> requestses)
        {
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            try
            {
                foreach (var each in requestses)
                {
                    var eachPerformer = each.Performer;
                    if (eachPerformer == null || eachPerformer.Equals("") || eachPerformer.Equals("-"))
                    {
                        eachPerformer = "Сотрудник";
                    }

                    if (!dictionary.ContainsKey(eachPerformer))
                    {
                        dictionary.Add(eachPerformer, 1);
                    }
                    else
                    {
                        dictionary[eachPerformer]++;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return dictionary;
        }

        public Command ChangeTheme { get; set; }
        void SetText()
        {
            UkName.Text = Settings.MobileSettings.main_name;
            SetAdminName();

            Color hexColor = (Color) Application.Current.Resources["MainColor"];
            IconViewLogin.SetAppThemeColor(IconView.ForegroundProperty, hexColor, Color.White);
            IconViewTech.SetAppThemeColor(IconView.ForegroundProperty, hexColor, Color.White);
            Pancake.SetAppThemeColor(PancakeView.BorderColorProperty, hexColor, Color.Transparent);
            PancakeViewIcon.SetAppThemeColor(PancakeView.BorderColorProperty, hexColor, Color.Transparent);if (Device.RuntimePlatform == Device.iOS){ if (AppInfo.PackageName == "rom.best.saburovo" || AppInfo.PackageName == "sys_rom.ru.tsg_saburovo"){PancakeViewIcon.Padding = new Thickness(0);}}
            Frame.SetAppThemeColor(Frame.BorderColorProperty, hexColor, Color.FromHex("#8D8D8D"));
            MaterialFrameNotDoingContainer.SetAppThemeColor(Frame.BorderColorProperty, hexColor, Color.White);
            MaterialFrameNotDoing.SetAppThemeColor(Frame.BorderColorProperty, hexColor, Color.White);
            LabelTech.SetAppThemeColor(Label.TextColorProperty, hexColor, Color.White);
        }

        private void SetAdminName()
        {
            FormattedString formatted = new FormattedString();
            OSAppTheme currentTheme = Application.Current.RequestedTheme;
            //if (Xamarin.Essentials.DeviceInfo.Platform == Xamarin.Essentials.DevicePlatform.iOS)
            //    currentTheme = OSAppTheme.Dark;
            
            formatted.Spans.Add(new Span
            {
                Text = Settings.Person.FIO + ", ",
                TextColor = currentTheme.Equals(OSAppTheme.Dark) ? Color.White : Color.Black,
                FontAttributes = FontAttributes.Bold,
                FontSize = 15
            });
            formatted.Spans.Add(new Span
            {
                Text = AppResources.GoodDay2,
                TextColor = currentTheme.Equals(OSAppTheme.Dark) ? Color.White : Color.Black,
                FontSize = 15
            });
            LabelPhone.FormattedText = formatted;
        }

        Dictionary<string, VisibleModel> _visibleModels = new Dictionary<string, VisibleModel>();

        class VisibleModel
        {
            public IconView IconView { get; set; }
            public MaterialFrame _materialFrame { get; set; }
            public StackLayout _grid { get; set; }
        }
    }
}