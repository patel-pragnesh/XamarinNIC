using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using xamarinJKH.InterfacesIntegration;
using xamarinJKH.iOS.DependencyImplementation;

[assembly: Xamarin.Forms.Dependency(typeof(MessageIos))]

namespace xamarinJKH.iOS.DependencyImplementation
{
    public class MessageIos : IMessage
    {
        public void LongAlert(string message)
        {
            //throw new NotImplementedException();
            var alert = new UIAlertView()
            {                
                Message = message
            };            
            alert.AddButton("OK");
            alert.Show();            
        }

        public void ShortAlert(string message)
        {
            var alert = new UIAlertView()
            {
                Message = message
            };
            alert.AddButton("OK");
            alert.Show();
        }


    }
}