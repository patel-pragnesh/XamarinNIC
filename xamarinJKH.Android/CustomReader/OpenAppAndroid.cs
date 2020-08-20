using Android.App;
using Android.Content;
using Android.Net;
using Xamarin.Forms;
using xamarinJKH.Droid.CustomReader;
using xamarinJKH.InterfacesIntegration;
using Application = Android.App.Application;

[assembly: Xamarin.Forms.Dependency(typeof(OpenAppAndroid))]

namespace xamarinJKH.Droid.CustomReader
{
    [Activity(Label = "OpenAppAndroid")]
    public class OpenAppAndroid : Activity, IOpenApp
    {
        public OpenAppAndroid()
        {
        }

        public void OpenExternalApp(string url)
        {
            Intent intent = Application.Context.PackageManager.GetLaunchIntentForPackage(url);

            // If not NULL run the app, if not, take the user to the app store
            if (intent != null)
            {
                intent.AddFlags(ActivityFlags.NewTask);
                Forms.Context.StartActivity(intent);
            }
            else
            {
                intent = new Intent(Intent.ActionView);
                intent.AddFlags(ActivityFlags.NewTask);
                intent.SetData(Uri.Parse("market://details?id=" + url));
                Forms.Context.StartActivity(intent);
            }
        }
    }
}