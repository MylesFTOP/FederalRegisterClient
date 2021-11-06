using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FederalRegisterClient
{
    public static class DocumentHandler
    {
        public static List<DocumentModel> GetDocuments(List<string> documentNumbers)
        {
            // TODO: Check if there's a way to pass an array or range in a single call instead of making repeated calls
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
