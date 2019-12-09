using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
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
                async ic => await ic.GetCompanyAsync());
        }


        [Test]
        public void GetCompanyInfoAsync_fail404()
        {
            var ic = ConfigureIc("GET", "someunreachablepath", "{}");
            Assert.ThrowsAsync<HttpRequestException>(() => ic.GetCompanyAsync());
        }

        [Test]
        public void GetCompanyInfoAsync_failRefuseConnection()
        {
            var uri = new Uri("http://localhost:56087"); //shouldn't be in use
            Assert.ThrowsAsync<HttpRequestException>(() => new InvisibleCollector(TestApiKey, uri).GetCompanyAsync());
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
        public void GetCustomerAsync_correct()
        {
            var builder = ModelBuilder.BuildRequestCustomerWithContactsBuilder();
            builder._fields[Customer.IdName] = TestId;
            AssertingModelRequest("GET", $"v1/customers/{TestId}", builder,
                async ic => await ic.GetCustomerAsync(TestId));
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
                FromDate = null,
                Attributes = new Dictionary<string, string>()
                {
                    {"a", "bxy"}
                }
            };
            var expectedQueryParams = new List<string> {"123", "number", "to_date", "2010-01-01", "from_date", WebUtility.UrlEncode("attributes[a]"), "bxy"};

            var ic = ConfigureIc("GET", "debts/find", replyJson, null, expectedQueryParams);

            var result = await ic.GetFindDebtsAsync(findDebts);

            for (var i = 0; i < replyDebts.Count; i++)
                Assert.AreEqual(replyDebts[i], result[i]);
        }

        [Test]
        public void SetCompanyInfoAsync_correct()
        {
            var replyBuilder = ModelBuilder.BuildReplyCompanyBuilder();
            var requestBuilder = ModelBuilder.BuildRequestCompanyBuilder();
            AssertingModelRequest("PUT", "companies", replyBuilder,
                async ic => await ic.SetCompanyAsync(requestBuilder.BuildModel<Company>()),
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
        public void SetCustomerAsync_correct()
        {
            var request = ModelBuilder.BuildRequestCustomerBuilder();
            var reply = ModelBuilder.BuildReplyCustomerBuilder();
            var requestModel = reply.BuildModel<Customer>();
            AssertingModelRequest("PUT", $"customers/{requestModel.Id}", reply,
                async ic => await ic.SetCustomerAsync(requestModel),
                request.BuildJson());
        }
        
        [Test]
        public async Task SetCustomerWithContactsAsync_correct()
        {
            const string id = "12345";
            
            var builder = ModelBuilder.BuildRequestCustomerWithContactsBuilder();
            var firstExpectedJson = builder.WithoutContacts()
                .BuildJson();
            builder[Customer.IdName] = id;
            var firstReplyJson = builder.WithoutContacts()
                .BuildJson();
            var finalExpectedJson = builder.BuildContactsJson();
            var finalReplyJson = builder.BuildJson();
            var requestModel = builder.BuildModel<Customer>();
            var expectedModel = builder.BuildModel<Customer>(true);
            
            _mockServer.AddRequest("PUT", $"customers/{id}", firstExpectedJson,
                    BodiedHeaders)
                .AddJsonResponse(firstReplyJson);
            
            _mockServer.AddRequest("POST", $"v1/customers/{id}/contacts", finalExpectedJson,
                    BodiedHeaders)
                .AddJsonResponse(finalReplyJson);
            

            var uri = _mockServer.GetUrl();
            var ic = new InvisibleCollector(TestApiKey, uri);
            
            var result = await ic.SetCustomerAsync(requestModel);
            Assert.AreEqual(expectedModel, result);
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
        public async Task SetNewCustomerWithContactsAsync_correct()
        {
            const string id = "12345";
            
            var builder = ModelBuilder.BuildRequestCustomerWithContactsBuilder();
            var firstExpectedJson = builder.WithoutContacts()
                .BuildJson();
            builder[Customer.IdName] = id;
            var firstReplyJson = builder.WithoutContacts()
                .BuildJson();
            var finalExpectedJson = builder.BuildContactsJson();
            var finalReplyJson = builder.BuildJson();
            var requestModel = builder.BuildModel<Customer>();
            var expectedModel = builder.BuildModel<Customer>(true);
            
            _mockServer.AddRequest("POST", "customers", firstExpectedJson,
                    BodiedHeaders)
                .AddJsonResponse(firstReplyJson);
            
            _mockServer.AddRequest("POST", $"v1/customers/{id}/contacts", finalExpectedJson,
                    BodiedHeaders)
                .AddJsonResponse(finalReplyJson);
            

            var uri = _mockServer.GetUrl();
            var ic = new InvisibleCollector(TestApiKey, uri);
            
            var result = await ic.SetNewCustomerAsync(requestModel);
            Assert.AreEqual(expectedModel, result);
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
        
        [Test]
        public void GetCustomerContactsAsync_correct()
        {
            var model1 = ModelBuilder.BuildCustomerContactBuilder("john");
            var model2 = ModelBuilder.BuildCustomerContactBuilder("mary");
            var listBuilder = new ModelListBuilder().Add(model1).Add(model2);
            var replyJson = listBuilder.BuildJson();
            
            var ic = ConfigureIc("GET", "v1/customers/3/contacts", replyJson);

            var actual = ic.GetCustomerContactsAsync("3").Result;

            var expectedReply = listBuilder.BuildModelList<CustomerContact>();
            Assert.AreEqual(expectedReply, actual);
        }
        
        [Test]
        public void SetNewCustomerContactsAsync_correct()
        {
            var model1 = ModelBuilder.BuildCustomerContactBuilder("john");
            var model2 = ModelBuilder.BuildCustomerContactBuilder("mary");
            var listBuilder = new ModelListBuilder().Add(model1).Add(model2);
            var expectedJson = listBuilder.Clone()
                .WithoutFields(CustomerContact.IdName)
                .BuildJson();
            
            var customerBuilder = ModelBuilder.BuildReplyCustomerBuilder();
            var ic = ConfigureIc("POST", "v1/customers/3/contacts", customerBuilder.BuildJson(), expectedJson);

            var actual = ic.SetNewCustomerContactsAsync("3", listBuilder.BuildModelList<CustomerContact>()).Result;
            
            var expectedReply = customerBuilder.BuildModel<Customer>(true);
            Assert.AreEqual(expectedReply, actual);
        }
        
        [Test]
        public async Task GetFindCorrect_correct()
        {
            var builder1 = ModelBuilder.BuildReplyCustomerBuilder("john");
            var builder2 = ModelBuilder.BuildReplyCustomerBuilder("manny");
            var listBuilder = new ModelListBuilder()
                .Add(builder1)
                .Add(builder2);
            
            var replyJson = listBuilder.BuildJson();

            var findCustomers = new FindCustomers()
            {
                Email = "a@b.com",
                Phone = "920920920",
            };
            var expectedQueryParams = new List<string> {"email", WebUtility.UrlEncode("a@b.com"), "phone", "920920920"};

            var ic = ConfigureIc("GET", "customers/find", replyJson, null, expectedQueryParams);

            var result = await ic.GetFindCustomersAsync(findCustomers);

            Assert.AreEqual(result.Count, 2);
            
            var model1 = builder1.BuildModel<Customer>(true);
            var model2 = builder2.BuildModel<Customer>(true);
            Assert.AreEqual(model1, result[0]);
            Assert.AreEqual(model2, result[1]);
        }
        
        [Test]
        public async Task SetNewDebitAsync_correct()
        {
            var builder = ModelBuilder.BuildDebtNoteBuilder();
            var requestModel = builder.BuildModel<Debit>();
            var expectedReply = builder.BuildModel<Debit>(true);
            const string id = "12345";
            
            var ic = ConfigureIc("POST", $"debts/{id}/debits", builder.BuildJson(), builder.BuildJson());

            var result = await ic.SetNewDebtDebitAsync(id, requestModel);
            Assert.AreEqual(expectedReply, result);
            
        }
        
        [Test]
        public async Task SetNewCreditAsync_correct()
        {
            var builder = ModelBuilder.BuildDebtNoteBuilder();
            var requestModel = builder.BuildModel<Credit>();
            var expectedReply = builder.BuildModel<Credit>(true);
            const string id = "12345";
            
            var ic = ConfigureIc("POST", $"debts/{id}/credits", builder.BuildJson(), builder.BuildJson());

            var result = await ic.SetNewDebtCreditAsync(id, requestModel);
            Assert.AreEqual(expectedReply, result);
        }
        
        [Test]
        public async Task GetGroups_correct()
        {
            var builder1 = ModelBuilder.BuildGroupBuilder("1", "john");
            var builder2 = ModelBuilder.BuildGroupBuilder("2", "Smith");
            var listBuilder = new ModelListBuilder().Add(builder1).Add(builder2);
            var replyJson = listBuilder.BuildJson();
            
            var ic = ConfigureIc("GET", "groups", replyJson);

            var result = await ic.GetGroupsAsync();

            Assert.AreEqual(result.Count, 2);
            
            var model1 = builder1.BuildModel<Group>(true);
            var model2 = builder2.BuildModel<Group>(true);
            Assert.AreEqual(model1, result[0]);
            Assert.AreEqual(model2, result[1]);
        }
        
        [Test]
        public async Task SetCustomerToGroupAsync_correct()
        {
            var builder = ModelBuilder.BuildGroupBuilder("1", "john");
            var replyJson = builder.BuildJson();
            var expectedModel = builder.BuildModel<Group>(true);
            const string groupId = "12";
            const string customerId = "ab";

            var ic = ConfigureIc("POST", $"groups/{groupId}/customers/{customerId}", replyJson);

            var result = await ic.SetCustomerToGroupAsync(customerId, groupId);
            Assert.AreEqual(expectedModel, result);
        }
        
        [Test]
        public void DeleteContactAsync_correct()
        {
            var customerGid = "a123";
            var contactGid = "b789";
            var reply = ModelBuilder.BuildCustomerContactBuilder(gid: contactGid);
            AssertingModelRequest("DELETE", $"customers/{customerGid}/contacts/{contactGid}", reply,
                async ic => await ic.DeleteCustomerContactAsync(customerGid, contactGid));
        }
    }
}