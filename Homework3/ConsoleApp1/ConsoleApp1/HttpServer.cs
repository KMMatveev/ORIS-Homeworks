using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Numerics;
using System.Xml.Linq;
using System.Net.Mail;

namespace ConsoleApp2
{
    public class AppSettings
    {
        public int Port { get; set; }
        public string Address { get; set; }
    }

    public class HttpServer
    {
        private HttpListener server { get; set; }
        private AppSettings appSettings { get; set; }
        private bool stop {  get; set; }

        public HttpServer()
        {
            this.server =new HttpListener();
            stop = false;
            string appSettingsPath = @".\\appsetting.json";
            if (File.Exists(appSettingsPath))
            {
                var json = File.ReadAllText(appSettingsPath);
                appSettings = JsonSerializer.Deserialize<AppSettings>(json);
                var prefix = $"{appSettings.Address}:{appSettings.Port}/connection/";
                Console.WriteLine($"Connected {prefix}");
                server.Prefixes.Add(prefix);
            }
            else
            {
                Console.WriteLine("Can not open the file \"appseting.json\"!");
                stop = true;
                return;
            }

        }

        async Task RequestHandler()
        {
            var context = await server.GetContextAsync();
            var request = context.Request;
            var response = context.Response;
            string url = (request.RawUrl).Substring(12);
            url="dodo.html";

            string filePath = url;
            Console.WriteLine(url);
            //if (request.RawUrl == null)
            //else { filePath = request.RawUrl; }
            
            if (File.Exists(filePath))
            {
                Console.WriteLine("Finded source file");
                byte[] buffer = File.ReadAllBytes(filePath);
                response.ContentLength64 = buffer.Length;
                using Stream output = response.OutputStream;
                await output.WriteAsync(buffer);
                await output.FlushAsync();
                Console.WriteLine("Request processed");
            }
            else
            {
                Console.WriteLine("Can not find source file");
            }
        }


        public async Task StartServer()
        {
            server.Start();
            Console.WriteLine("Server started, write \"stop\" to stop server.");

            Task.Run(() =>
            {
                while (true)
                {
                    string consoleInput = Console.ReadLine();
                    if (consoleInput == "stop")
                    {
                        Console.WriteLine("Stopping.");
                        stop = true;
                        server.Stop();
                        break;
                    }
                }
            });

            while(!stop)
            {
                await RequestHandler();
            }
            //server.Stop();
            Console.WriteLine("Server stopped.");
            //Console.ReadLine();
        }
    }
}