using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xamarin.Forms;
using xamarinJKH.InterfacesIntegration;
using AiForms.Dialogs.Abstractions;

namespace xamarinJKH.ViewModels
{
    public class PayViewModel : BaseViewModel
    {
        public FileStream Stream { get; set; }
        string filename;
        string ID;
        public Command LoadFile { get; set; }
        public PayViewModel(string file, string ID)
        {
            filename = file;
            this.ID = ID;
            LoadFile = new Command(() =>
            {
                System.Threading.Tasks.Task.Run(async () =>
                {
                    Device.BeginInvokeOnMainThread(() => IsBusy = true);
                    if (await DependencyService.Get<IFileWorker>().ExistsAsync(filename))
                    {
                        IsBusy = false;
                    }
                    else
                    {
                        byte[] stream;
                        stream = await Server.DownloadFileAsync(ID);
                        if (stream != null)
                        {
                            await DependencyService.Get<IFileWorker>().SaveTextAsync(filename, stream);
                            //await Launcher.OpenAsync(new OpenFileRequest
                            //{
                            //    File = new ReadOnlyFile(DependencyService.Get<IFileWorker>().GetFilePath(fileName))
                            //});
                        }
                        else
                        {
                            base.ShowError("Не удалось скачать файл");
                        }
                    }
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        Stream = File.OpenRead(DependencyService.Get<IFileWorker>().GetFilePath(filename));
                        await System.Threading.Tasks.Task.Delay(TimeSpan.FromMilliseconds(200));
                        MessagingCenter.Send<Object, FileStream>(this, "SetFileStream", Stream);
                        IsBusy = false;
                    });
                });
                
            });
        }
    }
}
