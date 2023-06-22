﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Twileloop.JetAPI.Authentication;
using Twileloop.JetAPI.Body;
using Twileloop.JetAPI.Types;

namespace Twileloop.JetAPI
{
    public class JetRequest<T>
    {

        private Request _apiRequest;
        private Interceptor _interceptor;
        private Action<JetResponse<T>> _successCapture;
        private Action<JetResponse<T>> _failureCapture;
        private Action<Exception> _onException;
        private List<(HttpStatusCode, Action)> _extendedCaptures;
        private ContentType _contentType = ContentType.Json;

        /// <summary>
        /// Initializes a new instance of the JetRequest class with default HttpMethod Get.
        /// </summary>
        public JetRequest()
        {
            _apiRequest = new Request();
        }

        /// <summary>
        /// Initializes a new instance of the JetRequest class with the specified HttpMethod.
        /// </summary>
        /// <param name="method">The HttpMethod to use for the request.</param>
        private JetRequest(HttpMethod method)
        {
            _apiRequest = new Request(method);
        }

        /// <summary>
        /// Returns a new JetRequest instance with HttpMethod Get.
        /// </summary>
        /// <returns>A new JetRequest instance with HttpMethod Get.</returns>
        public JetRequest<T> Get()
        {
            return new JetRequest<T>(HttpMethod.Get);
        }

        /// <summary>
        /// Returns a new JetRequest instance with HttpMethod Post.
        /// </summary>
        /// <returns>A new JetRequest instance with HttpMethod Post.</returns>
        public JetRequest<T> Post()
        {
            return new JetRequest<T>(HttpMethod.Post);
        }

        /// <summary>
        /// Returns a new JetRequest instance with HttpMethod Put.
        /// </summary>
        /// <returns>A new JetRequest instance with HttpMethod Put.</returns>
        public JetRequest<T> Put()
        {
            return new JetRequest<T>(HttpMethod.Put);
        }

        /// <summary>
        /// Returns a new JetRequest instance with HttpMethod Patch.
        /// </summary>
        /// <returns>A new JetRequest instance with HttpMethod Patch.</returns>
        public JetRequest<T> Patch()
        {
            return new JetRequest<T>(new HttpMethod("PATCH"));
        }

        /// <summary>
        /// Adds the specified headers to the request.
        /// </summary>
        /// <param name="headers">The headers to add to the request.</param>
        /// <returns>The current JetRequest instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when headers is null.</exception>
        public JetRequest<T> WithHeaders(params Param[] headers)
        {
            if (headers == null) throw new ArgumentNullException(nameof(headers));

            foreach (var param in headers)
            {
                if (param.Key == null) throw new ArgumentNullException(nameof(param.Key));
                if (param.Value == null) throw new ArgumentNullException(nameof(param.Value));
                _apiRequest.Headers[param.Key] = (string)param.Value;
            }

            return this;
        }
        
        public JetRequest<T> WithFormData(params Param[] formData)
        {
            if (formData == null) throw new ArgumentNullException(nameof(formData));
            var formDatas = new Dictionary<string, string>();
            foreach (var param in formData)
            {
                formDatas.Add(param.Key, (string)param.Value);
            }
            _apiRequest.HttpContent = new FormUrlEncodedContent(formDatas);
            return this;
        }

        /// <summary>
        /// Adds the specified queries to the request.
        /// </summary>
        /// <param name="queries">The queries to add to the request.</param>
        /// <returns>The current JetRequest instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when queries is null.</exception>
        public JetRequest<T> WithQueries(params Param[] queries)
        {
            if (queries == null) throw new ArgumentNullException(nameof(queries));

            foreach (var param in queries)
            {
                if (param.Key == null) throw new ArgumentNullException(nameof(param.Key));
                if (param.Value == null) throw new ArgumentNullException(nameof(param.Value));
                _apiRequest.QueryParameters[param.Key] = (string)param.Value;
            }

            return this;
        }

