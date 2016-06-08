using System.Threading.Tasks;
using Ober.Tool.Options;

namespace Ober.Tool.Interfaces
{
    internal interface ISubmitCommand
    {
        Task<int> CreateSubmission(SubmitOptions options);
    }
}