using Newtonsoft.Json;

namespace Conditional
{
    public static class ValueProvider
    {
        public static ValueProvider<T>? Create<T>(T? nullableValue)
            where T : struct => nullableValue.HasValue ? new ValueProvider<T>(nullableValue.GetValueOrDefault()) : null;

        public static ValueProvider<T>? Create<T>(T? nullableValue)
            where T : class => nullableValue != null ? new ValueProvider<T>(nullableValue) : null;
    }

    public class ValueProvider<T>
    {
        public static implicit operator ValueProvider<T>(T value) => new ValueProvider<T>(value);

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, TypeNameHandling = TypeNameHandling.None)]
        public Value<T>? Value { get; }

        public ValueProvider(T value)
        {
            Value = value;
        }

        protected ValueProvider()
        {
        }

        public T GetValue() => GetValue(null);

        public virtual T GetValue(object? context) => Value!.Value;

        internal protected virtual ValueProvider<T> CloneInternal() => this;
    }
}