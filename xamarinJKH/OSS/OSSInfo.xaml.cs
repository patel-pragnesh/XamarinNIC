using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.Messaging;
using Rg.Plugins.Popup.Extensions;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamarinJKH.InterfacesIntegration;
using xamarinJKH.Server;
using xamarinJKH.Server.RequestModel;
using xamarinJKH.Tech;
using xamarinJKH.Utils;

namespace xamarinJKH
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OSSInfo : ContentPage
    {
        OSS _oss=null;
        int GetOssStatus(OSS oss)
        {
            int statusInt = 0;//зеленый
            if (Convert.ToDateTime(oss.DateStart, new CultureInfo("ru-RU")) < DateTime.Now)
            {                
                statusInt = 1; // статус "уведомление о проведнии ОСС"
            }
            if (Convert.ToDateTime(oss.DateStart, new CultureInfo("ru-RU")) < DateTime.Now && Convert.ToDateTime(oss.DateEnd, new CultureInfo("ru-RU")) > DateTime.Now)
            {
                //статус - идет голосование
                //if (!oss.Questions.Any(_ => string.IsNullOrWhiteSpace(_.Answer)))
                //{
                    // Ваш голос учтен - страница личных результатов голосования
                    statusInt = 2;//желтый(стоит сейчас) или какой еще цвет?             
                //}                
            }

            return statusInt;
        }

        public OSSInfo(OSS sObj)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
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
                        phoneDialer.MakePhoneCall(Settings.Person.companyPhone);
                }

            
            };
            LabelPhone.GestureRecognizers.Add(call);
           
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    int statusBarHeight = DependencyService.Get<IStatusBar>().GetHeight();
                    Pancake.Padding = new Thickness(0, statusBarHeight, 0, 0);
                    BackgroundColor = Color.White;
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
                catch (Exception exception)
                {
                    _ = await Navigation.PopModalAsync();

                }
            };
            BackStackLayout.GestureRecognizers.Add(backClick);

            UkName.Text = Settings.MobileSettings.main_name;
            LabelPhone.Text =  "+" + Settings.Person.companyPhone.Replace("+","");

            Btn.BackgroundColor = colorFromMobileSettings;

            _oss = sObj;

            //заполнение статуса ОСС
            FiilOssStatusLayout(sObj);

            FillOssInfoAsync( sObj);

            SetTheme(sObj);

            SetProperty(sObj);

            SetAdmin(sObj);

            SetDesignOrder(sObj);
        }

        private void FiilOssStatusLayout(OSS oss)
        {
            int statusInt = 0;//зеленый            
            
            if (Convert.ToDateTime(oss.DateEnd, new CultureInfo("ru-RU")) < DateTime.Now)
            {
                statusInt = 3; // красный, голосование окончено , статус "итоги голосования" и переход на эту страницу 
            }
            if (Convert.ToDateTime(oss.DateStart, new CultureInfo("ru-RU")) < DateTime.Now && Convert.ToDateTime(oss.DateEnd, new CultureInfo("ru-RU")) > DateTime.Now)
            {
                //статус - идет голосование
                if (!oss.Questions.Any(_ => string.IsNullOrWhiteSpace(_.Answer)))
                {
                    // Ваш голос учтен - страница личных результатов голосования
                    statusInt = 2;//желтый(стоит сейчас) или какой еще цвет?
                }
                else
                {
                    //если не всё проголосовано, переходим на вопрос начиная с которго нет ответов, и продолжаем голосование
                    statusInt = 1;//желтый
                }
            }
            if (Convert.ToDateTime(oss.DateStart, new CultureInfo("ru-RU")) > DateTime.Now)
            {
                //зеленый, "Уведомление о проведении ОСС", statusInt = 0
                statusInt = 0;
            }

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
            else //2
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

            statusLayout.Children.Add(iconViewStatusNameIcon);

            Label statSting = new Label() { Text = textStatius, FontSize = 12, TextColor = ColorStatusTextString, HorizontalOptions = LayoutOptions.Start };
            statusLayout.Children.Add(statSting);


        }

        IDictionary<Guid, Guid> frames = new Dictionary<Guid, Guid>();
        IDictionary<Guid, Guid> arrows = new Dictionary<Guid, Guid>();

        Color colorFromMobileSettings = Color.FromHex(Settings.MobileSettings.color);

        Frame prevAddtionlExpanded = null;

        async void action(Frame frme, bool hide = false)
        {
            var idAdditional = frames[frme.Id];
            var additionalStack = ((StackLayout)frme.Content).Children.FirstOrDefault(_ => _.Id == idAdditional);
            if (hide)
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

        TapGestureRecognizer ShowHideAction()
        {            
            //по нажатию обработка в раскрытие
            TapGestureRecognizer tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += async (s, e) =>
            {
                //делаем видимыми/невидимыми все доп.элементы
                Device.BeginInvokeOnMainThread(async () =>
                {
                    var frme = (Frame)s;
                    if (prevAddtionlExpanded != null)
                    {
                        if (prevAddtionlExpanded.Id != frme.Id)
                            action(prevAddtionlExpanded, true);
                    }

                    action(frme);
                    prevAddtionlExpanded = frme;
                });
            };
            return tapGesture;
        }

        IconView getIconView(string iconame) {
            
            IconView icon = new IconView();
            icon.Source = iconame;
            icon.Foreground = colorFromMobileSettings;
            icon.HeightRequest = 15;
            icon.WidthRequest = 15;
            icon.VerticalOptions = LayoutOptions.Center;

            return icon;
        }

        async Task<IconView> getIconArrowAsync()
        {
            IconView iconViewArrow = new IconView();
            iconViewArrow.Source = "ic_arrow_forward";
            iconViewArrow.Foreground = colorFromMobileSettings;
            await iconViewArrow.RotateTo(90);
            iconViewArrow.HeightRequest = 15;
            iconViewArrow.WidthRequest = 15;
            iconViewArrow.VerticalOptions = LayoutOptions.Center;
            iconViewArrow.HorizontalOptions = LayoutOptions.End;

            return iconViewArrow;
        }

        Label HeadLabel(string txt)
        {
            Label Title = new Label() { Text = txt, FontSize = 18, TextColor = Color.Black, FontAttributes = FontAttributes.Bold, HorizontalOptions = LayoutOptions.FillAndExpand };
            return Title; 
        }

        Label detailLabel(string param, string content)
        {
            Label detail = new Label();
            FormattedString fs = new FormattedString();
            Span t = new Span() { TextColor = Color.FromHex("#545454"), FontSize = 14, Text = param };
            fs.Spans.Add(t);
            Span t0 = new Span() { TextColor = Color.Black, FontSize = 14, Text = content };
            fs.Spans.Add(t0);
            detail.FormattedText = fs;

            return detail;
        }

        Frame GetFrame()
        {
           return new Frame()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.White,
                MinimumHeightRequest = 50,
                CornerRadius = 40,
                Margin = new Thickness(0, 10)
            };
        }

        async void FillOssInfoAsync(OSS sObj)
        {
            /******* общая информация ********/
            //видимая часть
            Frame CommonInfo = GetFrame();
            CommonInfo.GestureRecognizers.Add(ShowHideAction());
            StackLayout rootStackCommon = new StackLayout() { Orientation = StackOrientation.Vertical, VerticalOptions = LayoutOptions.FillAndExpand };
            StackLayout stackCommonInfo = new StackLayout() { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.FillAndExpand };

            //добавляем иконку
            stackCommonInfo.Children.Add(getIconView("ic_info"));
            //основной текст
            stackCommonInfo.Children.Add(HeadLabel("Общая информация"));
            //стрелка
            var iconViewArrow = await getIconArrowAsync();
            stackCommonInfo.Children.Add(iconViewArrow);
            arrows.Add(CommonInfo.Id, iconViewArrow.Id);
            //добавляем видимю часть на старте
            rootStackCommon.Children.Add(stackCommonInfo);

            //невидимые на старте элементы фрейма, кроме 1го фрейма        
            prevAddtionlExpanded = CommonInfo;
            StackLayout additionslData = new StackLayout() { IsVisible = true, Orientation = StackOrientation.Vertical };

            //добавляем в список связку стека с невидимыми полями и родительского фрейма
            frames.Add(CommonInfo.Id, additionslData.Id);

            //наименование документа           
            var docName = detailLabel("Наименование документа: ", sObj.MeetingTitle);
            additionslData.Children.Add(docName);

            //инициатор            
            var initiator = detailLabel("Инициатор собрания: ", sObj.Author);
            additionslData.Children.Add(initiator);

            //Адрес дома
            Label adress = detailLabel("Адрес дома: ", sObj.HouseAddress);
            additionslData.Children.Add(adress);

            //Реквизиты, подтверждающие собственность
            Label document = detailLabel("Реквизиты, подтверждающие собственность: ", sObj.Accounts[0].Document);
            additionslData.Children.Add(document);

            //Номер собственности
            Label propNum = detailLabel("Номер собственности: ", sObj.Accounts[0].PremiseNumber);
            additionslData.Children.Add(propNum);
            //Площадь
            Label area = detailLabel("Площадь: ", $"{sObj.Accounts[0].Area} м.кв.");
            additionslData.Children.Add(area);

            //Доля
            Label propertyPercent = detailLabel("Доля: ", $"{sObj.Accounts[0].PropertyPercent} %");
            additionslData.Children.Add(propertyPercent);

            //дата собрания
            Label date = detailLabel("Дата собрания: ", $"{sObj.DateStart} по {sObj.DateEnd} включительно");
            additionslData.Children.Add(date);

            //форма проведения
            Label formAction = detailLabel("Форма проведения: ", sObj.Form);
            additionslData.Children.Add(formAction);

            rootStackCommon.Children.Add(additionslData);

            CommonInfo.Content = rootStackCommon;

            OSSListContent.Children.Add(CommonInfo);
        }

        async void SetTheme(OSS sObj)
        {
            /******* Повестка собрания ********/
            //видимая часть
            Frame CommonTheme = GetFrame();
            CommonTheme.GestureRecognizers.Add(ShowHideAction());
            StackLayout rootStack = new StackLayout() { Orientation = StackOrientation.Vertical, VerticalOptions = LayoutOptions.FillAndExpand };
            StackLayout stack = new StackLayout() { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.FillAndExpand };

            //добавляем иконку
            stack.Children.Add(getIconView("ic_info_theme"));
            //основной текст
            stack.Children.Add(HeadLabel("Повестка собрания"));
            //стрелка
            var iconViewArrowCommonTheme = await getIconArrowAsync();
            stack.Children.Add(iconViewArrowCommonTheme);
            arrows.Add(CommonTheme.Id, iconViewArrowCommonTheme.Id);
            //добавляем видимю часть на старте
            rootStack.Children.Add(stack);

            //невидимые на старте элементы фрейма, кроме 1го фрейма                                              
            StackLayout additionslData = new StackLayout() { IsVisible = false, Orientation = StackOrientation.Vertical };

            //добавляем в список связку стека с невидимыми полями и родительского фрейма
            frames.Add(CommonTheme.Id, additionslData.Id);

            //вопросы
            int i = 1;
            foreach(var q in sObj.Questions)
            {
                var docName = detailLabel($"Вопрос № {i}: ", q.Text);
                i++;
                additionslData.Children.Add(docName);
            }           

            rootStack.Children.Add(additionslData);

            CommonTheme.Content = rootStack;

            OSSListContent.Children.Add(CommonTheme);
        }

        async void SetProperty(OSS sObj)
        {
            /******* Сведения о собственности ********/
            //видимая часть
            Frame frame = GetFrame();
            frame.GestureRecognizers.Add(ShowHideAction());
            StackLayout rootStack = new StackLayout() { Orientation = StackOrientation.Vertical, VerticalOptions = LayoutOptions.FillAndExpand };
            StackLayout stack = new StackLayout() { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.FillAndExpand };

            //добавляем иконку
            stack.Children.Add(getIconView("ic_info_property"));
            //основной текст
            stack.Children.Add(HeadLabel("Сведения о собственности"));
            //стрелка
            var iconViewArrowCommonTheme = await getIconArrowAsync();
            stack.Children.Add(iconViewArrowCommonTheme);
            arrows.Add(frame.Id, iconViewArrowCommonTheme.Id);
            //добавляем видимю часть на старте
            rootStack.Children.Add(stack);

            //невидимые на старте элементы фрейма, кроме 1го фрейма                                              
            StackLayout additionslData = new StackLayout() { IsVisible = false, Orientation = StackOrientation.Vertical };

            //добавляем в список связку стека с невидимыми полями и родительского фрейма
            frames.Add(frame.Id, additionslData.Id);

            //Реквизиты, подтверждающие собственность
            Label document = detailLabel("Реквизиты, подтверждающие собственность: ", sObj.Accounts[0].Document);
            additionslData.Children.Add(document);
            //Номер собственности
            Label propNum = detailLabel("Номер собственности: ", sObj.Accounts[0].PremiseNumber);
            additionslData.Children.Add(propNum);
            //Площадь
            Label area = detailLabel("Площадь: ", $"{sObj.Accounts[0].Area} м.кв.");
            additionslData.Children.Add(area);

            //Доля
            Label propertyPercent = detailLabel("Доля: ", $"{sObj.Accounts[0].PropertyPercent} %");
            additionslData.Children.Add(propertyPercent);
            //Общая площадь помещений
            Label areaTotal = detailLabel("Общая площадь помещений: ", $"Жилые: {sObj.AreaResidential} м.кв.\nНежилые: {sObj.AreaNonresidential} м.кв.");
            additionslData.Children.Add(areaTotal);

            rootStack.Children.Add(additionslData);

            frame.Content = rootStack;

            OSSListContent.Children.Add(frame);
        }

        async void SetAdmin(OSS sObj)
        {
            /******* Сведения об администраторе ********/
            //видимая часть
            Frame frame = GetFrame();
            frame.GestureRecognizers.Add(ShowHideAction());
            StackLayout rootStack = new StackLayout() { Orientation = StackOrientation.Vertical, VerticalOptions = LayoutOptions.FillAndExpand };
            StackLayout stack = new StackLayout() { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.FillAndExpand };

            //добавляем иконку
            stack.Children.Add(getIconView("ic_info_admin"));
            //основной текст
            stack.Children.Add(HeadLabel("Сведения об администраторе"));
            //стрелка
            var iconViewArrowCommonTheme = await getIconArrowAsync();
            stack.Children.Add(iconViewArrowCommonTheme);
            arrows.Add(frame.Id, iconViewArrowCommonTheme.Id);
            //добавляем видимю часть на старте
            rootStack.Children.Add(stack);

            //невидимые на старте элементы фрейма, кроме 1го фрейма                                              
            StackLayout additionslData = new StackLayout() { IsVisible = false, Orientation = StackOrientation.Vertical };

            //добавляем в список связку стека с невидимыми полями и родительского фрейма
            frames.Add(frame.Id, additionslData.Id);

            //Фирменное наименование
            Label name = detailLabel("Фирменное наименование: ", sObj.AdminstratorName);
            additionslData.Children.Add(name);

            //Организационно-правовая форма
            Label opf = detailLabel("Организационно-правовая форма: ",  sObj.AdministratorIsUL? "Юридическое лицо" : "Физическое лицо");
            additionslData.Children.Add(opf);

            //Место нахождения            
            additionslData.Children.Add(detailLabel("Место нахождения: ", sObj.AdminstratorAddress));

            //Почтовый адрес           
            additionslData.Children.Add(detailLabel("Почтовый адрес: ", sObj.AdminstratorPostAddress));

            //Контактный номер телефона
            additionslData.Children.Add(detailLabel("Контактный номер телефона: ", sObj.AdminstratorPhone));

            //Официальный сайт
            var site = new Label();
            var fs = new FormattedString();
            Span t = new Span() { TextColor = Color.FromHex("#545454"), FontSize = 14, Text = "Официальный сайт: " };
            fs.Spans.Add(t);
            var fsSpanUrl = new Span() { Text = sObj.AdminstratorSite , TextColor=colorFromMobileSettings, TextDecorations=TextDecorations.Underline};
            fsSpanUrl.GestureRecognizers.Add(new TapGestureRecognizer
            {                
                Command = new Command(async () => await Launcher.OpenAsync(sObj.AdminstratorSite))
            });
            fs.Spans.Add(fsSpanUrl);
            site.FormattedText = fs;
            additionslData.Children.Add(site);

            //без заполнения, поля надо уточнить
            //ФИО
            var fio = sObj.AdminstratorName;
            additionslData.Children.Add(detailLabel("ФИО: ", fio));

            //Паспортные данные
            var passport = sObj.AdminstratorDocNumber;
            additionslData.Children.Add(detailLabel("Паспортные данные: ", passport));

            //Место постоянного проживания
            var passportAdress = sObj.AdminstratorAddress;
            additionslData.Children.Add(detailLabel("Место постоянного проживания: ", passportAdress));
            
            //Адрес электронной почты
            var emailAdress = sObj.AdminstratorEmail;
            additionslData.Children.Add(detailLabel("Адрес электронной почты: ", emailAdress));

            rootStack.Children.Add(additionslData);

            frame.Content = rootStack;

            OSSListContent.Children.Add(frame);

        }

        async void SetDesignOrder(OSS sObj)
        {
            /******* Порядок приема решений ********/
            //видимая часть
            Frame CommonTheme = GetFrame();
            CommonTheme.GestureRecognizers.Add(ShowHideAction());
            StackLayout rootStack = new StackLayout() { Orientation = StackOrientation.Vertical, VerticalOptions = LayoutOptions.FillAndExpand };
            StackLayout stack = new StackLayout() { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.FillAndExpand };

            //добавляем иконку
            stack.Children.Add(getIconView("ic_info_design_order"));
            //основной текст
            stack.Children.Add(HeadLabel("Порядок приема решений"));
            //стрелка
            var iconViewArrowCommonTheme = await getIconArrowAsync();
            stack.Children.Add(iconViewArrowCommonTheme);
            arrows.Add(CommonTheme.Id, iconViewArrowCommonTheme.Id);
            //добавляем видимю часть на старте
            rootStack.Children.Add(stack);

            //невидимые на старте элементы фрейма, кроме 1го фрейма                                              
            StackLayout additionslData = new StackLayout() { IsVisible = false, Orientation = StackOrientation.Vertical };

            //добавляем в список связку стека с невидимыми полями и родительского фрейма
            frames.Add(CommonTheme.Id, additionslData.Id);

            //Электронные образцы
            var site = new Label();
            var fs = new FormattedString();
            Span t = new Span() { TextColor = Color.FromHex("#545454"), FontSize = 14, Text = "Электронные образцы: " };
            fs.Spans.Add(t);
            var fsSpanUrl = new Span() { Text = sObj.WebSiteForScanDocView, TextColor = colorFromMobileSettings, TextDecorations = TextDecorations.Underline };
            fsSpanUrl.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(async () => await Launcher.OpenAsync(sObj.WebSiteForScanDocView))
            });
            fs.Spans.Add(fsSpanUrl);
            site.FormattedText = fs;
            additionslData.Children.Add(site);

            //Рукописные образцы: - содержимое поля надо уточнить
            var placeForWriteDesigns = sObj.PlaceOfReceiptSolutions;
            var docName = detailLabel("Рукописные образцы: ", placeForWriteDesigns);                
            additionslData.Children.Add(docName);
            


            rootStack.Children.Add(additionslData);

            CommonTheme.Content = rootStack;

            OSSListContent.Children.Add(CommonTheme);
        }


        private RestClientMP server = new RestClientMP();


        private async void Btn_Clicked(object sender, EventArgs e)
        {
            var intStatus = GetOssStatus(_oss);
            if(intStatus==2)
            {
                //записываем на сервер что пользователь начал голосование
                await server.SetStartVoiting(_oss.ID);
                OpenPage(new OSSPool(_oss));
                Navigation.RemovePage(this);
            }
            else if (intStatus == 1)
            {
                await DisplayAlert("Информация", $"Голосование начнется {_oss.DateStart} по местному времени, Вам будет прислано Push оповещение", "OK");
            }
            //тут добавить логику нажатия, в зависимости от статуса ОСС
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
        async void ClosePage()
        {
            try
            {
                await Navigation.PopAsync();
               
            }
            catch
            {
                await Navigation.PopModalAsync();
            }
        }
        
    }
}