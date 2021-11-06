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

namespace FederalRegisterClient.Tests
{
    public class HttpRequestTests
    {
        private readonly StringContent documentContentPresidentialRecordsAct = GenerateMockDocumentContent("01-27917");
        private readonly StringContent documentContentUnitedNationsDay = GenerateMockDocumentContent("2021-23559");

        private static StringContent GenerateMockDocumentContent(string documentNumber) 
            => new StringContent(@"{ ""document_number"" : """ + documentNumber + @""" }", Encoding.UTF8, "application/json");

        [Theory]
        [InlineData(HttpStatusCode.ServiceUnavailable, 3)]
        [InlineData(HttpStatusCode.TooManyRequests, 3)]
        [InlineData(HttpStatusCode.NotFound, 1)]
        public async Task GetDocumentAsJsonAsync_ShouldRetryExpectedNumberOfTimesIfStatusCodeIsRetryable(HttpStatusCode httpStatusCode, int numberOfTries)
        {
            var httpRequestHandler = new HttpRequestHandler();
            var handlerMock = new Mock<HttpMessageHandler>();
            TimeSpan timeSpanDelay = default;
            var retryCondition = new RetryConditionHeaderValue(timeSpanDelay).ToString();
            var httpResponseMessage = new HttpResponseMessage
            {
                StatusCode = httpStatusCode
            };
            httpResponseMessage.Headers.Add("Retry-After", retryCondition);

            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponseMessage);

            var httpClient = new HttpClient(handlerMock.Object);
            HttpRequestHandler.ConfigureClient(httpClient, "https://www.mock.test/");

            var document = await DocumentHandler.GetDocumentAsync("");

            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Exactly(numberOfTries),
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
                ItExpr.IsAny<CancellationToken>()
                );
        }

        [Theory]
        [InlineData(HttpStatusCode.ServiceUnavailable)]
        [InlineData(HttpStatusCode.TooManyRequests)]
        public async Task GetDocumentAsJsonAsync_ShouldReturnDocumentIfRetryIsSuccessful(HttpStatusCode httpStatusCode)
        {
            var httpRequestHandler = new HttpRequestHandler();
            var handlerMock = new Mock<HttpMessageHandler>();
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

            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponseMessage);

            var httpClient = new HttpClient(handlerMock.Object);
            HttpRequestHandler.ConfigureClient(httpClient, "https://www.mock.test/");

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
            var httpRequestHandler = new HttpRequestHandler();
            var handlerMock = new Mock<HttpMessageHandler>();
            var httpResponseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = documentContentPresidentialRecordsAct
            };

            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponseMessage);

            var httpClient = new HttpClient(handlerMock.Object);
            HttpRequestHandler.ConfigureClient(httpClient, "https://www.mock.test/");

            var documentNumber = "01-27917"; // EO 13233, "Further Implementation of the Presidential Records Act"
            var document = await DocumentHandler.GetDocumentAsync(documentNumber);
            Assert.IsType<DocumentModel>(document);
        }

        [Fact]
        public async Task DocumentHandler_ShouldReturnDocumentWithSentDetails()
        {
            var httpRequestHandler = new HttpRequestHandler();
            var handlerMock = new Mock<HttpMessageHandler>();
            var httpResponseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = documentContentPresidentialRecordsAct
            };

            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponseMessage);

            var httpClient = new HttpClient(handlerMock.Object);
            HttpRequestHandler.ConfigureClient(httpClient, "https://www.mock.test/");

            var expected = "01-27917"; // EO 13233, "Further Implementation of the Presidential Records Act"
            var document = await DocumentHandler.GetDocumentAsync(expected);
            Assert.NotNull(document);
            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Exactly(1),
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
                ItExpr.IsAny<CancellationToken>());
        }
    }
}
