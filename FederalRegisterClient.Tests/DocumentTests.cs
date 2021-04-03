using System;
using System.Threading.Tasks;
using Xunit;

namespace FederalRegisterClient.Tests
{
    public class DocumentTests
    {
        [Fact(Skip = "Test has external dependency")]
        public async Task DocumentHandler_GetDocumentAsync_ShouldRetrieveDocument() {
            HttpRequestHandler.ConfigureClient(
                Factory.CreateHttpClient(), "https://www.federalregister.gov/api/v1/documents/");
            var document = await DocumentHandler.GetDocumentAsync("01-27917");
            Assert.IsType<DocumentModel>(document);
        }
    }
}
