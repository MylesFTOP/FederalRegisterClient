using System;

namespace FederalRegisterClient
{
    class Program
    {
        static void Main() {
            HttpRequestHandler.ConfigureClient(Factory.CreateHttpClient());
            DocumentHandler
                .RunAsync().GetAwaiter().GetResult();
        }        
    }
}
