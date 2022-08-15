using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Kernel.Api.Client
{
    public sealed class HttpMessageBuilder
    {
        private sealed class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding => Encoding.UTF8;
        }

        private readonly List<KeyValuePair<string, string>> _headers;

        private readonly List<KeyValuePair<string, string>> _querys;

        public HttpMessageBuilder()
        {
            _headers = new List<KeyValuePair<string, string>>();
            _querys = new List<KeyValuePair<string, string>>();
        }

        public HttpMessageBuilder AddHeader<T>(string name, T value)
        {
            _headers.Add(new KeyValuePair<string, string>(name, value.ToString()));
            return this;
        }

        public HttpMessageBuilder AddQuerys<T>(T querys)
        {
            RecursiveObjectType(JObject.FromObject(querys).Children());
            return this;
        }

        private void RecursiveObjectType(JEnumerable<JToken> jTokens)
        {
            foreach (JToken item in jTokens)
            {
                switch (item.First!.Type)
                {
                    case JTokenType.Date:
                        if (item.First != null)
                        {
                            _querys.Add(new KeyValuePair<string, string>(item.Path, ((DateTimeOffset)item.First).ToString("yyyy-MM-ddTHH:mm:ss.fffffffzzz")));
                        }

                        break;
                    case JTokenType.Object:
                        RecursiveObjectType(item.First!.Children());
                        break;
                    case JTokenType.Array:
                        foreach (JToken item2 in (IEnumerable<JToken>)(item.First!))
                        {
                            _querys.Add(new KeyValuePair<string, string>(item.Path, item2.ToString()));
                        }

                        break;
                    default:
                        if (item.First != null && (item.First!.Type != JTokenType.String || !(item.First!.ToString() == string.Empty)))
                        {
                            _querys.Add(new KeyValuePair<string, string>(item.Path, item.First!.ToString()));
                        }

                        break;
                    case JTokenType.Null:
                        break;
                }
            }
        }

        public HttpMessageBuilder AddQuery<T>(string name, T value)
        {
            if (value != null)
            {
                _querys.Add(new KeyValuePair<string, string>(name, value.ToString()));
            }

            return this;
        }

        public HttpMessageBuilder ResetAll()
        {
            _headers.Clear();
            _querys.Clear();
            return this;
        }

        public HttpRequestMessage Generate(HttpMethod method, Uri baseAddress, string path, object data, string contentType)
        {
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(method, UrlHandler(baseAddress, path));
            if (data != null)
            {
                if (contentType == "application/xml" || contentType == "text/xml")
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(data.GetType());
                    MemoryStream memoryStream = new MemoryStream();
                    StreamWriter textWriter = new StreamWriter(memoryStream, Encoding.UTF8);
                    xmlSerializer.Serialize(textWriter, data);
                    memoryStream.Seek(0L, SeekOrigin.Begin);
                    StreamContent streamContent = new StreamContent(memoryStream);
                    streamContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                    httpRequestMessage.Content = streamContent;
                }
                else
                {
                    httpRequestMessage.Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, contentType);
                }
            }

            foreach (KeyValuePair<string, string> header in _headers)
            {
                httpRequestMessage.Headers.Add(header.Key, header.Value.ToString());
            }

            return httpRequestMessage;
        }

        private string UrlHandler(Uri uri, string url)
        {
            string[] array = url.Split(new char[1] { '?' });
            UriBuilder uriBuilder = new UriBuilder(uri.Scheme, uri.Host, uri.Port, string.Join("/", uri.AbsolutePath.TrimEnd(new char[1] { '/' }), array[0].TrimStart(new char[1] { '/' })));
            string path = HttpUtility.UrlDecode(uriBuilder.Path);
            string text = ToQueryString(_querys);
            if (array.Length > 1 && !string.IsNullOrEmpty(array[1]))
            {
                text = "?" + text + "&" + array[1];
            }

            uriBuilder.Path = path;
            uriBuilder.Query = text;
            return uriBuilder.Uri.PathAndQuery;
        }

        private string ToQueryString(List<KeyValuePair<string, string>> keyValuePairs)
        {
            IEnumerable<string> values = keyValuePairs.Select((KeyValuePair<string, string> kv) => HttpUtility.UrlEncode(kv.Key) + "=" + HttpUtility.UrlEncode(kv.Value));
            return string.Join("&", values);
        }
    }
}
