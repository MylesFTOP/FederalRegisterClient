using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FederalRegisterClient;

namespace FederalRegisterClient.ConsoleApp
{
    public static class ConsoleUI
    {
        public static async Task RunAsync()
        {
            System.Console.WriteLine("Please enter document reference to retrieve:");
            String documentNumberToRetrieve = System.Console.ReadLine();
            try
            {
                var document = await DocumentHandler.GetDocumentAsync(documentNumberToRetrieve);
                DocumentHandler.ShowDocument(document);
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
            }
        }
    }
}
