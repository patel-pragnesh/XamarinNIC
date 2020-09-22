using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AiForms.Dialogs;
using AiForms.Dialogs.Abstractions;
using Plugin.Media.Abstractions;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PancakeView;
using Xamarin.Forms.Xaml;
using xamarinJKH.Apps;
using xamarinJKH.CustomRenderers;
using xamarinJKH.DialogViews;
using xamarinJKH.InterfacesIntegration;
using xamarinJKH.Server;
using xamarinJKH.Server.RequestModel;
using xamarinJKH.Utils;

namespace xamarinJKH.Tech
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TechSendPage : ContentPage
    {
        public Color hex { get; set; } = (Color) Application.Current.Resources["MainColor"];
        private RestClientMP server = new RestClientMP();

        public TechSendPage(bool isVisibleApp = true)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    int statusBarHeight = DependencyService.Get<IStatusBar>().GetHeight();
                    Pancake.Padding = new Thickness(0, statusBarHeight, 0, 0);
                    //BackgroundColor = Color.White;
                    break;
                default:
                    break;
            }

            UkName.Text = Settings.MobileSettings.main_name;
            StackLayoutApp.IsVisible = isVisibleApp;
            if (Settings.Person.IsDispatcher || !Settings.AppIsVisible)
            {
                StackLayoutApp.IsVisible = false;
            }
            EntryPhone.Text = Settings.Person.Phone;
            LabelInfo.Text =
               AppResources.TechAdditionalText1 +
                Settings.MobileSettings.main_name + AppResources.TechAdditionalText2;
            BtnApp.Text = AppResources.TechAdditionalText3 + Settings.MobileSettings.main_name;
            var backClick = new TapGestureRecognizer();
            backClick.Tapped += async (s, e) => { ClosePage(); };
            BackStackLayout.GestureRecognizers.Add(backClick);
            var appOpen = new TapGestureRecognizer();
            appOpen.Tapped += async (s, e) =>
            {
                if (isVisibleApp)
                {
                    if (Settings.Person.Accounts.Count > 0)
                    {
                        if (Settings.TypeApp.Count > 0)
                        {
                            await Navigation.PushAsync(new NewAppPage());
                        }
                        else
                        {
                            await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorAppsNoTypes, "OK");
                        }
                    }
                    else
                    {
                        await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorAppsNoIdent, "OK");
                    }
                }
            };
            FrameBtnApp.GestureRecognizers.Add(appOpen);

            var send = new TapGestureRecognizer();
            send.Tapped += async (s, e) =>
            {
                string phone = EntryPhone.Text
                    .Replace("+", "")
                    .Replace(" ", "")
                    .Replace("(", "")
                    .Replace(")", "")
                    .Replace("-", "");
                ;
                string email = EntryEmail.Text;

                string text = EntryText.Text;
                if (phone.Length < 11)
                {
                    await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorTechNumberFormat, "OK");
                    return;
                }

                if (email.Equals(""))
                {
                    await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorEnterEmail, "OK");
                    return;
                }

                if (text.Equals(""))
                {
                    await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorTechDesc, "OK");
                    return;
                }

                await SendTechTask();
            };
            FrameBtnLogin.GestureRecognizers.Add(send);
            Color hexColor = (Color) Application.Current.Resources["MainColor"];
            IconViewLogin.SetAppThemeColor(IconView.ForegroundProperty, hexColor, Color.White);
            Pancake.SetAppThemeColor(PancakeView.BorderColorProperty, hexColor, Color.Transparent);
            PancakeViewIcon.SetAppThemeColor(PancakeView.BorderColorProperty, hexColor, Color.Transparent);if (Device.RuntimePlatform == Device.iOS){ if (AppInfo.PackageName == "rom.best.saburovo" || AppInfo.PackageName == "sys_rom.ru.tsg_saburovo"){PancakeViewIcon.Padding = new Thickness(0);}}
            Frame.SetAppThemeColor(MaterialFrame.BorderColorProperty, hexColor, Color.White);
            BindingContext = this;
        }

        async Task SendTech()
        {
            string phone = EntryPhone.Text
                .Replace("+", "")
                .Replace(" ", "")
                .Replace("(", "")
                .Replace(")", "")
                .Replace("-", "");
            ;
            string email = EntryEmail.Text;

            string text = EntryText.Text;


            TechSupportAppealArguments arguments = new TechSupportAppealArguments();
            arguments.OS = Device.RuntimePlatform;
            arguments.Phone = phone;
            arguments.Text = text;
            arguments.Mail = email;
            arguments.AppVersion = Xamarin.Essentials.AppInfo.VersionString;
            arguments.Info = GetIdent();
            arguments.Address = GetAdres();
            arguments.Login = Settings.Person.Login;

            TechId result = await server.TechSupportAppeal(arguments);
            if (result.Error == null)
            {
                await DisplayAlert(AppResources.AlertSuccess, AppResources.TechSendSuccess, "OK");
                ClosePage();
            }
            else
            {
                await DisplayAlert(AppResources.ErrorTitle, result.Error, "OK");
            }
        }

        string GetAdres()
        {
            try
            {
                return Settings.Person.Accounts[0].Address;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        string GetIdent()
        {
            try
            {
                return Settings.Person.Accounts[0].Ident;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public async Task SendTechTask()
        {
            // Loading settings
            Configurations.LoadingConfig = new LoadingConfig
            {
                IndicatorColor = hex,
                OverlayColor = Color.Black,
                Opacity = 0.8,
                DefaultMessage = AppResources.Sending,
            };

            await Loading.Instance.StartAsync(async progress =>
            {
                // some heavy process.
                await SendTech();
            });
        }

        void ClosePage()
        {
            try
            {
                Navigation.PopAsync();
            }
            catch (Exception e)
            {
                Navigation.PopModalAsync();
            }
        }
    }
}