using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Xamarin.Forms;
using xamarinJKH.Droid.CustomReader;
using xamarinJKH.InterfacesIntegration;

using Java.IO;
using Android.Print;
using System.IO;

[assembly:Dependency(typeof(CustomPrintManager))]
namespace xamarinJKH.Droid.CustomReader
{
    public class CustomPrintManager : IPrintManager
    {
        public void SendFileToPrint(byte[] content)
        {
            //Android print code goes here
            Stream inputStream = new MemoryStream(content);
            string fileName = "form.pdf";
            if (inputStream.CanSeek)
                //Reset the position of PDF document stream to be printed
                inputStream.Position = 0;
            //Create a new file in the Personal folder with the given name
            string createdFilePath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), fileName);
            //Save the stream to the created file
            using (var dest = System.IO.File.OpenWrite(createdFilePath))
                inputStream.CopyTo(dest);
            string filePath = createdFilePath;
            PrintManager printManager = (PrintManager)Forms.Context.GetSystemService(Context.PrintService);
            PrintDocumentAdapter pda = new PrintAdapter(filePath);
            //Print with null PrintAttributes
            printManager.Print(fileName, pda, null);
        }
    }
}