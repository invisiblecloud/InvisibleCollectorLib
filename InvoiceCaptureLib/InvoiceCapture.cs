using System;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net.Security;
using System.Text;
using System.Collections.Generic;

namespace InvoiceCaptureLib
{
    public class InvoiceCapture
    {
        private String baseEndPoint = "https://api.invcapture.com/";
        private String ApiKey = "";

        private String companyEndPoint = "companies";
        private String customerEndPoint = "customers";
        private String invoicesEndPoint = "invoices";
        private String creditNoteEndPoint = "credit_notes";
        private String paymentsEndPoint = "payments";

        private bool checkURI(String value)
        {
            Uri uriResult;
            return Uri.TryCreate(value, UriKind.Absolute, out uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

        public string BaseEndPoint
        {
            get
            {
                return baseEndPoint;
            }

            set
            {
                if (checkURI(value))
                    baseEndPoint = value;
                else
                    throw new UriFormatException("Not a valid URI");
            }
        }

        public string ApiKey1 { get => ApiKey; set =>ApiKey = value; }

        public InvoiceCapture()
        {
            baseEndPoint = "https://api.invcapture.com/";
        }

        public InvoiceCapture(bool isProduction, String myAPIKey)
        {
            this.ApiKey = myAPIKey;
            if(isProduction)
            { baseEndPoint = "https://api.invcapture.com/"; }
            else
            {
                baseEndPoint = "https://api.nxt.invcapture.com/";
                InitiateSSLTrust();
            }

        }

        public static void InitiateSSLTrust()
        {
            try
            {
                //Change SSL checks so that all checks pass
                ServicePointManager.ServerCertificateValidationCallback =
                   new RemoteCertificateValidationCallback(
                        delegate
                        { return true; }
                    );
            }
            catch (Exception){}
        }

        public InvoiceCapture(String myAPIKey, String myBaseEndpoint)
        {
            this.BaseEndPoint = myAPIKey;
            this.BaseEndPoint = myBaseEndpoint;
        }

        private WebClient getWebClient()
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);
            var client = new WebClient();
            client.Headers.Set("Content-Type", "application/json");
            client.Headers.Set("X-Api-Token", ApiKey1);
            client.Encoding = Encoding.UTF8;
            return client;
        }

        private String callAPI(String endpoint, String jsonString, String method)
        {
            String response = "";
            WebClient client = getWebClient();
            try
            {
                if (method == "POST" || method == "PUT" || method =="DELETE")
                {
                    response = client.UploadString(BaseEndPoint + endpoint, method, jsonString);
                }
                else if(method == "GET")
                {
                    response = client.DownloadString(BaseEndPoint + endpoint);
                }

            }
            catch (WebException e)
            {
                var responseStream = e.Response?.GetResponseStream();
                if (responseStream != null)
                {
                    using (var reader = new StreamReader(responseStream))
                    {
                        response = reader.ReadToEnd();
                        InvoiceCaptureError error = JsonConvert.DeserializeObject<InvoiceCaptureError>(response);
                        throw (new InvoiceCaptureException(error.Code+" "+error.Message));
                    }
                }
            }

            return response;
        }

        public Company getCompany()
        {
            String json = callAPI(companyEndPoint, "","GET");
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Company>(json);
        }

        public Customer createCustomer(Customer c)
        {
            String jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(c, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
            jsonString = callAPI(customerEndPoint, jsonString, "POST");
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Customer>(jsonString);
        }

        public Customer updateCustomer(Customer c, String id)
        {
            String jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(c, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
            jsonString = callAPI(customerEndPoint + "/" + id, jsonString, "PUT");
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Customer>(jsonString);
        }

        public Customer getCustomer(String id)
        {
            String jsonString = callAPI(customerEndPoint+"/"+id, "", "GET");
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Customer>(jsonString);
        }

        public List<Invoice> getCustomerDebts(String id)
        {
            String jsonString = callAPI(customerEndPoint + "/" + id+"/debts", "", "GET");
            return Newtonsoft.Json.JsonConvert.DeserializeObject<List<Invoice>>(jsonString);
        }

        public Invoice createInvoice(Invoice i)
        {
            WebClient client = getWebClient();
            JsonSerializerSettings s = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            String jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(i, s);
            jsonString = callAPI(invoicesEndPoint, jsonString, "POST");
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Invoice>(jsonString);
        }

        public Invoice getInvoice(String id)
        {
          String jsonString = callAPI(invoicesEndPoint + "/" + id, "", "GET");
          return Newtonsoft.Json.JsonConvert.DeserializeObject<Invoice>(jsonString);
        }

        public Invoice cancelInvoice(String id)
        {
            String jsonString = callAPI(invoicesEndPoint + "/" + id + "/cancel", "", "PUT");
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Invoice>(jsonString);
        }

        public CreditNote cancelCreditNote(String id)
        {
            String jsonString = callAPI(creditNoteEndPoint + "/" + id + "/cancel", "", "PUT");
            return Newtonsoft.Json.JsonConvert.DeserializeObject<CreditNote>(jsonString);
        }

        public Payment cancelPayment(String id)
        {
            String jsonString = callAPI(paymentsEndPoint + "/" + id + "/cancel", "", "PUT");
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Payment>(jsonString);
        }

        public Payment deletePayment(String id)
        {
            String jsonString = callAPI(paymentsEndPoint + "/" + id, "", "DELETE");
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Payment>(jsonString);
        }

        public CreditNote deleteCreditNote(String id)
        {
            String jsonString = callAPI(creditNoteEndPoint + "/" + id, "", "DELETE");
            return Newtonsoft.Json.JsonConvert.DeserializeObject<CreditNote>(jsonString);
        }

        public CreditNote createCreditNote(CreditNote c)
        {
            WebClient client = getWebClient();
            JsonSerializerSettings s = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            String jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(c, s);
            jsonString = callAPI(creditNoteEndPoint, jsonString, "POST");
            return Newtonsoft.Json.JsonConvert.DeserializeObject<CreditNote>(jsonString);
        }
        public Payment createPayment(Payment p)
        {
            String jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(p, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
            jsonString = callAPI(paymentsEndPoint, jsonString, "POST");
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Payment>(jsonString);
        }
    }
}
