namespace Twileloop.JetAPI.Authentication {
    /// <summary>
    /// API Key for using in authentication
    /// </summary>
    public class ApiKey {

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiKey"/> class with the specified header name and API key value.
        /// </summary>
        /// <param name="headerName">The name of the header used to send the API key.</param>
        /// <param name="apiKey">The value of the API key.</param>
        public ApiKey(string headerName, string apiKey) {
            HeaderName = headerName;
            APIKey = apiKey;
        }

        /// <summary>
        /// Gets the default header name used to send the API key.
        /// </summary>
        public string HeaderName { get; } = "Api-Key";

        /// <summary>
        /// Gets the value of the API key.
        /// </summary>
        public string APIKey { get; }
    }

}
}
