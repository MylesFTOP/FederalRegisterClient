using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FederalRegisterClient;

namespace FederalRegisterClient.ConsoleApp
{
    public static class ConsoleUI
    {
        private static void ShowDocuments(List<DocumentModel> documents)
        {
            documents.ForEach(x => ShowDocument(x));
        }

        public static void ShowDocument(DocumentModel document)
        {
            Dictionary<string, string> documentDisplay = new Dictionary<string, string> {
                { "Document Reference", document.FederalRegisterDocumentNumber },
                { "Document Title", document.DocumentTitle },
                { "Publication Date", document.PublicationDate.ToShortDateString() },
                { "Publication Type", document.PublicationType },
                { "Presidential Document Type", document.PresidentialDocumentType },
                { "Document citation", document.DocumentCitation },
                { "Abstract", document.Abstract },
            };

            foreach (KeyValuePair<string, string> item in documentDisplay)
            {
                if (!String.IsNullOrEmpty(item.Value))
                {
                    Console.WriteLine($"{item.Key}: {item.Value}");
                }
            }
        }

        public static async Task RunAsync()
        {
            await RetrieveSingleDocument();
        }

        public static async Task RetrieveSingleDocument()
        { 
            Console.WriteLine("Please enter document reference to retrieve:");
            String documentNumberToRetrieve = Console.ReadLine();
            try
            {
                var document = await DocumentHandler.GetDocumentAsync(documentNumberToRetrieve);
                ShowDocument(document);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
