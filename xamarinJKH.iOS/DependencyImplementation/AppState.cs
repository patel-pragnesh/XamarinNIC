using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Foundation;
using UIKit;
using Xamarin.Forms;
using xamarinJKH.InterfacesIntegration;
using xamarinJKH.iOS.DependencyImplementation;

[assembly: Dependency(typeof(AppState))]
namespace xamarinJKH.iOS.DependencyImplementation
{
    class AppState : IAppState
    {
        public bool IsAppBackbround()
        {
            bool isInBackbround = UIApplication.SharedApplication.ApplicationState == UIApplicationState.Background;
            return isInBackbround;
        }
    }
}