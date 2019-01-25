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

namespace test
{
    [TestFixture]
    internal class InvisibleCollectorIt : MockServerTesterBase
    {
        private const string TestId = "1234";

        private InvisibleCollector ConfigureIc(string expectedMethod, string expectedPath, string jsonReply,
            string expectedJson = null)
        {
            _mockServer.AddRequest(expectedMethod, expectedPath, expectedJson,
                    expectedJson is null ? BodylessHeaders : BodiedHeaders,
                    expectedJson is null ? BodyHeaderDifference : null)
                .AddJsonResponse(jsonReply);

            var uri = _mockServer.GetUrl();
            return new InvisibleCollector(TestApiKey, uri);
        }

        private static readonly IDictionary<string, string> Attributes1 = new Dictionary<string, string>
        {
            {"test-attr-1", "test-value-1"},
            {"test-attr-2", "test-value-2"}
        }.ToImmutableDictionary();

        private static readonly IDictionary<string, string> Attributes2 = new Dictionary<string, string>(Attributes1)
        {
            {"test-attr-3", "test-value-3"}
        }.ToImmutableDictionary();

        private void AssertingModelRequest<TModel>(string expectedMethod, string expectedPath,
            ModelBuilder replyModelBuilder,
            Func<InvisibleCollector, Task<TModel>> requestMethod, string expectedJson = null)
            where TModel : InvisibleCollectorLib.Model.Model, new()
        {
            var jsonReply = replyModelBuilder.BuildJson();
            // maybe not strip nulls
            var expectedModel = replyModelBuilder.BuildModel<TModel>(true);

            var ic = ConfigureIc(expectedMethod, expectedPath, jsonReply, expectedJson);
            var result = requestMethod(ic).Result;
            Assert.AreEqual(expectedModel, result);
        }

        [Test]
        public async Task GetCustomerAttributesAsync_correct()
        {
            var replyJson = ModelBuilder.DictToJson(Attributes2);
            var ic = ConfigureIc("GET", $"customers/{TestId}/attributes", replyJson);
            var returnedAttributes = await ic.GetCustomerAttributesAsync(TestId);
            Assert.True(Attributes2.EqualsCollection(returnedAttributes));
        }

        [Test]
        public async Task GetCustomerDebtsAsync_correct()
        {
            var replyDebts = new List<Debt>
            {
                ModelBuilder.BuildReplyDebtBuilder("1", "10").BuildModel<Debt>(),
                ModelBuilder.BuildReplyDebtBuilder("2", "20").BuildModel<Debt>()
            };

            var replyJson = ModelBuilder.ToJson(replyDebts);

            var ic = ConfigureIc("GET", $"customers/{TestId}/debts", replyJson);
            var result = await ic.GetCustomerDebtsAsync(TestId);
            for (var i = 0; i < replyDebts.Count; i++)
                Assert.AreEqual(replyDebts[i], result[i]);
        }


        [Test]
        public void GetDebtAsync_correct()
        {
            var builder = ModelBuilder.BuildReplyDebtBuilder();
            AssertingModelRequest("GET", $"debts/{TestId}", builder,
                async ic => await ic.GetDebtAsync(TestId));
        }

        [Test]
        public async Task GetFindDebts_correct()
        {
            var replyDebts = new List<Debt>
            {
                ModelBuilder.BuildReplyDebtBuilder("1", "10").BuildModel<Debt>(),
                ModelBuilder.BuildReplyDebtBuilder("2", "20").BuildModel<Debt>()
            };

            var replyJson = ModelBuilder.ToJson(replyDebts);

            var ic = ConfigureIc("GET", "debts/find", replyJson);
            
            var findDebts = new FindDebts
            {
                Number = "123"
            };
            var result = await ic.GetFindDebts(findDebts);
            
            for (var i = 0; i < replyDebts.Count; i++)
                Assert.AreEqual(replyDebts[i], result[i]);
        }
        
