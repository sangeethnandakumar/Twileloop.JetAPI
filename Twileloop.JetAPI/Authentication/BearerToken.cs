namespace Twileloop.JetAPI.Authentication {
    public class BearerToken {
        public BearerToken(string token) {
            Token = token;
        }

        public string Token { get; }
    }
}
