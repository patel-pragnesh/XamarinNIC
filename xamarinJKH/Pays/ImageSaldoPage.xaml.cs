using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AiForms.Dialogs;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;
using xamarinJKH.Server;
using xamarinJKH.Server.RequestModel;

namespace xamarinJKH.Pays
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ImageSaldoPage : ContentPage
    {
        public bool hex { get; set; }
        RestClientMP server = new RestClientMP();
        public string Period { get; set; }
        BillInfo _billInfo = new BillInfo();

        public ImageSaldoPage(BillInfo bill)
        {
            Period = bill.Period;
            _billInfo = bill;
            InitializeComponent();
            var backClick = new TapGestureRecognizer();
            backClick.Tapped += async (s, e) => { _ = await Navigation.PopAsync(); };
            BackStackLayout.GestureRecognizers.Add(backClick);
            LoadPdf();
            NavigationPage.SetHasNavigationBar(this, false);
            BindingContext = this;
        }

        async void LoadPdf()
        {
            new Task(async () =>
            {
                byte[] stream;
                stream = await server.DownloadFileAsync(_billInfo.ID.ToString(), 1);
                if (stream != null)
                {
                    Stream streamM = new MemoryStream(stream);
                    Device.BeginInvokeOnMainThread(async () =>
                        Image.Source = ImageSource.FromStream(() => { return streamM; }));
                }
                else
                {
                    await DisplayAlert(AppResources.ErrorTitle, "Не удалось скачать файл", "OK");
                }

                Device.BeginInvokeOnMainThread(async () => progress.IsVisible = false);
            }).Start();
        }

        async void ShareBill(object sender, EventArgs args)
        {
            await Xamarin.Essentials.Share.RequestAsync(new ShareTextRequest()
            {
                Uri = _billInfo.FileLink,
                Text = AppResources.ShareBill
            });
        }

        async void Print(object sender, EventArgs args)
        {
            HttpClient client = new HttpClient();
            Loading.Instance.Show(AppResources.Loading);
            try
            {
                var file = await client.GetByteArrayAsync(_billInfo.FileLink);
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
}