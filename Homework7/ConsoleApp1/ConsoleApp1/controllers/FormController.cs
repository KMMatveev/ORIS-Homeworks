using MyHTTPServer.attributes;
using MyHTTPServer.services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using MyHTTPServer.models;

namespace MyHTTPServer.controllers
{
    [HttpController("FormController")]
    public class FormController
    {
        [Post("SendForm")]
        async public void SendForm(Form objForm)
        {
            await new EmailSenderService().SendEmailAsync(objForm);//(HttpServer.GetAppSettings(),objForm);
        }

        [Get("SendForm2")]
        public string SendForm2()
        {
            var htmlCode = "<html><head></head><body>Hi SendForm2</body></html>";
            return htmlCode;
        }

    }
}
