using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace FederalRegisterClient
{
    public static class HttpRequestHandler
    {
        public static HttpClient _httpClient;

        public static void ConfigureClient(HttpClient client) {
            _httpClient = client;

            if (_httpClient.BaseAddress is null) {
                _httpClient.BaseAddress = new Uri("https://www.federalregister.gov/api/v1/documents/");
                _httpClient.Timeout = TimeSpan.FromMilliseconds(1000);
                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json")
                    );
            }
        }

        public static async Task<DocumentModel> GetDocumentAsJsonAsync(string documentNumber) {
            DocumentModel document = Factory.CreateDocument();

            HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync($"{documentNumber}.json");
            if (httpResponseMessage.IsSuccessStatusCode) {
                document = await httpResponseMessage.Content.ReadAsAsync<DocumentModel>();
            }
            return document;
        }
    }
}
