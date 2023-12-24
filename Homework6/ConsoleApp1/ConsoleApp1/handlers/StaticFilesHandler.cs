using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MyHTTPServer.config;

namespace MyHTTPServer.handlers
{
    public class StaticFilesHandler : Handler
    {

        private static Dictionary<string, string> MimeTypes = new Dictionary<string, string>(){
            {"html", "text/html"},
            {"/",  "text/html"},
            {"css" , "text/css"},
            {"jpg" , "image/jpeg"},
            {"svg", "image/svg+xml"},
            {"png", "image/png"},
            {"ico", "image/x-icon"},
            {"woff", "font/woff"},
            {"woff2", "font/woff2"},
            {"ttf", "application/octet-stream"}
        };
        private static readonly AppSettings _serverConfig = HttpServer.GetAppSettings();

        private static string url = "index.html";

        async public override void HandleRequest(HttpListenerContext context)
        {
            //if (IsSourseExist(context.Request)) await GiveStaticFileResponse(context);
            if(!(GiveStaticFileResponse(context))&&Successor!=null)
            Successor.HandleRequest(context);
        }
        //private static string GetContentType(HttpListenerRequest request) => MimeTypes[request.RawUrl!.Split(".").Last()];
        private static bool IsStaticFilesRequested(HttpListenerRequest request) => MimeTypes.Keys.Contains(request.RawUrl!.Split(".").Last());


        string ParseToProbably(string raw)
        {
            var rawArray=raw.Split('/');
            string result = HttpServer.GetAppSettings().StaticFilePath;
            string rawPath = String.Join('/',rawArray);
            result += rawPath;
            if (!rawPath.Contains('.') ) { result += "/index.html";  }
            return result;
        }

        private bool GiveStaticFileResponse(HttpListenerContext context)
        {
            var response = context.Response;
            var request = context.Request;

            string filePath = HttpServer.GetAppSettings().StaticFilePath;

            filePath=ParseToProbably(request.RawUrl);
            Console.WriteLine(filePath);

            if (File.Exists(filePath))
            {
                Console.WriteLine("Finded source file");
                byte[] buffer = File.ReadAllBytes(filePath);
                response.ContentLength64 = buffer.Length;
                using Stream output = response.OutputStream;
                output.Write(buffer);
                output.Flush();
                Console.WriteLine("Request processed");
                return true;
            }
            else
            {
                Console.WriteLine("Can not find source file");
                return false;
            }

            //response.Close();
        }
    }
}
