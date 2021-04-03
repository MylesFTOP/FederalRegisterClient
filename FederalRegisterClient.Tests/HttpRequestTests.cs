using System;
using System.Collections.Generic;
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
        [Fact]
        public async Task GetDocumentAsJsonAsync_ShouldRetryIfStatusCodeIsTooManyRequests() {
            var handlerMock = new Mock<HttpMessageHandler>();
            TimeSpan timeSpanDelay = default;
            var retryCondition = new RetryConditionHeaderValue(timeSpanDelay).ToString();
            var httpResponseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.TooManyRequests,
                Content = new StringContent(@"{ ""document_number"" : ""01-27917""}", Encoding.UTF8, "application/json")
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
            HttpRequestHandler.ConfigureClient(httpClient, "https://www.federalregister.gov/api/v1/documents/");

            var document = await DocumentHandler.GetDocumentAsync("placeholder");

            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Exactly(2),
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
                ItExpr.IsAny<CancellationToken>()
                );
        }
    }
}
