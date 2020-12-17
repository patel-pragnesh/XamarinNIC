using System.Windows.Input;
using Xamarin.Forms;

namespace xamarinJKH
{
    public class EntryWithCustomKeyboard: Entry
    {
        public static readonly BindableProperty EnterCommandProperty = BindableProperty.Create(
            nameof(EnterCommand),
            typeof(ICommand),
            typeof(EntryWithCustomKeyboard),
            default(ICommand),
            BindingMode.OneWay
        );
        
        public static readonly BindableProperty DecimalPointProperty = BindableProperty.Create(
            nameof(DecimalPoint),
            typeof(int),
            typeof(EntryWithCustomKeyboard),
            0,
            BindingMode.OneWay
        ); 
        
        public static readonly BindableProperty IntegerPointProperty = BindableProperty.Create(
            nameof(IntegerPoint),
            typeof(int),
            typeof(EntryWithCustomKeyboard),
            0,
            BindingMode.OneWay
        );

        public ICommand EnterCommand
        {
            get => (ICommand)GetValue(EnterCommandProperty);
            set => SetValue(EnterCommandProperty, value);
        } 
        public int DecimalPoint
        {
            get => (int)GetValue(DecimalPointProperty);
            set => SetValue(DecimalPointProperty, value);
        } 
        public int IntegerPoint
        {
            get => (int)GetValue(IntegerPointProperty);
            set => SetValue(IntegerPointProperty, value);
        }
    }
}