using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Plugin.CurrentActivity;
using Plugin.Media;
using Xamarin.Forms;
using xamarinJKH.Utils;


namespace xamarinJKH.Droid
{
    [Activity(Label = "xamarinJKH", Icon = "@drawable/icon_login", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
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
            Messier16.Forms.Android.Controls.Messier16Controls.InitAll();
            Rg.Plugins.Popup.Popup.Init(this, savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            await CrossMedia.Current.Initialize();
            SimpleImageButton.SimpleImageButton.Initializator.Initializator.Init();
            CrossCurrentActivity.Current.Init(this, savedInstanceState);
            LoadApplication(new App());
          

        }
    }
}