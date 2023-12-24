using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHTTPServer.config
{
    public class AppSettings
    {
        public int Port { get; set; }
        public string Address { get; set; }
        public string StaticFilePath { get; set; }
        public string EmailAdress { get; set; }
        public string EmailPassword { get; set; }
    }
}
