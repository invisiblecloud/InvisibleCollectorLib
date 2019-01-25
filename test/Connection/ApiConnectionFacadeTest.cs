using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using InvisibleCollectorLib.Connection;
using InvisibleCollectorLib.Exception;
using InvisibleCollectorLib.Utils;
using Moq;
using NUnit.Framework;
using test.Utils;

namespace test.Connection
{
    [TestFixture]
    internal class ApiConnectionFacadeTest : MockServerTesterBase
    {
        
        private const string DefaultErorMessage = "an error occured";
        private const string DefaultErrorCode = "400";
        private const string DefaultConflictingId = "3456";
        private const string TestPath = "umm/123";

        private static readonly ImmutableDictionary<string, string> ErrorDictionary = new Dictionary<string, string>
        {
            {"code", DefaultErrorCode},
            {"message", DefaultErorMessage}
        }.ToImmutableDictionary();

        private static readonly ImmutableDictionary<string, string> EmptyDictionary = new Dictionary<string, string>().ToImmutableDictionary();

        private static readonly ImmutableDictionary<string, string> ConflictErrorDictionary =
            new Dictionary<string, string>(ErrorDictionary) {{"gid", DefaultConflictingId}}
                .ToImmutableDictionary();

        private ApiConnectionFacade BuildApiFacade(IDictionary<string, string> errorJsonObject)
        {
            var mock = new Mock<Func<Stream, IDictionary<string, string>>>();

            mock.Setup(m => m(It.IsAny<Stream>())).Returns(errorJsonObject);
            return new ApiConnectionFacade(TestApiKey, mock.Object);
        }

        [Test]
        public void CallApiAsync_Error()
        {
            _mockServer.AddRequest("GET", TestPath)
                .AddJsonResponse("", 400); // supposed to fail here
            var exception = Assert.ThrowsAsync<IcException>(() =>
                BuildApiFacade(ErrorDictionary)
                    .CallJsonToJsonApi(_mockServer.GetUrl(TestPath), "GET"));

            TestingUtils.AssertStringContainsValues(exception.Message, DefaultErorMessage, DefaultErrorCode);
        }

        [Test]
        public void CallApiAsync_FailNotJsonResponse()
        {
            _mockServer.AddRequest("GET", TestPath)
                .AddHtmlResponse("", 400); // supposed to fail here
            Assert.ThrowsAsync<WebException>(() =>
                BuildApiFacade(ErrorDictionary)
                    .CallJsonToJsonApi(_mockServer.GetUrl(TestPath), "GET"));
        }

        [Test]
        public void CallApiAsync_200FailNotJsonResponse()
        {
            _mockServer.AddRequest("GET", TestPath)
                .AddHtmlResponse("", 200); 
            Assert.ThrowsAsync<IcException>(() =>
                BuildApiFacade(ErrorDictionary)
                    .CallJsonToJsonApi(_mockServer.GetUrl(TestPath), "GET"));
        }

        [Test]
        public void CallApiAsync_ErrorConflict()
        {
            _mockServer.AddRequest("GET", TestPath)
                .AddJsonResponse("", 400); 
            var exception = Assert.ThrowsAsync<IcModelConflictException>(() =>
                BuildApiFacade(ConflictErrorDictionary)
                    .CallJsonToJsonApi(_mockServer.GetUrl(TestPath), "GET"));

            Assert.AreEqual(exception.ConflictingId, DefaultConflictingId);
        }

        [Test]
        public void CallApiAsync_ErrorMissingFields()
        {
            _mockServer.AddRequest("GET", TestPath)
                .AddJsonResponse("", 400); 
            Assert.ThrowsAsync<WebException>(() =>
                BuildApiFacade(EmptyDictionary)
                    .CallJsonToJsonApi(_mockServer.GetUrl(TestPath), "GET"));
        }

        [Test]
        public void CallApiAsync_404Error()
        {
            _mockServer.AddRequest("GET", "unreachablPath")
                .AddJsonResponse("", 200); 
            Assert.ThrowsAsync<WebException>(() =>
                BuildApiFacade(EmptyDictionary)
                    .CallJsonToJsonApi(_mockServer.GetUrl(TestPath), "GET"));
        }

        [Test]
        public async Task CallApiAsync_GetBodyless()
        {
            const string Method = "GET";
            var json = TestingUtils.BuildJson(("a", "b"));

            _mockServer.AddRequest(Method, TestPath, expectedHeaders: BodylessHeaders,
                    notExpectedHeaders: BodyHeaderDifference)
                .AddJsonResponse(json);

            var uri = _mockServer.GetUrl(TestPath);
            var result = await BuildApiFacade(ErrorDictionary).CallJsonToJsonApi(uri, Method);
            Assert.AreEqual(json, result);
        }

        [Test]
        public async Task CallApiAsync_PostBodied()
        {
            const string Method = "POST";
            var returnJson = TestingUtils.BuildJson(("a", "b"));
            var sendingJson = TestingUtils.BuildJson(("q", "w"));

            _mockServer.AddRequest(Method, TestPath, sendingJson, BodiedHeaders)
                .AddJsonResponse(returnJson);

            var uri = _mockServer.GetUrl(TestPath);
            var result = await BuildApiFacade(ErrorDictionary).CallJsonToJsonApi(uri, Method, sendingJson);
            Assert.AreEqual(returnJson, result);
        }
    }
}