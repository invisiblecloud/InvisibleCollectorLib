using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace InvisibleCollectorLib.Connection
{
    internal class HttpUriBuilder
    {
        private UriBuilder _uriBuilder;

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
            var path = String.Join("/", pathFragments);
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

        private static void AssertValidHttpUri(Uri uri)
        {
            if (!uri.IsAbsoluteUri)
                throw new ArgumentException("Must be an absolute uri: " + uri);

            if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
                throw new ArgumentException("Not an HTTP Url: " + uri);
        }

        internal static string NormalizeUriComponent(string uriComponent)
        {
            if (String.IsNullOrWhiteSpace(uriComponent))
                throw new ArgumentException("Illegal uri component: " + uriComponent);

            return WebUtility.UrlEncode(uriComponent);
        }
    }
}
