using System;
using HTTPbuilder;

namespace test
{
    public class Program
    {
        public static void Main()
        {
            var args = new HTTPbuild.buildARG(5000, null);
            var app = new HTTPbuild(args);
            
            app.listen();

            app.handle("GET", "abc", handle);

            static void handle(object req, out Dictionary<string, string> res)
            {
                res = HTTPbuild.newRes();
                res["status"] = "200 OK";  
                res["body"] = "<html><body><h1>Hello, World!</h1></body></html>";
            }
            
        }
    }
}

