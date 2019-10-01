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
            string expectedJson = null, IEnumerable<string> expectedQuery = null)
        {
            var expectingJson = expectedJson is null;
            _mockServer.AddRequest(expectedMethod, expectedPath, expectedJson,
                    expectingJson ? BodylessHeaders : BodiedHeaders,
                    expectingJson ? BodyHeaderDifference : null,
                    expectedQuery)
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
            ModelBuilder replyModelBuilder, Func<InvisibleCollector, Task<TModel>> requestMethod, string expectedJson = null)
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
        public void GetCustomerInfoAsync_correct()
        {
            var builder = ModelBuilder.BuildReplyCustomerBuilder();
            AssertingModelRequest("GET", $"customers/{TestId}", builder,
                async ic => await ic.GetCustomerInfoAsync(TestId));
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

            var findDebts = new FindDebts
            {
                Number = "123",
                ToDate = new DateTime(2010, 1, 1),
                FromDate = null
            };
            var expectedQueryParams = new List<string> {"123", "number", "to_date", "2010-01-01", "from_date"};

            var ic = ConfigureIc("GET", "debts/find", replyJson, null, expectedQueryParams);

            var result = await ic.GetFindDebts(findDebts);

            for (var i = 0; i < replyDebts.Count; i++)
                Assert.AreEqual(replyDebts[i], result[i]);
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
        public void SetCustomerInfoAsync_correct()
        {
            var request = ModelBuilder.BuildRequestCustomerBuilder();
            var reply = ModelBuilder.BuildReplyCustomerBuilder();
            var requestModel = reply.BuildModel<Customer>();
            AssertingModelRequest("PUT", $"customers/{requestModel.RoutableId}", reply,
                async ic => await ic.SetCustomerInfoAsync(requestModel),
                request.BuildJson());
        }

        [Test]
        public void SetNewCustomerAsync_correct()
        {
            var request = ModelBuilder.BuildRequestCustomerBuilder();
            var reply = ModelBuilder.BuildRequestCustomerBuilder();
            AssertingModelRequest("POST", "customers", reply,
                async ic => await ic.SetNewCustomerAsync(request.BuildModel<Customer>()),
                request.BuildJson());
        }

        [Test]
        public void SetNewDebtAsync_correct()
        {
            var sentModel = new Debt()
            {
                Number = "1",
                CustomerId = "abcd",
                Type = "FT",
                Date = new DateTime(2018, 10, 5),
                DueDate = new DateTime(2019, 10, 5),
                Currency = null,
                Items = new List<Item>()
                {
                    new Item
                    {
                        Name = "jory",
                        Description = null
                    }
                },
                Attributes = new Dictionary<string, string>()
                {
                    {"key1", "val1"}
                }
            };
            
            // check for correct json serialization (no null in lines, etc)
            var expectedJson = @"{
                ""number"": ""1"",
                ""customerId"": ""abcd"",
                ""type"": ""FT"",
                ""date"": ""2018-10-05"",
                ""dueDate"": ""2019-10-05"",
                ""currency"": null,
                ""items"": [
                    {
                        ""name"": ""jory"",
                        ""description"": null
                    }
                ],
                ""attributes"": {
                    ""key1"": ""val1""
                }
            }";
            
            var reply = ModelBuilder.BuildReplyDebtBuilder();
            AssertingModelRequest("POST", "debts", reply,
                async ic => await ic.SetNewDebtAsync(sentModel),
                expectedJson);
        }
        
        [Test]
        public void SetNewPaymentAsync_correct()
        {
            var sentModel = new Payment
            {
                Number = "123",
                Status = "FINAL",
                Type = "RG",
                Date = new DateTime(2018, 10, 5),
                Currency = "EUR",
                Tax = null,
                Lines = new List<PaymentLine>
                {
                    new PaymentLine
                    {
                        Number = "1",
                        Amount = 10
                    }
                }
            };

            // check for correct json serialization (no null in lines, etc)
            var expectedJson = @"{
                ""number"": ""123"",
                ""status"": ""FINAL"",
                ""type"": ""RG"",
                ""date"": ""2018-10-05"",
                ""currency"": ""EUR"",
                ""tax"": null,
                ""lines"": [
                    {
                        ""number"": ""1"",
                        ""amount"": 10.0
                    }
                ]
            }";

            var reply = ModelBuilder.BuildReplyPaymentBuilder();
            AssertingModelRequest("POST", "payments", reply,
                async ic => await ic.SetNewPayment(sentModel),
                expectedJson);
        }
        
        [Test]
        public void GetPaymentAsync_correct()
        {
            var reply = ModelBuilder.BuildReplyPaymentBuilder();
            AssertingModelRequest("GET", $"payments/{TestId}", reply,
                async ic => await ic.GetPaymentAsync(TestId));
        }
        
        [Test]
        public void CancelPaymentAsync_correct()
        {
            var reply = ModelBuilder.BuildReplyPaymentBuilder();
            AssertingModelRequest("PUT", $"payments/{TestId}/cancel", reply,
                async ic => await ic.CancelPaymentAsync(TestId));
        }
        
        [Test]
        public void DeletePaymentAsync_correct()
        {
            var reply = ModelBuilder.BuildReplyPaymentBuilder();
            AssertingModelRequest("DELETE", $"payments/{TestId}", reply,
                async ic => await ic.DeletePaymentAsync(TestId));
        }
    }
}