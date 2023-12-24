using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHTTPServer.attributes
{
    public class HTMLAttribute : Attribute, IHttpMethodAttribute
    {
        public HTMLAttribute(string name)
        {
            ModelName = name;
        }

        public string ModelName { get; }
    }
}
