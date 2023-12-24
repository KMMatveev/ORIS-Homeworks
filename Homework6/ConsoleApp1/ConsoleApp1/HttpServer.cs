using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Numerics;
using System.Xml.Linq;
using MyHTTPServer.config;
using MyHTTPServer.handlers;

namespace MyHTTPServer
{

    public class HttpServer
    {
        private HttpListener server { get; set; }
        private static AppSettings _appSettings { get; set; }
        private bool stop {  get; set; }

        public static AppSettings GetAppSettings()
        {
            return _appSettings;
        }

        public HttpServer()
        {
            this.server =new HttpListener();
            stop = false;
            string appSettingsPath = @".\\appsetting.json";
            if (File.Exists(appSettingsPath))
            {
                var json = File.ReadAllText(appSettingsPath);
                _appSettings = JsonSerializer.Deserialize<AppSettings>(json);
                var prefix = $"{_appSettings.Address}:{_appSettings.Port}/";
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

        //async Task RequestHandler()
        //{
            
        //}

        

        public async Task StartServer()
        {
            string staticPath =$@"{_appSettings.StaticFilePath}";
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
                        //server.GetContext();
                        //server.Stop();
                        break;
                    }
                }
            });

            if (Directory.Exists(staticPath))
            {
                Console.WriteLine("find directory");
                Handler staticFilesHandler = new StaticFilesHandler();
                Handler errorHandler = new ErrorHandler();
                staticFilesHandler.Successor = errorHandler;
                while (!stop)
                {
                    var context = await server.GetContextAsync();
                    staticFilesHandler.HandleRequest(context);
                }
            }
            else
            {
                Console.WriteLine("Can not find folder \"static\"!");
                Console.WriteLine("Creating folder \"static\".");
                Directory.CreateDirectory(staticPath);
            }
            server.Stop();
            Console.WriteLine("Server stopped.");
            //Console.ReadLine();
        }
    }
}