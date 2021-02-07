using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FederalRegisterClient
{
    class Program
    {
        static HttpClient client = Factory.CreateHttpClient();

        static void ShowDocument(DocumentModel document) {
            Console.WriteLine($"Document Reference: {document.FederalRegisterDocumentNumber}");
        }

        static async Task<DocumentModel> GetDocumentAsync(DocumentModel document) {
            HttpResponseMessage response = await client
                .GetAsync($"{document.FederalRegisterDocumentNumber}.json");
            string responseBody = null;
            if (response.IsSuccessStatusCode) {
                responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Response body: {responseBody}");
            }
            return document;
        }

        static void Main() {
            RunAsync().GetAwaiter().GetResult();
        }

        static async Task RunAsync() {
            client.BaseAddress = new Uri("https://www.federalregister.gov/api/v1/documents/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json")
                );

            try {
                DocumentModel document = Factory.CreateDocument();
                document.FederalRegisterDocumentNumber = "01-27917"; // EO 13233, "Further Implementation of the Presidential Records Act"
                document = await GetDocumentAsync(document);
                ShowDocument(document);
            }
            catch(Exception e) {
                Console.WriteLine(e.Message);
            }
        }
    }
}
