using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extensions;
using Kernel.Api.Client;
using Sample.Api.Client.Sample.Interface;

namespace Sample.Api.Client.Sample
{
    public partial class SampleApiClient : BaseClient, ISampleApiClient
    {
        private HttpClient _httpClient { get; set; }
        public SampleApiClient(HttpClient httpClient) : base(httpClient)
        {
            _httpClient = httpClient;
        }
        protected Task<string> GetStringAsync(string url, Action<HttpMessageBuilder> builder = null, string contentType = ContentTypes.ApplicationJson, bool isSerializeContent = true, bool isReplaceBaseAddress = false, bool isEncoding = true, CancellationToken cancellationToken = default)
        {
            return SenderAndReceiverAsync(HttpMethod.Get, url, null, builder, contentType, cancellationToken);
        }

        private async Task<string> SenderAndReceiverAsync(HttpMethod method, string url, object data, Action<HttpMessageBuilder> builderAction, string contentType, CancellationToken cancellationToken)
        {
            var builder = new HttpMessageBuilder();
            var request = default(HttpRequestMessage);
            builderAction?.Invoke(builder);

            request = builder.Generate(method, _httpClient.BaseAddress, url, data, contentType);

            var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
            var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var validationErrors = content.DeserializeJson<Dictionary<string, List<string>>>();
                if (validationErrors != default)
                {
                    throw new ApiValidationException(request.Method.ToString(), url, "Api Validation fail", validationErrors);
                }
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(content);
            }

            return content;
        }
    }
}
