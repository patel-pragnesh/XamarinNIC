using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using xamarinJKH.ViewModels;
using AiForms.Dialogs.Abstractions;
using xamarinJKH.Utils;
using xamarinJKH.InterfacesIntegration;
using AiForms.Dialogs;

namespace xamarinJKH.DialogViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UpdateNotificationDialog : DialogView
    {
        public UpdateNotificationDialog()
        {
            InitializeComponent();
            IsCanceledOnTouchOutside = false;
            BindingContext = new UpdateNotificationDialogViewModel();
        }

        async void CloseDialog(object sender, EventArgs args)
        {
            DialogNotifier.Cancel();
        }

        async void OpenMarket(object sender, EventArgs args)
        {
            string uri = string.Empty;
            string app_name = Xamarin.Essentials.AppInfo.PackageName;
            string name = Xamarin.Essentials.AppInfo.Name.ToLower().Trim().Replace(" ","-");
            if (Device.RuntimePlatform == "Android")
            {
                uri = $"market://details?{app_name}";
            }

            if (Device.RuntimePlatform == "iOS")
            {
                throw new NotImplementedException("Нужно добавить id приложения в ссылку ниже");
                var id = string.Empty;
                uri = $"itms-apps://itunes.apple.com/ru/app/{name}/id{id}?mt=8";
            }

            if (!string.IsNullOrEmpty(uri))
            {
                await Xamarin.Essentials.Launcher.OpenAsync(uri);
            }
        }
    }

    public class UpdateNotificationDialogViewModel : BaseViewModel
    {
        public Color ButtonColor
        {
            get
            {
                try
                {
                    return Color.FromHex(Settings.MobileSettings.color);
                }
                catch
                {
                    return Color.Blue;
                }
            }
                 
        }
    }
}