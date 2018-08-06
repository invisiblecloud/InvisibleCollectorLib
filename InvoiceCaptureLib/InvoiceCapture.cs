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
        private const string CompanyEndpoint = "companies";
        private const string CustomerEndpoint = "customers";
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

        private async Task<TModel> MakeBodylessRequest<TModel>(string method, params string[] pathFragments) where TModel: Model.Model, new()
        {
            var requestUri = _uriBuilder.BuildUri(pathFragments);
            var json = await _apiFacade.CallApiAsync(requestUri, method);
            return _jsonFacade.JsonToModel<TModel>(json);
        }

        private async Task<TModel> MakeBodiedRequest<TModel>(string method, TModel modelToSend, params string[] pathFragments) where TModel : Model.Model, new()
        {
            modelToSend.AssertHasMandatoryFields();
            var requestJson = _jsonFacade.ModelToSendableJson(modelToSend);
            var requestUri = _uriBuilder.BuildUri(pathFragments);
            var returnJson = await _apiFacade.CallApiAsync(requestUri, method, requestJson);
            return _jsonFacade.JsonToModel<TModel>(returnJson);
        }

        public async Task<Company> RequestCompanyInfoAsync()
        {
            return await MakeBodylessRequest<Company>("GET", CompanyEndpoint);
        }

        public async Task<Company> UpdateCompanyInfoAsync(Company company)
        {
            return await MakeBodiedRequest("PUT", company, CompanyEndpoint);
        }

        public async Task<Company> SetCompanyNotifications(bool bEnableNotifications)
        {
            const string EnableNotifications = "enableNotifications";
            const string DisableNotifications = "disableNotifications";

            var endpoint = bEnableNotifications ? EnableNotifications : DisableNotifications;
            return await MakeBodylessRequest<Company>("PUT", CompanyEndpoint, endpoint);
        }

        //public async Task<>

    }
}