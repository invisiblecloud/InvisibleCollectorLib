using System;
using System.Collections.Generic;
using System.Text;

namespace InvisibleCollectorLib.Connection
{
    internal class HttpUriBuilder
    {
        private readonly Uri _baseUri;

        internal HttpUriBuilder(string absoluteBaseUri) : this(new Uri(absoluteBaseUri))
        {
        }

        internal HttpUriBuilder(Uri absoluteBaseUri)
        {
            AssertValidHttpUri(absoluteBaseUri);
            _baseUri = absoluteBaseUri;
        }

        internal Uri BaseUri => _baseUri;

        internal static void AssertValidHttpUri(Uri uri)
        {
            if (!uri.IsAbsoluteUri)
                throw new ArgumentException("Must be an absolute uri: " + uri.ToString());

            if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
                throw new ArgumentException("Not an HTTP Url: " + uri.ToString());
        }

        internal Uri BuildUri(params string[] fragments)
        {
            var relativePath = string.Join("/", fragments);
            return new Uri(_baseUri, relativePath);
        }
    }
}
