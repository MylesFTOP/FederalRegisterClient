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
        [Theory]
        [InlineData(HttpStatusCode.ServiceUnavailable, 3)]
        [InlineData(HttpStatusCode.TooManyRequests, 3)]
        [InlineData(HttpStatusCode.NotFound, 1)]
        public async Task GetDocumentAsJsonAsync_ShouldRetryIfStatusCodeIsRetryable(HttpStatusCode httpStatusCode, int numberOfTries)
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

        [Fact(Skip = "Test has external dependency")]
        public async Task DocumentHandler_ShouldReturnDocument()
        {
            var httpRequestHandler = new HttpRequestHandler();
            var handlerMock = new Mock<HttpMessageHandler>();
            var httpResponseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(@"{ ""document_number"" : ""01-27917""}", Encoding.UTF8, "application/json")
            };

            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponseMessage);

            var httpClient = new HttpClient(handlerMock.Object);
            HttpRequestHandler.ConfigureClient(httpClient, "https://www.federalregister.gov/api/v1/documents/");

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
