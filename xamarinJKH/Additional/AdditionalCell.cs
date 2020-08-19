using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Linq;
using Akavache;
using Xamarin.Forms;
using Xamarin.Forms.PancakeView;
using xamarinJKH.CustomRenderers;
using xamarinJKH.Server;

namespace xamarinJKH.Additional
{
    public class AdditionalCell : ViewCell
    {
        Image image;

        //MaterialFrame frame;
        PancakeView frame;
        RestClientMP _server = new RestClientMP();

        public AdditionalCell()
        {
            image = new Image();
            

            frame = new PancakeView(); // MaterialFrame();
            //frame.Elevation = 20;
            frame.HorizontalOptions = LayoutOptions.FillAndExpand;
            frame.VerticalOptions = LayoutOptions.Start;
            // frame.BackgroundColor =  Color.White;
            frame.IsClippedToBounds = true;
            frame.Margin = new Thickness(10, 0, 10, 10);
            frame.Padding = new Thickness(0);
            frame.CornerRadius = 40;

            //frame.BackgroundColor = Color.Red;

            //Frame cell = new Frame();
            //cell.Padding = new Thickness(0);
            //cell.HorizontalOptions = LayoutOptions.FillAndExpand;
            //cell.VerticalOptions = LayoutOptions.Start;
            //cell.BackgroundColor = Color.White;
            //cell.IsClippedToBounds = true;
            //cell.CornerRadius = 40;
            //cell.Content = image;
            //cell.Children.Add(image);

            frame.Content = image;

            View = frame;
        }

        public static readonly BindableProperty ImagePathProperty =
            BindableProperty.Create("ImagePath", typeof(ImageSource), typeof(AdditionalCell), null);

        public static readonly BindableProperty ImageWidthProperty =
            BindableProperty.Create("ImageWidth", typeof(int), typeof(AdditionalCell), 100);

        public static readonly BindableProperty ImageHeightProperty =
            BindableProperty.Create("ImageHeight", typeof(int), typeof(AdditionalCell), 100);

        public static readonly BindableProperty DetailProperty =
            BindableProperty.Create("Detail", typeof(string), typeof(AdditionalCell), "");
 
        public static readonly BindableProperty LogoFileIdProperty =
            BindableProperty.Create("LogoFileId", typeof(string), typeof(AdditionalCell), "");

        public int ImageWidth
        {
            get { return (int) GetValue(ImageWidthProperty); }
            set { SetValue(ImageWidthProperty, value); }
        }

        public int ImageHeight
        {
            get { return (int) GetValue(ImageHeightProperty); }
            set { SetValue(ImageHeightProperty, value); }
        }

        public String ImagePath
        {
            get { return (String) GetValue(ImagePathProperty); }
            set { SetValue(ImagePathProperty, value); }
        }

        public string Detail
        {
            get { return (string) GetValue(DetailProperty); }
            set { SetValue(DetailProperty, value); }
        } 
        
        public string LogoFileId
        {
            get { return (string) GetValue(LogoFileIdProperty); }
            set { SetValue(LogoFileIdProperty, value); }
        }

        protected override async void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (BindingContext != null)
            {
               
                    byte[] imageByte = null;
                    try {
                        imageByte = await BlobCache.UserAccount.GetObject<byte[]>(LogoFileId);
                    } catch (KeyNotFoundException ex) {
                        imageByte = await _server.GetPhotoAdditional(Detail);
                        await BlobCache.UserAccount.InsertObject(LogoFileId, imageByte);
                    }
                    if (imageByte != null)
                    {
                        Stream stream = new MemoryStream(imageByte);
                        //image = new Image();       
                        
                        image.Source = ImageSource.FromStream(() => { return stream; });
                        image.VerticalOptions = LayoutOptions.FillAndExpand;
                        image.Aspect = Aspect.AspectFill;
                        image.HorizontalOptions = LayoutOptions.FillAndExpand;
                        image.HeightRequest = ImageHeight;

                        //frame.Content = image;
                    }
            }
        }
    }
}