using System.Collections.Generic;
using System.Net;

namespace Twileloop.JetAPI.Types {
    public class JetResponse<T> {
        public int StatusCode { get; set; }
        public HttpStatusCode Status { get; set; }
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
        public ResponseData<T> Response { get; set; } = new();

        public override string ToString()
        {
            return $"{StatusCode} - {Status} | {Message}";
        }
    }

    public class ResponseData<T>
    {
        public string Content { get; set; }
        public T Data { get; set; }
        public byte[] Binary { get; set; }
        public Dictionary<string, string> ResponseHeaders { get; set; }
        public Dictionary<string, string> ResponseCookies { get; set; }
    }
}
