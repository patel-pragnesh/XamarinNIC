using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

using Foundation;
using UIKit;
using xamarinJKH.InterfacesIntegration;
using Xamarin.Forms;
using DependencyAttribute = Xamarin.Forms.DependencyAttribute;
using xamarinJKH.iOS.DependencyImplementation;
using System.IO;

[assembly: Dependency(typeof(PrintService))]
namespace xamarinJKH.iOS.DependencyImplementation
{
    public class PrintService : IPrintManager
    {
        public void SendFileToPrint(byte[] content)
        {
            Stream file = new MemoryStream(content);



            var printInfo = UIPrintInfo.PrintInfo;
            printInfo.OutputType = UIPrintInfoOutputType.General;
            printInfo.JobName = "Print PDF";

            //Get the path of the MyDocuments folder
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            //Get the path of the Library folder within the MyDocuments folder
            var library = Path.Combine(documents, "..", "Library");
            //Create a new file with the input file name in the Library folder
            var filepath = Path.Combine(library, "PrintSampleFile");

            //Write the contents of the input file to the newly created file
            using (MemoryStream tempStream = new MemoryStream())
            {
                file.Position = 0;
                file.CopyTo(tempStream);
                File.WriteAllBytes(filepath, tempStream.ToArray());
            }

            var printer = UIPrintInteractionController.SharedPrintController;
            printInfo.OutputType = UIPrintInfoOutputType.General;

            printer.PrintingItem = NSUrl.FromFilename(filepath);
            printer.PrintInfo = printInfo;


            printer.ShowsPageRange = true;

            printer.Present(true, (handler, completed, err) => {
                if (!completed && err != null)
                {
                    Console.WriteLine("error");
                }
            });
            file.Dispose();
            //return true;
        }
    }
}