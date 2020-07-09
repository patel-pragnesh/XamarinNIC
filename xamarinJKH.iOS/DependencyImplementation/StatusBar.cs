using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using Xamarin.Forms;
using xamarinJKH.InterfacesIntegration;
using xamarinJKH.iOS.DependencyImplementation;

[assembly: Dependency(typeof(StatusBar))]
namespace xamarinJKH.iOS.DependencyImplementation
{
    class StatusBar : IStatusBar
    {
        public int GetHeight()
        {
            return (int)UIApplication.SharedApplication.StatusBarFrame.Height;
        }
    }
}