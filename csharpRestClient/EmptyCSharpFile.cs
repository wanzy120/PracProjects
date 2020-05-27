using System;
using System.IO;
using System.Net;
using System.Text;
//add https
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
//end of adding https

namespace csharpRestClient2
{
    public enum HttpVerbNew
    {
        GET,    //retrieve
        POST,   //create
        PUT,    //update
        DELETE  //delete
    }

    public class ContentType
    {
        public string Text = "text/plain";
        public string JSON = "application/json";
        public string javascript = "application/javascript";
        public string XML = "application/xml";
        public string TextXML = "text/xml";
        public string HTML = "text/html";
    }

    public class RestApiClient
    {
        public string EndPoint { get; set; }    //url of the request
        public HttpVerbNew Method { get; set; } //method of the request
        public string ContentType { get; set; } //format type
        public string PostData { get; set; }    //data transferred


        public  RestApiClient()
        {
            EndPoint = "";
            Method = HttpVerbNew.GET;
            ContentType = "text/xml";
            PostData = "";
        }

        public RestApiClient(string endpoint, string contentType)
        {
            EndPoint = endpoint;
            Method = HttpVerbNew.GET;
            ContentType = contentType;
            PostData = "";
        }

        public RestApiClient(string endpoint, HttpVerbNew method, string contentType)
        {
            EndPoint = endpoint;
            Method = method;
            ContentType = contentType;
            PostData = "";
        }

        public RestApiClient(string endpoint, HttpVerbNew method, string contentType, string postData)
        {
            EndPoint = endpoint;
            Method = method;
            ContentType = contentType;
            PostData = postData;
        }


        //add https
        //private static readonly string DefaultUserAgent = ""

        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;    //always accept
        }
        //end of adding https


        public string MakeRequest()
        {
            return MakeRequest("");
        }


        public string MakeRequest(string parameters)
        {
            var request = (HttpWebRequest)WebRequest.Create(EndPoint + parameters);

            //add https
            if (EndPoint.Substring(0, 8) == "https://")
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
            }
            //end of adding https


            request.Method = Method.ToString();
            request.ContentLength = 0;
            request.ContentType = ContentType;


            if (!string.IsNullOrEmpty(PostData) && Method == HttpVerbNew.POST)  //if transfer data is not null, and method is POST
            {
                var encoding = new UTF8Encoding();
                var bytes = Encoding.GetEncoding("iso-8859-1").GetBytes(PostData);
                request.ContentLength = bytes.Length;


                using (var writeStream = request.GetRequestStream())
                {
                    writeStream.Write(bytes, 0, bytes.Length);
                }
            }



            if (!string.IsNullOrEmpty(PostData) && Method == HttpVerbNew.PUT)  //if transfer data is not null, and method is POST
            {
                var encoding = new UTF8Encoding();
                var bytes = Encoding.GetEncoding("iso-8859-1").GetBytes(PostData);
                request.ContentLength = bytes.Length;


                using (var writeStream = request.GetRequestStream())
                {
                    writeStream.Write(bytes, 0, bytes.Length);
                }
            }



            using (var response = (HttpWebResponse)request.GetResponse())
            {
                var responseValue = string.Empty;


                if (response.StatusCode != HttpStatusCode.OK)
                {
                    var message = String.Format("Request failed. Received HTTP {0}", response.StatusCode);
                    throw new ApplicationException(message);
                }



                //grab the response
                using (var responseStream = response.GetResponseStream())
                {
                    if (responseStream != null)
                        using (var reader = new StreamReader(responseStream))
                        {
                            responseValue = reader.ReadToEnd();
                        }
                }

                return responseValue;
            }
        }


        public bool CheckUrl(string parameters)
        {
            bool bResult = true;

            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(EndPoint + parameters);
            myRequest.Method = Method.ToString();
            myRequest.Timeout = 10001;
            myRequest.AllowAutoRedirect = false;
            HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
            bResult = (myResponse.StatusCode == HttpStatusCode.OK);

            return bResult;

        }
    }
}