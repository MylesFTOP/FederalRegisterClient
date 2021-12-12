using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FederalRegisterClient
{
    public static class DocumentHandler
    {
        public static List<DocumentModel> GetDocuments(List<string> documentNumbers)
        {
            List<DocumentModel> documents = Factory.CreateDocumentList();
            documents = GetDocumentsInMultipleCalls(documentNumbers);
            return documents;
        }

        private static List<DocumentModel> GetDocumentsInSingleCall(List<string> documentNumbers)
        {
            var documents = Factory.CreateDocumentList();
            return documents;
        }
        
        private static List<DocumentModel> GetDocumentsInMultipleCalls(List<string> documentNumbers)
        {
            List<DocumentModel> documents = Factory.CreateDocumentList();
            foreach (var documentNumber in documentNumbers)
            {
                documents.Add(GetDocumentAsync(documentNumber).Result);
            }
            return documents;
        }

        public static async Task<DocumentModel> GetDocumentAsync(string documentNumber)
        {
            return await HttpRequestHandler.GetDocumentAsJsonAsync(documentNumber);
        }
    }
}
