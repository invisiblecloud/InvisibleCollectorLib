using System;
using System.IO;
using System.Net;
using System.Text;
using InvoiceCaptureLib.Model;
using InvoiceCaptureLib.Model.Json;
using Newtonsoft.Json;

namespace InvoiceCaptureLib
{
    public class InvoiceCapture
    {
        private const string CompanyEndPoint = "companies";
        private const string CustomerEndPoint = "customers";
        private const string DebtsEndpoint = "debts";
        private const string ProdutionUri = "https://api.invisiblecollector.com/";
        private readonly string _apiKey;
        private readonly JsonModelConverterFacade _jsonFacade;
        private readonly Uri _remoteUri;


        public InvoiceCapture(string apiKey, string remoteUri = ProdutionUri) : this(apiKey, remoteUri,
            new JsonModelConverterFacade())
        {
        }

        internal InvoiceCapture(string apiKey, string remoteUri, JsonModelConverterFacade jsonFacade)
        {
            this._apiKey = apiKey;
            _jsonFacade = jsonFacade;
            _remoteUri = buildUri(remoteUri);

        }

        public Company RequestCompanyInfo()
        {
            var requestUri = buildUri(_remoteUri, CompanyEndPoint);
            var json = callAPI(requestUri, "", "GET");
            return _jsonFacade.JsonToModel<Company>(json);
        }

        public Company UpdateCompanyInfo(Company company)
        {
            company.AssertHasMandatoryFields();
            var json = _jsonFacade.ModelToSendableJson(company);
            var requestUri = buildUri(_remoteUri, CompanyEndPoint);
            var returnedJson = callAPI(requestUri, json, "PUT");
            return _jsonFacade.JsonToModel<Company>(returnedJson);
        }



        private string callAPI(Uri endpoint, string jsonString, string method)
        {
            var response = "";
            var client = BuildWebClient();
            try
            {
                if (method == "POST" || method == "PUT" || method == "DELETE")
                    response = client.UploadString(endpoint, method, jsonString);
                else if (method == "GET")
                    response = client.DownloadString(endpoint);
            }
            catch (WebException e)
            {
                var responseStream = e.Response?.GetResponseStream();
                if (responseStream != null)
                    using (var reader = new StreamReader(responseStream))
                    {
                        response = reader.ReadToEnd();
                        var error = JsonConvert.DeserializeObject<InvoiceCaptureError>(response);
                        throw new InvoiceCaptureException(error.Code + " " + error.Message);
                    }
            }

            return response;
        }

        private Uri buildUri(string uri)
        {
            Uri uriResult;
            if (! (Uri.TryCreate(uri, UriKind.Absolute, out uriResult) &&
                (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps)))
            {
                throw new UriFormatException("Not a valid HTTP URI: " + uri);
            }

            return uriResult;
        }

        private Uri buildUri(Uri baseUri, params string[] fragments)
        {
            var relativePath = string.Join("/", fragments);
            return new Uri(baseUri, relativePath);
        }

        private WebClient BuildWebClient()
        {
            ServicePointManager.ServerCertificateValidationCallback =
                (sender, certificate, chain, sslPolicyErrors) => true;
            var client = new WebClient();
            client.Headers.Set("Content-Type", "application/json");
            client.Headers.Set("X-Api-Token", this._apiKey);
            client.Encoding = Encoding.UTF8;
            return client;
        }
    }
}