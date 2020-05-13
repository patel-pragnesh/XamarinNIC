using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamarinJKH.Server.RequestModel;
using xamarinJKH.Utils;

namespace xamarinJKH.Questions
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QuestionsPage : ContentPage
    {
        public List<PollInfo> Quest { get; set; }
        public QuestionsPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            var backClick = new TapGestureRecognizer();
            backClick.Tapped += async (s, e) => { _ = await Navigation.PopAsync(); };
            BackStackLayout.GestureRecognizers.Add(backClick);
            SetText();
            Quest = Settings.EventBlockData.Polls;
            this.BindingContext = this;
        }
        void SetText()
        {
            UkName.Text = Settings.MobileSettings.main_name;
            LabelPhone.Text = "+" + Settings.Person.Phone;
            SwitchQuest.ThumbColor =  Color.FromHex(Settings.MobileSettings.color);
        }

        private async void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            PollInfo select = e.Item as PollInfo;
            await Navigation.PushAsync(new PollsPage(select));
        }
    }
}