using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace FederalRegisterClient
{
    public static class RequestHandler
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

        public static void ConfigureClient(HttpClient client) {
            _httpClient = client;

            if (_httpClient.BaseAddress is null) {
                _httpClient.BaseAddress = new Uri("https://www.federalregister.gov/api/v1/documents/");
                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json")
                    );
            }
        }
    }
}
