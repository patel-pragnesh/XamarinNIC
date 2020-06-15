using System.ComponentModel;
using Android.Animation;
using Android.Support.V4.View;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using xamarinJKH.CustomRenderers;
using xamarinJKH.Droid.CustomRenderers;

[assembly: ExportRenderer(typeof(MaterialFrame), typeof(MaterialFrameRenderer))]
namespace xamarinJKH.Droid.CustomRenderers
{
    public class MaterialFrameRenderer  : Xamarin.Forms.Platform.Android.AppCompat.FrameRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
        {
            base.OnElementChanged(e);
            if (e.NewElement == null)
                return;
 
            UpdateElevation();
        }
 
 
        private void UpdateElevation()
        {
            var materialFrame= (MaterialFrame)Element;
 
            // we need to reset the StateListAnimator to override the setting of Elevation on touch down and release.
            Control.StateListAnimator = new StateListAnimator();
 
            // set the elevation manually
            ViewCompat.SetElevation(this, materialFrame.Elevation);
            ViewCompat.SetElevation(Control, materialFrame.Elevation);
        }
 
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if(e.PropertyName == "Elevation")
            {
                UpdateElevation();
            }
        }
    }

  
}