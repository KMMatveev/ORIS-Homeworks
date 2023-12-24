using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MyHTTPServer.config;
using MyHTTPServer.attributes;
using MyHTTPServer.models;

namespace MyHTTPServer.handlers
{
    internal class ControllerHandler:Handler
    {
        private Assembly? _controllerAssembly;
        
        async public override void HandleRequest(HttpListenerContext context)
        {
            //try 
            { 
                var strParams = context.Request.Url!
                    .Segments
                    .Skip(1)
                    .Select(s => s.Replace("/", ""))
                    .ToArray();
 
                var controllerName = strParams[0];
                var methodName = strParams[1];
                
                //var objName= strParams[2];

                _controllerAssembly = Assembly.GetEntryAssembly();

                var controller = _controllerAssembly!.GetTypes()
                    .Where(t => Attribute.IsDefined(t, typeof(HttpController)))
                    .FirstOrDefault(c => (((HttpController)Attribute.GetCustomAttribute(c, typeof(HttpController))!)!)
                        .ControllerName.Equals(controllerName + "controller", StringComparison.OrdinalIgnoreCase));

                if (controller == null) throw new ArgumentException("null controller");//Successor.HandleRequest(context);//throw new ArgumentException("null controller");


                var method = controller.GetMethods()
                    .Where(x => x.GetCustomAttributes(true)
                        .Any(attr => attr.GetType().Name.Equals($"{context.Request.HttpMethod}Attribute",
                            StringComparison.OrdinalIgnoreCase)))
                    .FirstOrDefault(m => m.Name.Equals(methodName, StringComparison.OrdinalIgnoreCase));

                if (method == null) throw new ArgumentException("null method");

                var strPar = ParseRequest(context.Request).Result;

                var queryParams = Array.Empty<object>();
                var objAssembly = Assembly.GetEntryAssembly();

                if (strPar.Length > 0)
                {
                    if (controllerName == "Form") 
                    {
                        //var obj = _controllerAssembly!.GetTypes()
                        //.Where(t => Attribute.IsDefined(t, typeof(HttpController)))
                        //.FirstOrDefault(c => (((HttpController)Attribute.GetCustomAttribute(c, typeof(HttpController))!)!)
                        //.ControllerName.Equals(objName, StringComparison.OrdinalIgnoreCase));
                        var formConstructor = typeof(Form).GetConstructors().FirstOrDefault();
                        //var answerConstructor = method.GetParameters().First().GetType().GetConstructors().First();//(BindingFlags.Instance | BindingFlags.Public, (Type[])strParams.Select(s => s.GetType()).ToArray());//getConstructor();
                        var answer = formConstructor.Invoke(strPar);
                        var paramArray = new object[1];
                        paramArray[0] = answer;
                        queryParams = paramArray;//method.GetParameters()
                        //.Select((p, i) => Convert.ChangeType(strParams[i], p.ParameterType))
                        //.ToArray();
                    }
                    else
                    {
                        throw new ArgumentException("Unknown controller");
                    }

                }

                var ret = method.Invoke(Activator.CreateInstance(controller), queryParams);
                byte[]? responseBuffer = Array.Empty<byte>();

                if (ret is string)
                    responseBuffer = Encoding.UTF8.GetBytes((ret as string)!);
                else if (!(ret is null))
                {
                    var serializeObj = JsonSerializer.Serialize(ret);
                    responseBuffer = Encoding.UTF8.GetBytes(serializeObj);
                }

                var response = context.Response;

                if (responseBuffer.Length > 0)
                {
                    response.ContentLength64 = responseBuffer.Length;
                    using Stream output = response.OutputStream;
                    await output.WriteAsync(responseBuffer);
                    await output.FlushAsync();
                    output.Close();
                }

                if (context.Request.HttpMethod.Equals("post", StringComparison.OrdinalIgnoreCase))
                {
                    var _serverConfig = HttpServer.GetAppSettings();
                    response.Redirect($"{_serverConfig.Address}:{_serverConfig.Port}/");
                }
                response.Close();
            }
            //catch { Successor.HandleRequest(context); }
        }

        async private Task<string[]> ParseRequest(HttpListenerRequest request)
        {
            if (!request.HasEntityBody)
                return Array.Empty<string>();

            var stream = new StreamReader(request.InputStream);
            var requestData = await stream.ReadToEndAsync();
            requestData = Uri.UnescapeDataString(Regex.Unescape(requestData));
            requestData = requestData.Replace("&", "\n");
            requestData = requestData.Replace("=", ": ");
            requestData = requestData.Replace("+", " ");
            var array = requestData.Split('\n', ':').ToArray();

            var classData = array.Where(val => Array.IndexOf(array, val) % 2 == 1).ToArray();
            return classData;
        }
    }
}
