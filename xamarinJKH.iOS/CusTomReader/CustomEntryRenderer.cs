using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
 using xamarinJKH.iOS;

 [assembly: ExportRenderer(typeof(Entry), typeof(CustomEntryRenderer))]
namespace xamarinJKH.iOS
{
    public class CustomEntryRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);
            Control.Layer.BorderWidth = 0;
            Control.BorderStyle = UITextBorderStyle.None;
        }
    }
}