using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xamarin.Forms;

namespace xamarinJKH.CustomRenderers
{
    public class CounterEntry:Entry
    {
        //public static BindableProperty CharachterCount = BindableProperty.Create("CharCount", typeof(string), typeof(string), 9);
        //public string CharCount
        //{
        //    set => SetValue(CharachterCount, value);
        //}

        bool DecimalPart;
        public CounterEntry() : base()
        {
            //var characters = int.Parse((string)GetValue(CharachterCount));
            //this.CharacterSpacing = this.Width / characters * 2;
            this.CursorPosition = 5;
            this.Focus();
            this.TextChanged += (sender, e) =>
            {
                //try
                //{
                //    if (e.NewTextValue == "00000.000")
                //    {
                //        this.Text = string.Empty;
                //        return;
                //    }
                //    var newval = 0.0; double.TryParse(e.NewTextValue.Replace(',', '.'), out newval);
                //    var oldval = 0.0; double.TryParse(e.OldTextValue.Replace(',', '.'), out oldval);
                //    this.Text = String.Format("{0:00000.000}", newval);
                //    var Decimal = newval - Math.Floor(newval) > 0;
                //    if (!Decimal)
                //        this.CursorPosition = 5;

                //    if (e.NewTextValue.Length < e.OldTextValue.Length)
                //    {
                //        this.CursorPosition = e.NewTextValue.Length - 1;
                //    }
                    
                    
                //}
                //catch
                //{

                //}
            };
        }
    }
}
