using Newtonsoft.Json;

namespace Conditional
{
    public class ValueProvider<T>
    {
        public static implicit operator ValueProvider<T>(T value) => new ValueProvider<T>(value);

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, TypeNameHandling = TypeNameHandling.None)]
        public Value<T>? Value { get; }

        [JsonConstructor]
        public ValueProvider(T value)
        {
            Value = value;
        }

        protected ValueProvider()
        {
        }

        public T GetValue() => GetValue(null);

        public virtual T GetValue(object? context) => Value!.Value;
    }
}