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
            SetupLayer(); //UpdateShadow();
        }
 
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName ||
          e.PropertyName == Xamarin.Forms.Frame.BorderColorProperty.PropertyName ||
          e.PropertyName == Xamarin.Forms.Frame.HasShadowProperty.PropertyName ||
          e.PropertyName == Xamarin.Forms.Frame.CornerRadiusProperty.PropertyName || e.PropertyName == "Elevation")
                SetupLayer();
            //if(e.PropertyName == "Elevation")
            //{
            //    UpdateShadow();
            //}
        }

        void SetupLayer()
        {
            if (Element.HasShadow)
            {
                var materialFrame = (MaterialFrame)Element;
                float sr = 7;
                if (materialFrame.Elevation < 10)
                    sr = materialFrame.Elevation;
                Layer.ShadowRadius = sr; 
                Layer.ShadowColor = UIColor.Gray.CGColor;
                Layer.ShadowOffset = new CGSize(2, 2);
                Layer.ShadowOpacity = 0.80f;
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
            Layer.MasksToBounds = true;
        }

    }
}