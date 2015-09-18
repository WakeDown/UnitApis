using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;

namespace AdSyncronyzer
{
    class Program
    {
        static void Main(string[] args)
        {
            Uri uri = new Uri(ConfigurationManager.AppSettings["webApiUri"]);
            Synchronyze(uri);
        }

        private static void Synchronyze(Uri uri)
        {
            Write2Log("Start");
            string result = String.Empty;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);

            try
            {
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    result = reader.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                WebResponse errorResponse = ex.Response;
                using (Stream responseStream = errorResponse.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
                    String errorText = reader.ReadToEnd();
                    Write2Log("ERROR: " + errorText);
                }
            }
            Write2Log("End");
            //Uri uriEx = new Uri(ConfigurationManager.AppSettings["exceptionUri"]);
            //HttpWebRequest requestEx = (HttpWebRequest)WebRequest.Create(uriEx);
        }

        public static void Write2Log(string text)
        {
            text = String.Format("{1}: {0}", text, DateTime.Now);

            string path = Path.Combine(Directory.GetCurrentDirectory(), "log.txt");
            if (!File.Exists(path))
            {
               using(File.Create(path))
               { }
                    TextWriter tw = new StreamWriter(path);
                    tw.WriteLine(text);
                    tw.Close();
                
            }
            else if (File.Exists(path))
            {
                TextWriter tw = new StreamWriter(path, true);
                tw.WriteLine(text);
                tw.Close();
            }
        }
    }
}
