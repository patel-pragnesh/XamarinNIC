using System.ComponentModel;
using CoreGraphics;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using xamarinJKH.CustomRenderers;
using xamarinJKH.iOS.CustomRenderers;

[assembly: ExportRenderer(typeof(MaterialFrame), typeof(MaterialFrameRenderer))]
namespace xamarinJKH.iOS.CustomRenderers
{
    public class MaterialFrameRenderer: FrameRenderer
    {
        public static void Initialize()
        {
            // empty, but used for beating the linker
        }
        protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
        {
            base.OnElementChanged(e);
 
            if (e.NewElement == null)
                return;
            UpdateShadow();
        }
 
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if(e.PropertyName == "Elevation")
            {
                UpdateShadow();
            }
        }
 
        private void UpdateShadow()
        {
 
            var materialFrame = (MaterialFrame)Element;
 
            // Update shadow to match better material design standards of elevation
            Layer.ShadowRadius = materialFrame.Elevation;
            Layer.ShadowColor = UIColor.Gray.CGColor;
            Layer.ShadowOffset = new CGSize(2, 2);
            Layer.ShadowOpacity = 0.80f;
            Layer.ShadowPath = UIBezierPath.FromRect(Layer.Bounds).CGPath;
            Layer.MasksToBounds = false;
 
        }
    }
}