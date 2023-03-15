namespace Twileloop.JetAPI.Authentication {
    public class ApiKey {
        public ApiKey(string headerName, string aPIKey) {
            HeaderName = headerName;
            APIKey = aPIKey;
        }

        public string HeaderName { get; } = "Api-Key";
        public string APIKey { get; }
    }
}
