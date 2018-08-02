using System;
using System.Threading.Tasks;
using InvoiceCaptureLib.Connection;
using InvoiceCaptureLib.Exception;
using InvoiceCaptureLib.Json;
using InvoiceCaptureLib.Model;

namespace InvoiceCaptureLib
{
    public class InvoiceCapture
    {
        private const string CompanyEndPoint = "companies";
        private const string CustomerEndPoint = "customers";
        private const string DebtsEndpoint = "debts";
        private const string ProdutionUri = "https://api.invisiblecollector.com/";
        private readonly ApiConnectionFacade _apiFacade;
        private readonly JsonConvertFacade _jsonFacade;
        private readonly HttpUriBuilder _uriBuilder;


        public InvoiceCapture(string apiKey, string remoteUri = ProdutionUri)
        {
            _uriBuilder = new HttpUriBuilder(remoteUri);
            _jsonFacade = new JsonConvertFacade();
            _apiFacade = new ApiConnectionFacade(apiKey, _jsonFacade.JsonStreamToStringDictionary);
        }

        public InvoiceCapture(string apiKey, Uri remoteUri) : this(apiKey, remoteUri.AbsoluteUri)
        {
        }

        internal InvoiceCapture(HttpUriBuilder uriBuilder,
            ApiConnectionFacade apiFacade,
            JsonConvertFacade jsonFacade)
        {
            _jsonFacade = jsonFacade;
            _uriBuilder = uriBuilder;
            _apiFacade = apiFacade;
        }

        public async Task<Company> RequestCompanyInfoAsync()
        {
            var requestUri = _uriBuilder.BuildUri(CompanyEndPoint);
            var json = await _apiFacade.CallApiAsync(requestUri, "GET", null);
            return _jsonFacade.JsonToModel<Company>(json);
        }

        public async Task<Company> UpdateCompanyInfoAsync(Company company)
        {
            company.AssertHasMandatoryFields();
            var json = _jsonFacade.ModelToSendableJson(company);
            var requestUri = _uriBuilder.BuildUri(CompanyEndPoint);
            var returnedJson = await _apiFacade.CallApiAsync(requestUri, "PUT", json);
            return _jsonFacade.JsonToModel<Company>(returnedJson);
        }

        public async Task<Company> SetCompanyNotifications(bool bEnableNotifications)
        {
            const string EnableNotifications = "enableNotifications";
            const string DisableNotifications = "disableNotifications";

            var endpoint = bEnableNotifications ? EnableNotifications : DisableNotifications;
            var requestUri = _uriBuilder.BuildUri(CompanyEndPoint, endpoint);
            var json = await _apiFacade.CallApiAsync(requestUri, "PUT", null);
            return _jsonFacade.JsonToModel<Company>(json);
        }
    }
}