using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using Newtonsoft.Json;

namespace Stuff.Objects
{
    public class DbModel
    {
        public const string OdataServiceUri = "http://uiis-1:10002/odata";

        private static string AuthorizationHeaderValue
        {
            get
            {
                String username = "UN1T\\sqlUnit_prog";
                String password = "1qazXSW@";
                String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));
                return "Basic " + encoded;
            }
        }

        protected static string GetJson(Uri uri)
        {
            string result = String.Empty;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.ContentType = "application/json";
            request.Headers.Add("Authorization", AuthorizationHeaderValue);

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
                }
                throw;
            }

            return result;
        }

        protected static bool PostJson(Uri uri, string json, out ResponseMessage responseMessage)
        {
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.Headers.Add("Authorization", AuthorizationHeaderValue);
            request.ContentType = "text/json";
            request.Method = "POST";

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var response = (HttpWebResponse)request.GetResponse();
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    string responseContent = streamReader.ReadToEnd();
                    responseMessage = JsonConvert.DeserializeObject<ResponseMessage>(responseContent);
                }
            
            return response.StatusCode == HttpStatusCode.Created;
        }

        ////protected static byte[] GetImage(Uri uri)
        ////{
        ////    var request = (HttpWebRequest)WebRequest.Create(uri);
        ////    request.Headers.Add("Authorization", AuthorizationHeaderValue);
        ////    request.ContentType = "image/gif";
        ////    request.Method = "GET";

        ////    //using (var streamWriter = new StreamWriter(request.GetRequestStream()))
        ////    //{
        ////    //    streamWriter.Flush();
        ////    //    streamWriter.Close();
        ////    //}
        ////    byte[] responseContent;
        ////    var response = (HttpWebResponse)request.GetResponse();
        ////    using (var streamReader = new StreamReader(response.GetResponseStream()))
        ////    {
        ////        responseContent = streamReader.ReadToEnd();
        ////        //responseMessage = JsonConvert.DeserializeObject<ResponseMessage>(responseContent);
        ////    }

        ////    return responseContent;
        ////}
    }
}