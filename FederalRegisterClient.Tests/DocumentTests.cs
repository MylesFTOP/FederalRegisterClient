using Moq;
using Moq.Protected;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace FederalRegisterClient.Tests
{
    public class DocumentTests
    {
        [Fact]
        public async Task DocumentHandler_ShouldReturnDocument() {
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
            RequestHandler.ConfigureClient(httpClient);

            var expected = "01-27917"; // EO 13233, "Further Implementation of the Presidential Records Act"
            var document = await DocumentHandler.GetDocumentAsync(expected);
            Assert.NotNull(document);
            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Exactly(1),
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
                ItExpr.IsAny<CancellationToken>());
        }

        [Fact(Skip = "Not mocked")]
        public async Task DocumentHandler_GetDocumentAsync_ShouldRetrieveDocument() {
            RequestHandler.ConfigureClient(Factory.CreateHttpClient());
            var document = await DocumentHandler.GetDocumentAsync("01-27917");
            Assert.IsType<DocumentModel>(document);
        }

        [Fact(Skip = "Not mocked")]
        public async Task DocumentHandler_GetDocumentAsync_ShouldRetrieveExpectedDocument() {
            RequestHandler.ConfigureClient(Factory.CreateHttpClient());
            var expected = "01-27917"; // EO 13233, "Further Implementation of the Presidential Records Act"
            var document = await DocumentHandler.GetDocumentAsync(expected);
            var actual = document.FederalRegisterDocumentNumber;
            Assert.Equal(expected, actual);
        }
    }
}
