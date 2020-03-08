using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Conditional
{
    public static class MultiValueProvider
    {
        public static MultiValueProvider<T> Create<T>(params ValueProvider<T>[] values) => new MultiValueProvider<T>(values);

        public static MultiValueProvider<T> Create<T>(IEnumerable<ValueProvider<T>> values) => new MultiValueProvider<T>(values);
    }

    public sealed class MultiValueProvider<T> : ValueProvider<IEnumerable<T>>
    {
        [JsonProperty(TypeNameHandling = TypeNameHandling.None)]
        public IReadOnlyList<ValueProvider<T>> Values { get; }

        public MultiValueProvider(params ValueProvider<T>[] values)
            : this((IEnumerable<ValueProvider<T>>)values)
        {
        }

        [JsonConstructor]
        public MultiValueProvider(IEnumerable<ValueProvider<T>> values)
        {
            Values = (values ?? throw new ArgumentNullException(nameof(values))).ToList().AsReadOnly();
        }

        public override IEnumerable<T> GetValue(object? context)
        {
            foreach (var value in Values)
            {
                yield return value.GetValue(context);
            }
        }
    }
}