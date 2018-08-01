using System;
using System.Threading.Tasks;
using InvoiceCaptureLib.Connection;
using InvoiceCaptureLib.Model;
using InvoiceCaptureLib.Model.Json;

namespace InvoiceCaptureLib
{
    public class InvoiceCapture
    {
        private const string CompanyEndPoint = "companies";
        private const string CustomerEndPoint = "customers";
        private const string DebtsEndpoint = "debts";
        private const string ProdutionUri = "https://api.invisiblecollector.com/";
        private readonly JsonModelConverterFacade _jsonFacade;
        private readonly ApiConnectionFacade _connectionFacade;
        private readonly HttpUriBuilder _uriBuilder;


        public InvoiceCapture(string apiKey, string remoteUri = ProdutionUri) : this(new HttpUriBuilder(remoteUri), new ApiConnectionFacade(apiKey), new JsonModelConverterFacade())
        {
        }

        public InvoiceCapture(string apiKey, Uri remoteUri) : this(new HttpUriBuilder(remoteUri), new ApiConnectionFacade(apiKey), new JsonModelConverterFacade())
        {
        }

        internal InvoiceCapture(HttpUriBuilder uriBuilder,
            ApiConnectionFacade connectionFacade,
            JsonModelConverterFacade jsonFacade)
        {
            _jsonFacade = jsonFacade;
            _uriBuilder = uriBuilder;
            _connectionFacade = connectionFacade;
        }

        public async Task<Company> RequestCompanyInfoAsync()
        {
            var requestUri = _uriBuilder.BuildUri(CompanyEndPoint);
            var json = await _connectionFacade.CallApiAsync(requestUri, "GET", null);
            return _jsonFacade.JsonToModel<Company>(json);
        }

        public async Task<Company> UpdateCompanyInfoAsync(Company company)
        {
            company.AssertHasMandatoryFields();
            var json = _jsonFacade.ModelToSendableJson(company);
            var requestUri = _uriBuilder.BuildUri(CompanyEndPoint);
            var returnedJson = await _connectionFacade.CallApiAsync(requestUri, "PUT", json);
            return _jsonFacade.JsonToModel<Company>(returnedJson);
        }
    }
}