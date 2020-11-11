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

using WebK = Android.Webkit.WebViewClient;
using Cache = Android.Webkit.CacheModes;

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
                Control.SetWebViewClient(new CustomViewClient());
                Control.Settings.CacheMode = Cache.NoCache;
                Control.Settings.AllowContentAccess = true;
                Control.Settings.DatabaseEnabled = false;
                Control.LoadUrl(String.Format("https://docs.google.com/viewer?url={0}", customWebView.Uri));
            }
        }
    }
}