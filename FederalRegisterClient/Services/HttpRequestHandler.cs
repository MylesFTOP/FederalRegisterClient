using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FederalRegisterClient
{
    public static class HttpRequestHandler
    {
        public static HttpClient _httpClient;
        private static int RetryAttempts { get; set; }

        public static void ConfigureClient(HttpClient client, string url) {
            _httpClient = client;

            if (_httpClient.BaseAddress is null) {
                _httpClient.BaseAddress = new Uri(url);
                _httpClient.Timeout = TimeSpan.FromMilliseconds(1000);
                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json")
                    );
            }
        }

        public static async Task<DocumentModel> GetDocumentAsJsonAsync(string documentNumber) {
            DocumentModel document = Factory.CreateDocument();
            var cancellationTokenSource = new CancellationTokenSource();

            HttpResponseMessage httpResponseMessage = await ReturnDocumentAsJsonAsync(documentNumber);
            if (httpResponseMessage.IsSuccessStatusCode) {
                document = await httpResponseMessage.Content.ReadAsAsync<DocumentModel>();
            }
            else if (StatusCodeIsRetryable(httpResponseMessage.StatusCode)) {
                TimeSpan retryDelay = httpResponseMessage.Headers.RetryAfter.Delta ?? default;
                await Task.Delay(retryDelay);
                httpResponseMessage = await ReturnDocumentAsJsonAsync(documentNumber);
            }
            return document;
        }   

        private static bool StatusCodeIsRetryable(HttpStatusCode statusCode)
        {
            HttpStatusCode[] retryableStatusCodes = { 
                HttpStatusCode.TooManyRequests
            };
            return retryableStatusCodes.Contains(statusCode);
        }

        private static async Task<HttpResponseMessage> ReturnDocumentAsJsonAsync(string documentNumber)
        {
            return await _httpClient.GetAsync($"{documentNumber}.json");
        }
    }
}
