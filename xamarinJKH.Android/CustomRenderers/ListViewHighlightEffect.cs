using Android.Graphics.Drawables;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using xamarinJKH.Droid.CustomRenderers;
using Color = Android.Graphics.Color;
using ListView = Android.Widget.ListView;

[assembly: ResolutionGroupName("MyEffects")]

[assembly: ExportEffect(typeof(ListViewHighlightEffect), nameof(ListViewHighlightEffect))]
namespace xamarinJKH.Droid.CustomRenderers
{
    public class ListViewHighlightEffect: PlatformEffect
    {
        protected override void OnAttached()

        {

            var listView = (ListView)Control;
            listView.Selector = new ColorDrawable(Color.Transparent);
            listView.ChoiceMode = ChoiceMode.None; // !!!

        }

 

        protected override void OnDetached()

        {

 

        }
    }
}