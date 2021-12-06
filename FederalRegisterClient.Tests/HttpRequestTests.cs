using System;
using System.Net;
using System.Net.Http;
using System.Text;
using Xunit;
using Moq;
using Moq.Protected;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Linq;

namespace FederalRegisterClient.Tests
{
    public class HttpRequestTests
    {
        private readonly StringContent documentContentPresidentialRecordsAct = GenerateMockDocumentContent("01-27917"); // EO 13233, "Further Implementation of the Presidential Records Act"
        private readonly StringContent documentContentUnitedNationsDay = GenerateMockDocumentContent("2021-23559");
        
        private static StringContent GenerateMockDocumentContent(string documentNumber) 
            => new StringContent(BuildSingleDocumentInner(documentNumber), Encoding.UTF8, "application/json");
        
        private static StringContent GenerateMockDocumentContent(string[] documentNumbers) 
            => new StringContent(BuildDocumentResponse(documentNumbers), Encoding.UTF8, "application/json");

        private static string BuildDocumentResponse(string[] documentNumbers) {
            string documentResponse = "";
            foreach (var documentNumber in documentNumbers)
            {
                documentResponse += BuildSingleDocumentInner(documentNumber);
            }
            return documentResponse;
        }

        private static string BuildSingleDocumentInner(string documentNumber) 
            => @"{ ""document_number"" : """ + documentNumber + @""" }";

        private Mock<HttpMessageHandler> CreateMockMessageHandler(HttpResponseMessage httpResponseMessage) {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponseMessage);
            return handlerMock;
        }

        private HttpClient CreateMockHttpClient(HttpMessageHandler messageHandler) {
            HttpClient httpClient = new HttpClient(messageHandler);
            HttpRequestHandler.ConfigureClient(httpClient, "https://www.mock.test/");
            return httpClient;
        }

        [Theory]
        [InlineData(HttpStatusCode.ServiceUnavailable, 3)]
        [InlineData(HttpStatusCode.TooManyRequests, 3)]
        [InlineData(HttpStatusCode.NotFound, 1)]
        public async Task GetDocumentAsJsonAsync_ShouldRetryExpectedNumberOfTimesIfStatusCodeIsRetryable(HttpStatusCode httpStatusCode, int numberOfTries)
        {
            TimeSpan timeSpanDelay = default;
            var retryCondition = new RetryConditionHeaderValue(timeSpanDelay).ToString();
            var httpResponseMessage = new HttpResponseMessage
            {
                StatusCode = httpStatusCode
            };
            httpResponseMessage.Headers.Add("Retry-After", retryCondition);

            var handlerMock = CreateMockMessageHandler(httpResponseMessage);
            var httpClient = CreateMockHttpClient(handlerMock.Object);

            var document = await DocumentHandler.GetDocumentAsync("");

            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Exactly(numberOfTries),
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
                ItExpr.IsAny<CancellationToken>()
                );
        }


        [Fact(Skip="New test - asserted behaviour has not yet been added")]
        public async Task GetDocumentAsJsonAsync_ShouldThrowExceptionIfDocumentFails()
        {
            TimeSpan timeSpanDelay = default;
            var retryCondition = new RetryConditionHeaderValue(timeSpanDelay).ToString();
            var httpResponseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound
            };
            httpResponseMessage.Headers.Add("Retry-After", retryCondition);

            var handlerMock = CreateMockMessageHandler(httpResponseMessage);
            var httpClient = CreateMockHttpClient(handlerMock.Object);


            await Assert.ThrowsAsync<Exception>(
                () => DocumentHandler.GetDocumentAsync("")
                );
        }

        [Theory]
        [InlineData(HttpStatusCode.ServiceUnavailable)]
        [InlineData(HttpStatusCode.TooManyRequests)]
        public async Task GetDocumentAsJsonAsync_ShouldReturnDocumentIfRetryIsSuccessful(HttpStatusCode httpStatusCode)
        {
            TimeSpan timeSpanDelay = default;
            var retryCondition = new RetryConditionHeaderValue(timeSpanDelay).ToString();
            var httpResponseMessage = new HttpResponseMessage
            {
                StatusCode = httpStatusCode,
                Content = httpStatusCode == HttpStatusCode.OK 
                    ? documentContentPresidentialRecordsAct
                    : new StringContent("")
            };
            httpResponseMessage.Headers.Add("Retry-After", retryCondition);

            var handlerMock = CreateMockMessageHandler(httpResponseMessage);
            var httpClient = CreateMockHttpClient(handlerMock.Object);

            var expected = "01-27917"; // EO 13233, "Further Implementation of the Presidential Records Act"
            var document = await DocumentHandler.GetDocumentAsync(expected);
            Assert.NotNull(document);
            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Exactly(3),
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
                ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task DocumentHandler_ShouldReturnADocument()
        {
            var httpResponseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = documentContentPresidentialRecordsAct
            };

            var handlerMock = CreateMockMessageHandler(httpResponseMessage);
            var httpClient = CreateMockHttpClient(handlerMock.Object);

            var documentNumber = "01-27917"; // EO 13233, "Further Implementation of the Presidential Records Act"
            var document = await DocumentHandler.GetDocumentAsync(documentNumber);
            Assert.IsType<DocumentModel>(document);
        }

        [Fact]
        public async Task DocumentHandler_ShouldReturnDocumentWithSentDetails()
        {
            var httpResponseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = documentContentPresidentialRecordsAct
            };

            var handlerMock = CreateMockMessageHandler(httpResponseMessage);
            var httpClient = CreateMockHttpClient(handlerMock.Object);

            var expected = "01-27917"; // EO 13233, "Further Implementation of the Presidential Records Act"
            var document = await DocumentHandler.GetDocumentAsync(expected);
            Assert.NotNull(document);
            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Exactly(1),
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
                ItExpr.IsAny<CancellationToken>());
        }

        [Fact(Skip ="Test is still being developed to use mocked response")]
        public void DocumentHandler_ShouldReturnMultipleDocumentsWhenMultipleDocumentsRequested()
        {
            var documentNumbers = new string[2] { "01-27917", "2021-23559" };
            var httpResponseMessage = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = GenerateMockDocumentContent(documentNumbers),
            };

            var handlerMock = CreateMockMessageHandler(httpResponseMessage);
            var httpClient = CreateMockHttpClient(handlerMock.Object);

            //HttpRequestHandler.ConfigureClient(
            //    Factory.CreateHttpClient(), "https://www.federalregister.gov/api/v1/documents/");

            List<string> expected = documentNumbers.ToList();
            var document = DocumentHandler.GetDocuments(expected);
            var expectedCount = 2;
            var actualCount = document.OfType<DocumentModel>().ToList().Count();
            Assert.Equal(expectedCount, actualCount);
        }
    }
}
