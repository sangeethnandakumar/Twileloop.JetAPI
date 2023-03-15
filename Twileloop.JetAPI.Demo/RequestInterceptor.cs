using Twileloop.JetAPI.Types;

namespace Twileloop.JetAPI.Demo {
    public class CustomInterceptor : Interceptor {

        public override void OnInit() {
            base.OnInit();
        }

        public override void OnRequesting(Request request) {
            base.OnRequesting(request);
        }

        public override void OnResponseReceived() {
            base.OnResponseReceived();
        }

    }
}
