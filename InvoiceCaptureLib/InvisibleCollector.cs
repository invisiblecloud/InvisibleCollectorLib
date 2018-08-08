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

        public async Task<Company> GetCompanyInfoAsync()
        {
            return await MakeBodylessRequest<Company>("GET", CompaniesEndpoint);
        }

        public async Task<IDictionary<string, string>> GetCustomerAttributesAsync(string customerId)
        {
            var id = HttpUriBuilder.NormalizeUriComponent(customerId);
            return await MakeBodylessRequest<Dictionary<string, string>>("GET", CustomersEndpoint, id,
                CustomersAttributesPath);
        }

        public async Task<IList<Debt>> GetCustomerDebts(string customerId)
        {
            const string customerDebtsPath = "debts";

            var id = HttpUriBuilder.NormalizeUriComponent(customerId);
            return await MakeBodylessRequest<List<Debt>>("GET", CustomersEndpoint, id, customerDebtsPath);
        }

        public async Task<Customer> GetCustomerInfoAsync(string customerId)
        {
            var id = HttpUriBuilder.NormalizeUriComponent(customerId);
            return await MakeBodylessRequest<Customer>("GET", CustomersEndpoint, id);
        }

        public async Task<Debt> GetDebt(string debtId)
        {
            var id = HttpUriBuilder.NormalizeUriComponent(debtId);
            return await MakeBodylessRequest<Debt>("GET", DebtsEndpoint, id);
        }

        public async Task<Company> SetCompanyInfoAsync(Company company)
        {
            company.AssertHasMandatoryFields(Company.NameName, Company.VatNumberName);
            return await MakeRequest<Company, object>("PUT", company.SendableDictionary, CompaniesEndpoint);
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
            var id = HttpUriBuilder.NormalizeUriComponent(customerId);
            return await MakeRequest<Dictionary<string, string>, string>("POST", attributes, CustomersEndpoint, id,
                CustomersAttributesPath);
        }

        public async Task<Customer> SetCustomerInfoAsync(Customer customer)
        {
            var id = HttpUriBuilder.NormalizeUriComponent(customer.RoutableId);
            customer.AssertHasMandatoryFields(Customer.CountryName);
            return await MakeRequest<Customer, object>("PUT", customer.SendableDictionary, CustomersEndpoint, id);
        }

        public async Task<Customer> SetNewCustomerAsync(Customer customer)
        {
            customer.AssertHasMandatoryFields(Customer.NameName, Customer.VatNumberName, Customer.CountryName);
            return await MakeRequest<Customer, object>("POST", customer.SendableDictionary, CustomersEndpoint);
        }

        public async Task<Debt> SetNewDebt(Debt debt)
        {
            debt.AssertHasMandatoryFields(Debt.NumberName, Debt.CustomerIdName, Debt.TypeName, Debt.DateName,
                Debt.DueDateName);
            debt.AssertItemsHaveMandatoryFields(Item.NameName);
            return await MakeRequest<Debt, object>("POST", debt.SendableDictionary, DebtsEndpoint);
        }

        /// <summary>
        /// Makes an api request
        /// </summary>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="method"></param>
        /// <param name="requestBody"> if null the request doesn't have a body</param>
        /// <param name="pathFragments"></param>
        /// <returns></returns>
        private async Task<TReturn> MakeRequest<TReturn, TDictValue>(string method, IDictionary<string, TDictValue> requestBody = null, params string[] pathFragments) where TReturn : new()
        {
            var requestJson = requestBody is null ? null : _jsonFacade.DictionaryToJson(requestBody);
            var requestUri = _uriBuilder.BuildUri(pathFragments);
            var json = await _apiFacade.CallApiAsync(requestUri, method, requestJson);
            return _jsonFacade.JsonToObject<TReturn>(json);
        }

        private async Task<TReturn> MakeBodylessRequest<TReturn>(string method, params string[] pathFragments) where TReturn : new()
        {
            return await MakeRequest<TReturn, object>(method, null, pathFragments);
        }
    }
}