using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using InvoiceCaptureLib.Model;
using InvoiceCaptureLib.Model.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace InvoiceCaptureLib
{
    public class InvoiceCapture
    {
        private const string CompanyEndPoint = "companies";
        private const string CustomerEndPoint = "customers";
        private const string DebtsEndpoint = "debts";
        private const string ProdutionUri = "https://api.invisiblecollector.com/";

        private string _remoteUri;
        private string _apiKey;
        private JsonModelConverterFacade _jsonFacade;

        public InvoiceCapture(string apiKey, string remoteUri = ProdutionUri) : this(apiKey, remoteUri, new JsonModelConverterFacade())
        { }

        internal InvoiceCapture(string apiKey, string remoteUri, JsonModelConverterFacade jsonFacade)
        {
            ApiKey = apiKey;
            RemoteUri = remoteUri;
            _jsonFacade = jsonFacade;
        }

        public string ApiKey { get; private set;  }

        public string RemoteUri
        {
            get => _remoteUri;

            private set
            {
                if (checkURI(value))
                    _remoteUri = value;
                else
                    throw new UriFormatException("Not a valid URI");
            }
        }

        public Company RequestCompanyInfo()
        {
            var json = callAPI(CompanyEndPoint, "", "GET");
            return this._jsonFacade.JsonToModel<Company>(json);
        }

        public Company UpdateCompanyInfo(Company company)
        {
            string json = this._jsonFacade.ModelToSendableJson(company);
            string returnedJson = callAPI(CompanyEndPoint, json, "PUT");
            return this._jsonFacade.JsonToModel<Company>(returnedJson);
        }

        private string callAPI(string endpoint, string jsonString, string method)
        {
            var response = "";
            var client = getWebClient();
            try
            {
                if (method == "POST" || method == "PUT" || method == "DELETE")
                    response = client.UploadString(RemoteUri + endpoint, method, jsonString);
                else if (method == "GET")
                    response = client.DownloadString(RemoteUri + endpoint);
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

        private bool checkURI(string value)
        {
            Uri uriResult;
            return Uri.TryCreate(value, UriKind.Absolute, out uriResult) &&
                   (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

        private WebClient getWebClient()
        {
            ServicePointManager.ServerCertificateValidationCallback =
                (sender, certificate, chain, sslPolicyErrors) => true;
            var client = new WebClient();
            client.Headers.Set("Content-Type", "application/json");
            client.Headers.Set("X-Api-Token", ApiKey);
            client.Encoding = Encoding.UTF8;
            return client;
        }
    }
}