using Newtonsoft.Json;
using System;

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

        [JsonProperty("citation")]
        public string DocumentCitation { get; set; }

        [JsonProperty("subtype")]
        public string PresidentialDocumentType { get; set; }

        [JsonProperty("abstract")]
        public string Abstract { get; set; }
    }
}
