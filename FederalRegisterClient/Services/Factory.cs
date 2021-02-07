using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;

namespace FederalRegisterClient
{
    public class Factory
    {
        public static HttpClient CreateHttpClient() {
            return new HttpClient();
        }

        public static DocumentModel CreateDocument() {
            return new DocumentModel();
        }
    }
}
