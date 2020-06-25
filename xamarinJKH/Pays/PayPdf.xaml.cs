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

namespace xamarinJKH.Pays
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PayPdf : ContentPage
    {
        public PayPdfViewModel viewModel { get; set; }
        public PayPdf(PayPdfViewModel vm)
        {
            InitializeComponent();
            BindingContext = viewModel = vm;
            viewModel.LoadPdf.Execute(null);
            View pdfview;
            if (Device.RuntimePlatform == "Android")
            {
                pdfview = new CustomWebView()
                {
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    Uri = "http://www.africau.edu/images/default/sample.pdf"
                };
                Content.Children.Add(pdfview);
            }
            else
            {
                pdfview = new WebView()
                {
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    Source = "http://www.africau.edu/images/default/sample.pdf"
                };
                Content.Children.Add(pdfview);
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        async void GoBack(object sender, EventArgs args)
        {
            await Navigation.PopAsync();
        }

        async void ShareBill(object sender, EventArgs args)
        {
            await Xamarin.Essentials.Share.RequestAsync(new ShareTextRequest()
            {
                Uri = viewModel.Path,
                Text = "Поделиться квитанцией"
            });
        }

        async void Print(object sender, EventArgs args)
        {
            HttpClient client = new HttpClient();
            DependencyService.Get<xamarinJKH.InterfacesIntegration.IPrintManager>().SendFileToPrint(await client.GetByteArrayAsync("http://www.africau.edu/images/default/sample.pdf"));
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
            get => "+" + Settings.Person.Phone;
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
            
            LoadPdf = new Command(async () =>
            {
               //TODO: Получение ссылки на настоящий файл квитанции с бека
                Path = "http://www.africau.edu/images/default/sample.pdf";//"file:///" + DependencyService.Get<IFileWorker>().GetFilePath(filename);
            });
        }
    }
}