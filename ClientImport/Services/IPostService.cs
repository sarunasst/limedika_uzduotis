using System.Threading.Tasks;

namespace ClientImport.Services
{
    public interface IPostService
    {
        Task<string> GetPostIndexAsync(string address);
    }
}