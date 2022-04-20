using System.Collections.Generic;
using System.Net.Http;

namespace FederalRegisterClient
{
    public static class Factory
    {
        public static HttpClient CreateHttpClient() {
            return new HttpClient();
        }

        public static DocumentModel CreateDocument() {
            return new DocumentModel();
        }

        public static List<DocumentModel> CreateDocumentList() {
            return new List<DocumentModel>();
        }
    }
}
