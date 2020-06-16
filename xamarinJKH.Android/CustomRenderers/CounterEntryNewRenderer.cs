using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using xamarinJKH;
using xamarinJKH.Droid.CustomRenderers;
using Color = Android.Graphics.Color;

[assembly: ExportRenderer(typeof(CounterEntryNew), typeof(CounterEntryNewRenderer))]

namespace xamarinJKH.Droid.CustomRenderers
{
    public class CounterEntryNewRenderer: EntryRenderer
    {
        public CounterEntryNewRenderer(Context context) : base(context)
        {

        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            var color = Xamarin.Forms.Color.Transparent; 

            Color aColor = color.ToAndroid();

            base.OnElementChanged(e);

            if (Control == null || e.NewElement == null) return;

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                Control.BackgroundTintList = ColorStateList.ValueOf(aColor);
               
            }
                
            else
                Control.Background.SetColorFilter(aColor, PorterDuff.Mode.SrcAtop);
        }

        public override bool DispatchKeyEvent(KeyEvent e)
        {
            if (e.Action == KeyEventActions.Down)
            {
                if (e.KeyCode == Keycode.Del)
                {
                    if (string.IsNullOrWhiteSpace(Control.Text))
                    {
                        var entry = (CounterEntryNew)Element;
                        entry.OnBackspacePressed();
                    }
                }
            }
            return base.DispatchKeyEvent(e);
        }

        
    }
}