using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace FTPIP
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Írd be az új hosztot: ");
            string host = int.Parse(Console.ReadLine()).ToString();

            Console.Write("\nFTP felhasználónév: ");
            string username = Console.ReadLine();
            Console.Write("FTP jelszó: ");
            string password = Console.ReadLine();
            NetworkCredential login = new NetworkCredential(username, password);

            //string pass = string.Empty;
            //ConsoleKey key;
            //do
            //{
            //    var keyInfo = Console.ReadKey(intercept: true);
            //    key = keyInfo.Key;

            //    if (key == ConsoleKey.Backspace && pass.Length > 0)
            //    {
            //        Console.Write("\b \b");
            //        pass = pass[0..^1];
            //    }
            //    else if (!char.IsControl(keyInfo.KeyChar))
            //    {
            //        Console.Write("*");
            //        pass += keyInfo.KeyChar;
            //    }
            //} while (key != ConsoleKey.Enter);

            FtpWebRequest checkReq = (FtpWebRequest)WebRequest.Create(@"ftp://ftp.nethely.hu/main/vrcams/hostip.txt");
            checkReq.Credentials = login;
            checkReq.Method = WebRequestMethods.Ftp.GetFileSize;

            try { FtpWebResponse checkRes = (FtpWebResponse)checkReq.GetResponse(); }
            catch (WebException e)
            {
                FtpWebResponse checkRes = (FtpWebResponse)e.Response;
                if (checkRes.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable) goto UPLOADNEWFILE;
            }

            // delete the original file
            FtpWebRequest delRequest = (FtpWebRequest)WebRequest.Create(@"ftp://ftp.nethely.hu/main/vrcams/hostip.txt");
            delRequest.Method = WebRequestMethods.Ftp.DeleteFile;

            delRequest.Credentials = login;

            using (FtpWebResponse response = (FtpWebResponse)delRequest.GetResponse())
               Console.WriteLine($"Törlés kész. Státusz: {response.StatusDescription}");

            // upload the new file
            UPLOADNEWFILE:
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(@"ftp://ftp.nethely.hu/main/vrcams/hostip.txt");
            request.Method = WebRequestMethods.Ftp.UploadFile;

            request.Credentials = login;

            byte[] fileContents = Encoding.UTF8.GetBytes(host);
            request.ContentLength = fileContents.Length;

            using (Stream requestStream = request.GetRequestStream())
                requestStream.Write(fileContents, 0, fileContents.Length);
            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                Console.WriteLine($"Feltöltés kész. Státusz: {response.StatusDescription}");

            Console.ReadKey(true);
        }
    }
}
