using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FederalRegisterClient.Tests
{
    public class DocumentTests
    {
        [Fact]
        public async Task DocumentHandler_GetDocumentAsync_ShouldRetrieveDocument() {
            DocumentHandler.ConfigureClient();
            var document = await DocumentHandler.GetDocumentAsync("01-27917");
            Assert.IsType<DocumentModel>(document);
        }

        [Fact]
        public async Task DocumentHandler_GetDocumentAsync_ShouldRetrieveExpectedDocument() {
            DocumentHandler.ConfigureClient();
            var expected = "01-27917"; // EO 13233, "Further Implementation of the Presidential Records Act"
            var document = await DocumentHandler.GetDocumentAsync(expected);
            var actual = document.FederalRegisterDocumentNumber;
            Assert.Equal(expected, actual);
        }
    }
}
