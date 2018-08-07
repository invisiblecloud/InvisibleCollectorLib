using System;
using System.Net;
using System.Threading.Tasks;
using InvoiceCaptureLib.Connection;
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

        public async Task<Customer> RegisterNewCustomerAsync(Customer customer)
        {
            customer.AssertHasMandatoryFields(Customer.NameName, Customer.VatNumberName, Customer.CountryName);
            return await MakeBodiedRequest("POST", customer, CustomerEndpoint);
        }

        public async Task<Company> RequestCompanyInfoAsync()
        {
            return await MakeBodylessRequest<Company>("GET", CompanyEndpoint);
        }

        public async Task<Customer> RequestCustomerInfoAsync(string customerId)
        {
            var id = AssertValidAndNormalizeId(customerId);
            return await MakeBodylessRequest<Customer>("GET", CustomerEndpoint, id);
        }

        public async Task<Company> SetCompanyNotificationsAsync(bool bEnableNotifications)
        {
            const string EnableNotifications = "enableNotifications";
            const string DisableNotifications = "disableNotifications";

            var endpoint = bEnableNotifications ? EnableNotifications : DisableNotifications;
            return await MakeBodylessRequest<Company>("PUT", CompanyEndpoint, endpoint);
        }

        public async Task<Company> UpdateCompanyInfoAsync(Company company)
        {
            company.AssertHasMandatoryFields(Company.NameName, Company.VatNumberName);
            return await MakeBodiedRequest("PUT", company, CompanyEndpoint);
        }

        public async Task<Customer> UpdateCustomerInfoAsync(Customer customer)
        {
            var id = AssertValidAndNormalizeId(customer.RoutableId);
            customer.AssertHasMandatoryFields(Customer.CountryName);
            return await MakeBodiedRequest("PUT", customer, CustomerEndpoint, id);
        }

        private string AssertValidAndNormalizeId(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Illegal routing id: " + id);

            return WebUtility.UrlEncode(id);
        }

        private async Task<TModel> MakeBodiedRequest<TModel>(string method, TModel modelToSend,
            params string[] pathFragments) where TModel : Model.Model, new()
        {
            var requestJson = _jsonFacade.ModelToSendableJson(modelToSend);
            var requestUri = _uriBuilder.BuildUri(pathFragments);
            var returnJson = await _apiFacade.CallApiAsync(requestUri, method, requestJson);
            return _jsonFacade.JsonToModel<TModel>(returnJson);
        }

        private async Task<TModel> MakeBodylessRequest<TModel>(string method, params string[] pathFragments)
            where TModel : Model.Model, new()
        {
            var requestUri = _uriBuilder.BuildUri(pathFragments);
            var json = await _apiFacade.CallApiAsync(requestUri, method);
            return _jsonFacade.JsonToModel<TModel>(json);
        }
    }
}