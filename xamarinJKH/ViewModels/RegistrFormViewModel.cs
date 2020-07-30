using System;
using System.Collections.Generic;
using System.Text;

using Xamarin.Forms;
using xamarinJKH;
using xamarinJKH.Utils;

namespace xamarinJKH.ViewModels
{
    public class RegistrFormViewModel:BaseViewModel
    {
        readonly INavigation Navigation;
        public Command Register { get; set; }
        public RegistrFormViewModel(INavigation navigation)
        {
            Navigation = navigation;
            Register = new Command<Tuple<string, string>>(async (data) =>
            {

            });
        }
    }
}
