using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MyHTTPServer
{
    class Program
    {
        static async Task Main()
        {
            HttpServer server = new HttpServer();
            await server.StartServer();

            Console.ReadLine();

        }
    }
}