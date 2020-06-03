
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using xamarinJKH.iOS.CustomRenderers;

[assembly: ResolutionGroupName("MyEffects")]

[assembly: ExportEffect(typeof(ListViewHighlightEffect), nameof(ListViewHighlightEffect))]
namespace xamarinJKH.iOS.CustomRenderers
{
    public class ListViewHighlightEffect : PlatformEffect
    {
        protected override void OnAttached()

        {

            var listView = (UIKit.UITableView)Control;

            
            listView.AllowsSelection = false; // !!!

        }

 

        protected override void OnDetached()

        {

 

        }
    }
}