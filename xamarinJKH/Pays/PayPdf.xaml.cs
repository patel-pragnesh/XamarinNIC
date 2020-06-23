using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using xamarinJKH.ViewModels;
using xamarinJKH.Server.RequestModel;
using xamarinJKH.Utils;
using RestSharp;

namespace xamarinJKH.Pays
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PayPdf : ContentPage
    {
        public PayPdfViewModel viewModel { get; set; }
        public PayPdf(PayPdfViewModel vm)
        {
            InitializeComponent();
            BindingContext = viewModel = vm;
            //PDF.Source = viewModel.Path;
            //PDF.Navigating += async (s, e) =>
            //{
            //    await PDF.EvaluateJavaScriptAsync("PDF.js");
            //};
        }

        async void GoBack(object sender, EventArgs args)
        {
            await Navigation.PopAsync();
        }

        async void ShareBill(object sender, EventArgs args)
        {
            await Xamarin.Essentials.Share.RequestAsync(this.viewModel.Bill.Period);
        }
    }

    

    public class PayPdfViewModel : BaseViewModel
    {
        BillInfo _bill;
        public string Theme
        {
            get => "#" + Settings.MobileSettings.color;
        }
        public BillInfo Bill
        {
            get => _bill;
            set
            {
                _bill = value;
                OnPropertyChanged("Bill");
            }
        }
        public string Phone
        {
            get => "+" + Settings.Person.Phone;
        }

        public string Name
        {
            get => Settings.MobileSettings.main_name;
        }
        public PayPdfViewModel(BillInfo info)
        {
            Bill = info;
        }
    }
}