namespace Twileloop.JetAPI.Types {
    public abstract class Interceptor {
        public virtual void OnInit() {

        }

        public virtual void OnRequesting(APIRequest request) {

        }

        public virtual void OnResponseReceived() {

        }
    }
}
