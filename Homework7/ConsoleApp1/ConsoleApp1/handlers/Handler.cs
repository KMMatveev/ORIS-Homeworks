using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace MyHTTPServer.handlers
{
    public abstract class Handler
    {
        public Handler? Successor { get; set; }
        public abstract void HandleRequest(HttpListenerContext context);
    }
}
