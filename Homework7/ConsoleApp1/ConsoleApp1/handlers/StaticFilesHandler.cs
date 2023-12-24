using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MyHTTPServer.attributes;
using MyHTTPServer.config;
using MyHTTPServer.models;

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
        private static string GetContentType(HttpListenerRequest request) => MimeTypes[request.RawUrl!.Split(".").Last()];
        private static bool IsStaticFilesRequested(HttpListenerRequest request) => MimeTypes.Keys.Contains(request.RawUrl!.Split(".").Last());

        private static bool IsSourseExist(HttpListenerRequest request) 
        {
            Console.WriteLine(request.RawUrl);

            if (request.RawUrl!.EndsWith("/"))
            {
                if (File.Exists(HttpServer.GetAppSettings().StaticFilePath + url))
                    return true;
                url = request.RawUrl.TrimEnd() + ".html"; 
                if (File.Exists(HttpServer.GetAppSettings().StaticFilePath + url))
                    return true;
            }
            else if(IsStaticFilesRequested(request))
            {
                url = request.RawUrl;
                return File.Exists(HttpServer.GetAppSettings().StaticFilePath + url);
            }
            return false;
        }

        string ParseToProbably(string raw)
        {
            var rawArray=raw.Split('/');
            string result = HttpServer.GetAppSettings().StaticFilePath;
            string rawPath = String.Join('/',rawArray);
            result += rawPath;
            if (!rawPath.Contains('.') ) { result += "/index.html";  }
            return result;
        }


        string RewriteHtml(string html)
        {
            Assembly? htmlAssembly= Assembly.GetEntryAssembly();
            StringBuilder result = new StringBuilder();
            result.Append(html);
            result.Replace("", "");
            //string result = "Здравствуйте, @{name}. Вы прописаны по адресу @{address}";
            //var properties = obj.GetType().GetProperties();
            var strProps = result.ToString().Split('@', '}', ')').Where(s => s.StartsWith('{')||s.StartsWith('(')).ToArray();
            //var strExtras = result.ToString().Split('@', ')').Where(s => s.StartsWith('(')).ToArray();

            Type type = typeof(StaticFilesHandler);
            MethodInfo[] methods = type.GetMethods();
            for (int i=0;i<strProps.Length;i++)
            {
                var p = strProps[i];
                string prop, value="";
                if (p.StartsWith('{'))
                    prop = p.TrimStart();
                else
                    break;
                if (strProps[i + 1].StartsWith('('))
                { 
                    value = strProps[i + 1].TrimStart();
                    i++; 
                }
                //var prop = p.TrimStart();

                MethodInfo method = methods
                    .Select(m => new { Method = m, Attribute = m.GetCustomAttribute<HTMLAttribute>() })
                    .Where(m => m.Attribute != null && m.Attribute.ModelName == prop)
                    .Select(m => m.Method)
                    .FirstOrDefault()!;
                //var method= methods.Select(s=>s.GetCustomAttributes(typeof(HTMLAttribute), false).FirstOrDefault()).Where(m=>m.);
                //var method = htmlAssembly!.GetTypes()
                //    .Where(t => Attribute.IsDefined(t, typeof(HTMLAttribute)))
                //    .FirstOrDefault(c => (((HTMLAttribute)Attribute.GetCustomAttribute(c, typeof(HTMLAttribute))!)!)
                //        .HTMLName.Equals(prop, StringComparison.OrdinalIgnoreCase));

                object instance = Activator.CreateInstance(method.ReturnType);
                //var value = p.Where(p => p.ToString() == prop).ToArray().First().GetValue(obj).ToString();
                if (value != "") { 
                    object[] values=new object[1];
                    values[0] = value;
                    result.Replace($"@{{{prop}}}", (method.Invoke(instance, values)).ToString());
                }
                else
                {
                    result.Replace($"@{{{prop}}}", (method.Invoke(instance, null)).ToString());
                }

            }
            return result.ToString();
        }




        private bool GiveStaticFileResponse(HttpListenerContext context)
        {
            var response = context.Response;
            var request = context.Request;

            string filePath = HttpServer.GetAppSettings().StaticFilePath;

            if (request.RawUrl!.EndsWith("/"))
            {
                filePath +=request.RawUrl+"index.html";
            }
            else 
            { 
                filePath += request.RawUrl;
            }
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

            response.Close();
        }
    }
}
