using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MyHTTPServer.handlers
{
    internal class ErrorHandler:Handler
    {
        async public override void HandleRequest(HttpListenerContext context)
        {
            var response = context.Response;
            //var request = context.Request;
            string filePath = HttpServer.GetAppSettings().StaticFilePath+"error404.html";
            byte[] buffer = File.ReadAllBytes(filePath);
            response.ContentLength64 = buffer.Length;
            using Stream output = response.OutputStream;
            await output.WriteAsync(buffer);
            await output.FlushAsync();
            Console.WriteLine("Error 404");
            Console.WriteLine("Request processed");
            response.Close();
        }
    }
}
