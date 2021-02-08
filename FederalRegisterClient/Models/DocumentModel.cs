using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace FederalRegisterClient
{
    public class DocumentModel
    {
        [JsonProperty("document_number")]
        public string FederalRegisterDocumentNumber { get; set; }
    }
}
