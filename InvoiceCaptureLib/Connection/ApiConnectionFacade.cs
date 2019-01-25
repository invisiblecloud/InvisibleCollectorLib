using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using InvisibleCollectorLib.Exception;
using InvisibleCollectorLib.Utils;

namespace InvisibleCollectorLib.Connection
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
        internal async Task<string> CallJsonToJsonApi(Uri requestUri, string method, string jsonString = null)
        {
            return await CallApiAsync(requestUri, method, IcConstants.JsonMimeType, jsonString);
        }
        
        
        /// <summary>
        ///     Make an Api Request.
        /// </summary>
        /// <param name="requestUri">The absolute request Url</param>
        /// the absolute uri of the request
        /// <param name="method">An http method: "GET", "POST", "PUT", "DELETE"</param>
        /// <param name="jsonString">The request body string. Can be null or empty if no request body is to be sent</param>
        /// <returns>the response string task. Null or empty string is returned if no response is received.</returns>
        internal async Task<string> CallApiAsync(Uri requestUri, string method, string contentType, string jsonString = null)
        {
            HttpUriBuilder.AssertValidHttpUri(requestUri);
            
            var client = BuildWebClient(requestUri.Host);
            if (string.IsNullOrEmpty(jsonString))
                jsonString = "";
            else
                client.Headers.Set("Content-Type", $"{contentType}; charset=UTF-8");
                
            string response;
            try
            {
                switch (method)
                {
                    case "POST":
                    case "PUT":
                    // does DELETE have a return json?
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
                var errorResponse = webException.Response;
                if (webException.Status != WebExceptionStatus.ProtocolError || errorResponse?.GetResponseStream() is null || !IsContentTypeJson(errorResponse.Headers))
                    throw;

                var ex = BuildException(errorResponse.GetResponseStream());
                if (!(ex is null))
                    throw ex;
                
                throw;
            }

            // check that the response has 'Content-Type' header set to json
            if (!IsContentTypeJson(client.ResponseHeaders))
                throw new IcException("Request valid but no JSON response HTTP header received");

            return response;
        }

        private bool IsContentTypeJson(WebHeaderCollection headers)
        {
            if (headers is null)
                return false;

            var headerValue = headers.Get("Content-Type");
            return !(headerValue is null) && headerValue.Contains(IcConstants.JsonMimeType);
        }

        private IcException BuildException(Stream jsonStream)
        {
            // json key names
            const string messageName = "message";
            const string codeName = "code";
            const string gidName = "gid";
            const string idName = "id";

            var jsonObject = _jsonParser(jsonStream);
            if (!jsonObject.ContainsKey(messageName) || !jsonObject.ContainsKey(codeName))
                return null;

            // will check for the various ways a model ID can be named.
            // First accepted key will also initialize the previous local
            var containsId = jsonObject.TryGetValue(gidName, out var conflictingId) || 
                             jsonObject.TryGetValue(idName, out conflictingId); 
            if (containsId)
                return new IcModelConflictException(jsonObject[messageName], conflictingId);
            
            return new IcException($"{jsonObject[messageName]} (HTTP Code: {jsonObject[codeName]})");
        }

        private WebClient BuildWebClient(string requestUriHost)
        {
            var client = new WebClient();
            client.Headers.Set("Accept", IcConstants.JsonMimeType);
            client.Headers.Set("Authorization", $"Bearer {_apiKey}");
            client.Headers.Set("Host", requestUriHost);
            client.Encoding = Encoding.UTF8;
            return client;
        }
    }
}