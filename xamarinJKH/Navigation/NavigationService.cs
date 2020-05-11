using System.Threading.Tasks;
using Xamarin.Forms;

namespace xamarinJKH.Navigation
{
    public class NavigationService
    {
        private static Application CurrentApplication
        {
            get { return Application.Current; }
        }

        public static async Task NavigateToAsync(Page page)
        {
            if (CurrentApplication.MainPage is NavigationPage navigationPage)
            {
                await navigationPage.Navigation.PushAsync(page);
            }
        }
    }
}