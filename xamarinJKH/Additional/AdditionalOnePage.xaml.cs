using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xamarinJKH.Apps;
using xamarinJKH.Server;
using xamarinJKH.Server.RequestModel;
using xamarinJKH.Utils;

namespace xamarinJKH.Additional
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AdditionalOnePage : ContentPage
    {
        private AdditionalService additionalService;
        private RestClientMP _server = new RestClientMP();

        public AdditionalOnePage(AdditionalService additionalService)
        {
            this.additionalService = additionalService;
            InitializeComponent();
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    BackgroundColor = Color.White;
                    // ImageTop.Margin = new Thickness(0, 0, 0, 0);
                    // StackLayout.Margin = new Thickness(0, 33, 0, 0);
                    // IconViewNameUk.Margin = new Thickness(0, 33, 0, 0);
                    break;
                case Device.Android:
                default:
                    break;
            }
            NavigationPage.SetHasNavigationBar(this, false);
            var backClick = new TapGestureRecognizer();
            backClick.Tapped += async (s, e) => { _ = await Navigation.PopAsync(); };
            BackStackLayout.GestureRecognizers.Add(backClick);
            SetText();
        }

        async void SetText()
        {
            UkName.Text = Settings.MobileSettings.main_name;
            LabelPhone.Text = "+" + Settings.Person.Phone;

            if (additionalService.Name != null && !additionalService.Name.Equals(""))
            {
                LabelTitle.Text = additionalService.Name;
            }
            else
            {
                LabelDesc.IsVisible = false;
            }

            if (additionalService.Address != null && !additionalService.Address.Equals(""))
            {
                LabelAdress.Text = additionalService.Address;
            }
            else
            {
                LabelAdress.IsVisible = false;
            }

            if (additionalService.Description != null && !additionalService.Description.Equals(""))
            {
                LabelDesc.Text = additionalService.Description;
            }
            else
            {
                LabelDesc.IsVisible = false;
            }

            byte[] imageByte = await _server.GetPhotoAdditional(additionalService.ID.ToString());
            Stream stream = new MemoryStream(imageByte);
            ImageAdd.Source = ImageSource.FromStream(() => { return stream; });
            FrameBtnQuest.BackgroundColor = Color.FromHex(Settings.MobileSettings.color);
            if (additionalService.CanBeOrdered)
            {
                FrameBtnQuest.IsVisible = true;
            }
        }


        private async void ButtonClick(object sender, EventArgs e)
        {
            FrameBtnQuest.IsVisible = false;
            progress.IsVisible = true;
            if (Settings.Person.Accounts.Count > 0)
            {
                IDResult result = await _server.newApp(Settings.Person.Accounts[0].Ident,
                    additionalService.id_RequestType.ToString(),
                    "Ваш заказ принят. В ближайшее время сотрудник свяжется с Вами для уточнения деталей\n" +
                    additionalService.Description);
                if (result.Error == null)
                {
                    RequestsUpdate requestsUpdate =
                        await _server.GetRequestsUpdates(Settings.UpdateKey, result.ID.ToString());
                    if (requestsUpdate.Error == null)
                    {
                        Settings.UpdateKey = requestsUpdate.NewUpdateKey;
                    }

                    await DisplayAlert("Успешно", "Заказ успешно оформлен", "OK");
                    RequestInfo requestInfo = new RequestInfo();
                    requestInfo.ID = result.ID;
                    await Navigation.PushAsync(new AppPage(requestInfo, true));
                }
                else
                {
                    await DisplayAlert("Ошибка", result.Error, "OK");
                }
            }
            else
            {
                await DisplayAlert("Ошибка", "Подключите лицевой счет", "OK");
            }
            FrameBtnQuest.IsVisible = true;
            progress.IsVisible = false;
        }
    }
}