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
        public static HttpClient _httpClient;

        // TODO: Change to retrieve as JSON (update names as well), then call separate method to deserialize JSON to DocumentModel
        public static async Task<DocumentModel> GetDocumentAsync(string documentNumber) {
            DocumentModel document = Factory.CreateDocument();

            HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync($"{documentNumber}.json");
            if (httpResponseMessage.IsSuccessStatusCode) {
                document = await httpResponseMessage.Content.ReadAsAsync<DocumentModel>();
            }
            return document;
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
            var documentNumberToRetrieve = Console.ReadLine();
            try {
                var document = await GetDocumentAsync(documentNumberToRetrieve);
                ShowDocument(document);
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
            }
        }

        public static void ConfigureClient(HttpClient client) {
            _httpClient = client;

            if(_httpClient.BaseAddress is null) {
                _httpClient.BaseAddress = new Uri("https://www.federalregister.gov/api/v1/documents/");
                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json")
                    );
            }
        }
    }
}
