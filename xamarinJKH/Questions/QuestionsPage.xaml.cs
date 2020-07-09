using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Plugin.Messaging;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamarinJKH.Server;
using xamarinJKH.Server.RequestModel;
using xamarinJKH.Tech;
using xamarinJKH.Utils;

namespace xamarinJKH.Questions
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QuestionsPage : ContentPage
    {
        public List<PollInfo> Quest { get; set; }
        public List<PollInfo> QuestComlite { get; set; }
        public List<PollInfo> QuestNotComlite { get; set; }
        private bool _isRefreshing = false;
        private RestClientMP server = new RestClientMP();

        public bool IsRefreshing
        {
            get { return _isRefreshing; }
            set
            {
                _isRefreshing = value;
                OnPropertyChanged(nameof(IsRefreshing));
            }
        }

        public ICommand RefreshCommand
        {
            get
            {
                return new Command(async () =>
                {
                    IsRefreshing = true;

                    await RefreshData();

                    IsRefreshing = false;
                });
            }
        }

        private async Task RefreshData()
        {
            if (Settings.EventBlockData.Error == null)
            {
                Settings.EventBlockData = await server.GetEventBlockData();
                isComplite();
                if (!SwitchQuest.IsToggled)
                {
                    Quest = QuestNotComlite;
                }
                else
                {
                    Quest = QuestComlite;
                }

                additionalList.ItemsSource = null;
                additionalList.ItemsSource = Quest;
            }
            else
            {
                await DisplayAlert("Ошибка", "Не удалось получить информацию об опросах", "OK");
            }
        }

        public QuestionsPage()
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
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    BackgroundColor = Color.White;
                    // ImageTop.Margin = new Thickness(0, 0, 0, 0);
                    // StackLayout.Margin = new Thickness(0, 33, 0, 0);
                    // IconViewNameUk.Margin = new Thickness(0, 33, 0, 0);
                    // RelativeLayoutTop.Margin = new Thickness(0,0,0,0);
                    // if (App.ScreenHeight <= 667)//iPhone6
                    // {
                    //     //NotificationList.Margin = new Thickness(0,-110,0,0);
                    //     RelativeLayoutTop.Margin = new Thickness(0, 0, 0, -110);
                    // }
                    // else if (App.ScreenHeight <= 736)//iPhone8Plus Height=736
                    // {
                    //     RelativeLayoutTop.Margin = new Thickness(0, 0, 0, -145);
                    // }
                    // else
                    // {
                    //     RelativeLayoutTop.Margin = new Thickness(0, 0, 0, -145);
                    // }
                    break;
                case Device.Android:
                    double or = Math.Round(((double) App.ScreenWidth / (double) App.ScreenHeight), 2);
                    // if (Math.Abs(or - 0.5) < 0.02)
                    // {
                    //     RelativeLayoutTop.Margin = new Thickness(0, 0, 0, -80);
                    // }

                    break;
                default:
                    break;
            }

            NavigationPage.SetHasNavigationBar(this, false);
            var backClick = new TapGestureRecognizer();
            backClick.Tapped += async (s, e) => { _ = await Navigation.PopAsync(); };
            BackStackLayout.GestureRecognizers.Add(backClick);
            SetText();
            // isComplite();
            // Quest = QuestNotComlite;
            this.BindingContext = this;
            additionalList.BackgroundColor = Color.Transparent;
            additionalList.Effects.Add(Effect.Resolve("MyEffects.ListViewHighlightEffect"));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            new Task(SyncSetup).Start(); // This could be an await'd task if need be
        }

        async void SyncSetup()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                // Assuming this function needs to use Main/UI thread to move to your "Main Menu" Page
                RefreshData();
            });
        }

        void isComplite()
        {
            QuestComlite = new List<PollInfo>();
            QuestNotComlite = new List<PollInfo>();
            foreach (var each in Settings.EventBlockData.Polls)
            {
                bool flag = true;
                foreach (var quest in each.Questions)
                {
                    if (!quest.IsCompleteByUser)
                    {
                        flag = false;
                        break;
                    }
                }

                if (flag)
                {
                    each.IsComplite = true;
                    QuestComlite.Add(each);
                }
                else
                {
                    each.IsComplite = false;
                    QuestNotComlite.Add(each);
                }
            }
        }

        void SetText()
        {
            UkName.Text = Settings.MobileSettings.main_name;
            LabelPhone.Text =  "+" + Settings.Person.companyPhone.Replace("+","");
            SwitchQuest.ThumbColor = Color.Black;
            SwitchQuest.OnColor = Color.FromHex(Settings.MobileSettings.color);
        }

        private async void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            PollInfo select = e.Item as PollInfo;
            await Navigation.PushAsync(new PollsPage(select, SwitchQuest.IsToggled));
        }

        private void chage(object sender, PropertyChangedEventArgs e)
        {
            if (!SwitchQuest.IsToggled)
            {
                Quest = QuestNotComlite;
            }
            else
            {
                Quest = QuestComlite;
            }

            additionalList.ItemsSource = null;
            additionalList.ItemsSource = Quest;
        }
    }
}