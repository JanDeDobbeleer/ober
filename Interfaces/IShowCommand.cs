using System.Threading.Tasks;
using Ober.Tool.Options;

namespace Ober.Tool.Interfaces
{
    internal interface IShowCommand
    {
        Task<int> ShowSubmission(ShowOptions options);
    }
}