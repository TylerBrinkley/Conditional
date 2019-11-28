using System;
using System.Collections.Concurrent;
using Newtonsoft.Json;

namespace Conditional
{
    [JsonConverter(typeof(ValueConverter))]
    public struct Value<T>
    {
        public static implicit operator T(Value<T> value) => value._value;

        public static implicit operator Value<T>(T value) => new Value<T>(value);

        private readonly T _value;

        public Value(T value)
        {
            _value = value;
        }

        public override int GetHashCode() => _value?.GetHashCode() ?? 0;

        public override bool Equals(object? obj) => _value?.Equals(obj) == true;

        public override string? ToString() => _value?.ToString();
    }

    internal sealed class ValueConverter : JsonConverter
    {
        private readonly ConcurrentDictionary<Type, ValueConverterInternal> _converters = new ConcurrentDictionary<Type, ValueConverterInternal>();

        private ValueConverterInternal? GetConverter(Type objectType)
        {
            if (!_converters.TryGetValue(objectType, out var converter) && objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(Value<>))
            {
                converter = _converters.GetOrAdd(objectType, (ValueConverterInternal)Activator.CreateInstance(typeof(ValueConverterInternal<>).MakeGenericType(objectType.GenericTypeArguments))!);
            }
            return converter;
        }

        public override bool CanConvert(Type objectType) => GetConverter(objectType) != null;

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            var converter = GetConverter(objectType);
            if (converter == null)
            {
                throw new ArgumentException("must be of type Value<T>", nameof(objectType));
            }
            return converter.ReadJson(reader, existingValue, serializer);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var converter = GetConverter(value.GetType());
            if (converter == null)
            {
                throw new ArgumentException("must be of type Value<T>", nameof(value));
            }
            converter.WriteJson(writer, value, serializer);
        }

        private abstract class ValueConverterInternal
        {
            public abstract object ReadJson(JsonReader reader, object? existingValue, JsonSerializer serializer);

            public abstract void WriteJson(JsonWriter writer, object value, JsonSerializer serializer);
        }

        private sealed class ValueConverterInternal<T> : ValueConverterInternal
        {
            public override object ReadJson(JsonReader reader, object? existingValue, JsonSerializer serializer)
            {
                if (existingValue != null)
                {
                    serializer.Populate(reader, existingValue);
                    return existingValue;
                }
                return new Value<T>(serializer.Deserialize<T>(reader));
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => serializer.Serialize(writer, (Value<T>)value);
        }
    }
}