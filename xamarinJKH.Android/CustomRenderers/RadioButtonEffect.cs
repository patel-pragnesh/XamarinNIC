using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using xamarinJKH.Droid.CustomRenderers;
using xamarinJKH.Utils;
using ListView = Xamarin.Forms.ListView;
using RadioButton = Android.Widget.RadioButton;


[assembly: ExportEffect(typeof(RadioButtonEffect), nameof(RadioButtonEffect))]
namespace xamarinJKH.Droid.CustomRenderers
{
    public class RadioButtonEffect: PlatformEffect
    {
        protected override void OnAttached()

        {
            var radioButton = (RadioButton)Control;
           
            radioButton.ButtonTintList = ColorStateList.ValueOf(global::Android.Graphics.Color.ParseColor("#" + Settings.MobileSettings.color));
        }

        
        protected override void OnDetached()

        {

 

        }
    }
}