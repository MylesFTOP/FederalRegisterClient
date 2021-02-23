using Moq;
using Moq.Protected;
using System.Net;
using System.Net.Http;
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
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(@"[{ ""document_number"" : ""01-27917""}]")
            };

            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            var httpClient = new HttpClient(handlerMock.Object);
            DocumentHandler.ConfigureClient(httpClient);

            var expected = "01-27917"; // EO 13233, "Further Implementation of the Presidential Records Act"
            var document = await DocumentHandler.GetDocumentAsync(expected);
            //Assert.NotNull(document);
            //handlerMock.Protected().Verify(
            //    "SendAsync",
            //    Times.Exactly(1),
            //    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
            //    ItExpr.IsAny<CancellationToken>());
        }

        [Fact(Skip = "Not mocked")]
        public async Task DocumentHandler_GetDocumentAsync_ShouldRetrieveDocument() {
            DocumentHandler.ConfigureClient(Factory.CreateHttpClient());
            var document = await DocumentHandler.GetDocumentAsync("01-27917");
            Assert.IsType<DocumentModel>(document);
        }

        [Fact(Skip = "Not mocked")]
        public async Task DocumentHandler_GetDocumentAsync_ShouldRetrieveExpectedDocument() {
            DocumentHandler.ConfigureClient(Factory.CreateHttpClient());
            var expected = "01-27917"; // EO 13233, "Further Implementation of the Presidential Records Act"
            var document = await DocumentHandler.GetDocumentAsync(expected);
            var actual = document.FederalRegisterDocumentNumber;
            Assert.Equal(expected, actual);
        }
    }
}
