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
using xamarinJKH.InterfacesIntegration;
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

            if (Device.RuntimePlatform == Device.iOS)
            {
                int statusBarHeight = DependencyService.Get<IStatusBar>().GetHeight();
                Pancake2.HeightRequest = statusBarHeight;// new Thickness(0, statusBarHeight, 0, 0);
            }

           // var pinchGesture = new PinchGestureRecognizer();
           // pinchGesture.PinchUpdated += OnPinchUpdated;
           //Image.GestureRecognizers.Add(pinchGesture);

            var backClick = new TapGestureRecognizer();
            backClick.Tapped += async (s, e) => { _ = await Navigation.PopAsync(); };
            BackStackLayout.GestureRecognizers.Add(backClick);
            LoadPdf();
            NavigationPage.SetHasNavigationBar(this, false);
            BindingContext = this;
        }

        //double currentScale = 1;
        //double startScale = 1;
        //double xOffset = 0;
        //double yOffset = 0;

        //private void OnPinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
        //{

        //    if (e.Status == GestureStatus.Started)
        //    {
        //        // Store the current scale factor applied to the wrapped user interface element,
        //        // and zero the components for the center point of the translate transform.
        //        startScale = Content.Scale;
        //        Content.AnchorX = 0;
        //        Content.AnchorY = 0;
        //    }
        //    if (e.Status == GestureStatus.Running)
        //    {
        //        // Calculate the scale factor to be applied.
        //        currentScale += (e.Scale - 1) * startScale;
        //        currentScale = Math.Max(1, currentScale);

        //        // The ScaleOrigin is in relative coordinates to the wrapped user interface element,
        //        // so get the X pixel coordinate.
        //        double renderedX = Content.X + xOffset;
        //        double deltaX = renderedX / Width;
        //        double deltaWidth = Width / (Content.Width * startScale);
        //        double originX = (e.ScaleOrigin.X - deltaX) * deltaWidth;

        //        // The ScaleOrigin is in relative coordinates to the wrapped user interface element,
        //        // so get the Y pixel coordinate.
        //        double renderedY = Content.Y + yOffset;
        //        double deltaY = renderedY / Height;
        //        double deltaHeight = Height / (Content.Height * startScale);
        //        double originY = (e.ScaleOrigin.Y - deltaY) * deltaHeight;

        //        // Calculate the transformed element pixel coordinates.
        //        double targetX = xOffset - (originX * Content.Width) * (currentScale - startScale);
        //        double targetY = yOffset - (originY * Content.Height) * (currentScale - startScale);

        //        // Apply translation based on the change in origin.
        //        Content.TranslationX = targetX.Clamp(-Content.Width * (currentScale - 1), 0);
        //        Content.TranslationY = targetY.Clamp(-Content.Height * (currentScale - 1), 0);

        //        // Apply scale factor.
        //        Content.Scale = currentScale;
        //    }
        //    if (e.Status == GestureStatus.Completed)
        //    {
        //        // Store the translation delta's of the wrapped user interface element.
        //        xOffset = Content.TranslationX;
        //        yOffset = Content.TranslationY;
        //    }

        //}

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