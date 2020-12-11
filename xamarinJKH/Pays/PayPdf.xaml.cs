using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamarinJKH.ViewModels;
using xamarinJKH.Server.RequestModel;
using xamarinJKH.Utils;
using xamarinJKH.Server;
using xamarinJKH.InterfacesIntegration;
using Xamarin.Essentials;
using xamarinJKH.CustomRenderers;
using System.Net.Http;
using AiForms.Dialogs;
using Microsoft.AppCenter.Analytics;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms.PancakeView;
using xamarinJKH.DialogViews;
using xamarinJKH.Tech;
using xamarinJKH.Main;
using System.Linq;

namespace xamarinJKH.Pays
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PayPdf : ContentPage
    {
        public PayPdfViewModel viewModel { get; set; }
        View pdfview;
        public PayPdf(BillInfo info)
        {
            InitializeComponent();

            var profile = new TapGestureRecognizer();
            profile.Tapped += async (s, e) =>
            {
                if (Navigation.NavigationStack.FirstOrDefault(x => x is ProfilePage) == null)
                    await Navigation.PushAsync(new ProfilePage());
            };
            IconViewProfile.GestureRecognizers.Add(profile);

            var techSend = new TapGestureRecognizer();
            techSend.Tapped += async (s, e) => { await Navigation.PushAsync(new AppPage());  };
            LabelTech.GestureRecognizers.Add(techSend);
            BindingContext = viewModel = new PayPdfViewModel(info);
            Analytics.TrackEvent("Просмотр ПДФ по квитанции №" + viewModel.Bill.ID);
            viewModel.LoadPdf.Execute(null);
            if (Device.RuntimePlatform == "Android")
            {
                pdfview = new CustomWebView()
                {
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    Uri = viewModel.Bill.FileLink
                };
                Content.Children.Add(pdfview);
            }
            else
            {
                int statusBarHeight = DependencyService.Get<IStatusBar>().GetHeight();                
                gridMain.Padding = new Thickness(0, statusBarHeight , 0, 0);
                
                pdfview = new WebView()
                {
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    Source = viewModel.Bill.FileLink
                };
                Content.Children.Add(pdfview);
            }
            
            Color hexColor = (Color) Application.Current.Resources["MainColor"];
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        async void GoBack(object sender, EventArgs args)
        {
            try
            {
                _ = await Navigation.PopAsync();
            }
            catch { }
        }

        async void ShareBill(object sender, EventArgs args)
        {
            await Xamarin.Essentials.Share.RequestAsync(new ShareTextRequest()
            {
                Uri = viewModel.Bill.FileLink,
                Text = AppResources.ShareBill
            });
        }

        async void Print(object sender, EventArgs args)
        {
            HttpClient client = new HttpClient();
            Loading.Instance.Show(AppResources.Loading);
            try
            {
                var file = await client.GetByteArrayAsync(viewModel.Bill.FileLink);
                if (file != null)
                    DependencyService.Get<xamarinJKH.InterfacesIntegration.IPrintManager>().SendFileToPrint(file);
                else
                    await DisplayAlert(null, AppResources.ErrorFileLoading, "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert(null, AppResources.ErrorReboot, "ОК");
            }
            finally
            {
                Loading.Instance.Hide();
            }
        }
    }

    

    public class PayPdfViewModel : BaseViewModel
    {
        BillInfo _bill;
        public string Theme
        {
            get => "#" + Settings.MobileSettings.color;
        }
        public BillInfo Bill
        {
            get => _bill;
            set
            {
                _bill = value;
                OnPropertyChanged("Bill");
            }
        }
        public string Phone
        {
            get =>  "+" + Settings.Person.companyPhone.Replace("+","");
        }

        public Command LoadPdf { get; set; }
        string _path;
        public string Path
        {
            get => _path;
            set
            {
                _path = value;
                OnPropertyChanged("Path");
            }
        }
        public string Name
        {
            get => Settings.MobileSettings.main_name;
        }
        public PayPdfViewModel(BillInfo info)
        {
            Bill = info;
            RestClientMP server = new RestClientMP();
            IsBusy = true;
            Path = null;
            
            LoadPdf = new Command(async () =>
            {
                //TODO: Получение ссылки на настоящий файл квитанции с бека
                if (!info.HasFile)
                {
                    IsBusy = false;
                }
                else
                {
                    Path = info.FileLink;//"file:///" + DependencyService.Get<IFileWorker>().GetFilePath(filename);
                }
            });

            MessagingCenter.Subscribe<Object>(this, "ReleasePdfLoading", (sender) =>
            {
                IsBusy = false;
            });
        }
    }
}