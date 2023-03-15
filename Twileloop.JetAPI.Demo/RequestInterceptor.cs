using Twileloop.JetAPI.Types;

namespace Twileloop.JetAPI.Demo {
    public class CustomInterceptor : Interceptor {

        public override void OnInit() {
            Console.WriteLine("Started...");
            base.OnInit();
        }

        public override void OnRequesting(Request request) {
            Console.WriteLine("Let's modify request from interceptor");
            request.HttpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            Console.WriteLine("Enough. Now start requesting...");
            base.OnRequesting(request);
        }

        public override void OnResponseReceived() {
            Console.WriteLine("Got response...");
            base.OnResponseReceived();
        }

    }
}
