using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Ober.Tool.Interfaces;

namespace Ober.Tool.Api
{
    public enum Method
    {
        Get,
        Post,
        Put
    }

    internal class StoreClientBase
    {
        protected string AccessToken;

        protected const string BaseUrl = "https://manage.devcenter.microsoft.com/";
        protected const string ApplicationJson = "application/json";
        protected const string ApplicationXForm = "application/x-www-form-urlencoded";
        protected HttpClient HttpClient;

        protected ILogger Logger;

        public StoreClientBase(ILogger logger)
        {
            Logger = logger;
        }

        /// <summary>
        /// Retrieve the response of an Oauth request. Add the parameters needed to get the information from the API.
        /// The ResponseObject contains the status (Response.StatusCode) and the Data (JObject class).
        /// </summary>
        /// <param name="body">Body of the request</param>
        /// <param name="resource">Endpoint of the REST API</param>
        /// <param name="baseUrl">The root URL of the REST API</param>
        /// <param name="method">Defines the method of the request, defaults to GET</param>
        /// <param name="contentType">specify the contenttype for Post, defaults to application/x-www-form-urlencoded</param>
        /// <returns>Tuple, contains the Response that holds the StatusCode and object which holds the content. On Response, Response.Message will hold the information.</returns>
        protected async Task<Tuple<JObject, HttpStatusCode>> RestRequest(string body, string resource, string baseUrl = BaseUrl , Method method = Method.Get, string contentType = ApplicationXForm)            
        {
            try
            {
                if (HttpClient == null)
                    HttpClient = GetDefaultClient(baseUrl, AccessToken);
                HttpResponseMessage response;
                switch (method)
                {
                    case Method.Post:
                        response = await HttpClient.PostAsync(resource, new StringContent(body, Encoding.UTF8, contentType));
                        break;
                    case Method.Put:
                        response = await HttpClient.PutAsync(resource, new StringContent(body, Encoding.UTF8, contentType));
                        break;
                    default:
                        response = await HttpClient.GetAsync(resource + body);
                        break;
                }
                var jsonContent = await response.Content.ReadAsStringAsync();
                Logger.Debug($"Response body:{Environment.NewLine}{jsonContent}");
                if (!response.StatusCode.Equals(HttpStatusCode.OK) && !response.StatusCode.Equals(HttpStatusCode.Created))
                    return new Tuple<JObject, HttpStatusCode>(null, response.StatusCode);
                return new Tuple<JObject, HttpStatusCode>(JObject.Parse(jsonContent), response.StatusCode);
            }
            catch (Exception e)
            {
                Logger.Error(e.Message + Environment.NewLine);
                return new Tuple<JObject, HttpStatusCode>(null, HttpStatusCode.BadRequest);
            }
        }

        protected string GetQueryRequestParameters(Dictionary<string, object> parameters, bool post = false)
        {
            if (parameters == null || !parameters.Any())
                return string.Empty;
            var sb = new StringBuilder();
            if (!post)
                sb.Append("?");
            for (var i = 0; i < parameters.Count; i++)
            {
                var p = parameters.ElementAt(i);
                sb.AppendFormat("{0}={1}", p.Key, p.Value);

                if (i < parameters.Count - 1)
                {
                    sb.Append("&");
                }
            }
            return sb.ToString();
        }
        
        protected virtual HttpClient GetDefaultClient(string baseUrl, string accessToken)
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            };
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
            return client;
        }
    }
}
