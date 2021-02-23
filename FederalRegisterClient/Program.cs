using System;

namespace FederalRegisterClient
{
    class Program
    {
        static void Main() {
            DocumentHandler.ConfigureClient(Factory.CreateHttpClient());
            DocumentHandler
                .RunAsync().GetAwaiter().GetResult();
        }        
    }
}
