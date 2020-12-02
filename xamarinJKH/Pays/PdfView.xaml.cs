using Syncfusion.Pdf;
using Syncfusion.SfPdfViewer.XForms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AppCenter.Analytics;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamarinJKH.ViewModels;

using Syncfusion.SfPdfViewer.XForms;
using Syncfusion.SfPdfViewer;
using Syncfusion.Pdf;

namespace xamarinJKH.Pays
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PdfView : ContentPage
    {
        FileStream Stream { get; set; }
        PayViewModel viewModel { get; set; }
        public PdfView(string filename, string id, bool insurance = false)
        {
            InitializeComponent();
            
            
            BindingContext = viewModel = new PayViewModel(filename, id, insurance);
            //((NavigationPage)Application.Current.MainPage).BarBackgroundColor = Color.White;
            MessagingCenter.Subscribe<Object, FileStream>(this, "SetFileStream", (sender, Stream) =>
            {
                Analytics.TrackEvent("Инициализация pdfView");
                var Viewer = new SfPdfViewer();
                
                Viewer.InputFileStream = Stream;
                Viewer.VerticalOptions = LayoutOptions.FillAndExpand;
                Viewer.BackgroundColor = Color.Black;
                Viewer.PageViewMode = PageViewMode.Continuous;
                Viewer.ShowPageNumber = false;
                Viewer.IsTextSearchEnabled = false;
                Viewer.Toolbar.SetToolbarItemVisibility("search", false);
                Viewer.Toolbar.SetToolbarItemVisibility("page-view-mode", false);
                Viewer.Toolbar.SetToolbarItemVisibility("bookmark", false);
                Viewer.Toolbar.SetToolbarItemVisibility("annotation", false);
                ContentStack.Children.Add(Viewer);
            });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            viewModel.LoadFile.Execute(null);
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            try
            {
                ContentStack.Children.Remove(ContentStack.Children.FirstOrDefault(x => x is SfPdfViewer));

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

}