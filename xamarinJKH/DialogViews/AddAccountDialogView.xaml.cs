using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using AiForms.Dialogs.Abstractions;
using AiForms.Dialogs;

using xamarinJKH.Server;
using xamarinJKH.Server.RequestModel;
using xamarinJKH;
using xamarinJKH.InterfacesIntegration;
using xamarinJKH.Utils;

namespace xamarinJKH.DialogViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddAccountDialogView : DialogView
    {
        public AddAccountDialogView()
        {
            InitializeComponent();
            BindingContext = new AddAccountDialogViewModel(this);
        }

        public void CloseDialog()
        {
            DialogNotifier.Cancel();
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            CloseDialog();
        }
    }

    public class AddAccountDialogViewModel : xamarinJKH.ViewModels.BaseViewModel
    {
        private AddAccountDialogView accountDialogView;
        public AddAccountDialogViewModel(AddAccountDialogView accountDialogView)
        {
            this.accountDialogView = accountDialogView;
        }

        public Color ButtonColor
        {
            get => Color.FromHex(xamarinJKH.Utils.Settings.MobileSettings.color);
        }
        bool _progress;
        public bool Progress 
        {
            get => _progress;
            set
            {
                _progress = value;
                OnPropertyChanged("Progress");
            }
        }
        bool _btn;
        public bool Button
        {
            get => _btn;
            set
            {
                _btn = value;
                OnPropertyChanged("Button");
            }
        }

        public Server.RestClientMP _server => new Server.RestClientMP();
        readonly INavigation Navigation;

        public Command AddIdent
        {
            get => new Command<string>(async (ident) =>
            {
                if (ident != "")
                {
                    Progress = true;
                    Button = false;
                    Progress = true;
                    AddAccountResult result = await _server.AddIdent(ident, true);
                    if (result.Error == null)
                    {
                        Console.WriteLine(result.Address);
                        Console.WriteLine("Отправлено");
                        bool answer = await Application.Current.MainPage.DisplayAlert("Проверьте правильность адреса", result.Address, "Добавить лицевой счет", "Отмена");
                        if (answer)
                        {
                           
                            AcceptAddIdentAccount(ident, Settings.Person.Email);
                        }
                        else
                        {
                            Button = true;
                            Progress = false;
                        }
                    }
                    else
                    {
                        Console.WriteLine("---ОШИБКА---");
                        Console.WriteLine(result.ToString());
                        Button = true;
                        Progress = false;
                        if (Device.RuntimePlatform == Device.iOS)
                        {
                            await Application.Current.MainPage.DisplayAlert("ОШИБКА", result.Error, "OK");
                        }
                        else
                        {
                            DependencyService.Get<IMessage>().ShortAlert(result.Error);
                        }
                    }

                    Progress = false;
                    Button = true;
                }
                else
                {
                    if (ident == "")
                    {
                        await Application.Current.MainPage.DisplayAlert("", "Заполните номер счета", "OK");
                    }
                }
            });
        }
        public async void AcceptAddIdentAccount(string ident, string email)
        {
            if (ident != "")
            {
                Progress = true;
                Button = false;
                Progress = true;
                AddAccountResult result = await _server.AddIdent(ident);
                if (result.Error == null)
                {
                    Console.WriteLine(result.Address);
                    Console.WriteLine("Отправлено");
                    await Application.Current.MainPage.DisplayAlert("", "Лс/ч " + ident + " успешно подключён", "ОК");
                    Button = true;
                    Progress = false;

                    accountDialogView.CloseDialog();
                    //_paysPage.RefreshPaysData();
                }
                else
                {
                    Console.WriteLine("---ОШИБКА---");
                    Console.WriteLine(result.ToString());
                    Button = true;
                    Progress = false;
                    if (Device.RuntimePlatform == Device.iOS)
                    {
                        await Application.Current.MainPage.DisplayAlert("ОШИБКА", result.Error, "OK");
                    }
                    else
                    {
                        DependencyService.Get<IMessage>().ShortAlert(result.Error);
                    }
                }

                Progress = false;
                Button = true;
            }
            else
            {
                if (ident == "")
                {
                    await Application.Current.MainPage.DisplayAlert("", "Заполните номер счета", "OK");
                }
            }
        }
    }

}