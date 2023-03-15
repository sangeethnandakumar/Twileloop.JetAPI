using System.Collections.Generic;
using System.Net;

namespace Twileloop.JetAPI.Types {
    public class JetResponse<T> {
        public HttpStatusCode StatusCode { get; set; }
        public string Content { get; set; }
        public T Data { get; set; }
        public byte[] Binary { get; set; }
        public bool IsSuccessfully { get; set; }
        public Dictionary<string, string> ResponseHeaders { get; set; }
        public Dictionary<string, string> ResponseCookies { get; set; }
    }
}
