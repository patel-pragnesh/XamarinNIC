using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using xamarinJKH.CustomRenderers;
using xamarinJKH.Droid.CustomRenderers;

using System.ComponentModel;

//[assembly: ExportRenderer(typeof(CounterEntry), typeof(CounterEntryRenderer))]
namespace xamarinJKH.Droid.CustomRenderers
{
    public class CounterEntryRenderer : EntryRenderer
    {
        public CounterEntryRenderer(Context context) : base(context)
        {

        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == "Text")
            {
                var charachter = Control.Text;
            }

        }
    }
}