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

        [JsonProperty("title")]
        public string DocumentTitle { get; set; }

        [JsonProperty("type")]
        public string PublicationType { get; set; }

        [JsonProperty("publication_date")]
        public DateTime PublicationDate { get; set; }
    }
}
