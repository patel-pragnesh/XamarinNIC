using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamarinJKH.Server.RequestModel;
using xamarinJKH.Utils;

namespace xamarinJKH.News
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewsAllPage : ContentPage
    {
        public ObservableCollection<NewsInfo> News { get; set; }
        public NewsAllPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            News = new ObservableCollection<NewsInfo>(Settings.EventBlockData.News);
            InitializeComponent();
            HeaderViewMain.BackClick = new Command(async () =>
            {
                if (Application.Current.MainPage.Navigation.ModalStack.Count > 1)
                {
                    await Navigation.PopModalAsync();
                }
                else
                {
                    await Navigation.PopAsync();
                }
            });
            var pickDo = new TapGestureRecognizer();
            pickDo.Tapped += async (s, e) => {  
                Device.BeginInvokeOnMainThread(() =>
                {
                    DatePicker.Focus();
                });
            };
            StackLayoutDo.GestureRecognizers.Add(pickDo);
            var pickPosle = new TapGestureRecognizer();
            pickPosle.Tapped += async (s, e) => {  
                Device.BeginInvokeOnMainThread(() =>
                {
                    DatePicker2.Focus();
                });
            };
            StackLayoutDo.GestureRecognizers.Add(pickPosle);
            HeaderViewMain.DarkImage = "ic_news_top";
            HeaderViewMain.LightImage = "ic_news_top_light";
            BindingContext = this;
        }

        private void DatePicker_OnDateSelected(object sender, DateChangedEventArgs e)
        {
            SetDate();
        }

        private void DatePicker2_OnDateSelected(object sender, DateChangedEventArgs e)
        {
            SetDate();
        }

        private async void NotifSelect(object sender, SelectionChangedEventArgs e)
        {
            NewsInfo select =  e.CurrentSelection.FirstOrDefault()  as NewsInfo;

            if (Navigation.NavigationStack.FirstOrDefault(x => x is NewPage) == null)
                await Navigation.PushAsync(new NewPage(select));
            try
            {
                CollectionView.SelectedItem = null;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
        
        void SetDate()
        {
            DateTime one = DatePicker.Date;
            DateTime two = DatePicker2.Date;
            News.Clear();
            foreach (var each in Settings.EventBlockData.News)
            {
                DateTime timeNotif = new DateTime();
                try
                {
                    timeNotif =  DateTime.ParseExact(each.Created, "dd.MM.yyyy",
                        System.Globalization.CultureInfo.InvariantCulture);
                    if (timeNotif >= one && timeNotif <= two)
                    {
                        Device.BeginInvokeOnMainThread(async () => News.Add(each));
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                
            }
        }
    }
}