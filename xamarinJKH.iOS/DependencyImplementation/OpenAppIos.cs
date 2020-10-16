using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using xamarinJKH.InterfacesIntegration;
using xamarinJKH.iOS.DependencyImplementation;

[assembly: Xamarin.Forms.Dependency(typeof(OpenAppIos))]
namespace xamarinJKH.iOS.DependencyImplementation
{
    public class OpenAppIos : IOpenApp
    {
        public bool IsOpenApp(string package)
        {
            if(package.Contains("what"))
            {
                var s = new NSUrl(new NSString("whatsapp://send?text=Hello%2C%20World!"));
                if (UIApplication.SharedApplication.CanOpenUrl(s))
                {
                    return true;
                }                
            }
            if (package.Contains("viber"))
            {
                var s = new NSUrl(new NSString("viber://public"));
                if (UIApplication.SharedApplication.CanOpenUrl(s))
                {
                    return true;
                }
            }
            if (package.Contains("teleg"))
            {
                var s = new NSUrl(new NSString("tg://resolve?domain=MyBot"));
                if (UIApplication.SharedApplication.CanOpenUrl(s))
                {
                    return true;
                }             
            }

            return false;
        }
    }
}