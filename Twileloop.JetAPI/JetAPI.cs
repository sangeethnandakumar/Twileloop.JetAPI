using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Twileloop.JetAPI.Authentication;
using Twileloop.JetAPI.Body;
using Twileloop.JetAPI.Types;

namespace Twileloop.JetAPI {
    public class JetRequest {
        private readonly HttpClient _httpClient;
        private readonly HttpMethod _method;
        private readonly Dictionary<string, string> _headers;
        private readonly Dictionary<string, string> _queries;
        private HttpContent _content;

        /// <summary>
        /// Initializes a new instance of the JetRequest class with default HttpMethod Get.
        /// </summary>
        public JetRequest() {
            _httpClient = new HttpClient();
            _method = HttpMethod.Get;
            _headers = new Dictionary<string, string>();
            _queries = new Dictionary<string, string>();
        }

        /// <summary>
        /// Initializes a new instance of the JetRequest class with the specified HttpMethod.
        /// </summary>
        /// <param name="method">The HttpMethod to use for the request.</param>
        public JetRequest(HttpMethod method) {
            _httpClient = new HttpClient();
            _method = method ?? throw new ArgumentNullException(nameof(method));
            _headers = new Dictionary<string, string>();
            _queries = new Dictionary<string, string>();
        }

        /// <summary>
        /// Returns a new JetRequest instance with HttpMethod Get.
        /// </summary>
        /// <returns>A new JetRequest instance with HttpMethod Get.</returns>
        public JetRequest Get() {
            return new JetRequest(HttpMethod.Get);
        }

        /// <summary>
        /// Returns a new JetRequest instance with HttpMethod Post.
        /// </summary>
        /// <returns>A new JetRequest instance with HttpMethod Post.</returns>
        public JetRequest Post() {
            return new JetRequest(HttpMethod.Post);
        }

        /// <summary>
        /// Returns a new JetRequest instance with HttpMethod Put.
        /// </summary>
        /// <returns>A new JetRequest instance with HttpMethod Put.</returns>
        public JetRequest Put() {
            return new JetRequest(HttpMethod.Put);
        }

        /// <summary>
        /// Returns a new JetRequest instance with HttpMethod Patch.
        /// </summary>
        /// <returns>A new JetRequest instance with HttpMethod Patch.</returns>
        public JetRequest Patch() {
            return new JetRequest(new HttpMethod("PATCH"));
        }

        /// <summary>
        /// Adds the specified headers to the request.
        /// </summary>
        /// <param name="headers">The headers to add to the request.</param>
        /// <returns>The current JetRequest instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when headers is null.</exception>
        public JetRequest WithHeaders(params Param[] headers) {
            if (headers == null) throw new ArgumentNullException(nameof(headers));

            foreach (var param in headers) {
                if (param.Key == null) throw new ArgumentNullException(nameof(param.Key));
                if (param.Value == null) throw new ArgumentNullException(nameof(param.Value));
                _headers[param.Key] = param.ValueString;
            }

            return this;
        }

        /// <summary>
        /// Adds the specified queries to the request.
        /// </summary>
        /// <param name="queries">The queries to add to the request.</param>
        /// <returns>The current JetRequest instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when queries is null.</exception>
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

        public JetRequest WithAuthentication(BasicAuthentication basicAuth) {
            if (basicAuth == null) {
                throw new ArgumentNullException(nameof(basicAuth));
            }
            if (basicAuth.EncodeAsBase64) {
                var encodedCredentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{basicAuth.Username}:{basicAuth.Password}"));
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", encodedCredentials);
            }
            else {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", $"{basicAuth.Username}:{basicAuth.Password}");
            }
            return this;
        }

        /// <summary>
        /// Sets the API key authentication header for the HTTP request.
        /// </summary>
        /// <param name="apiKey">The API key information to use for authentication.</param>
        /// <returns>The JetRequest instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="apiKey"/> is null.</exception>
        public JetRequest WithAuthentication(ApiKey apiKey) {
            if (apiKey == null) {
                throw new ArgumentNullException(nameof(apiKey));
            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(apiKey.HeaderName, apiKey.APIKey);
            return this;
        }

        /// <summary>
        /// Sets the bearer token authentication header for the HTTP request.
        /// </summary>
        /// <param name="bearerToken">The bearer token information to use for authentication.</param>
        /// <returns>The JetRequest instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="bearerToken"/> is null.</exception>
        public JetRequest WithAuthentication(BearerToken bearerToken) {
            if (bearerToken == null) {
                throw new ArgumentNullException(nameof(bearerToken));
            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken.Token);
            return this;
        }

        /// <summary>
        /// Sends the HTTP request asynchronously and deserializes the JSON response into the specified type.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the response into.</typeparam>
        /// <param name="url">The URL to send the HTTP request to.</param>
        /// <returns>A task representing the asynchronous operation. The result of the task is the deserialized response.</returns>
        /// <exception cref="HttpRequestException">Thrown when the HTTP request is not successful (status code is not in the 2xx range).</exception>
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

        /// <summary>
        /// Local function to build query string safely
        /// </summary>
        /// <param name="queries"></param>
        /// <returns></returns>
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
}

