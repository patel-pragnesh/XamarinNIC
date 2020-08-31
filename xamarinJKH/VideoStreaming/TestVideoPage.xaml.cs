using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace xamarinJKH.VideoStreaming
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TestVideoPage : ContentPage
    {
        public string TestUrl
        {
            get => "https://vs.domru.ru/translation?id=331972335&guid=dc3b372c7e421f19192a&mode=hls";
        }
        public TestVideoPage()
        {
            InitializeComponent();
            BindingContext = this;
        }
    }
}