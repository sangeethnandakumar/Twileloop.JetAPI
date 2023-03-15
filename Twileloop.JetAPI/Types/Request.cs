using System.Collections.Generic;
using System.Net.Http;

namespace Twileloop.JetAPI.Types {
    public class Request {
        public HttpClient HttpClient { get; set; }
        public HttpMethod HttpMethod { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public Dictionary<string, string> QueryParameters { get; set; }
        public HttpContent HttpContent { get; set; }

        public Request(HttpMethod method) {
            HttpClient = new HttpClient();
            HttpMethod = method;
            Headers = new Dictionary<string, string>();
            QueryParameters = new Dictionary<string, string>();
        }

        public Request() {
            HttpClient = new HttpClient();
            HttpMethod = HttpMethod.Get;
            Headers = new Dictionary<string, string>();
            QueryParameters = new Dictionary<string, string>();
        }
    }
}
