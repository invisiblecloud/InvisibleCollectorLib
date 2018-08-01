using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using InvoiceCaptureLib.Exception;

namespace InvoiceCaptureLib.Connection
{
    // TODO: optimize webclient connections with ServicePointManager class

    internal class ApiConnectionFacade
    {
        private readonly string _apiKey;
        private readonly Func<Stream, IDictionary<string, string>> _jsonParser;

        internal ApiConnectionFacade(string apiKey, Func<Stream, IDictionary<string, string>> jsonParser)
        {
            _apiKey = apiKey;
            _jsonParser = jsonParser;
        }

        /// <summary>
        ///     Make an Api Request.
        /// </summary>
        /// <param name="requestUri">The absolute request Url</param>
        /// the absolute uri of the request
        /// <param name="method">An http method: "GET", "POST", "PUT", "DELETE"</param>
        /// <param name="jsonString">The request body string. Can be null or empty if no request body is to be sent</param>
        /// <returns>the response string task. Null or empty string is returned if no response is received.</returns>
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
            catch (WebException webException)
            {
                if (webException.Status == WebExceptionStatus.ProtocolError && webException.Response != null)
                {
                    var contentType = webException.Response.ContentType;
                    var isJsonResponse = contentType?.Contains("application/json") ?? false;
                    if (isJsonResponse)
                    {
                        var responseStream = webException.Response.GetResponseStream();
                        throw BuildException(responseStream);
                    }
                }

                throw;
            }
        }

        private IcException BuildException(Stream jsonStream)
        {
            var jsonObject = _jsonParser(jsonStream);
            if (!jsonObject.ContainsKey("message") || !jsonObject.ContainsKey("code"))
                throw new IcException($"Invalid json error response received: {jsonObject.ToString()}");
            else if (jsonObject.ContainsKey("gid"))
                return new IcModelConflictException(jsonObject["message"], jsonObject["gid"]);
            else 
                return new IcException($"{jsonObject["message"]} (HTTP Code: {jsonObject["code"]})");
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