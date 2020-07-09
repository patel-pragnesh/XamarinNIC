using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.Messaging;
using Xamarin.Forms;

using Xamarin.Forms.Xaml;
using xamarinJKH.Server;
using xamarinJKH.Tech;
using xamarinJKH.Utils;

namespace xamarinJKH
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OSSMain : ContentPage
    {
        public OSSMain()
        {
            InitializeComponent();
            var techSend = new TapGestureRecognizer();
            techSend.Tapped += async (s, e) => {     await Navigation.PushAsync(new TechSendPage()); };
            LabelTech.GestureRecognizers.Add(techSend);
            var call = new TapGestureRecognizer();
            call.Tapped += async (s, e) =>
            {
                if (Settings.Person.Phone != null)
                {
                    IPhoneCallTask phoneDialer;
                    phoneDialer = CrossMessaging.Current.PhoneDialer;
                    if (phoneDialer.CanMakePhoneCall) 
                        phoneDialer.MakePhoneCall(Settings.Person.Phone);
                }

            
            };
            LabelPhone.GestureRecognizers.Add(call);
            //switch (Device.RuntimePlatform)
            //{
            //    case Device.iOS:
            //        BackgroundColor = Color.White;
            //        BackgroundColor = Color.White;
            //        ImageTop.Margin = new Thickness(0, 0, 0, 0);
            //        StackLayout.Margin = new Thickness(0, 33, 0, 0);
            //        IconViewNameUk.Margin = new Thickness(0, 33, 0, 0);
            //        RelativeLayoutTop.Margin = new Thickness(0, 0, 0, 0);
            //        if (App.ScreenHeight <= 667)//iPhone6
            //        {
            //            RelativeLayoutTop.Margin = new Thickness(0, 0, 0, -110);
            //        }
            //        else if (App.ScreenHeight <= 736)//iPhone8Plus Height=736
            //        {
            //            RelativeLayoutTop.Margin = new Thickness(0, 0, 0, -145);
            //        }
            //        else
            //        {
            //            RelativeLayoutTop.Margin = new Thickness(0, 0, 0, -145);
            //        }


            //        break;
            //    case Device.Android:
            //        RelativeLayoutTop.Margin = new Thickness(0, 0, 0, -135);
            //        double or = Math.Round(((double)App.ScreenWidth / (double)App.ScreenHeight), 2);
            //        if (Math.Abs(or - 0.5) < 0.02)
            //        {
            //            RelativeLayoutTop.Margin = new Thickness(0, 0, 0, -90);
            //        }
            //        else
            //        {
            //            ossContent.Margin = new Thickness(20, 30, 20, 0);
            //        }                           

            //        break;
            //    default:
            //        break;
            //}
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    BackgroundColor = Color.White;
                    if (Application.Current.MainPage.Height < 800)
                    {
                        ossContent.Margin = new Thickness(10, -180, 10, 0);
                    }
                    else
                    {
                        ossContent.Margin = new Thickness(10, -185, 10, 0);
                    }
                    break;
                case Device.Android:
                    double or = Math.Round(((double)App.ScreenWidth / (double)App.ScreenHeight), 2);
                    //ossContent.Margin = new Thickness(10, 45, 10, 0);
                    break;
                default:
                    break;
            }

            var backClick = new TapGestureRecognizer();
            backClick.Tapped += async (s, e) =>
            {
                try
                {
                    _ = await Navigation.PopAsync();
                }
                catch (Exception ex)
                {
                    _ = await Navigation.PopModalAsync();
                }
            };
            BackStackLayout.GestureRecognizers.Add(backClick);

            UkName.Text = Settings.MobileSettings.main_name;
            LabelPhone.Text =  "+" + Settings.Person.companyPhone.Replace("+","");

            ButtonActive.TextColor = colorFromMobileSettings;
            GetOssData(1);

            NavigationPage.SetHasNavigationBar(this, false);

            
        }

        readonly RestClientMP rc = new RestClientMP();

        IDictionary<Guid, Guid> frames = new Dictionary<Guid, Guid>();
        IDictionary<Guid, Guid> arrows = new Dictionary<Guid, Guid>();

        Color colorFromMobileSettings = Color.FromHex(Settings.MobileSettings.color);

        Frame prevAddtionlExpanded=null;

        async void action(Frame frme, bool hide=false)
        {
            var idAdditional = frames[frme.Id];
            var additionalStack = ((StackLayout)frme.Content).Children.FirstOrDefault(_ => _.Id == idAdditional);
            if(hide)
            {
                additionalStack.IsVisible = false;
            }
            else
            {
                var additionalStackVisibility = additionalStack.IsVisible;
                additionalStack.IsVisible = !additionalStackVisibility;
            }

            var idArrow = arrows[frme.Id];
            var arrow = ((StackLayout)((StackLayout)frme.Content).Children.First()).Children.FirstOrDefault(_ => _.Id == idArrow);
            if (additionalStack.IsVisible)
                await arrow.RotateTo(270);
            else
            {
                await arrow.RotateTo(90);
            }
        }

        
        private async Task<bool> GetOssData(int type=0)
        {
            //получаем данные от сервера по ОСС
            var result = await rc.GetOss(0);

            var haveP =result.Data.Where(_ => _.HasProtocolFile).ToList();


            if(result.Error==null)
            {
                var dateNow = DateTime.Now;
                //Завершенные
                if (type == 0)
                {
                    var rr = result.Data.Where(_ => Convert.ToDateTime(_.DateEnd, new CultureInfo("ru-RU")) < dateNow ).OrderByDescending(_ => _.DateEnd).ToList();
                    result.Data = rr;
                }
                //Активные
                if(type==1)
                {
                    var rr = result.Data.Where(_ => Convert.ToDateTime(_.DateEnd, new CultureInfo("ru-RU")) > dateNow ).OrderByDescending(_=>_.DateEnd).ToList();
                    result.Data = rr;
                }

                frames.Clear();
                arrows.Clear();
#if DEBUG
                //var ossf = new List<int>() { 128 ,83 ,80 ,78 ,57 };

                //var g = result.Data.Where(_ => Convert.ToDateTime(_.DateEnd, new CultureInfo("ru-RU")) > DateTime.Now.AddDays(-31));
                //var g1 = 0;
#endif

                //Device.BeginInvokeOnMainThread(async () =>
                //{
                    if(result.Data.Count>10)
                    {
                        OSSList.Margin = new Thickness(0, -65, 0, 0);
                    }

                    OSSListContent.Children.Clear();
                    
                    bool isFirst = true;
                    
                    foreach (var oss in result.Data)
                    {

#if DEBUG
                        if(oss.Accounts.Count>1)
                        {

                        }
                        //отладочная инфа                    
                        if(!oss.Questions.Any(_=>string.IsNullOrWhiteSpace(_.Answer)))
                        {

                        }
                        else
                        {

                        }
                        if (oss.HouseAddress!=null)
                        {

                        }
                        //if (!ossf.Contains(oss.ID)  )
                        //    continue;
#endif

                        Frame f = new Frame();
                        f.MinimumHeightRequest = 50;
                        f.BackgroundColor = Color.White;
                        f.CornerRadius = 40;
                        f.HorizontalOptions = LayoutOptions.FillAndExpand;
                        f.Margin = new Thickness(0, 10);                        

                        //по нажатию обработка в раскрытие                        
                        TapGestureRecognizer tapGesture = new TapGestureRecognizer();
                        tapGesture.Tapped += async (s, e) =>
                        {
                            //делаем видимыми/невидимыми все доп.элементы
                            
                            Device.BeginInvokeOnMainThread(async () =>
                            {
                                var frme = (Frame)s;
                                if (prevAddtionlExpanded!=null)
                                {
                                    if (prevAddtionlExpanded.Id!=frme.Id)                                        
                                        action(prevAddtionlExpanded, true);
                                }
                                
                                action(frme);
                                prevAddtionlExpanded = frme;
                            });
                            };
                        f.GestureRecognizers.Add(tapGesture);

                        StackLayout rootStack = new StackLayout() { Orientation = StackOrientation.Vertical, VerticalOptions = LayoutOptions.FillAndExpand };

                        //добавление краткой инфо о собраниях
                        StackLayout stack = new StackLayout() { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.FillAndExpand };

                        IconView iconViewStatus = new IconView();
                        iconViewStatus.Source =  "ic_OssCircleStatus";


                        int statusInt = 0;//зеленый
                        if(Convert.ToDateTime(oss.DateEnd, new CultureInfo("ru-RU")) < DateTime.Now)
                        {
                            iconViewStatus.Foreground= Color.FromHex("#ed2e37");
                            statusInt = 3; // красный, голосование окончено , статус "итоги голосования" и переход на эту страницу 
                        }
                        if (Convert.ToDateTime(oss.DateStart, new CultureInfo("ru-RU")) < DateTime.Now && Convert.ToDateTime(oss.DateEnd, new CultureInfo("ru-RU")) > DateTime.Now)
                        {
                            //статус - идет голосование
                            if (!oss.Questions.Any(_ => string.IsNullOrWhiteSpace(_.Answer)))
                            {
                                // Ваш голос учтен - страница личных результатов голосования
                                statusInt = 2;//желтый(стоит сейчас) или какой еще цвет?
                                iconViewStatus.Foreground = Color.FromHex("#50ac2f");
                            }
                            else
                            {
                                //если не все проголосовано, переходим на вопрос начиная с которго нет ответов, и продолжаем голосование
                                iconViewStatus.Foreground = Color.FromHex("#ff971c");
                                statusInt = 1;//желтый
                            }
                            
                        }
                        if (Convert.ToDateTime(oss.DateStart, new CultureInfo("ru-RU")) > DateTime.Now)
                        {
                            //зеленый, "Уведомление о проведении ОСС", statusInt = 0
                            iconViewStatus.Foreground = Color.FromHex("#50ac2f");
                        }
                       

                        iconViewStatus.HeightRequest = 15;
                        iconViewStatus.WidthRequest = 15;
                        iconViewStatus.VerticalOptions = LayoutOptions.Center;
                        stack.Children.Add(iconViewStatus);
                        
                        Label MeetingTitle = new Label() { Text = oss.MeetingTitle, FontSize = 18 ,TextColor=Color.Black, FontAttributes=FontAttributes.Bold, HorizontalOptions=LayoutOptions.FillAndExpand};
                        stack.Children.Add(MeetingTitle);

                        IconView iconViewArrow = new IconView();
                        iconViewArrow.Source = "ic_arrow_forward";
                        iconViewArrow.Foreground = colorFromMobileSettings;
                        await iconViewArrow.RotateTo(90);
                        iconViewArrow.HeightRequest = 15;
                        iconViewArrow.WidthRequest = 15;
                        iconViewArrow.VerticalOptions = LayoutOptions.Center;
                        iconViewArrow.HorizontalOptions = LayoutOptions.End;

                        stack.Children.Add(iconViewArrow);
                        
                        arrows.Add(f.Id, iconViewArrow.Id);

                        //добавляем видимю часть на старте
                        rootStack.Children.Add(stack);

                        //невидимые на старте элементы фрейма, кроме 1го фрейма                                              
                        StackLayout additionslData = new StackLayout() { IsVisible = isFirst , Orientation= StackOrientation.Vertical};
                        if (isFirst)
                        {
                            prevAddtionlExpanded = f;
                            isFirst = false;
                        }

                        //добавляем в список связку стека с невидимыми плями и родительского фрейма
                        frames.Add(f.Id, additionslData.Id);

                        //инициатор
                        Label initiator = new Label() {};
                        FormattedString text = new FormattedString();
                        Span t1 = new Span() { TextColor =Color.FromHex("#545454"), FontSize=14, Text="Инициатор собрания: "};
                        text.Spans.Add(t1);
                        Span t2 = new Span() { TextColor = Color.Black, FontSize = 14, Text = oss.Author };
                        text.Spans.Add(t2);
                        initiator.FormattedText = text;

                        additionslData.Children.Add(initiator);
                        //дата собрания
                        Label date = new Label() {  };
                        FormattedString datetext = new FormattedString();
                        Span datet1 = new Span() { TextColor = Color.FromHex("#545454"), FontSize = 14, Text = "Дата собрания: " };
                        datetext.Spans.Add(datet1);
                        Span datet2 = new Span() { TextColor = Color.Black, FontSize = 14, Text = $"{oss.DateStart} по {oss.DateEnd}" };
                        datetext.Spans.Add(datet2);
                        date.FormattedText = datetext;

                        additionslData.Children.Add(date);

                        //Адрес дома
                        Label adress = new Label() {  };
                        FormattedString adresstext = new FormattedString();
                        Span adresst1 = new Span() { TextColor = Color.FromHex("#545454"), FontSize = 14, Text = "Адрес дома: " };
                        adresstext.Spans.Add(adresst1);
                        Span adresst2 = new Span() { TextColor = Color.Black, FontSize = 14, Text = $"{oss.HouseAddress}" };
                        adresstext.Spans.Add(adresst2);
                        adress.FormattedText = adresstext;

                        additionslData.Children.Add(adress);

                        //форма проведения
                        Label formAction = new Label() {  };
                        FormattedString formActiontext = new FormattedString();
                        Span formActiont1 = new Span() { TextColor = Color.FromHex("#545454"), FontSize = 14, Text = "Форма проведения: " };
                        formActiontext.Spans.Add(formActiont1);
                        Span formActiont2 = new Span() { TextColor = Color.Black, FontSize = 14, Text = $"{oss.Form}" };
                        formActiontext.Spans.Add(formActiont2);
                        formAction.FormattedText = formActiontext;

                        additionslData.Children.Add(formAction);

                        //разделитель
                        BoxView delim = new BoxView() { BackgroundColor = Color.FromHex("#545454"), 
                            HorizontalOptions = LayoutOptions.FillAndExpand, HeightRequest = 1, Margin=new Thickness(0,10) };
                        additionslData.Children.Add(delim);

                        //статус собрания:
                        StackLayout status = new StackLayout() { Orientation = StackOrientation.Horizontal , HorizontalOptions=LayoutOptions.FillAndExpand};
                        
                        StackLayout statusNameIcon = new StackLayout() { Orientation = StackOrientation.Vertical, HorizontalOptions = LayoutOptions.FillAndExpand };
                        
                        Label statusName = new Label() { Text = "Статус собрания: ", FontAttributes = FontAttributes.Bold, FontSize = 14 , TextColor=Color.Black,                             
                            HorizontalOptions = LayoutOptions.Start};

                        statusNameIcon.Children.Add(statusName);

                        StackLayout coloredStatus = new StackLayout() { HorizontalOptions=LayoutOptions.Start, Orientation=StackOrientation.Horizontal};
                        IconView iconViewStatusNameIcon = new IconView();

                        string textStatius = "";
                        Color ColorStatusTextString;
                        if (statusInt == 0)
                        {
                            iconViewStatusNameIcon.Source = "ic_status_green";
                            iconViewStatusNameIcon.Foreground = Color.FromHex("#50ac2f");
                            ColorStatusTextString = Color.FromHex("#50ac2f");
                            textStatius = "Уведомление о проведении ОСС";
                        }
                        else if (statusInt == 1) 
                        {
                            iconViewStatusNameIcon.Source = "ic_status_yellow";
                            iconViewStatusNameIcon.Foreground = Color.FromHex("#ff971c");
                            ColorStatusTextString = Color.FromHex("#ff971c");
                            textStatius = "Идет голосование";
                        }
                        else if(statusInt == 2)
                        {
                            iconViewStatusNameIcon.Source = "ic_status_done";
                            iconViewStatusNameIcon.Foreground = Color.FromHex("#50ac2f");
                            ColorStatusTextString = Color.FromHex("#50ac2f");
                            textStatius = "Ваш голос учтён";
                        }
                        else //3
                        {
                            iconViewStatusNameIcon.Source = "ic_status_red";
                            iconViewStatusNameIcon.Foreground = Color.FromHex("#ed2e37");
                            ColorStatusTextString = Color.FromHex("#ed2e37");
                            textStatius = "Завершено";
                        }                                            
                        iconViewStatusNameIcon.HeightRequest = 15;
                        iconViewStatusNameIcon.WidthRequest = 15;
                        iconViewStatusNameIcon.VerticalOptions = LayoutOptions.Center;
                        iconViewStatusNameIcon.HorizontalOptions = LayoutOptions.Start;

                        coloredStatus.Children.Add(iconViewStatusNameIcon);

                        Label statSting = new Label() { Text = textStatius, FontSize=12, TextColor = ColorStatusTextString, HorizontalOptions=LayoutOptions.Start };
                        coloredStatus.Children.Add(statSting);
                        
                        statusNameIcon.Children.Add(coloredStatus);

                        status.Children.Add(statusNameIcon);

                        //кнопка раскрытия осс
                        Frame buttonFrame = new Frame() { HeightRequest = 40, WidthRequest = 40, BackgroundColor = colorFromMobileSettings, CornerRadius=10 , Padding=0};
                        IconView iconViewArrowButton = new IconView();
                        iconViewArrowButton.Source = "ic_arrow_forward";
                        iconViewArrowButton.Foreground = Color.White;
                        iconViewArrowButton.HeightRequest = 15;
                        iconViewArrowButton.WidthRequest = 15;
                        iconViewArrowButton.VerticalOptions = LayoutOptions.Center;
                        iconViewArrowButton.HorizontalOptions = LayoutOptions.Center;
                        
                        buttonFrame.Content = iconViewArrowButton;

                        TapGestureRecognizer buttonFrametapGesture = new TapGestureRecognizer();
                        buttonFrametapGesture.Tapped += async (s, e) => { 

                            switch (statusInt){
                                case 0: 
                                case 1:
                                   var setAcquintedResult = await rc.SetAcquainted(oss.ID);
                                    if(string.IsNullOrWhiteSpace( setAcquintedResult.Error) )
                                    {
                                        OpenPage(new OSSInfo(oss));
                                    }
                                    else
                                    {
                                        await DisplayAlert("Ошибка", "Не удалось отправить уведомление об ознакомлении с повесткой ОСС", "OK");

                                    }

                                    break;
                                case 3: //"Итоги голосования"/"завершено" - открываем форму общих результатов голосования
                                     OpenPage(new OSSTotalVotingResult(oss));

                                    break;
                                case 2: //"Ваш голос учтен"  - открываем форму личных результатов голосования
                                     OpenPage(new OSSPersonalVotingResult(oss));

                                    break;
                                default: OpenPage(new OSSInfo(oss));
                                    return;
                            } 
                            //await Navigation.PushAsync(new OSSInfo(oss)); 
                        };
                        buttonFrame.GestureRecognizers.Add(buttonFrametapGesture);

                        status.Children.Add(buttonFrame);
                        //Button bFrame = new Button() { ImageSource };
                        
                        
                        additionslData.Children.Add(status);

                        //добавляем "невидимую" часть
                        rootStack.Children.Add(additionslData);

                        f.Content = rootStack;


                        OSSListContent.Children.Add(f);
                    }     
                //});
                return true;
            }
            else
            {
                await DisplayAlert("Ошибка", "Не удалось получить информацию об ОСС", "OK");
                return false;
            }

            
        }

        private async void Button_Clicked(object sender, EventArgs e)
        { 
            //активные
            try
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    ButtonActive.IsEnabled = false;                   
                    ButtonArchive.IsEnabled = false;

                    frames.Clear();
                    arrows.Clear();

                    var r = await GetOssData(1);


                    ((Button)sender).TextColor = colorFromMobileSettings;
                    ButtonArchive.TextColor = Color.White;

                    ButtonActive.IsEnabled = true;
                    ButtonArchive.IsEnabled = true;
                });
            }
            catch(Exception ex)
            {
                await DisplayAlert("Ошибка", "Не удалось получить информацию об ОСС", "OK");
            }
            
        }

        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            try
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    ButtonActive.IsEnabled = false;
                    ButtonArchive.IsEnabled = false;

                    frames.Clear();
                    arrows.Clear();

                    var r =  await GetOssData(0);


                    ((Button)sender).TextColor = colorFromMobileSettings;
                    ButtonActive.TextColor = Color.White;

                    ButtonActive.IsEnabled = true;
                    ButtonArchive.IsEnabled = true;
                });
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", "Не удалось получить информацию об ОСС", "OK");
            }
            
            //архивные
            //GetOssData(0);
            //Device.BeginInvokeOnMainThread(() =>
            //{
            //    ((Button)sender).TextColor = colorFromMobileSettings;
            //    ButtonActive.TextColor = Color.White;
            //});
        }

        async void OpenPage(Page page)
        {
            try
            {
                await Navigation.PushAsync(page);
            }
            catch
            {
                await Navigation.PushModalAsync(page);
            }
        }
        
        
    }
}