using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using InvisibleCollectorLib.Connection;
using InvisibleCollectorLib.Json;
using InvisibleCollectorLib.Model;

namespace InvisibleCollectorLib
{
    public class InvisibleCollector
    {
        private const string CompaniesEndpoint = "companies";
        private const string CustomersAttributesPath = "attributes";
        private const string CustomersEndpoint = "customers";
        private const string DebtsEndpoint = "debts";
        private const string ProdutionUri = "https://api.invisiblecollector.com/";
        private readonly ApiConnectionFacade _apiFacade;
        private readonly JsonConvertFacade _jsonFacade;
        private readonly HttpUriBuilder _uriBuilder;

        public InvisibleCollector(string apiKey, string remoteUri = ProdutionUri)
        {
            _uriBuilder = new HttpUriBuilder(remoteUri);
            _jsonFacade = new JsonConvertFacade();
            _apiFacade = new ApiConnectionFacade(apiKey, _jsonFacade.JsonStreamToStringDictionary);
        }

        public InvisibleCollector(string apiKey, Uri remoteUri) : this(apiKey, remoteUri.AbsoluteUri)
        {
        }

        internal InvisibleCollector(HttpUriBuilder uriBuilder,
            ApiConnectionFacade apiFacade,
            JsonConvertFacade jsonFacade)
        {
            _jsonFacade = jsonFacade;
            _uriBuilder = uriBuilder;
            _apiFacade = apiFacade;
        }

        public async Task<IDictionary<string, string>> GetCustomerAttributesAsync(string customerId)
        {
            var id = AssertValidAndNormalizeId(customerId);
            var requestUri = _uriBuilder.BuildUri(CustomersEndpoint, id, CustomersAttributesPath);
            var returnJson = await _apiFacade.CallApiAsync(requestUri, "GET");
            return _jsonFacade.JsonToDictionary<string>(returnJson);
        }

        public async Task<Customer> SetNewCustomerAsync(Customer customer)
        {
            customer.AssertHasMandatoryFields(Customer.NameName, Customer.VatNumberName, Customer.CountryName);
            return await MakeBodiedModelRequest("POST", customer, CustomersEndpoint);
        }

        public async Task<Company> GetCompanyInfoAsync()
        {
            return await MakeBodylessRequest<Company>("GET", CompaniesEndpoint);
        }

        public async Task<Customer> GetCustomerInfoAsync(string customerId)
        {
            var id = AssertValidAndNormalizeId(customerId);
            return await MakeBodylessRequest<Customer>("GET", CustomersEndpoint, id);
        }

        public async Task<Company> SetCompanyNotificationsAsync(bool bEnableNotifications)
        {
            const string EnableNotifications = "enableNotifications";
            const string DisableNotifications = "disableNotifications";

            var endpoint = bEnableNotifications ? EnableNotifications : DisableNotifications;
            return await MakeBodylessRequest<Company>("PUT", CompaniesEndpoint, endpoint);
        }

        public async Task<IDictionary<string, string>> SetCustomerAttributesAsync(string customerId,
            IDictionary<string, string> attributes)
        {
            var id = AssertValidAndNormalizeId(customerId);
            var requestJson = _jsonFacade.DictionaryToJson(attributes);
            var requestUri = _uriBuilder.BuildUri(CustomersEndpoint, id, CustomersAttributesPath);
            var returnJson = await _apiFacade.CallApiAsync(requestUri, "POST", requestJson);
            return _jsonFacade.JsonToDictionary<string>(returnJson);
        }

        public async Task<Company> SetCompanyInfoAsync(Company company)
        {
            company.AssertHasMandatoryFields(Company.NameName, Company.VatNumberName);
            return await MakeBodiedModelRequest("PUT", company, CompaniesEndpoint);
        }

        public async Task<Customer> SetCustomerInfoAsync(Customer customer)
        {
            var id = AssertValidAndNormalizeId(customer.RoutableId);
            customer.AssertHasMandatoryFields(Customer.CountryName);
            return await MakeBodiedModelRequest("PUT", customer, CustomersEndpoint, id);
        }

        public async Task<Debt> SetNewDebt(Debt debt)
        {
            debt.AssertHasMandatoryFields(Debt.NumberName, Debt.CustomerIdName, Debt.TypeName, Debt.DateName, Debt.DueDateName);
            debt.AssertItemsHaveMandatoryFields(Item.NameName);
            return await MakeBodiedModelRequest("POST", debt, DebtsEndpoint);
        }

        public async Task<Debt> GetDebt(string debtId)
        {
            var id = AssertValidAndNormalizeId(debtId);
            return await MakeBodylessRequest<Debt>("GET", DebtsEndpoint, id);
        }

        public async Task<IList<Debt>> GetCustomerDebts(string customerId)
        {
            const string customerDebtsPath = "debts";

            var id = AssertValidAndNormalizeId(customerId);
            return await MakeBodylessRequest<List<Debt>>("GET", CustomersEndpoint, id, customerDebtsPath);
        }

        private string AssertValidAndNormalizeId(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Illegal routing id: " + id);

            return WebUtility.UrlEncode(id);
        }

        private async Task<TModel> MakeBodiedModelRequest<TModel>(string method, TModel modelToSend,
            params string[] pathFragments) where TModel : Model.Model, new()
        {
            var requestJson = _jsonFacade.ModelToSendableJson(modelToSend);
            var requestUri = _uriBuilder.BuildUri(pathFragments);
            var returnJson = await _apiFacade.CallApiAsync(requestUri, method, requestJson);
            return _jsonFacade.JsonToObject<TModel>(returnJson);
        }

        private async Task<TModel> MakeBodylessRequest<TModel>(string method, params string[] pathFragments)
            where TModel : new()
        {
            var requestUri = _uriBuilder.BuildUri(pathFragments);
            var json = await _apiFacade.CallApiAsync(requestUri, method);
            return _jsonFacade.JsonToObject<TModel>(json);
        }
    }
}