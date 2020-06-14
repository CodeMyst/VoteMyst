using System.IO;
using System.Web;
using System.Net;

namespace VoteMyst 
{
    public static class DownloadHelper
    {
        public static void DownloadFile(string sourceUrl, string destinationPath)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));
            using (var client = new WebClient())
            {
                client.DownloadFile(sourceUrl, destinationPath);
            }
        }
    }
}