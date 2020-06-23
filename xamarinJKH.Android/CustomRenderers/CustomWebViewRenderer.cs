using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using xamarinJKH.Droid.CustomRenderers;
using xamarinJKH.CustomRenderers;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

using System.Net;

[assembly: ExportRenderer(typeof(CustomWebView), typeof(CustomWebViewRenderer))]
namespace xamarinJKH.Droid.CustomRenderers
{
    public class CustomWebViewRenderer : WebViewRenderer
    {
        public CustomWebViewRenderer(Context context) : base(context) { }
        protected override void OnElementChanged(ElementChangedEventArgs<WebView> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                var customWebView = Element as CustomWebView;
                Control.Settings.AllowUniversalAccessFromFileURLs = true;
                Control.Settings.JavaScriptEnabled = true;
                Control.Settings.JavaScriptCanOpenWindowsAutomatically = true;
                Control.LoadUrl(String.Format("https://docs.google.com/viewer?url={0}", customWebView.Uri));
                //Control.LoadUrl(string.Format("file:///android_asset/pdfjs/web/viewer.html?file={0}", string.Format("file:///{0}", customWebView.Uri)));
            }
        }
    }
}