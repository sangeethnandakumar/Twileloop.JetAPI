using System.Text.Json;

namespace Twileloop.JetAPI.Types {

    public struct Param {
        public Param(string key, object value) : this() {
            Key = key;
            Value = value;
        }

        public string Key { get; set; }
        public object Value { get; set; }
        public string ValueString { get => JsonSerializer.Serialize(Value); }
    }
}
