using System.Net.Http;
using System.Threading.Tasks;

namespace FederalRegisterClient
{
    public interface IRequestHandler
    {
        Task<DocumentModel> GetDocumentAsJsonAsync(string documentNumber);
    }
}