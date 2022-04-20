namespace FederalRegisterClient.ConsoleApp
{
    class Program
    {
        static void Main()
        {
            HttpRequestHandler.ConfigureClient(
                Factory.CreateHttpClient(), "https://www.federalregister.gov/api/v1/documents/");
            ConsoleUI
                .RunAsync().GetAwaiter().GetResult();
        }
    }
}
