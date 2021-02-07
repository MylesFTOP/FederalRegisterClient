using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace FederalRegisterClient
{
    class Program
    {
        static HttpClient client = Factory.CreateHttpClient();

        static void Main() {
            RunAsync().GetAwaiter().GetResult();
        }

        static async Task RunAsync() {
            client.BaseAddress = new Uri("https://www.federalregister.gov/api/v1/documents/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            try { }
            catch(Exception e) {
                Console.WriteLine(e.Message);
            }
        }
    }
}
