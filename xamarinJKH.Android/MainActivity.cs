using System;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V4.App;
using Plugin.CurrentActivity;
using Plugin.FirebasePushNotification;
using Plugin.Media;
using Xamarin.Forms;
using xamarinJKH.Android;
using xamarinJKH.Utils;


namespace xamarinJKH.Droid
{
    [Activity(Label = "Управдом Чебоксары", Icon = "@drawable/icon_login", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity 
    {
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.SetFlags("RadioButton_Experimental");
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            XamEffects.Droid.Effects.Init();
            AiForms.Dialogs.Dialogs.Init(this);
            App.ScreenHeight = (int) (Resources.DisplayMetrics.HeightPixels / Resources.DisplayMetrics.Density);
            App.ScreenWidth = (int) (Resources.DisplayMetrics.WidthPixels / Resources.DisplayMetrics.Density);
            App.version = Build.VERSION.Sdk;
            App.model = Build.Model;
            Messier16.Forms.Android.Controls.Messier16Controls.InitAll();
            Rg.Plugins.Popup.Popup.Init(this, savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            await CrossMedia.Current.Initialize();
            SimpleImageButton.SimpleImageButton.Initializator.Initializator.Init();
            CrossCurrentActivity.Current.Init(this, savedInstanceState);
            ActivityCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.Camera);
            ActivityCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.ReadExternalStorage);
            LoadApplication(new App());
            FirebasePushNotificationManager.ProcessIntent(this,Intent);
            Fabric.Fabric.With(this, new Crashlytics.Crashlytics());

        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        
        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            FirebasePushNotificationManager.ProcessIntent(this, intent);
        }
    }
}