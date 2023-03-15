namespace Twileloop.JetAPI.Authentication {
    /// <summary>
    /// Represents a bearer token used for authentication in an API request.
    /// </summary>
    public class BearerToken {
        /// <summary>
        /// Initializes a new instance of the <see cref="BearerToken"/> class with the specified token value.
        /// </summary>
        /// <param name="token">The bearer token string.</param>
        public BearerToken(string token) {
            Token = token;
        }

        /// <summary>
        /// Gets the bearer token string.
        /// </summary>
        public string Token { get; }
    }
}
