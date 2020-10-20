using AiForms.Dialogs.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AppCenter.Analytics;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamarinJKH.Server;
using xamarinJKH.Server.RequestModel;

using xamarinJKH.Additional;
using xamarinJKH.Shop;

namespace xamarinJKH.DialogViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapShopDialogView : DialogView
    {
        public AdditionalService Service { get; set; }
        string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        string _image;
        public string Image
        {
            get => _image;
            set
            {
                _image = value;
                OnPropertyChanged(nameof(Image));
            }
        }

        public Command Open { get; set; }
        public MapShopDialogView(AdditionalService service)
        {
            InitializeComponent();
            Analytics.TrackEvent("Диалог выбора товара из магазина");
            Service = service;
            if (Service != null)
            {
                Name = Service.Name;
                Image = RestClientMP.SERVER_ADDR + "/AdditionalServices/Logo/" + Service.ID.ToString();
            }

            Open = new Command(async () =>
            {
                DialogNotifier.Cancel();
                AdditionalService select = Service;
                MessagingCenter.Send<Object, AdditionalService>(this, "OpenService", select);
            });
            BindingContext = this;
        }
    }
}