using System;
using System.IO;
using System.Net;
using System.Text;
using InvoiceCaptureLib.Exception;
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

        /// <summary>
        ///     Make an Api Request.
        /// </summary>
        /// <param name="requestUri"></param>
        /// the absolute uri of the request
        /// <param name="method"></param>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        internal string CallApi(Uri requestUri, string method, string jsonString)
        {
            HttpUriBuilder.AssertValidHttpUri(requestUri);
            var requestHasBody = !string.IsNullOrEmpty(jsonString);
            var client = BuildWebClient(requestUri.Host, requestHasBody);
            try
            {
                if (method == "POST" || method == "PUT" || method == "DELETE")
                    return client.UploadString(requestUri, method, jsonString);
                if (method == "GET")
                    return client.DownloadString(requestUri);
                throw new ArgumentException("Invalid HTTP method");
            }
            catch (WebException e)
            {
                // TODO: improve error handling
                // TODO: optimize webclient connections
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
            if (requestHasBody)
                client.Headers.Set("Content-Type", "application/json");
            client.Headers.Set("Accept", "application/json");
            client.Headers.Set("Authorization", $"Bearer {_apiKey}");
            client.Headers.Set("Host", requestUriHost);
            client.Encoding = Encoding.UTF8;
            return client;
        }
    }
}