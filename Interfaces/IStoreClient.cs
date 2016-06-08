using System;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Ober.Tool.Interfaces
{
    public interface IStoreClient
    {
        Task<bool> Login(string clientId, string key, string tenantId);
        Task<Tuple<JObject, HttpStatusCode>> CreateSubmission(string appId);
        Task<Tuple<JObject, HttpStatusCode>> CreateSubmission(string appId, string flightId);
        Task<bool> UpdateSubmission(string appId, string submissionId, string body);
        Task<bool> UpdateSubmission(string appId, string flightId, string submissionId, string body);
        Task UploadPackages(string packageLocation, string fileUploadUrl);
        Task<bool> Commit(string appId, string submissionId);
        Task<bool> Commit(string appId, string flightId, string submissionId);
    }
}