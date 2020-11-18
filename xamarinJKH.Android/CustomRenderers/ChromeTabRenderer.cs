
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Net;

// using Xamarin.Android.ChromeCustomTabs;
// using Android.Support.CustomTabs.Chromium.SharedUtilities;
using Android.Support.CustomTabs;
// using Android.Support.CustomTabs.Shared;
using Xamarin.Forms.Platform.Android;
using Android.Webkit;

using xamarinJKH.InterfacesIntegration;
using xamarinJKH.Droid.CustomRenderers;
using Xamarin.Forms;
using Plugin.CurrentActivity;

// using Org.Chromium.Chrome;
// using Org.Chromium.Chrome.Browser.Hosted;
// using Org;
// using Org.Chromium;
// using Org.Chromium.Chrome.Browser;

[assembly:Dependency(typeof(ChromeTabRenderer))]
namespace xamarinJKH.Droid.CustomRenderers
{
    public class ChromeTabRenderer:IChromeTabs
    {
        Activity activity { get; set; }
        public ChromeTabRenderer()
        {
            activity = CrossCurrentActivity.Current.Activity;
        }
        public void OpenTab(string uri)
        {
            //var uiBuilder = new HostedUIBuilder();
            
            //var manager = (CrossCurrentActivity.Current.Activity as MainActivity).hostedManager;
            //manager.BindService();
            //manager.LoadUrl("https://google.com/", uiBuilder);

            //var url = "https://www.xamarin.com";
            //var customTabsIntent = new CustomTabsIntent.Builder().Build();
            //var helper = new CustomTabActivityHelper();
            //helper.LaunchUrlWithCustomTabsOrFallback(activity, customTabsIntent, Xamarin.Essentials.AppInfo.PackageName, Uri.Parse(url), null);
            //Device.OpenUri(new System.Uri(url));
        }

    }
}
