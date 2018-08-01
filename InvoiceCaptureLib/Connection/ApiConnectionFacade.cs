using System;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace InvoiceCaptureLib.Connection
{
    internal class ApiConnectionFacade
    {
        private readonly string _apiKey;

        internal ApiConnectionFacade(string apiKey)
        {
            _apiKey = apiKey;
        }

        internal string CallAPI(Uri requestUri, string method, string jsonString)
        {
            var requestHasBody = !string.IsNullOrEmpty(jsonString);
            var client = BuildWebClient(requestUri.Host, requestHasBody);
            try
            {
                if (method == "POST" || method == "PUT" || method == "DELETE")
                    return client.UploadString(requestUri, method, jsonString);
                if (method == "GET")
                    return client.DownloadString("https://api.invisiblecollector.com/companies/");
                throw new ArgumentException("Invalid HTTP method");
            }
            catch (WebException e)
            {
                // TODO: improve error handling
                // TODO: optimize connections
                var responseStream = e.Response?.GetResponseStream();
                if (responseStream != null)
                    using (var reader = new StreamReader(responseStream))
                    {
                        var errorString = reader.ReadToEnd();
                        var error = JsonConvert.DeserializeObject<InvoiceCaptureError>(errorString);
                        throw new InvoiceCaptureException(error.Code + " " + error.Message);
                    }

                throw e;
            }
        }

        private WebClient BuildWebClient(string requestUriHost, bool requestHasBody)
        {
            var client = new WebClient();
            //if (requestHasBody)
                client.Headers.Set("Content-Type", "application/json");
            //client.Headers.Set("Accept", "application/json");
            client.Headers.Set("X-Api-Token", $"{_apiKey}");
            //client.Headers.Set("Host", requestUriHost);
            client.Encoding = Encoding.UTF8;
            return client;
        }
    }
}