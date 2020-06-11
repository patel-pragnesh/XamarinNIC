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
        int Changed;
        public CounterEntry() : base()
        {
            //var characters = int.Parse((string)GetValue(CharachterCount));
            //this.CharacterSpacing = this.Width / characters * 2;
            this.CursorPosition = 5;
            this.Focus();
            this.TextChanged += (sender, e) =>
            {
                try
                {
                    
                    var val = e.NewTextValue.Replace('.', ',');
                    
                    if (e.NewTextValue != e.OldTextValue)
                    {
                        if (val[e.NewTextValue.Length - 1] == ',')
                        {
                            this.Text = String.Format("{0:00000.###}", double.Parse(val)).Replace('.', ',');

                        }
                        if (string.IsNullOrEmpty(e.OldTextValue) && double.Parse(val.Replace(',','.')) > 0 && Changed < 1)
                        {
                            Changed++;
                            decimal value = decimal.Parse(val.Replace(',', '.'));
                            value += 0.0001M;
                            this.Text = String.Format("{0:00000.000}", value).Replace('.', ',');
                            this.CursorPosition = 9;
                        }
                    }
                    
                }
                catch
                {

                }
                
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

        public static int GetDecimalPlaces(decimal n)
        {
            n = Math.Abs(n); //make sure it is positive.
            n -= (int)n;     //remove the integer part of the number.
            var decimalPlaces = 0;
            while (n > 0)
            {
                decimalPlaces++;
                n *= 10;
                n -= (int)n;
            }
            return decimalPlaces;
        }
    }
}
