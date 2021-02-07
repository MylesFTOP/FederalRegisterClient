using System;
using System.Collections.Generic;
using System.Text;

namespace FederalRegisterClient
{
    public static class DocumentHandler
    {
        public static void ShowDocument(DocumentModel document) {
            Console.WriteLine($"Document Reference: {document.FederalRegisterDocumentNumber}");
        }
    }
}
