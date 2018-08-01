using System;
using System.Collections.Generic;
using System.Text;

namespace InvoiceCaptureLib.Connection
{
    internal class HttpUriBuilder
    {
        private readonly Uri _baseUri;

        internal HttpUriBuilder(string absoluteBaseUri)
        {
            if (! Uri.TryCreate(absoluteBaseUri, UriKind.Absolute, out _baseUri) ||
                  (_baseUri.Scheme != Uri.UriSchemeHttp && _baseUri.Scheme != Uri.UriSchemeHttps))
                throw new UriFormatException("Not a valid HTTP URI: " + absoluteBaseUri);
        }

        internal HttpUriBuilder(Uri absoluteBaseUri)
        {
            if (_baseUri.Scheme != Uri.UriSchemeHttp && _baseUri.Scheme != Uri.UriSchemeHttps)
                throw new UriFormatException("Not a valid HTTP URI: " + absoluteBaseUri);
        }

        internal Uri BuildUri(params string[] fragments)
        {
            var relativePath = string.Join("/", fragments);
            return new Uri(_baseUri, relativePath);
        }
    }
}
