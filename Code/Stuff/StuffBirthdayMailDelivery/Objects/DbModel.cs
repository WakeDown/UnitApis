using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using Newtonsoft.Json;

namespace Stuff.Objects
{
    public class DbModel
    {
        public const string OdataServiceUri = "http://localhost:10001/odata";

        protected static string GetJson(Uri uri)
        {
            string result = String.Empty;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            CredentialCache cc = new CredentialCache();
            cc.Add(uri, "NTLM", CredentialCache.DefaultNetworkCredentials);
            request.Credentials = cc;
            //cc.Add(
            //uri,
            //"NTLM",
            //new NetworkCredential("UN1T\\sqlUnit_prog", "1qazXSW@")
            //);
            request.ContentType = "application/json";

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
            CredentialCache cc = new CredentialCache();
            cc.Add(uri, "NTLM", CredentialCache.DefaultNetworkCredentials);
            request.Credentials = cc;
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