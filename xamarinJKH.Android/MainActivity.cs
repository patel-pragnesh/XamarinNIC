using System;
using Android;
using Android.App;
using Context = Android.Content.Context;
using Intent = Android.Content.Intent;
using Android.Content.PM;
using Android.Net;
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

using Firebase.Iid;


namespace xamarinJKH.Droid
{
    [Activity(Label = "Тихая Гавань", Icon = "@drawable/icon_login", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity 
    {
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.SetFlags("RadioButton_Experimental", "AppTheme_Experimental");
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
            //CreateNotificationChannel();
            FirebasePushNotificationManager.ProcessIntent(this, Intent);
            Fabric.Fabric.With(this, new Crashlytics.Crashlytics());
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init(true);
            LoadApplication(new App());

        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                // Notification channels are new in API 26 (and not a part of the
                // support library). There is no need to create a notification
                // channel on older versions of Android.
                return;
            }

            var channel = new NotificationChannel("occ_test_notification_channel",
                                                  "OCC",
                                                  NotificationImportance.Default)
            {

                Description = "Firebase Cloud Messages appear in this channel"
            };


            var notificationManager = (NotificationManager)GetSystemService(Context.NotificationService);
            notificationManager.CreateNotificationChannel(channel);
        }

        //[Service]
        //[IntentFilter(new[] { "com.google.firebase.INSTANCE_ID_EVENT" })]
        //public class MyFirebaseIIDService : FirebaseInstanceIdService
        //{
        //    const string TAG = "MyFirebaseIIDService";
        //    public override void OnTokenRefresh()
        //    {
        //        var refreshedToken = FirebaseInstanceId.Instance.Token;
        //        SendRegistrationToServer(refreshedToken);
        //    }
        //    void SendRegistrationToServer(string token)
        //    {
        //        App.token = token;
        //    }
        //}

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            FirebasePushNotificationManager.ProcessIntent(this, intent);
        }
    }
}