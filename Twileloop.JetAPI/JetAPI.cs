using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Twileloop.JetAPI {
    public class JetRequest {
        private readonly HttpClient _httpClient;
        private readonly HttpMethod _method;
        private readonly Dictionary<string, string> _headers;
        private readonly Dictionary<string, string> _queries;
        private HttpContent _content;

        public JetRequest() {
            _httpClient = new HttpClient();
            _method = HttpMethod.Get;
            _headers = new Dictionary<string, string>();
            _queries = new Dictionary<string, string>();
        }

        public JetRequest(HttpMethod method) {
            _httpClient = new HttpClient();
            _method = method ?? throw new ArgumentNullException(nameof(method));
            _headers = new Dictionary<string, string>();
            _queries = new Dictionary<string, string>();
        }

        public JetRequest Get() {
            return new JetRequest(HttpMethod.Get);
        }

        public JetRequest Post() {
            return new JetRequest(HttpMethod.Post);
        }

        public JetRequest Put() {
            return new JetRequest(HttpMethod.Put);
        }

        public JetRequest Patch() {
            return new JetRequest(new HttpMethod("PATCH"));
        }

        public JetRequest WithHeaders(params Param[] headers) {
            if (headers == null) throw new ArgumentNullException(nameof(headers));

            foreach (var param in headers) {
                if (param.Key == null) throw new ArgumentNullException(nameof(param.Key));
                if (param.Value == null) throw new ArgumentNullException(nameof(param.Value));
                _headers[param.Key] = param.ValueString;
            }

            return this;
        }

        public JetRequest WithQueries(params Param[] queries) {
            if (queries == null) throw new ArgumentNullException(nameof(queries));

            foreach (var param in queries) {
                if (param.Key == null) throw new ArgumentNullException(nameof(param.Key));
                if (param.Value == null) throw new ArgumentNullException(nameof(param.Value));
                _queries[param.Key] = param.ValueString;
            }

            return this;
        }

        public JetRequest WithBody(RawBody body) {
            if (body == null) throw new ArgumentNullException(nameof(body));
            _content = new StringContent(body.Content, Encoding.UTF8, body.ContentType);
            return this;
        }

        public async Task<T> ExecuteAsync<T>(string url) {
            var uriBuilder = new UriBuilder(url);
            uriBuilder.Query = BuildQueryString(_queries);

            var request = new HttpRequestMessage {
                Method = _method,
                RequestUri = uriBuilder.Uri,
                Content = _content,
            };

            foreach (var (key, value) in _headers) {
                request.Headers.Add(key, value);
            }

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode) {
                throw new HttpRequestException($"Status code: {response.StatusCode}, Reason phrase: {response.ReasonPhrase}");
            }

            using var responseStream = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<T>(responseStream);
        }

        private static string BuildQueryString(Dictionary<string, string> queries) {
            var queryString = new StringBuilder();

            foreach (var (key, value) in queries) {
                queryString.Append(Uri.EscapeDataString(key));
                queryString.Append("=");
                queryString.Append(Uri.EscapeDataString(value));
                queryString.Append("&");
            }

            if (queryString.Length > 0) {
                queryString.Length--;
            }

            return queryString.ToString();
        }
    }

    public class RawBody {
        public string Content { get; }
        public string ContentType { get; }

        public RawBody(BodyType bodyType, string content) {
            if (content == null) throw new ArgumentNullException(nameof(content));

            ContentType = bodyType switch {
                BodyType.Json => "application/json",
                BodyType.XML => "application/xml",
                BodyType.Text => "text/plain",
                BodyType.HTML => "text/html",
                BodyType.JavaScript => "application/javascript",
                _ => throw new ArgumentOutOfRangeException(nameof(bodyType), bodyType, null)
            };

            Content = content;
        }

        public RawBody(BodyType bodyType, object content) {
            if (content == null) throw new ArgumentNullException(nameof(content));

            ContentType = bodyType switch {
                BodyType.Json => "application/json",
                BodyType.XML => "application/xml",
                BodyType.Text => "text/plain",
                BodyType.HTML => "text/html",
                BodyType.JavaScript => "application/javascript",
                _ => throw new ArgumentOutOfRangeException(nameof(bodyType), bodyType, null)
            };

            switch (bodyType) {
                case BodyType.Json:
                    Content = JsonSerializer.Serialize(content);
                    break;
                case BodyType.XML:
                    var serializer = new XmlSerializer(content.GetType());
                    using (var writer = new StringWriter())
                    {
                        serializer.Serialize(writer, content);
                        Content = writer.ToString();
                    }
                    break;
                default:
                    Content = content.ToString();
                    break;
            }
        }

    }

    public enum BodyType {
        Json,
        XML,
        Text,
        HTML,
        JavaScript
    }

    public struct Param {
        public Param(string key, object value) : this() {
            Key = key;
            Value = value;
        }

        public string Key { get; set; }
        public object Value { get; set; }
        public string ValueString { get => JsonSerializer.Serialize(Value);}
}

}

