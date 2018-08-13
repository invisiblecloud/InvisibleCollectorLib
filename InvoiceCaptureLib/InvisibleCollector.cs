using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InvisibleCollectorLib.Connection;
using InvisibleCollectorLib.Json;
using InvisibleCollectorLib.Model;
using InvisibleCollectorLib.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace InvisibleCollectorLib
{
    public class InvisibleCollector
    {
        private readonly ILogger _logger;
        private const string CompaniesEndpoint = "companies";
        private const string CustomersAttributesPath = "attributes";
        private const string CustomersEndpoint = "customers";
        private const string DebtsEndpoint = "debts";
        private const string ProdutionUri = "https://api.invisiblecollector.com/";
        private readonly ApiConnectionFacade _apiFacade;
        private readonly JsonConvertFacade _jsonFacade;
        private readonly HttpUriBuilder _uriBuilder;

        public InvisibleCollector(string apiKey, string remoteUri = ProdutionUri, ILogger<InvisibleCollector> logger = null)
        {
            _uriBuilder = new HttpUriBuilder(remoteUri);
            _jsonFacade = new JsonConvertFacade();
            _apiFacade = new ApiConnectionFacade(apiKey, _jsonFacade.JsonStreamToStringDictionary);
            _logger = logger ?? NullLogger<InvisibleCollector>.Instance;

            _logger.LogInformation("Started InvisibleCollector instance");
        }

        public InvisibleCollector(string apiKey, Uri remoteUri) : this(apiKey, remoteUri.AbsoluteUri)
        {
        }

        ~InvisibleCollector()
        {
            _logger.LogInformation("InvisibleCollector Instance destroyed");
        }

        public async Task<Company> GetCompanyInfoAsync()
        {
            _logger.LogDebug("Making a request to get company information");
            var ret = await MakeBodylessRequestAsync<Company>("GET", CompaniesEndpoint);
            _logger.LogDebug("Received company info: {Model}", ret);
            return ret;
        }

        public async Task<IDictionary<string, string>> GetCustomerAttributesAsync(string customerId)
        {
            var id = HttpUriBuilder.NormalizeUriComponent(customerId);
            _logger.LogDebug("Making a request to get customer attributes for customer ID: {Id}", customerId);
            var ret = await MakeBodylessRequestAsync<Dictionary<string, string>>("GET", CustomersEndpoint, id,
                CustomersAttributesPath);
            _logger.LogDebug("Received for customer with id: {Id} attributes: {Attributes}", customerId, ret.StringifyDictionary());
            return ret;
        }

        public async Task<IList<Debt>> GetCustomerDebtsAsync(string customerId)
        {
            const string customerDebtsPath = "debts";
            _logger.LogDebug("Making a request to get customer debts for customer ID: {Id}", customerId);
            var id = HttpUriBuilder.NormalizeUriComponent(customerId);
            var ret = await MakeBodylessRequestAsync<List<Debt>>("GET", CustomersEndpoint, id, customerDebtsPath);
            _logger.LogDebug("Received for customer with id: {Id} debts: {Models}", customerId, ret.StringifyList());
            return ret;
        }

        public async Task<Customer> GetCustomerInfoAsync(string customerId)
        {
            var id = HttpUriBuilder.NormalizeUriComponent(customerId);
            _logger.LogDebug("Making request to get customer information for customer ID: {Id}", customerId);
            var ret = await MakeBodylessRequestAsync<Customer>("GET", CustomersEndpoint, id);
            _logger.LogDebug("Received for customer with id: {Id} information: {Model}", customerId, ret);
            return ret;
        }

        public async Task<Debt> GetDebtAsync(string debtId)
        {
            var id = HttpUriBuilder.NormalizeUriComponent(debtId);
            _logger.LogDebug("Making request to get debt information for debt ID: {Id}", debtId);
            var ret = await MakeBodylessRequestAsync<Debt>("GET", DebtsEndpoint, id);
            _logger.LogDebug("Received for debt with id: {Id} information: {Model}", debtId, ret);
            return ret;
        }

        public async Task<Company> SetCompanyInfoAsync(Company company)
        {
            company.AssertHasMandatoryFields(Company.NameName, Company.VatNumberName);
            _logger.LogDebug("Making request to update company information with the following info: {Model}", company);
            var ret = await MakeRequestAsync<Company, object>("PUT", company.SendableDictionary, CompaniesEndpoint);
            _logger.LogDebug("Updated company with new information: {Model}", ret);
            return ret;
        }

        public async Task<Company> SetCompanyNotificationsAsync(bool bEnableNotifications)
        {
            const string EnableNotifications = "enableNotifications";
            const string DisableNotifications = "disableNotifications";

            _logger.LogDebug("Making request to {Model} notifications", bEnableNotifications ? "enable" : "disable");
            var endpoint = bEnableNotifications ? EnableNotifications : DisableNotifications;
            var ret = await MakeBodylessRequestAsync<Company>("PUT", CompaniesEndpoint, endpoint);
            _logger.LogDebug("Updated company notifications to: {Model}", ret);
            return ret;
        }

        public async Task<IDictionary<string, string>> SetCustomerAttributesAsync(string customerId,
            IDictionary<string, string> attributes)
        {
            var id = HttpUriBuilder.NormalizeUriComponent(customerId);
            _logger.LogDebug("Making a request to set or update the customer's with ID: {Id} attributes: {Attributes}", customerId, attributes.StringifyDictionary());
            var ret = await MakeRequestAsync<Dictionary<string, string>, string>("POST", attributes, CustomersEndpoint, id,
                CustomersAttributesPath);
            _logger.LogDebug("Updated for the customer with ID: {Id} attributes: {Attributes}", customerId, ret.StringifyDictionary());
            return ret;
        }

        public async Task<Customer> SetCustomerInfoAsync(Customer customer)
        {
            var id = HttpUriBuilder.NormalizeUriComponent(customer.RoutableId);
            customer.AssertHasMandatoryFields(Customer.CountryName);
            _logger.LogDebug("Making a request to update the customer's with ID: {Id} information: {Model}", customer.RoutableId, customer);
            var ret = await MakeRequestAsync<Customer, object>("PUT", customer.SendableDictionary, CustomersEndpoint, id);
            _logger.LogDebug("Updated for the customer with ID: {Id} information: {Model}", customer.RoutableId, ret);
            return ret;
        }

        public async Task<Customer> SetNewCustomerAsync(Customer customer)
        {
            customer.AssertHasMandatoryFields(Customer.NameName, Customer.VatNumberName, Customer.CountryName);
            _logger.LogDebug("Making a request to create a new customer with information: {Model}", customer);
            var ret = await MakeRequestAsync<Customer, object>("POST", customer.SendableDictionary, CustomersEndpoint);
            _logger.LogDebug("Created a new customer with the information: {Model}", ret);
            return ret;
        }

        public async Task<Debt> SetNewDebtAsync(Debt debt)
        {
            debt.AssertHasMandatoryFields(Debt.NumberName, Debt.CustomerIdName, Debt.TypeName, Debt.DateName,
                Debt.DueDateName);
            debt.AssertItemsHaveMandatoryFields(Item.NameName);
            _logger.LogDebug("Making a request to create a new debt with information: {Model}", debt);
            var ret = await MakeRequestAsync<Debt, object>("POST", debt.SendableDictionary, DebtsEndpoint);
            _logger.LogDebug("Created a new debt with the information: {Model}", ret);
            return ret;
        }

        private async Task<TReturn> MakeBodylessRequestAsync<TReturn>(string method, params string[] pathFragments)
            where TReturn : new()
        {
            return await MakeRequestAsync<TReturn, object>(method, null, pathFragments);
        }

        /// <summary>
        ///     Makes an api request
        /// </summary>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="method"></param>
        /// <param name="requestBody"> if null the request doesn't have a body</param>
        /// <param name="pathFragments"></param>
        /// <returns></returns>
        private async Task<TReturn> MakeRequestAsync<TReturn, TDictValue>(string method,
            IDictionary<string, TDictValue> requestBody = null, params string[] pathFragments) where TReturn : new()
        {
            try
            {
                var requestJson = requestBody is null ? null : _jsonFacade.DictionaryToJson(requestBody);
                var requestUri = _uriBuilder.BuildUri(pathFragments);
                var json = await _apiFacade.CallApiAsync(requestUri, method, requestJson);
                return _jsonFacade.JsonToObject<TReturn>(json);
            }
            catch (System.Exception e)
            {
                _logger.LogError(e, "An InvisibleCollector error occured");
                throw;
            }
        }
    }
}