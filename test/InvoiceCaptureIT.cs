using System;
using System.Net;
using System.Threading.Tasks;
using InvoiceCaptureLib;
using InvoiceCaptureLib.Model;
using NUnit.Framework;
using test.Model;

namespace test
{
    [TestFixture]
    internal class InvoiceCaptureIt : MockServerTesterBase
    {
        private InvoiceCapture ConfigureIc(string method, string path, string returnedJson,
            string expectedJson = null)
        {
            _mockServer.AddRequest(method, path, expectedJson,
                    expectedJson is null ? BodylessHeaders : BodiedHeaders, expectedJson is null? BodyHeaderDifference : null)
                .AddJsonResponse(returnedJson);

            var uri = _mockServer.GetUrl();
            return new InvoiceCapture(TestApiKey, uri);
        }

        private void AssertingRequest<TModel>(string method, string path, ModelBuilder replyModelBuilder,
            Func<InvoiceCapture, Task<TModel>> requestMethod, string expectedJson = null)
            where TModel : InvoiceCaptureLib.Model.Model, new()
        {
            var returnedJson = replyModelBuilder.buildJson();
            var expectedModel = replyModelBuilder.buildModel<TModel>();

            var ic = ConfigureIc(method, path, returnedJson, expectedJson);
            var result = requestMethod(ic).Result;
            Assert.AreEqual(expectedModel, result);
        }

        [Test]
        public void RequestCompanyInfoAsync_correct()
        {
            var builder = ModelBuilder.BuildReplyCompanyBuilder();
            AssertingRequest("GET", "companies", builder,
                async ic => await ic.RequestCompanyInfoAsync());
        }


        [Test]
        public void RequestCompanyInfoAsync_fail404()
        {
            var ic = ConfigureIc("GET", "someunreachablepath", "{}");
            Assert.ThrowsAsync<WebException>(() => ic.RequestCompanyInfoAsync());
        }

        [Test]
        public void RequestCompanyInfoAsync_failRefuseConnection()
        {
            var uri = new Uri("http://localhost:56087"); //shouldn't be in use
            Assert.ThrowsAsync<WebException>(() => new InvoiceCapture(TestApiKey, uri).RequestCompanyInfoAsync());
        }

        [Test]
        public void SetCompanyNotifications_correct()
        {
            var builder = ModelBuilder.BuildReplyCompanyBuilder();
            builder[Company.NotificationsName] = true;
            AssertingRequest("PUT", "companies/enableNotifications", builder,
                async ic => await ic.SetCompanyNotificationsAsync(true));

            builder[Company.NotificationsName] = false;
            AssertingRequest("PUT", "companies/disableNotifications", builder,
                async ic => await ic.SetCompanyNotificationsAsync(false));
        }

        [Test]
        public void UpdateCompanyInfoAsync_correct()
        {
            var replyBuilder = ModelBuilder.BuildReplyCompanyBuilder();
            var requestBuilder = ModelBuilder.BuildRequestCompanyBuilder(); 
            AssertingRequest("PUT", "companies", replyBuilder,
                async ic => await ic.UpdateCompanyInfoAsync(requestBuilder.buildModel<Company>()),
                requestBuilder.buildJson());
        }



    }
}