        [Test]
        public void SetNewCustomerAsync_correct()
        {
            var request = ModelBuilder.BuildRequestCustomerBuilder();
            var reply = ModelBuilder.BuildRequestCustomerBuilder();
            AssertingModelRequest("POST", $"customers", reply,
                async ic => await ic.SetNewCustomerAsync(request.BuildModel<Customer>()),
                request.BuildJson());
        }

        [Test]
        public void GetCompanyInfoAsync_correct()
        {
            var builder = ModelBuilder.BuildReplyCompanyBuilder();
            AssertingModelRequest("GET", "companies", builder,
                async ic => await ic.GetCompanyInfoAsync());
        }


        [Test]
        public void GetCompanyInfoAsync_fail404()
        {
            var ic = ConfigureIc("GET", "someunreachablepath", "{}");
            Assert.ThrowsAsync<WebException>(() => ic.GetCompanyInfoAsync());
        }

        [Test]
        public void GetCompanyInfoAsync_failRefuseConnection()
        {
            var uri = new Uri("http://localhost:56087"); //shouldn't be in use
            Assert.ThrowsAsync<WebException>(() => new InvisibleCollector(TestApiKey, uri).GetCompanyInfoAsync());
        }

        [Test]
        public void GetCustomerInfoAsync_correct()
        {
            var builder = ModelBuilder.BuildReplyCustomerBuilder();
            AssertingModelRequest("GET", $"customers/{TestId}", builder,
                async ic => await ic.GetCustomerInfoAsync(TestId));
        }

        [Test]
        public void SetCompanyNotifications_correct()
        {
            var builder = ModelBuilder.BuildReplyCompanyBuilder();
            builder[Company.NotificationsName] = true;
            AssertingModelRequest("PUT", "companies/enableNotifications", builder,
                async ic => await ic.SetCompanyNotificationsAsync(true));

            builder[Company.NotificationsName] = false;
            AssertingModelRequest("PUT", "companies/disableNotifications", builder,
                async ic => await ic.SetCompanyNotificationsAsync(false));
        }

        [Test]
        public async Task SetCustomerAttributesAsync_correct()
        {
            var requestJson = ModelBuilder.DictToJson(Attributes1);
            var replyJson = ModelBuilder.DictToJson(Attributes2);
            var ic = ConfigureIc("POST", $"customers/{TestId}/attributes", replyJson, requestJson);
            var returnedAttributes = await ic.SetCustomerAttributesAsync(TestId, Attributes1);
            Assert.True(Attributes2.EqualsCollection(returnedAttributes));
        }

        [Test]
        public void SetNewDebtAsync_correct()
        {
            var request = ModelBuilder.BuildRequestDebtBuilder();
            var reply = ModelBuilder.BuildReplyDebtBuilder();
            AssertingModelRequest("POST", $"debts", reply,
                async ic => await ic.SetNewDebtAsync(request.BuildModel<Debt>()),
                request.BuildJson());
        }

        [Test]
        public void SetCompanyInfoAsync_correct()
        {
            var replyBuilder = ModelBuilder.BuildReplyCompanyBuilder();
            var requestBuilder = ModelBuilder.BuildRequestCompanyBuilder();
            AssertingModelRequest("PUT", "companies", replyBuilder,
                async ic => await ic.SetCompanyInfoAsync(requestBuilder.BuildModel<Company>()),
                requestBuilder.BuildJson());
        }

        [Test]
        public void SetCustomerInfoAsync_correct()
        {
            var request = ModelBuilder.BuildRequestCustomerBuilder();
            var reply = ModelBuilder.BuildReplyCustomerBuilder();
            var requestModel = reply.BuildModel<Customer>();
            AssertingModelRequest("PUT", $"customers/{requestModel.RoutableId}", reply,
                async ic => await ic.SetCustomerInfoAsync(requestModel),
                request.BuildJson());
        }
    }
}