using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamarinJKH.Server.RequestModel;
using xamarinJKH.ViewModels;

namespace xamarinJKH.DialogViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SearchDialogView : PopupPage
    {
        int Type;
        SearchDialogViewModel viewModel { get; set; }
        public SearchDialogView(int type)
        {
            InitializeComponent();
            Type = type;
            BindingContext = viewModel = new SearchDialogViewModel(type);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            viewModel.LoadResults.Execute(null);
        }

        private void BordlessEditor_TextChanged(object sender, TextChangedEventArgs e)
        {
            var text = (sender as BordlessEditor).Text;
            viewModel.Filter.Execute(text);
        }

        private void ItemSelected(object sender, SelectionChangedEventArgs e)
        {
            var item = e.CurrentSelection[0] as NamedValue;
            if (item != null)
            {
                if (Type == (int)SearchType.DISTRICT)
                {
                    MessagingCenter.Send<Object, int?>(this, "SetDistrict", Convert.ToInt32(item.ID));
                }
            }
        }
    }


    public class SearchDialogViewModel : BaseViewModel
    {
        public ObservableCollection<NamedValue> Items { get; set; }
        public Command LoadResults { get; set; }
        public Command Filter { get; set; }
         
        public int Type { get; set; }
        public SearchDialogViewModel(int type)
        {
            Type = type;
            Items = new ObservableCollection<NamedValue>();
            LoadResults = new Command<string>((text) =>
            {
                string searchTerm = Convert.ToString(text);
                IsBusy = true;
                Task.Run(async () =>
                {
                    var result = new ItemsList<NamedValue>();
                    if (Type == (int)SearchType.DISTRICT)
                    {
                        result = await Server.GetHouseGroups();
                    }

                    IsBusy = false;
                    if (result.Error == null)
                    {
                        Items.Clear();
                        foreach (var item in result.Data)
                            Device.BeginInvokeOnMainThread(() => Items.Add(item));
                    }
                });
                
            });
            Filter = new Command<string>((text) =>
            {
                if (string.IsNullOrEmpty(text.ToString()))
                {
                    LoadResults.Execute(null);
                }
                else
                {
                    var filtered = Items.Where(x => x.Name.ToLower().Contains(text.ToString().ToLower())).ToList();
                    Items.Clear();
                    foreach (var item in filtered)
                    {
                        Items.Add(item);
                    }
                }
            });
        }
    }
}