using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Linq;
using xamarinJKH.InterfacesIntegration;

[assembly: Dependency(typeof(xamarinJKH.iOS.FileWorker))]
namespace xamarinJKH.iOS
{
    public class FileWorker : IFileWorker
    {
        public Task DeleteAsync(string filename)
        {
            File.Delete(GetFilePath(filename));
            return Task.FromResult(true);
        }
 
        public Task<bool> ExistsAsync(string filename)
        {
            string filepath = GetFilePath(filename);
            bool exists = File.Exists(filepath);
            return Task<bool>.FromResult(exists);
        }
 
        public Task<IEnumerable<string>> GetFilesAsync()
        {
            IEnumerable<string> filenames = from filepath in Directory.EnumerateFiles(GetDocsPath())
                select Path.GetFileName(filepath);
            return Task<IEnumerable<string>>.FromResult(filenames);
        }
 
        public async Task<string> LoadTextAsync(string filename)
        {
            string filepath = GetFilePath(filename);
            using (StreamReader reader = File.OpenText(filepath))
            {
                return await reader.ReadToEndAsync();
            }
        }
 
        public async Task SaveTextAsync(string filename, byte[] text)
        {
            string filepath = GetFilePath(filename);
            using (FileStream writer = new FileStream(filepath, FileMode.OpenOrCreate))
            {
                await writer.WriteAsync(text);
            }
        }

        public string GetFilePath(string filename)
        {
            return Path.Combine(GetDocsPath(), filename);
        }
        string GetDocsPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }
    }
}