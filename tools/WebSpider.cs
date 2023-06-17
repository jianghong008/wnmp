using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;

namespace wnmp.tools
{
    internal class WebSpider
    {
        private static string nginxWebSite = "http://nginx.org";

        public static void GetNginxVersions()
        {
            HttpWebRequest req = HttpWebRequest.CreateHttp("http://nginx.org/en/download.html");
            req.Method = "Get";
            WebResponse res = req.GetResponse();
            Stream s = res.GetResponseStream();
            StreamReader readStream = new StreamReader(s, Encoding.UTF8);
            
            Trace.WriteLine(readStream.ReadToEnd());
            readStream.Close();
            s.Close();
        }
    }
}
