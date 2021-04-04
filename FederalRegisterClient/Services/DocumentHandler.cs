using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FederalRegisterClient
{
    public static class DocumentHandler
    {
        public static List<DocumentModel> GetDocuments(List<string> documentNumbers)
        {
            List<DocumentModel> documents = Factory.CreateDocumentList();
            foreach (var documentNumber in documentNumbers)
            {
                documents.Add(GetDocumentAsync(documentNumber).Result);
            }
            return documents;
        }

        public static async Task<DocumentModel> GetDocumentAsync(string documentNumber) {
            return await HttpRequestHandler.GetDocumentAsJsonAsync(documentNumber);
        }

        private static void ShowDocuments(List<DocumentModel> documents) {
            documents.ForEach(x => ShowDocument(x));
        }

        public static void ShowDocument(DocumentModel document) {
            Console.WriteLine($"Document Reference: {document.FederalRegisterDocumentNumber}");
            Console.WriteLine($"Document Title: {document.DocumentTitle}");
            Console.WriteLine($"Publication Date: {document.PublicationDate.ToShortDateString()}");
            Console.WriteLine($"Publication Type: {document.PublicationType}");
            Console.WriteLine($"Presidential Document Type: {document.PresidentialDocumentType}");
            Console.WriteLine($"Document citation: {document.DocumentCitation}");
            Console.WriteLine($"Abstract: {document.Abstract}");
        }

        public static async Task RunAsync() {
            Console.WriteLine("Please enter document reference to retrieve:");
            String documentNumberToRetrieve = Console.ReadLine();
            try {
                var document = await GetDocumentAsync(documentNumberToRetrieve);
                ShowDocument(document);
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
            }
        }
    }
}
