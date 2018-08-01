using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
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
        /// <param name="requestUri">The absolute request Url</param>
        /// the absolute uri of the request
        /// <param name="method">An http method: "GET", "POST", "PUT", "DELETE"</param>
        /// <param name="jsonString">The request body string. Can be null or empty if no request body is to be sent</param>
        /// <returns></returns>
        internal async Task<string> CallApiAsync(Uri requestUri, string method, string jsonString = null)
        {
            HttpUriBuilder.AssertValidHttpUri(requestUri);
            var requestHasBody = !string.IsNullOrEmpty(jsonString);
            jsonString = requestHasBody ? jsonString : "";
            var client = BuildWebClient(requestUri.Host, requestHasBody);

            try
            {
                switch (method)
                {
                    case "POST":
                    case "PUT":
                    case "DELETE":
                        return await client.UploadStringTaskAsync(requestUri, method, jsonString);
                    case "GET":
                        return await client.DownloadStringTaskAsync(requestUri);
                    default:
                        throw new ArgumentException("Invalid HTTP method");
                }
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
                        var error = JsonConvert.DeserializeObject<InvoiceCaptureJsonError>(errorString);
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