        public JetRequest<T> WithBody(RawBody body)
        {
            if (body == null) throw new ArgumentNullException(nameof(body));
            _apiRequest.HttpContent = new StringContent(body.Content, Encoding.UTF8, body.ContentType);
            return this;
        }

        public JetRequest<T> WithAuthentication(BasicAuthentication basicAuth)
        {
            if (basicAuth == null)
            {
                throw new ArgumentNullException(nameof(basicAuth));
            }
            if (basicAuth.EncodeAsBase64)
            {
                var encodedCredentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{basicAuth.Username}:{basicAuth.Password}"));
                _apiRequest.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", encodedCredentials);
            }
            else
            {
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
        public JetRequest<T> WithAuthentication(ApiKey apiKey)
        {
            if (apiKey == null)
            {
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
        public JetRequest<T> WithAuthentication(BearerToken bearerToken)
        {
            if (bearerToken == null)
            {
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
        public JetRequest<T> WithInterceptor<V>(V instance) where V : Interceptor
        {
            _interceptor = instance;
            return this;
        }

        /// <summary>
        /// Add a basic capture block to capture success and failures
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance">An instance of Interceptor</param>
        /// <returns></returns>
        public JetRequest<T> WithCaptures(Action<JetResponse<T>> onSuccess, Action<JetResponse<T>> onFailure)
        {
            _successCapture = onSuccess;
            _failureCapture = onFailure;
            return this;
        }

        /// <summary>
        ///  Add a advanced capture block to capture success and failures
        /// </summary>
        /// <param name="extendedCaptures"></param>
        /// <returns></returns>
        public JetRequest<T> WithCaptures(params (HttpStatusCode, Action)[] extendedCaptures)
        {
            _extendedCaptures = extendedCaptures.ToList();
            return this;
        }

        /// <summary>
        /// Adds a set of cookies to the ongoing request
        /// </summary>
        /// <param name="requestCookies"></param>
        /// <returns></returns>
        public JetRequest<T> WithCookies(params Param[] cookies)
        {
            var cookieHeader = string.Join("; ", cookies.Select(c => $"{c.Key}={(string)c.Value}"));
            _apiRequest.Headers.Add("Cookie", cookieHeader);
            return this;
        }

        /// <summary>
        /// Sets the content type of application
        /// </summary>
        /// <param name="responseContentType"></param>
        /// <returns></returns>
        public JetRequest<T> FetchAs(ContentType responseContentType = ContentType.Json)
        {
            _contentType = responseContentType;
            return this;
        }

        /// <summary>
        /// Add a handler to capture exceptions
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public JetRequest<T> HandleExceptions(Action<Exception> exception)
        {
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
        public async Task<JetResponse<T>> ExecuteAsync(string url)
        {
            try
            {
                _interceptor?.OnInit();
                var uriBuilder = new UriBuilder(url);
                uriBuilder.Query = BuildQueryString(_apiRequest.QueryParameters);

                var request = new HttpRequestMessage
                {
                    Method = _apiRequest.HttpMethod,
                    RequestUri = uriBuilder.Uri,
                    Content = _apiRequest.HttpContent,
                };

                foreach (var (key, value) in _apiRequest.Headers)
                {
                    request.Headers.Add(key, value);
                }

                //Interceptors
                _interceptor?.OnRequesting(_apiRequest);
                var response = await _apiRequest.HttpClient.SendAsync(request);
                _interceptor?.OnResponseReceived();

                //Extended captures
                if (_extendedCaptures != null)
                {
                    foreach (var capture in _extendedCaptures)
                    {
                        if (capture.Item1 == response.StatusCode)
                        {
                            capture.Item2();
                            break;
                        }
                    }
                }

                //Basic captures
                using var responseStream = await response.Content.ReadAsStreamAsync();
                if (!response.IsSuccessStatusCode)
                {
                    var errorResponse = new JetResponse<T>
                    {
                        Status = response.StatusCode,
                        StatusCode = (int)response.StatusCode,
                        Message = "Request Failed",
                        IsSuccess = false,
                        Response = new ResponseData<T>
                        {
                            Binary = await ReadStreamAsByteArray(responseStream),
                            Content = await response.Content.ReadAsStringAsync(),
                            ResponseCookies = new Dictionary<string, string>(),
                            ResponseHeaders = new Dictionary<string, string>(),
                        }
                    };
                    _failureCapture?.Invoke(errorResponse);
                    return errorResponse;
                }

                var jetResponse = new JetResponse<T>
                {
                    Status = response.StatusCode,
                    StatusCode = (int)response.StatusCode,
                    IsSuccess = response.IsSuccessStatusCode,
                    Message = "Request Successfull",
                    Response = new ResponseData<T>
                    {
                        Content = await response.Content.ReadAsStringAsync(),
                        ResponseHeaders = response.Headers.ToDictionary(h => h.Key, h => string.Join(",", h.Value)),
                        Binary = await ReadStreamAsByteArray(responseStream)
                    },
                };
                _successCapture?.Invoke(jetResponse);

                //Response headers
                var isResponseCookieExists = response.Headers.Any(x => x.Key == "Set-Cookie");
                if (isResponseCookieExists)
                {
                    var responseCookie = new Dictionary<string, string>();
                    foreach (var header in response.Headers.GetValues("Set-Cookie"))
                    {
                        foreach (var cookie in header.Split(';'))
                        {
                            var cookieParts = cookie.Trim().Split('=');
                            if (cookieParts.Length == 2)
                            {
                                var key = cookieParts[0];
                                var value = cookieParts[1];
                                if (responseCookie.ContainsKey(key))
                                {
                                    // Update existing key
                                    responseCookie[key] = value;
                                }
                                else
                                {
                                    // Add new key-value pair
                                    responseCookie.Add(key, value);
                                }
                            }
                        }
                    }
                    jetResponse.Response.ResponseCookies = responseCookie;
                }

                //Response data
                if (jetResponse.IsSuccess)
                {
                    switch (_contentType)
                    {
                        case ContentType.Json:
                            if (jetResponse.Response.Content != null)
                            {
                                jetResponse.Response.Data = JsonSerializer.Deserialize<T>(jetResponse.Response.Content);
                            }
                            break;
                        case ContentType.XML:
                            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                            jetResponse.Response.Data = (T)xmlSerializer.Deserialize(responseStream);
                            break;
                        default:
                            jetResponse.Response.Data = default;
                            break;
                    }
                }

                return jetResponse;
            }
            catch (Exception ex)
            {
                if (_onException != null)
                {
                    _onException(ex);
                }
                return new JetResponse<T>
                {
                    IsSuccess = false,
                    Message = "An error cooured within JetAPI library",
                    Response = new ResponseData<T>
                    {
                        Content = string.Empty,
                        Data = default,
                        ResponseCookies = new Dictionary<string, string>(),
                        ResponseHeaders = new Dictionary<string, string>(),
                    }
                };
            }
        }

        /// <summary>
        /// Converts a stream to byte array
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private async Task<byte[]> ReadStreamAsByteArray(Stream stream)
        {
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            return ms.ToArray();
        }

        /// <summary>
        /// Local function to build query string safely
        /// </summary>
        /// <param name="queries"></param>
        /// <returns></returns>
        private static string BuildQueryString(Dictionary<string, string> queries)
        {
            var queryString = new StringBuilder();

            foreach (var (key, value) in queries)
            {
                queryString.Append(Uri.EscapeDataString(key));
                queryString.Append("=");
                queryString.Append(Uri.EscapeDataString(value));
                queryString.Append("&");
            }

            if (queryString.Length > 0)
            {
                queryString.Length--;
            }

            return queryString.ToString();
        }
    }
}