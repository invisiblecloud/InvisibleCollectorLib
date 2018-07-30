using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace InvoiceCaptureLib
{
    public class InvoiceCapture
    {
        
        private const string companyEndPoint = "companies";
        private const string customerEndPoint = "customers";
        private const string invoicesEndPoint = "invoices";
        private const string creditNoteEndPoint = "credit_notes";
        private const string paymentsEndPoint = "payments";

        private string _baseEndPoint = "https://api.invisiblecollector.com/";

        public InvoiceCapture(bool isProduction, string myAPIKey)
        {
            ApiKey = myAPIKey;
            if (!isProduction)
            {
                _baseEndPoint = "https://api.nxt.invisiblecollector.com/";
                InitiateSSLTrust();
            }
        }

        public InvoiceCapture(string myAPIKey, string myBaseEndpoint)
        {
            ApiKey = myAPIKey;
            BaseEndPoint = myBaseEndpoint;
        }

        public string ApiKey { get; set; } = "";

        public string BaseEndPoint
        {
            get => _baseEndPoint;

            set
            {
                if (checkURI(value))
                    _baseEndPoint = value;
                else
                    throw new UriFormatException("Not a valid URI");
            }
        }

        public CreditNote cancelCreditNote(string id)
        {
            var jsonString = callAPI(creditNoteEndPoint + "/" + id + "/cancel", "", "PUT");
            return JsonConvert.DeserializeObject<CreditNote>(jsonString);
        }

        public Invoice cancelInvoice(string id)
        {
            var jsonString = callAPI(invoicesEndPoint + "/" + id + "/cancel", "", "PUT");
            return JsonConvert.DeserializeObject<Invoice>(jsonString);
        }

        public Payment cancelPayment(string id)
        {
            var jsonString = callAPI(paymentsEndPoint + "/" + id + "/cancel", "", "PUT");
            return JsonConvert.DeserializeObject<Payment>(jsonString);
        }

        public CreditNote createCreditNote(CreditNote c)
        {
            var client = getWebClient();
            var s = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            var jsonString = JsonConvert.SerializeObject(c, s);
            jsonString = callAPI(creditNoteEndPoint, jsonString, "POST");
            return JsonConvert.DeserializeObject<CreditNote>(jsonString);
        }

        public List<Reference> createCreditNoteReferences(CreditNote c, List<Reference> references)
        {
            return createCreditNoteReferences(c.ExternalId, references);
        }

        public List<Reference> createCreditNoteReferences(string id, List<Reference> references)
        {
            var client = getWebClient();
            var s = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            var jsonString = JsonConvert.SerializeObject(references, s);
            jsonString = callAPI(creditNoteEndPoint + "/" + id + "/references", jsonString, "POST");
            return JsonConvert.DeserializeObject<List<Reference>>(jsonString);
        }

        public Customer createCustomer(Customer c)
        {
            var jsonString = JsonConvert.SerializeObject(c,
                new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()});
            jsonString = callAPI(customerEndPoint, jsonString, "POST");
            return JsonConvert.DeserializeObject<Customer>(jsonString);
        }

        public Invoice createInvoice(Invoice i)
        {
            var client = getWebClient();
            var s = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            var jsonString = JsonConvert.SerializeObject(i, s);
            jsonString = callAPI(invoicesEndPoint, jsonString, "POST");
            return JsonConvert.DeserializeObject<Invoice>(jsonString);
        }

        public Payment createPayment(Payment p)
        {
            var jsonString = JsonConvert.SerializeObject(p,
                new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()});
            jsonString = callAPI(paymentsEndPoint, jsonString, "POST");
            return JsonConvert.DeserializeObject<Payment>(jsonString);
        }

        public CreditNote deleteCreditNote(string id)
        {
            var jsonString = callAPI(creditNoteEndPoint + "/" + id, "", "DELETE");
            return JsonConvert.DeserializeObject<CreditNote>(jsonString);
        }

        public Payment deletePayment(string id)
        {
            var jsonString = callAPI(paymentsEndPoint + "/" + id, "", "DELETE");
            return JsonConvert.DeserializeObject<Payment>(jsonString);
        }

        public Company getCompany()
        {
            var json = callAPI(companyEndPoint, "", "GET");
            return JsonConvert.DeserializeObject<Company>(json);
        }

        public CreditNote getCreditNote(string id)
        {
            var jsonString = callAPI(creditNoteEndPoint + "/" + id, "", "GET");
            return JsonConvert.DeserializeObject<CreditNote>(jsonString);
        }

        public Customer getCustomer(string id)
        {
            var jsonString = callAPI(customerEndPoint + "/" + id, "", "GET");
            return JsonConvert.DeserializeObject<Customer>(jsonString);
        }

        public List<Invoice> getCustomerDebts(string id)
        {
            var jsonString = callAPI(customerEndPoint + "/" + id + "/debts", "", "GET");
            return JsonConvert.DeserializeObject<List<Invoice>>(jsonString);
        }

        public Invoice getInvoice(string id)
        {
            var jsonString = callAPI(invoicesEndPoint + "/" + id, "", "GET");
            return JsonConvert.DeserializeObject<Invoice>(jsonString);
        }

        public List<Payment> getInvoicePayments(string id)
        {
            var jsonString = callAPI(invoicesEndPoint + "/" + id + "/payments", "", "GET");
            return JsonConvert.DeserializeObject<List<Payment>>(jsonString);
        }


        public static void InitiateSSLTrust()
        {
            try
            {
                //Change SSL checks so that all checks pass
                ServicePointManager.ServerCertificateValidationCallback =
                    delegate { return true; };
            }
            catch (Exception)
            {
            }
        }

        public Customer updateCustomer(Customer c, string id)
        {
            var jsonString = JsonConvert.SerializeObject(c,
                new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()});
            jsonString = callAPI(customerEndPoint + "/" + id, jsonString, "PUT");
            return JsonConvert.DeserializeObject<Customer>(jsonString);
        }

        private string callAPI(string endpoint, string jsonString, string method)
        {
            var response = "";
            var client = getWebClient();
            try
            {
                if (method == "POST" || method == "PUT" || method == "DELETE")
                    response = client.UploadString(BaseEndPoint + endpoint, method, jsonString);
                else if (method == "GET") response = client.DownloadString(BaseEndPoint + endpoint);
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