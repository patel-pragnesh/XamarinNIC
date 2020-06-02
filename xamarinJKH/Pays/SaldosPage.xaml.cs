﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AiForms.Dialogs;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamarinJKH.InterfacesIntegration;
using xamarinJKH.Server;
using xamarinJKH.Server.RequestModel;
using xamarinJKH.Utils;

namespace xamarinJKH.Pays
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SaldosPage : ContentPage
    {
        public List<BillInfo> BillInfos { get; set; }
        private RestClientMP _server = new RestClientMP();
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
            ItemsList<AccountAccountingInfo> info = await _server.GetAccountingInfo();
            if (info.Error == null)
            {
                SetBills(info.Data);
                additionalList.ItemsSource = null;
                additionalList.ItemsSource = BillInfos;
            }
            else
            {
                await DisplayAlert("Ошибка", "Не удалось получить информацию о квитанциях", "OK");
            }
        }

        public SaldosPage(List<AccountAccountingInfo> infos)
        {
            SetBills(infos);
            InitializeComponent();
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    BackgroundColor = Color.White;
                    ImageTop.Margin = new Thickness(0, 0, 0, 0);
                    StackLayout.Margin = new Thickness(0, 33, 0, 0);
                    IconViewNameUk.Margin = new Thickness(0, 33, 0, 0);
                    break;
                case Device.Android:
                    double or = Math.Round(((double) App.ScreenWidth / (double) App.ScreenHeight), 2);
                    if (Math.Abs(or - 0.5) < 0.02)
                    {
                        ScrollViewContainer.Margin = new Thickness(0,0,0,-135);
                        BackStackLayout.Margin = new Thickness(-5, 15, 0, 0);
                    }
                    break;
                default:
                    break;
            }

            NavigationPage.SetHasNavigationBar(this, false);
            var backClick = new TapGestureRecognizer();
            backClick.Tapped += async (s, e) => { _ = await Navigation.PopAsync(); };
            BackStackLayout.GestureRecognizers.Add(backClick);
            SetText();
            this.BindingContext = this;
        }

        void SetBills(List<AccountAccountingInfo> infos)
        {
            BillInfos = new List<BillInfo>();
            foreach (var each in infos)
            {
                foreach (var VARIABLE in each.Bills)
                {
                    BillInfos.Add(VARIABLE);
                }
            }
        }

        void SetText()
        {
            UkName.Text = Settings.MobileSettings.main_name;
            LabelPhone.Text = "+" + Settings.Person.Phone;
            IconViewSortIdent.Foreground = Color.FromHex(Settings.MobileSettings.color);
            IconViewSortDate.Foreground = Color.FromHex(Settings.MobileSettings.color);
            LabelDate.TextColor = Color.FromHex(Settings.MobileSettings.color);
        }

        private async void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            BillInfo select = e.Item as BillInfo;
            string filename = @select.Period + ".pdf";
            if (!select.HasFile)
            {
                return;
            }

            if (await DependencyService.Get<IFileWorker>().ExistsAsync(filename))
            {
                await Launcher.OpenAsync(new OpenFileRequest
                {
                    File = new ReadOnlyFile(DependencyService.Get<IFileWorker>().GetFilePath(filename))
                });
            }
            else
            {
                await Settings.StartProgressBar();
                var stream = await server.DownloadFileAsync(select.ID.ToString());
                if (stream != null)
                {
                    await DependencyService.Get<IFileWorker>().SaveTextAsync(filename, stream);
                    Loading.Instance.Hide();
                    await Launcher.OpenAsync(new OpenFileRequest
                    {
                        File = new ReadOnlyFile(DependencyService.Get<IFileWorker>().GetFilePath(filename))
                    });
                }
            }
        }
    }
}