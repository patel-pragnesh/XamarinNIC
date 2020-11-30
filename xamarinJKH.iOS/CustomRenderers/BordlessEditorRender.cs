
using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using xamarinJKH;
using xamarinJKH.iOS.CustomRenderers;

[assembly: ExportRenderer(typeof(BordlessEditor), typeof(BordlessEditorRender))]
namespace xamarinJKH.iOS.CustomRenderers
{
    public class BordlessEditorRender : EditorRenderer
    {
        public static void Init() { }
        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {
            try
            {
                base.OnElementChanged(e);
                if (this.Control != null)
                {
                    
                    this.Control.InputAccessoryView = null;
                }                
            }
            catch(Exception ex)
            {

            }
            
        }
    }
}