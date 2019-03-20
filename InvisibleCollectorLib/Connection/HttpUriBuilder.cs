using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace InvisibleCollectorLib.Connection
{
    internal class HttpUriBuilder
    {
        private readonly UriBuilder _uriBuilder;

        internal HttpUriBuilder(string absoluteBaseUri) : this(new Uri(absoluteBaseUri))
        {
        }

        internal HttpUriBuilder(Uri absoluteBaseUri)
        {
            AssertValidHttpUri(absoluteBaseUri);
            _uriBuilder = new UriBuilder(absoluteBaseUri);
        }

        internal HttpUriBuilder WithPath(params string[] pathFragments)
        {
            var path = string.Join("/", pathFragments.Select(UriEscape));
            _uriBuilder.Path = path;
            return this;
        }

        internal Uri BuildUri()
        {
            AssertValidHttpUri(_uriBuilder.Uri);
            return _uriBuilder.Uri;
        }

        internal HttpUriBuilder Clone()
        {
            return new HttpUriBuilder(_uriBuilder.Uri);
        }

        internal HttpUriBuilder WithQuery(IDictionary<string, string> values)
        {
            if (values.Count == 0)
                return this;

            var query = string.Join("&",
                values.Select(pair => $"{UriEscape(pair.Key)}={UriEscape(pair.Value)}")
                    .ToArray()
            );
            _uriBuilder.Query = query;
            return this;
        }

        private static void AssertValidHttpUri(Uri uri)
        {
            if (!uri.IsAbsoluteUri)
                throw new ArgumentException("Must be an absolute uri: " + uri);

            if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
                throw new ArgumentException("Not an HTTP Url: " + uri);
        }

        internal static string UriEscape(string uriComponent)
        {
            if (uriComponent == null)
                throw new ArgumentException("Illegal uri component: " + uriComponent);

            return WebUtility.UrlEncode(uriComponent);
        }
    }
}