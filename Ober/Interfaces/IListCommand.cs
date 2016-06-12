using System.Threading.Tasks;
using Ober.Tool.Options;

namespace Ober.Tool.Interfaces
{
    internal interface IListCommand
    {
        Task<int> ListSubmissions(ListOptions options);
    }
}