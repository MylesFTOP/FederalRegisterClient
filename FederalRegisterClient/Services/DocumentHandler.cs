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
        public static HttpClient client = Factory.CreateHttpClient();

        public static async Task<DocumentModel> GetDocumentAsync(string documentNumber) {
            DocumentModel document = Factory.CreateDocument();

            HttpResponseMessage response = await client
                .GetAsync($"{documentNumber}.json");
            if (response.IsSuccessStatusCode) {
                document = await response.Content.ReadAsAsync<DocumentModel>();
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
            ConfigureClient();

            try {
                var document = await GetDocumentAsync("01-27917"); // EO 13233, "Further Implementation of the Presidential Records Act"
                ShowDocument(document);
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
            }
        }

        public static void ConfigureClient() {
            if(client.BaseAddress is null) {
                client.BaseAddress = new Uri("https://www.federalregister.gov/api/v1/documents/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json")
                    );
            }
        }
    }
}
