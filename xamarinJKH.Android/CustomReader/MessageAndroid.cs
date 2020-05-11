﻿using Android.App;
using Android.Widget;
 using xamarinJKH.Android;
 using xamarinJKH.InterfacesIntegration;

 [assembly: Xamarin.Forms.Dependency(typeof(MessageAndroid))]
namespace xamarinJKH.Android
{
    public class MessageAndroid : IMessage
    {
       
        public void LongAlert(string message)
        {
            Toast.MakeText(Application.Context, message, ToastLength.Long).Show();
        }

        public void ShortAlert(string message)
        {
            Toast.MakeText(Application.Context, message, ToastLength.Short).Show();
        }
    }
}