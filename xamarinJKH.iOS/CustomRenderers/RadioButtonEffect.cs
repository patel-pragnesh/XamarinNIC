using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using xamarinJKH.iOS.CustomRenderers;
using ListView = Xamarin.Forms.ListView;

[assembly: ExportEffect(typeof(RadioButtonEffect), nameof(RadioButtonEffect))]

namespace xamarinJKH.iOS.CustomRenderers
{
    public class RadioButtonEffect : PlatformEffect
    {
        protected override void OnAttached()

        {
            // var radioButton = (RadioButton)Control;
            // var radioButton = (RadioButton)Control;
           
            // radioButton.BorderColor = ColorStateList.ValueOf(global::Android.Graphics.Color.ParseColor("#" + Settings.MobileSettings.color));
        }

 

        protected override void OnDetached()

        {

 

        }
        
    }
}