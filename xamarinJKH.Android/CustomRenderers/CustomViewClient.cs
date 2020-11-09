using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;

using Messaging = Xamarin.Forms.MessagingCenter;

namespace xamarinJKH.Droid.CustomRenderers
{
    public class CustomViewClient:WebViewClient
    {
        Activity mActivity => Plugin.CurrentActivity.CrossCurrentActivity.Current.Activity;
        public CustomViewClient() { }
        public override void OnPageFinished(WebView view, string url)
        {
            base.OnPageFinished(view, url);
            Messaging.Send<Object>(this, "ReleasePdfLoading");
        }
    }
}