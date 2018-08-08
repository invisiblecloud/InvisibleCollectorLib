using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Net;
using System.Threading.Tasks;
using InvisibleCollectorLib;
using InvisibleCollectorLib.Model;
using InvisibleCollectorLib.Utils;
using NUnit.Framework;
using test.Model;
using WireMock.Util;

namespace test
{
    [TestFixture]
    internal class InvisibleCollectorIt : MockServerTesterBase
    {
        private const string TestId = "1234";

        private InvisibleCollector ConfigureIc(string expectedMethod, string expectedPath, string responseJson,
            string expectedJson = null)
        {
            _mockServer.AddRequest(expectedMethod, expectedPath, expectedJson,
                    expectedJson is null ? BodylessHeaders : BodiedHeaders,
                    expectedJson is null ? BodyHeaderDifference : null)
                .AddJsonResponse(responseJson);

            var uri = _mockServer.GetUrl();
            return new InvisibleCollector(TestApiKey, uri);
        }

        private void AssertingRequest<TModel>(string expectedMethod, string expectedPath,
            ModelBuilder replyModelBuilder,
            Func<InvisibleCollector, Task<TModel>> requestMethod, string expectedJson = null)
            where TModel : InvisibleCollectorLib.Model.Model, new()
        {
            var returnedJson = replyModelBuilder.BuildJson();
            // maybe not strip nulls
            var expectedModel = replyModelBuilder.BuildModel<TModel>(true);

            var ic = ConfigureIc(expectedMethod, expectedPath, returnedJson, expectedJson);
            var result = requestMethod(ic).Result;
            Assert.AreEqual(expectedModel, result);
        }

        [Test]
        public void RegisterNewCustomerAsync_correct()
        {
            var request = ModelBuilder.BuildRequestCustomerBuilder();
            var reply = ModelBuilder.BuildRequestCustomerBuilder();
            AssertingRequest("POST", $"customers", reply,
                async ic => await ic.SetNewCustomerAsync(request.BuildModel<Customer>()),
                request.BuildJson());
        }

        [Test]
        public void RequestCompanyInfoAsync_correct()
        {
            var builder = ModelBuilder.BuildReplyCompanyBuilder();
            AssertingRequest("GET", "companies", builder,
                async ic => await ic.GetCompanyInfoAsync());
        }


        [Test]
        public void RequestCompanyInfoAsync_fail404()
        {
            var ic = ConfigureIc("GET", "someunreachablepath", "{}");
            Assert.ThrowsAsync<WebException>(() => ic.GetCompanyInfoAsync());
        }

        [Test]
        public void RequestCompanyInfoAsync_failRefuseConnection()
        {
            var uri = new Uri("http://localhost:56087"); //shouldn't be in use
            Assert.ThrowsAsync<WebException>(() => new InvisibleCollector(TestApiKey, uri).GetCompanyInfoAsync());
        }

        [Test]
        public void RequestCustomerInfoAsync_correct()
        {
            var builder = ModelBuilder.BuildReplyCustomerBuilder();
            AssertingRequest("GET", $"customers/{TestId}", builder,
                async ic => await ic.GetCustomerInfoAsync(TestId));
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
                async ic => await ic.SetCompanyInfoAsync(requestBuilder.BuildModel<Company>()),
                requestBuilder.BuildJson());
        }

        [Test]
        public void UpdateCustomerInfoAsync_correct()
        {
            var request = ModelBuilder.BuildRequestCustomerBuilder();
            var reply = ModelBuilder.BuildReplyCustomerBuilder();
            var requestModel = reply.BuildModel<Customer>();
            AssertingRequest("PUT", $"customers/{requestModel.RoutableId}", reply,
                async ic => await ic.SetCustomerInfoAsync(requestModel),
                request.BuildJson());
        }

        private static readonly IDictionary<string, string> Attributes1 = new Dictionary<string, string>()
        {
            {"test-attr-1", "test-value-1"},
            {"test-attr-2", "test-value-2"},
        }.ToImmutableDictionary();

        private static readonly IDictionary<string, string> Attributes2 = new Dictionary<string, string>(Attributes1)
        {
            {"test-attr-3", "test-value-3"},
        }.ToImmutableDictionary();

        [Test]
        public async Task SetCustomerAttributesAsync_correct()
        {
            var requestJson = ModelBuilder.DictToJson(Attributes1);
            var replyJson = ModelBuilder.DictToJson(Attributes2);
            var ic = ConfigureIc("POST", $"customers/{TestId}/attributes", replyJson, requestJson);
            var returnedAttributes = await ic.SetCustomerAttributesAsync(TestId, Attributes1);
            Assert.True(Attributes2.EqualsDict(returnedAttributes));
        }

        [Test]
        public async Task GetCustomerAttributesAsync_correct()
        {
            var replyJson = ModelBuilder.DictToJson(Attributes2);
            var ic = ConfigureIc("GET", $"customers/{TestId}/attributes", replyJson);
            var returnedAttributes = await ic.GetCustomerAttributesAsync(TestId);
            Assert.True(Attributes2.EqualsDict(returnedAttributes));
        }
    }
}