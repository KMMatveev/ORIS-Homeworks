using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyHTTPServer.config;
using MyHTTPServer.models;

namespace MyHTTPServer.services
{
    public interface IEmailSenderService
    {
        Task SendEmailAsync(Form form);
    }
}
