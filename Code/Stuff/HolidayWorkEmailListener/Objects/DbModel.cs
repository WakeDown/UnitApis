using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Exchange.WebServices.Data;
using Newtonsoft.Json;

namespace HolidayWorkEmailListener.Objects
{
    public class DbModel
    {
        public static string OdataServiceUri = ConfigurationManager.AppSettings["OdataServiceUri"];

        
        protected static string GetJson(Uri uri)
        {
            string result = String.Empty;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            CredentialCache cc = new CredentialCache();
            cc.Add(uri, "NTLM", CredentialCache.DefaultNetworkCredentials);
            request.Credentials = cc;
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
    }
}
