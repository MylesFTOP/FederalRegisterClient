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
        public async Task DocumentHandler_GetDocumentAsync_ShouldRetrieveDocument() {
            DocumentHandler.ConfigureClient(Factory.CreateHttpClient());
            var document = await DocumentHandler.GetDocumentAsync("01-27917");
            Assert.IsType<DocumentModel>(document);
        }

        [Fact]
        public async Task DocumentHandler_GetDocumentAsync_ShouldRetrieveExpectedDocument() {

            //var handlerMock = new Mock<HttpMessageHandler>();
            //var response = new HttpResponseMessage
            //{
            //    StatusCode = HttpStatusCode.OK,
            //    Content = new StringContent(@"[{ ""document_number"" : ""01-27917""}]")
            //};

            //handlerMock
            //    .Protected()
            //    .Setup<Task<HttpResponseMessage>>(
            //        "SendAsync",
            //        It.IsAny<HttpRequestMessage>(),
            //        It.IsAny<CancellationToken>())
            //    .ReturnsAsync(response);

            //var httpClient = new HttpClient(handlerMock.Object);


            DocumentHandler.ConfigureClient(Factory.CreateHttpClient());
            var expected = "01-27917"; // EO 13233, "Further Implementation of the Presidential Records Act"
            var document = await DocumentHandler.GetDocumentAsync(expected);
            var actual = document.FederalRegisterDocumentNumber;
            Assert.Equal(expected, actual);
        }
    }
}
