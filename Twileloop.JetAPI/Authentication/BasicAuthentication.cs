namespace Twileloop.JetAPI.Authentication {
    
    /// <summary>
    /// Class representing basic authentication credentials that include a username, password, and flag to indicate whether
    /// the credentials should be encoded as base64.
    /// </summary>
    public class BasicAuthentication {
        /// <summary>
        /// The username to use for authentication.
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// The password to use for authentication.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Indicates whether the credentials should be encoded as base64.
        /// </summary>
        public bool EncodeAsBase64 { get; set; }
    }
}
