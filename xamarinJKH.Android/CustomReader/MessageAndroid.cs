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
            try
            {
                Toast.MakeText(Application.Context, message, ToastLength.Long).Show();
            }
            catch { }
        }

        public void ShortAlert(string message)
        {
            try
            {
                Toast.MakeText(Application.Context, message, ToastLength.Short).Show();
            }
            catch { }
        }
    }
}