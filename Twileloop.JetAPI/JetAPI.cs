using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Twileloop.JetAPI.Authentication;
using Twileloop.JetAPI.Body;
using Twileloop.JetAPI.Types;

namespace Twileloop.JetAPI {

    public class APIRequest {
        public HttpClient HttpClient { get; set; }
        public HttpMethod HttpMethod { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public Dictionary<string, string> QueryParameters { get; set; }
        public HttpContent HttpContent { get; set; }

        public APIRequest(HttpMethod method) {
            HttpClient = new HttpClient();
            HttpMethod = method;
            Headers = new Dictionary<string, string>();
            QueryParameters = new Dictionary<string, string>();
        }

        public APIRequest() {
            HttpClient = new HttpClient();
            HttpMethod = HttpMethod.Get;
            Headers = new Dictionary<string, string>();
            QueryParameters = new Dictionary<string, string>();
        }
    }
    public class APIResponse {

    }


    public class JetRequest {

        private APIRequest _apiRequest;
        private Interceptor _interceptor;
        private Action _successCapture;
        private Action _failureCapture;
        private Action<Exception> _onException;
        private List<(HttpStatusCode, Action)> _extendedCaptures;

        /// <summary>
        /// Initializes a new instance of the JetRequest class with default HttpMethod Get.
        /// </summary>
        public JetRequest() {
            _apiRequest = new APIRequest();
        }

        /// <summary>
        /// Initializes a new instance of the JetRequest class with the specified HttpMethod.
        /// </summary>
        /// <param name="method">The HttpMethod to use for the request.</param>
        private JetRequest(HttpMethod method) {
            _apiRequest = new APIRequest(method);
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
                _apiRequest.Headers[param.Key] = param.ValueString;
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
                _apiRequest.QueryParameters[param.Key] = param.ValueString;
            }

            return this;
        }

        public JetRequest WithBody(RawBody body) {
            if (body == null) throw new ArgumentNullException(nameof(body));
            _apiRequest.HttpContent = new StringContent(body.Content, Encoding.UTF8, body.ContentType);
            return this;
        }

        public JetRequest WithAuthentication(BasicAuthentication basicAuth) {
            if (basicAuth == null) {
                throw new ArgumentNullException(nameof(basicAuth));
            }
            if (basicAuth.EncodeAsBase64) {
                var encodedCredentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{basicAuth.Username}:{basicAuth.Password}"));
                _apiRequest.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", encodedCredentials);
            }
            else {
                _apiRequest.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", $"{basicAuth.Username}:{basicAuth.Password}");
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
            _apiRequest.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(apiKey.HeaderName, apiKey.APIKey);
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
            _apiRequest.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken.Token);
            return this;
        }

        /// <summary>
        /// Pass an instance of interceptor to make hook events
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance">An instance of Interceptor</param>
        /// <returns></returns>
        public JetRequest WithInterceptor<T>(T instance) where T : Interceptor {
            _interceptor = instance;
            return this;
        }

        /// <summary>
        /// Add a basic capture block to capture success and failures
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance">An instance of Interceptor</param>
        /// <returns></returns>
        public JetRequest WithCaptures(Action onSuccess, Action onFailure) {
            _successCapture = onSuccess;
            _failureCapture = onFailure;
            return this;
        }

        /// <summary>
        ///  Add a advanced capture block to capture success and failures
        /// </summary>
        /// <param name="extendedCaptures"></param>
        /// <returns></returns>
        public JetRequest WithCaptures(params (HttpStatusCode, Action)[] extendedCaptures) {
            _extendedCaptures = extendedCaptures.ToList();
            return this;
        }

        /// <summary>
        /// Add a handler to capture exceptions
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public JetRequest HandleExceptions(Action<Exception> exception) {
            _onException = exception;
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
            try {
                _interceptor?.OnInit();
                var uriBuilder = new UriBuilder(url);
                uriBuilder.Query = BuildQueryString(_apiRequest.QueryParameters);

                var request = new HttpRequestMessage {
                    Method = _apiRequest.HttpMethod,
                    RequestUri = uriBuilder.Uri,
                    Content = _apiRequest.HttpContent,
                };

                foreach (var (key, value) in _apiRequest.Headers) {
                    request.Headers.Add(key, value);
                }

                //Interceptors
                _interceptor?.OnRequesting(_apiRequest);
                var response = await _apiRequest.HttpClient.SendAsync(request);
                _interceptor?.OnResponseReceived();

                //Extended captures
                if (_extendedCaptures != null) {
                    foreach (var capture in _extendedCaptures) {
                        if (capture.Item1 == response.StatusCode) {
                            capture.Item2();
                            break;
                        }
                    }
                }

                //Basic captures
                if (!response.IsSuccessStatusCode) {
                    _failureCapture?.Invoke();
                }

                using var responseStream = await response.Content.ReadAsStreamAsync();
                _successCapture?.Invoke();

                return await JsonSerializer.DeserializeAsync<T>(responseStream);
            }
            catch (Exception ex) {
                _onException(ex);
                return default;
            }
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

