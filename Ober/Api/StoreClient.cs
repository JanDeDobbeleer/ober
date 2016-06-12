using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json.Linq;
using Ober.Tool.Interfaces;

namespace Ober.Tool.Api
{
    internal class StoreClient: StoreClientBase, IStoreClient
    {
        private readonly string _authenticationBase = "https://login.microsoftonline.com";
        private readonly string _authenticationResource = "/{0}/oauth2/token";
        private readonly string _createRelease = "/v1.0/my/applications/{0}/submissions";
        private readonly string _createFlight = "/v1.0/my/applications/{0}/flights/{1}/submissions";
        private readonly string _updateRelease = "/v1.0/my/applications/{0}/submissions/{1}";
        private readonly string _updateFlight = "/v1.0/my/applications/{0}/flights/{1}/submissions/{2}";
        private readonly string _commitRelease = "/v1.0/my/applications/{0}/submissions/{1}/commit";
        private readonly string _commitFlight = "/v1.0/my/applications/{0}/flights/{1}/submissions/{2}/commit";

        public StoreClient(ILogger logger): base(logger) { }

        public async Task<bool> Login(string clientId, string key, string tenantId)
        {
            Logger.Debug("Preparing login");
            var parameters = new Dictionary<string, object>
            {
                { "grant_type", "client_credentials" },
                { "client_id", WebUtility.UrlEncode(clientId) },
                { "client_secret", WebUtility.UrlEncode(key) },
                { "resource", "https://manage.devcenter.microsoft.com" }
            };
            var response = await RestRequest(GetQueryRequestParameters(parameters, true), string.Format(_authenticationResource, tenantId), _authenticationBase, Method.Post);
            //force new HttpClient creation afterwards
            HttpClient = null;
            if (!response.Item2.Equals(HttpStatusCode.OK))
            {
                Logger.Debug($"Error logging in, response statuscode = {response.Item2}");
                return false;
            }
            Logger.Debug("Login successful, setting accesstoken for future use.");
            AccessToken = response.Item1["access_token"].ToString();
            return true;
        }

        public async Task<Tuple<JObject, HttpStatusCode>> CreateSubmission(string appId)
        {
            return await RestRequest(string.Empty, string.Format(_createRelease, appId), method:Method.Post, contentType:ApplicationJson);
        }

        public async Task<Tuple<JObject, HttpStatusCode>> CreateSubmission(string appId, string flightId)
        {
            return await RestRequest(string.Empty, string.Format(_createFlight, appId, flightId), method: Method.Post, contentType: ApplicationJson);
        }

        public async Task<bool> UpdateSubmission(string appId, string submissionId, string body)
        {
            var response = await RestRequest(body, string.Format(_updateRelease, appId, submissionId), method: Method.Put, contentType: ApplicationJson);
            return response.Item2.Equals(HttpStatusCode.OK);
        }

        public async Task<bool> UpdateSubmission(string appId, string flightId, string submissionId, string body)
        {
            var response = await RestRequest(body, string.Format(_updateFlight, appId, flightId, submissionId), method: Method.Put, contentType: ApplicationJson);
            return response.Item2.Equals(HttpStatusCode.OK);
        }

        public async Task UploadPackages(string packageLocation, string fileUploadUrl)
        {
            using (Stream stream = new FileStream(packageLocation, FileMode.Open))
            {
                var blockBob = new CloudBlockBlob(new Uri(fileUploadUrl));
                await blockBob.UploadFromStreamAsync(stream);
            }
        }

        public async Task<bool> Commit(string appId, string submissionId)
        {
            var response = await RestRequest(string.Empty, string.Format(_commitRelease, appId, submissionId), method: Method.Post, contentType: ApplicationJson);
            return response.Item2.Equals(HttpStatusCode.Accepted);
        }

        public async Task<bool> Commit(string appId, string flightId, string submissionId)
        {
            var response = await RestRequest(string.Empty, string.Format(_commitFlight, appId, flightId, submissionId), method: Method.Post, contentType: ApplicationJson);
            return response.Item2.Equals(HttpStatusCode.Accepted);
        } 
    }
}
