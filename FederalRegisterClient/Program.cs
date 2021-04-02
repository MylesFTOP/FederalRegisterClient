using System;

namespace FederalRegisterClient
{
    class Program
    {
        static void Main() {
            HttpRequestHandler.ConfigureClient(
                Factory.CreateHttpClient(), "https://www.federalregister.gov/api/v1/documents/");
            DocumentHandler
                .RunAsync().GetAwaiter().GetResult();
        }        
    }
}
