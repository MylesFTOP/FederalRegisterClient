using System;

namespace FederalRegisterClient
{
    class Program
    {
        static void Main() {
            RequestHandler.ConfigureClient(Factory.CreateHttpClient());
            DocumentHandler
                .RunAsync().GetAwaiter().GetResult();
        }        
    }
}
