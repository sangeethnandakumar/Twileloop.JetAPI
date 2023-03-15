namespace Twileloop.JetAPI.Types {
    public abstract class Interceptor {
        public virtual void OnInit() {

        }

        public virtual void OnRequesting(Request request) {

        }

        public virtual void OnResponseReceived() {

        }
    }
}
