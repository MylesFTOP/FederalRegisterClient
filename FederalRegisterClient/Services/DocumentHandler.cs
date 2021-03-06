﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FederalRegisterClient
{
    public static class DocumentHandler
    {
        public static List<DocumentModel> GetDocuments(List<string> documentNumbers)
        {
            // TODO: Check if there's a way to pass an array or range in a single call instead of making repeated calls
            List<DocumentModel> documents = Factory.CreateDocumentList();
            foreach (var documentNumber in documentNumbers)
            {
                documents.Add(GetDocumentAsync(documentNumber).Result);
            }
            return documents;
        }

        public static async Task<DocumentModel> GetDocumentAsync(string documentNumber)
        {
            return await HttpRequestHandler.GetDocumentAsJsonAsync(documentNumber);
        }

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
                Console.WriteLine($"{item.Key}: {item.Value}");
            }
        }

        public static async Task RunAsync()
        {
            Console.WriteLine("Please enter document reference to retrieve:");
            String documentNumberToRetrieve = Console.ReadLine();
            try
            {
                var document = await GetDocumentAsync(documentNumberToRetrieve);
                ShowDocument(document);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
