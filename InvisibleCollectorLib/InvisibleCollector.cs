using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using InvisibleCollectorLib.Connection;
using InvisibleCollectorLib.Exception;
using InvisibleCollectorLib.Json;
using InvisibleCollectorLib.Model;
using InvisibleCollectorLib.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace InvisibleCollectorLib
{
    /// <summary>
    ///     The entry point for this library.
    /// </summary>
    /// <remarks>
    ///     Contains all the possible requests. See the <see cref="Model" /> namespace for the possible arguments and return
    ///     values of the methods in this class.
    /// </remarks>
    public class InvisibleCollector
    {
        private const string CompaniesEndpoint = "companies";
        private const string CustomersAttributesPath = "attributes";
        private const string CustomersEndpoint = "customers";
        private const string DebtsEndpoint = "debts";
        private const string PaymentsEndpoint = "payments";
        private const string ProdutionUri = "https://api.invisiblecollector.com/";
        private readonly ApiConnectionFacade _apiFacade;
        private readonly JsonConvertFacade _jsonFacade;
        private readonly ILogger _logger;
        private readonly HttpUriBuilder _uriBuilder;

        /// <summary>
        ///     Build an instance
        /// </summary>
        /// <remarks>
        ///     You can specify a logger to this class such as the NLog with an adapter.
        /// </remarks>
        /// <param name="apiKey">The company API Key</param>
        /// <param name="remoteUri">The InvisibleCollector service address.</param>
        /// <param name="logger">The logger to be used by the lib</param>
        public InvisibleCollector(string apiKey, string remoteUri = ProdutionUri,
            ILogger<InvisibleCollector> logger = null)
        {
            _uriBuilder = new HttpUriBuilder(remoteUri);
            _jsonFacade = new JsonConvertFacade();
            _apiFacade = new ApiConnectionFacade(apiKey, _jsonFacade.JsonStreamToStringDictionary);
            _logger = logger ?? NullLogger<InvisibleCollector>.Instance;

            _logger.LogInformation("Started Instance");
        }

        /// <summary>
        ///     Same as
        ///     <see
        ///         cref="InvisibleCollector(string,string,Microsoft.Extensions.Logging.ILogger{InvisibleCollectorLib.InvisibleCollector})" />
        ///     but the <paramref name="remoteUri" /> in Uri format.
        /// </summary>
        /// <param name="remoteUri">The Invisible Collector service address</param>
        public InvisibleCollector(string apiKey, Uri remoteUri, ILogger<InvisibleCollector> logger = null) : this(
            apiKey, remoteUri.AbsoluteUri, logger)
        {
        }

        /// <summary>
        ///     Finishes any finalizations.
        /// </summary>
        ~InvisibleCollector()
        {
            _logger.LogInformation("Instance destroyed");
        }

        /// <summary>
        ///     Get the company's information.
        /// </summary>
        /// <returns>The up-to-date company information</returns>
        /// <exception cref="IcException">
        ///     On bad json (sent or received) and when the server rejects the request (conflict, bad
        ///     request, invalid parameters, etc)
        /// </exception>
        /// <exception cref="WebException">
        ///     On connection or protocol related errors (except for the protocol errors sent by the
        ///     Invisible Collector)
        /// </exception>
        /// <seealso cref="SetCompanyInfoAsync" />
        /// <seealso cref="SetCompanyNotificationsAsync" />
        public async Task<Company> GetCompanyInfoAsync()
        {
            _logger.LogDebug("Making a request to get company information");
            
            var ret = await MakeRequestAsync<Company>("GET", new[] {CompaniesEndpoint});
            
            _logger.LogDebug("Received company info: {Model}", ret);
            
            return ret;
        }

        /// <summary>
        ///     Get customer attributes.
        /// </summary>
        /// <param name="customerId">
        ///     The ID of the customer whose attributes are to be retrieved. It can be the 'gid' or
        ///     'externalId' of the customer (or just use <see cref="Customer.RoutableId" />)
        /// </param>
        /// <returns>The up-to-date customer attributes.</returns>
        /// <exception cref="IcException">
        ///     On bad json (sent or received) and when the server rejects the request (conflict, bad
        ///     request, invalid parameters, etc)
        /// </exception>
        /// <exception cref="WebException">
        ///     On connection or protocol related errors (except for the protocol errors sent by the
        ///     Invisible Collector)
        /// </exception>
        /// <seealso cref="SetCustomerAttributesAsync" />
        public async Task<IDictionary<string, string>> GetCustomerAttributesAsync(string customerId)
        {
            _logger.LogDebug("Making a request to get customer attributes for customer ID: {Id}", customerId);
            
            var ret = await MakeRequestAsync<Dictionary<string, string>>("GET", new[] {CustomersEndpoint, customerId, CustomersAttributesPath});
            
            _logger.LogDebug("Received for customer with id: {Id} attributes: {Attributes}", customerId,
                ret.StringifyDictionary());
            
            return ret;
        }

        /// <summary>
        ///     Get a list of the customer's debts
        /// </summary>
        /// <param name="customerId">
        ///     The ID of the customer whose debts are to be retrieved. It can be the 'gid' or 'externalId' of
        ///     the customer (or just use <see cref="Customer.RoutableId" />)
        /// </param>
        /// <returns>The up-to-date list of debts</returns>
        /// <exception cref="IcException">
        ///     On bad json (sent or received) and when the server rejects the request (conflict, bad
        ///     request, invalid parameters, etc)
        /// </exception>
        /// <exception cref="WebException">
        ///     On connection or protocol related errors (except for the protocol errors sent by the
        ///     Invisible Collector)
        /// </exception>
        /// <seealso cref="SetNewDebtAsync" />
        public async Task<IList<Debt>> GetCustomerDebtsAsync(string customerId)
        {
            _logger.LogDebug("Making a request to get customer debts for customer ID: {Id}", customerId);
            
            var ret = await MakeRequestAsync<List<Debt>>("GET", new[] {CustomersEndpoint, customerId, "debts"});
            
            _logger.LogDebug("Received for customer with id: {Id} debts: {Models}", customerId, ret.StringifyList());
            
            return ret;
        }

        /// <summary>
        ///     Get customer info
        /// </summary>
        /// <param name="customerId">
        ///     The ID of the customer whose information is to be retrieved. It can be the 'gid' or
        ///     'externalId' of the customer (or just use <see cref="Customer.RoutableId" />)
        /// </param>
        /// <returns>The up-to-date customer information</returns>
        /// <exception cref="IcException">
        ///     On bad json (sent or received) and when the server rejects the request (conflict, bad
        ///     request, invalid parameters, etc)
        /// </exception>
        /// <exception cref="WebException">
        ///     On connection or protocol related errors (except for the protocol errors sent by the
        ///     Invisible Collector)
        /// </exception>
        public async Task<Customer> GetCustomerInfoAsync(string customerId)
        {
            _logger.LogDebug("Making request to get customer information for customer ID: {Id}", customerId);
            
            var ret = await MakeRequestAsync<Customer>("GET", new[] {CustomersEndpoint, customerId});
            
            _logger.LogDebug("Received for customer with id: {Id} information: {Model}", customerId, ret);
            
            return ret;
        }

        /// <summary>
        ///     Get a debt
        /// </summary>
        /// <param name="debtId">The debt id, you can use <see cref="Debt.RoutableId" /></param>
        /// <returns>The up-to-date debt</returns>
        /// <exception cref="IcException">
        ///     On bad json (sent or received) and when the server rejects the request (conflict, bad
        ///     request, invalid parameters, etc)
        /// </exception>
        /// <exception cref="WebException">
        ///     On connection or protocol related errors (except for the protocol errors sent by the
        ///     Invisible Collector)
        /// </exception>
        /// <seealso cref="SetNewDebtAsync" />
        public async Task<Debt> GetDebtAsync(string debtId)
        {
            var id = HttpUriBuilder.UriEscape(debtId);
            _logger.LogDebug("Making request to get debt information for debt ID: {Id}", debtId);
            var ret = await MakeRequestAsync<Debt>("GET", new[] {DebtsEndpoint, id});
            _logger.LogDebug("Received for debt with id: {Id} information: {Model}", debtId, ret);
            return ret;
        }

        /// <summary>
        ///     Updates the company's information in the database
        /// </summary>
        /// <remarks>
        ///     You can use <see cref="GetCompanyInfoAsync" /> to get the <paramref name="company" /> fields used for validation.
        /// </remarks>
        /// <param name="company">
        ///     The company information to be updated. It the following mandatory fields used for validation:
        ///     <see cref="Company.Name" /> and <see cref="Company.VatNumber" />
        /// </param>
        /// <returns>The updated up-to-date company information </returns>
        /// <exception cref="IcException">
        ///     On bad json (sent or received) and when the server rejects the request (conflict, bad
        ///     request, invalid parameters, etc)
        /// </exception>
        /// <exception cref="WebException">
        ///     On connection or protocol related errors (except for the protocol errors sent by the
        ///     Invisible Collector)
        /// </exception>
        /// <seealso cref="GetCompanyInfoAsync" />
        /// <seealso cref="SetCompanyNotificationsAsync" />
        public async Task<Company> SetCompanyInfoAsync(Company company)
        {
            _logger.LogDebug("Making request to update company information with the following info: {Model}", company);
            var ret = await MakeRequestAsync<Company, object>("PUT", company.SendableDictionary, CompaniesEndpoint);
            _logger.LogDebug("Updated company with new information: {Model}", ret);
            return ret;
        }

        /// <summary>
        ///     Enables or disables the company notifications
        /// </summary>
        /// <param name="bEnableNotifications">set to <c>true</c> to enable notifications and <c>false</c> to disable</param>
        /// <returns>The updated up-to-date compay information</returns>
        /// <exception cref="IcException">
        ///     On bad json (sent or received) and when the server rejects the request (conflict, bad
        ///     request, invalid parameters, etc)
        /// </exception>
        /// <exception cref="WebException">
        ///     On connection or protocol related errors (except for the protocol errors sent by the
        ///     Invisible Collector)
        /// </exception>
        /// <seealso cref="GetCompanyInfoAsync" />
        /// <seealso cref="SetCompanyInfoAsync" />
        public async Task<Company> SetCompanyNotificationsAsync(bool bEnableNotifications)
        {
            const string EnableNotifications = "enableNotifications";
            const string DisableNotifications = "disableNotifications";

            _logger.LogDebug("Making request to {Model} notifications", bEnableNotifications ? "enable" : "disable");
            var endpoint = bEnableNotifications ? EnableNotifications : DisableNotifications;
            var ret = await MakeRequestAsync<Company>("PUT", new[] {CompaniesEndpoint, endpoint});
            _logger.LogDebug("Updated company notifications to: {Model}", ret);
            return ret;
        }

        /// <summary>
        ///     Updates the customer's attributes
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Any previously existing attributes won't be deleted, existing attributes will be updated and not-previously
        ///         existing attributes will be created.
        ///     </para>
        /// </remarks>
        /// <param name="customerId">
        ///     The ID of the customer whose information is to be retrieved. It can be the 'gid' or
        ///     'externalId' of the customer (or just use <see cref="Customer.RoutableId" />)
        /// </param>
        /// <param name="attributes">The attributes to be set</param>
        /// <returns>All of the customer's up-to-date updated attributes</returns>
        /// <exception cref="IcException">
        ///     On bad json (sent or received) and when the server rejects the request (conflict, bad
        ///     request, invalid parameters, etc)
        /// </exception>
        /// <exception cref="WebException">
        ///     On connection or protocol related errors (except for the protocol errors sent by the
        ///     Invisible Collector)
        /// </exception>
        /// <seealso cref="GetCustomerAttributesAsync" />
        public async Task<IDictionary<string, string>> SetCustomerAttributesAsync(string customerId,
            IDictionary<string, string> attributes)
        {
            var id = HttpUriBuilder.UriEscape(customerId);
            _logger.LogDebug("Making a request to set or update the customer's with ID: {Id} attributes: {Attributes}",
                customerId, attributes.StringifyDictionary());
            var ret = await MakeRequestAsync<Dictionary<string, string>, string>("POST", attributes, CustomersEndpoint,
                id,
                CustomersAttributesPath);
            _logger.LogDebug("Updated for the customer with ID: {Id} attributes: {Attributes}", customerId,
                ret.StringifyDictionary());
            return ret;
        }

        /// <summary>
        ///     Updates the customer's information.
        /// </summary>
        /// <param name="customer">
        ///     The customer information to be updated. The <see cref="Customer.Gid" /> or
        ///     <see cref="Customer.ExternalId" /> field must be set, since they contain the id of the customer. The
        ///     <see cref="Customer.Country" /> field is mandatory.
        /// </param>
        /// <returns>The up-to-date updated customer information</returns>
        /// <exception cref="IcException">
        ///     On bad json (sent or received) and when the server rejects the request (conflict, bad
        ///     request, invalid parameters, etc)
        /// </exception>
        /// <exception cref="WebException">
        ///     On connection or protocol related errors (except for the protocol errors sent by the
        ///     Invisible Collector)
        /// </exception>
        /// <seealso cref="GetCustomerInfoAsync" />
        /// <seealso cref="SetNewCustomerAsync" />
        /// <seealso cref="Customer.RoutableId" />
        public async Task<Customer> SetCustomerInfoAsync(Customer customer)
        {
            var id = HttpUriBuilder.UriEscape(customer.RoutableId);
            _logger.LogDebug("Making a request to update the customer's with ID: {Id} information: {Model}",
                customer.RoutableId, customer);
            var ret = await MakeRequestAsync<Customer, object>("PUT", customer.SendableDictionary, CustomersEndpoint,
                id);
            _logger.LogDebug("Updated for the customer with ID: {Id} information: {Model}", customer.RoutableId, ret);
            return ret;
        }

        /// <summary>
        ///     Create a new customer.
        /// </summary>
        /// <param name="customer">
        ///     The customer to be created. The <see cref="Customer.Name" />, <see cref="Customer.VatNumber" />
        ///     and <see cref="Customer.Country" /> fields are mandatory.
        /// </param>
        /// <returns>The up-to-date created customer.</returns>
        /// <exception cref="IcException">
        ///     On bad json (sent or received) and when the server rejects the request (conflict, bad
        ///     request, invalid parameters, etc)
        /// </exception>
        /// <exception cref="WebException">
        ///     On connection or protocol related errors (except for the protocol errors sent by the
        ///     Invisible Collector)
        /// </exception>
        /// <exception cref="IcModelConflictException">
        ///     When a customer with the same customer Gid or externalId already exists in
        ///     the database.
        /// </exception>
        public async Task<Customer> SetNewCustomerAsync(Customer customer)
        {
            _logger.LogDebug("Making a request to create a new customer with information: {Model}", customer);
            var ret = await MakeRequestAsync<Customer, object>("POST", customer.SendableDictionary, CustomersEndpoint);
            _logger.LogDebug("Created a new customer with the information: {Model}", ret);
            return ret;
        }

        /// <summary>
        ///     Create a new debt related to a customer.
        /// </summary>
        /// <param name="debt">
        ///     The debt to be created. The <see cref="Debt.Number" />, <see cref="Debt.CustomerId" />,
        ///     <see cref="Debt.Type" />, <see cref="Debt.Date" /> and <see cref="Debt.DueDate" /> fields are mandatory. If it has
        ///     items (<see cref="Debt.Items" />) they must have the <see cref="Item.Name" /> field
        /// </param>
        /// <returns>The up-to-date created debt.</returns>
        /// <exception cref="IcException">
        ///     On bad json (sent or received) and when the server rejects the request (conflict, bad
        ///     request, invalid parameters, etc)
        /// </exception>
        /// <exception cref="WebException">
        ///     On connection or protocol related errors (except for the protocol errors sent by the
        ///     Invisible Collector)
        /// </exception>
        /// <exception cref="IcModelConflictException">When a debt with the same number already exists.</exception>
        public async Task<Debt> SetNewDebtAsync(Debt debt)
        {
            _logger.LogDebug("Making a request to create a new debt with information: {Model}", debt);
            
            var ret = await MakeRequestAsync<Debt, object>("POST", debt.SendableDictionary, DebtsEndpoint);
            
            _logger.LogDebug("Created a new debt with the information: {Model}", ret);
            return ret;
        }
        
        /// <summary>
        ///     Create a new debit associated with a debt
        /// </summary>
        /// <param name="debtGid">the debt global id <see cref="Debt.Id" /></param>
        /// <param name="debit">the debit to create</param>
        /// <returns></returns>
        public async Task<Debit> SetNewDebtDebitAsync(string debtGid, Debit debit)
        {
            _logger.LogDebug("Making a request to create a new debt debit note with information: {Model}", debit);
            var ret = await MakeRequestAsync<Debit, object>("POST", debit.FieldsShallow, DebtsEndpoint, debtGid, "debits");
            _logger.LogDebug("Created a new debt debit note with the information: {Model}", ret);
            return ret;
        }

        /// <summary>
        ///     Get the debts that math the search query
        /// </summary>
        /// <param name="findDebts">the search query</param>
        /// <returns>the debts list</returns>
        /// <exception cref="IcException">
        ///     On bad json (received) and when the server rejects the request (conflict, bad
        ///     request, invalid parameters, etc)
        /// </exception>
        /// <exception cref="WebException">
        ///     On connection or protocol related errors (except for the protocol errors sent by the
        ///     Invisible Collector)
        /// </exception>
        public async Task<IList<Debt>> GetFindDebtsAsync(FindDebts findDebts)
        {
            _logger.LogDebug("Making request to find debts with the following info: {Model}", findDebts);

            var ret = await MakeRequestAsync<List<Debt>>("GET", new[] {DebtsEndpoint, "find"}, findDebts.SendableStringDictionary);

            _logger.LogDebug("Received find result debts: {Models}", ret.StringifyList());
            return ret;
        }

        public async Task<IList<Group>> GetGroupsAsync()
        {
            _logger.LogDebug("Making request to get list of groups");

            var ret = await MakeRequestAsync<List<Group>>("GET", new[] {"groups"});

            _logger.LogDebug("Received groups list: {Models}", ret.StringifyList());
            return ret;
        }

        public async Task<Group> SetCustomerToGroupAsync(string customerId, string groupId)
        {
            _logger.LogDebug($"Making request to get set customer ({customerId}) to group ({groupId})");

            var ret = await MakeRequestAsync<Group>("POST", new[] {"groups", groupId, "customers", customerId});

            _logger.LogDebug("Added customer {Customer} to group: {Models}",  customerId, ret);
            return ret;
        }
        
        public async Task<IList<Customer>> GetFindCustomersAsync(FindCustomers findCustomers)
        {
            _logger.LogDebug("Making request to find customers with the following info: {Model}", findCustomers);
            
            var ret = await MakeRequestAsync<List<Customer>>("GET", new[] {CustomersEndpoint, "find"}, findCustomers.SendableStringDictionary);

            _logger.LogDebug("Received find result customers: {Models}", ret.StringifyList());
            return ret;
        }

        /// <summary>
        /// Create a new payment.
        /// </summary>
        /// <param name="payment">
        ///     The payment to be created. The <see cref="Payment.Number" />, <see cref="Payment.Status" />,
        ///     <see cref="Payment.Type" />, <see cref="Payment.Date" />, <see cref="Debt.DueDate" /> and <see cref="Payment.Lines" /> fields are mandatory. 
        ///     Lines must have the <see cref="PaymentLine.Number" /> and <see cref="PaymentLine.Amount" /> fields.
        /// </param>
        /// <returns>
        ///    The up-to-date created payment
        /// </returns>
        /// <exception cref="IcException">
        ///     On bad json (sent or received) and when the server rejects the request (conflict, bad
        ///     request, invalid parameters, etc)
        /// </exception>
        /// <exception cref="WebException">
        ///     On connection or protocol related errors (except for the protocol errors sent by the
        ///     Invisible Collector)
        /// </exception>
        /// <exception cref="IcModelConflictException">When a payment with the same number already exists.</exception>
        public async Task<Payment> SetNewPayment(Payment payment)
        {
            _logger.LogDebug("Making a request to create a new payment with information: {Model}", payment);

            var ret = await MakeRequestAsync<Payment, object>("POST", payment.SendableDictionary, PaymentsEndpoint);
            
            _logger.LogDebug("Created a new payment with the information: {Model}", ret);
            
            return ret;
        }

        /// <summary>
        /// Get the payment specified by id.
        /// </summary>
        /// <param name="paymentId">The <see cref="Payment.ExternalId"/> of the payment.</param>
        /// <returns>
        ///     The up-to-date payment
        /// </returns>
        /// <exception cref="IcException">
        ///     On bad json (sent or received) and when the server rejects the request (conflict, bad
        ///     request, invalid parameters, etc)
        /// </exception>
        /// <exception cref="WebException">
        ///     On connection or protocol related errors (except for the protocol errors sent by the
        ///     Invisible Collector)
        /// </exception>
        public async Task<Payment> GetPaymentAsync(string paymentId)
        {
            _logger.LogDebug("Making request to get payment information for payment ID: {Id}", paymentId);
            
            var ret = await MakeRequestAsync<Payment>("GET", new[] {PaymentsEndpoint, paymentId});
            
            _logger.LogDebug("Received for payment with id: {Id} information: {Model}", paymentId, ret);
            return ret;
        }

        /// <summary>
        /// Cancel the payment.
        /// </summary>
        /// <param name="paymentId">The <see cref="Payment.ExternalId"/> of the payment.</param>
        /// <returns>
        ///     The up-to-date canceled payment
        /// </returns>
        /// <exception cref="IcException">
        ///     On bad json (sent or received) and when the server rejects the request (conflict, bad
        ///     request, invalid parameters, etc)
        /// </exception>
        /// <exception cref="WebException">
        ///     On connection or protocol related errors (except for the protocol errors sent by the
        ///     Invisible Collector)
        /// </exception>
        public async Task<Payment> CancelPaymentAsync(string paymentId)
        {
            _logger.LogDebug("Making request to cancel payment information for payment ID: {Id}", paymentId);
            
            var ret = await MakeRequestAsync<Payment>("PUT", new[] {PaymentsEndpoint, paymentId, "cancel"});
            
            _logger.LogDebug("Received for payment with id: {Id} information: {Model}", paymentId, ret);
            return ret;
        }

        /// <summary>
        /// Delete the payment. Not idempotent.
        /// </summary>
        /// <param name="paymentId">The <see cref="Payment.ExternalId"/> of the payment.</param>
        /// <returns>
        ///     The deleted payment.
        /// </returns>
        /// <exception cref="IcException">
        ///     On bad json (sent or received) and when the server rejects the request (conflict, bad
        ///     request, invalid parameters, etc)
        /// </exception>
        /// <exception cref="WebException">
        ///     On connection or protocol related errors (except for the protocol errors sent by the
        ///     Invisible Collector)
        /// </exception>
        public async Task<Payment> DeletePaymentAsync(string paymentId)
        {
            _logger.LogDebug("Making request to delete payment information for payment ID: {Id}", paymentId);
            
            var ret = await MakeRequestAsync<Payment>("DELETE", new [] {PaymentsEndpoint, paymentId});
            
            _logger.LogDebug("Received for payment with id: {Id} information: {Model}", paymentId, ret);
            return ret;
        }

        /// <summary>
        /// Sets new or updates customer contacts. Contacts are identified by their <see cref="CustomerContact.Name"/>
        /// </summary>
        /// <param name="customerGid">custoemr gid</param>
        /// <param name="contacts">customer contacts</param>
        /// <returns>the up-to-date customer</returns>
        public async Task<Customer> SetNewCustomerContactsAsync(string customerGid,
            IList<CustomerContact> contacts)
        {
            _logger.LogDebug("Making request to create customer's {Id} contacts with the following info: {Model}", customerGid, contacts);

            var objects = contacts.Select(c => c.SendableDictionary);
            var json = _jsonFacade.DictionaryToJson(objects);
            var ret = await MakeRequestAsync<Customer>("POST", new[] {"v1", "customers", customerGid, "contacts"}, null, json);

            _logger.LogDebug("Received customer: {Models}", ret);
            return ret;
        }
        
        public async Task<IList<CustomerContact>> GetCustomerContactsAsync(string customerGid)
        {
            _logger.LogDebug("Making request to get customer's {} contacts'", customerGid);
            
            var ret = await MakeRequestAsync<List<CustomerContact>>("GET", new[]
                {"v1", "customers", customerGid, "contacts"});
            
            _logger.LogDebug("Received contacts: {Model} for customer {Id}", ret, customerGid);
            return ret;
        }

        private async Task<TReturn> MakeRequestAsync<TReturn>(string method, string[] pathFragments, IDictionary<string, string> query = null, string requestJson = null) where TReturn : new()
        {
            var requestUri = _uriBuilder.Clone().WithPath(pathFragments);
            if (query != null)
            {
                requestUri.WithQuery(query);
            }
            var json = await _apiFacade.CallJsonToJsonApi(requestUri.BuildUri(), method, requestJson);
            return _jsonFacade.JsonToObject<TReturn>(json);
        }

        /// <summary>
        ///     Makes an api request
        /// </summary>
        /// <typeparam name="TReturn"></typeparam>
        /// <typeparam name="TDictValue"></typeparam>
        /// <param name="method"></param>
        /// <param name="requestBody"> if null the request doesn't have a body</param>
        /// <param name="pathFragments"></param>
        /// <returns></returns>
        private async Task<TReturn> MakeRequestAsync<TReturn, TDictValue>(string method,
            IDictionary<string, TDictValue> requestBody = null, params string[] pathFragments) where TReturn : new()
        {
            string requestJson = requestBody is null ? null : _jsonFacade.DictionaryToJson(requestBody);
            return await MakeRequestAsync<TReturn>(method, pathFragments, null, requestJson);
        }
    }
}