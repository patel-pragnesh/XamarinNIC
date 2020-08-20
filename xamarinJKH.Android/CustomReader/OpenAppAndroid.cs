using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
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

        public bool IsOpenApp(string package)
        {
            try
            {
                PackageInfo packageInfo = Application.Context.PackageManager.GetPackageInfo(package, 0);
                
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
    }
}