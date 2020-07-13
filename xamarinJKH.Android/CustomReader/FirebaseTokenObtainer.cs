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

using xamarinJKH.InterfacesIntegration;
using Xamarin.Forms;
using xamarinJKH.Droid.CustomReader;

[assembly:Dependency(typeof(FirebaseTokenObtainer))]
namespace xamarinJKH.Droid.CustomReader
{
    public class FirebaseTokenObtainer : IFirebaseTokenObtainer
    {
        public string GetToken()
        {
            return Firebase.Iid.FirebaseInstanceId.Instance.Token;
        }
    }
}