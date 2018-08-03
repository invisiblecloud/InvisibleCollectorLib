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
        private const string JSON_MIME_TYPE = "application/json";

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
            string response = "";

            try
            {
                switch (method)
                {
                    case "POST":
                    case "PUT":
                    case "DELETE":
                        response = await client.UploadStringTaskAsync(requestUri, method, jsonString);
                        break;
                    case "GET":
                        response = await client.DownloadStringTaskAsync(requestUri);
                        break;
                    default:
                        throw new ArgumentException("Invalid HTTP method");
                }
            }
            catch (WebException webException)
            {
                if (webException.Status != WebExceptionStatus.ProtocolError || webException.Response == null ||
                    !IsJsonMimeType(webException.Response.Headers))
                    throw;

                var responseStream = webException.Response.GetResponseStream();
                throw BuildException(responseStream);

            }

            // check that the response has 'Content-Type' header set to json
            if (!IsJsonMimeType(client.ResponseHeaders))
                throw new IcException("Request valid but no JSON response HTTP header received");

            //this

            return response;
        }

        private bool IsJsonMimeType(WebHeaderCollection headers)
        {
            if (headers is null)
                return false;

            var headerValue = headers.Get("Content-Type");
            return !(headerValue is null) && headerValue.Contains(JSON_MIME_TYPE);
        }

        private IcException BuildException(Stream jsonStream)
        {
            // json key names
            const string messageName = "message";
            const string codeName = "code";
            const string idName = "gid";

            var jsonObject = _jsonParser(jsonStream);
            if (!jsonObject.ContainsKey(messageName) || !jsonObject.ContainsKey(codeName))
                throw new IcException($"Invalid json error response received: {Utils.IcUtils.StringifyDictionary(jsonObject)}");
            else if (jsonObject.ContainsKey(idName))
                return new IcModelConflictException(jsonObject[messageName], jsonObject[idName]);
            else 
                return new IcException($"{jsonObject[messageName]} (HTTP Code: {jsonObject[codeName]})");
        }

        private WebClient BuildWebClient(string requestUriHost, bool requestHasBody)
        {
            var client = new WebClient();
            if (requestHasBody)
                client.Headers.Set("Content-Type", JSON_MIME_TYPE);
            client.Headers.Set("Accept", JSON_MIME_TYPE);
            client.Headers.Set("Authorization", $"Bearer {_apiKey}");
            client.Headers.Set("Host", requestUriHost);
            client.Encoding = Encoding.UTF8;
            return client;
        }
    }
}