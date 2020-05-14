using System;
using Xamarin.Forms;

namespace xamarinJKH.InterfacesIntegration
{
    public class PdfView : WebView
    {
        public static readonly BindableProperty UriProperty = BindableProperty.Create(nameof(Uri), typeof(string), typeof(PdfView));
        
        public string Uri
        {
            get { return (string)GetValue(UriProperty); }
            set { SetValue(UriProperty, value); }
        }
    }
}