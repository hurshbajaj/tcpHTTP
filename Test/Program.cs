using System;
using HTTPbuilder;

namespace test
{
    public class Program
    {
        public static async Task Main()
        {
            //GET/abc http/1.1
            
            var app = new HTTPbuild(new HTTPbuild.buildARG(5000, null));
            await app.listen();
            
            app.handle("GET", "abc", handle);
            app.handle("POST", "abc", handle);

            await app.appendHandlers();
        }
        static void handle(object req, out Dictionary<string, string> res)
                    {
                        res = HTTPbuild.newRes();
                        res["status"] = "200 OK";
                        res["newProp"] = "Valid";
                        res["body"] = "<html><body><h1>Hello, World!</h1></body></html>";
                    }
        
    }
}

