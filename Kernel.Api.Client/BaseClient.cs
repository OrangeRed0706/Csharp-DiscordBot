using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using Extensions;

namespace Kernel.Api.Client
{
    public class BaseClient
    {
        private readonly HttpClient _httpClient;

        protected BaseClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        protected Task DeleteAsync(string url, Action<HttpMessageBuilder> builder = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return DeleteAsync(url, null, builder, cancellationToken);
        }

        protected Task DeleteAsync(string url, object data, Action<HttpMessageBuilder> builder = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return SenderAndReceiverAsync(HttpMethod.Delete, url, data, builder, cancellationToken);
        }

        protected Task<T> DeleteAsync<T>(string url, Action<HttpMessageBuilder> builder = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return DeleteAsync<T>(url, null, builder, cancellationToken);
        }

        protected Task<T> DeleteAsync<T>(string url, object data, Action<HttpMessageBuilder> builder = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return SenderAndReceiverAsync<T>(HttpMethod.Delete, url, data, builder, cancellationToken);
        }

        protected Task PutAsync(string url, Action<HttpMessageBuilder> builder = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return PutAsync(url, null, builder, cancellationToken);
        }

        protected Task PutAsync(string url, object data, Action<HttpMessageBuilder> builder = null, CancellationToken cancellationToken = default(CancellationToken), string contentType = "application/json")
        {
            return SenderAndReceiverAsync(HttpMethod.Put, url, data, builder, cancellationToken, contentType);
        }

        protected Task<T> PutAsync<T>(string url, Action<HttpMessageBuilder> builder = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return PutAsync<T>(url, null, builder, cancellationToken);
        }

        protected Task<T> PutAsync<T>(string url, object data, Action<HttpMessageBuilder> builder = null, CancellationToken cancellationToken = default(CancellationToken), string contentType = "application/json")
        {
            return SenderAndReceiverAsync<T>(HttpMethod.Put, url, data, builder, cancellationToken, contentType);
        }

        protected Task PostAsync(string url, Action<HttpMessageBuilder> builder = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return PostAsync(url, null, builder, cancellationToken);
        }

        protected Task PostAsync(string url, object data, Action<HttpMessageBuilder> builder = null, CancellationToken cancellationToken = default(CancellationToken), string contentType = "application/json")
        {
            return SenderAndReceiverAsync(HttpMethod.Post, url, data, builder, cancellationToken, contentType);
        }

        protected Task<T> PostAsync<T>(string url, Action<HttpMessageBuilder> builder = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return PostAsync<T>(url, null, builder, cancellationToken);
        }

        protected Task<T> PostAsync<T>(string url, object data, Action<HttpMessageBuilder> builder = null, CancellationToken cancellationToken = default(CancellationToken), string contentType = "application/json")
        {
            return SenderAndReceiverAsync<T>(HttpMethod.Post, url, data, builder, cancellationToken, contentType);
        }

        protected Task GetAsync(string url, Action<HttpMessageBuilder> builder = null, CancellationToken cancellationToken = default(CancellationToken), string contentType = "application/json")
        {
            return SenderAndReceiverAsync(HttpMethod.Get, url, null, builder, cancellationToken, contentType);
        }

        protected Task<T> GetAsync<T>(string url, Action<HttpMessageBuilder> builder = null, CancellationToken cancellationToken = default(CancellationToken), string contentType = "application/json")
        {
            return SenderAndReceiverAsync<T>(HttpMethod.Get, url, null, builder, cancellationToken, contentType);
        }

        private async Task<string> SenderAndReceiverAsync(HttpMethod method, string url, object data, Action<HttpMessageBuilder> builderAction, CancellationToken cancellationToken, string contentType = "application/json")
        {
            HttpMessageBuilder httpMessageBuilder = new HttpMessageBuilder();
            builderAction?.Invoke(httpMessageBuilder);
            HttpRequestMessage request = httpMessageBuilder.Generate(method, _httpClient.BaseAddress, url, data, contentType);
            HttpResponseMessage response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
            string text = await response.Content.ReadAsStringAsync().ConfigureAwait(continueOnCapturedContext: false);
            if (response.IsSuccessStatusCode)
            {
                return text;
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                JObject jObject = text.DeserializeJson<JObject>();
                Dictionary<string, List<string>> dictionary = jObject["errors"]!.ToString().DeserializeJson<Dictionary<string, List<string>>>();
                if (dictionary != null)
                {
                    throw new ApiValidationException(request.Method.Method, request.RequestUri!.ToString(), $"{GetType().Name} connect to {_httpClient.BaseAddress} api validation fail", dictionary);
                }
            }

            throw new HttpRequestException($"{GetType().Name} connect to {_httpClient.BaseAddress} error[{response.StatusCode}]. request[{JsonConvert.SerializeObject(response.RequestMessage)}]");
        }

        private async Task<T> SenderAndReceiverAsync<T>(HttpMethod method, string url, object data, Action<HttpMessageBuilder> builderAction, CancellationToken cancellationToken, string contentType = "application/json")
        {
            string content = await SenderAndReceiverAsync(method, url, data, builderAction, cancellationToken, contentType).ConfigureAwait(continueOnCapturedContext: false);
            if (contentType == "application/xml" || contentType == "text/xml")
            {
                return content.DeserializeXml<T>();
            }

            return content.DeserializeJson<T>();
        }
    }
}
