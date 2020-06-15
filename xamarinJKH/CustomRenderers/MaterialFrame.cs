using Xamarin.Forms;

namespace xamarinJKH.CustomRenderers
{
    public class MaterialFrame: Frame
    {
        public static BindableProperty ElevationProperty = BindableProperty.Create(nameof(Elevation), typeof(float), typeof(Button), 4.0f);
 
        public float Elevation
        {
            get
            {
                return (float)GetValue(ElevationProperty);
            }
            set
            {
                SetValue(ElevationProperty, value);
            }
        }
    }
}