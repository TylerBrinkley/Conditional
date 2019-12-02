using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Conditional
{
    public sealed class MultiValueProvider<T> : ValueProvider<IEnumerable<T>>
    {
        private Collection? _nullContextCollection;

        [JsonRequired]
        public IReadOnlyCollection<ValueProvider<T>> Values { get; }

        public MultiValueProvider(params ValueProvider<T>[] values)
            : this((IEnumerable<ValueProvider<T>>)values)
        {
        }

        [JsonConstructor]
        public MultiValueProvider(IEnumerable<ValueProvider<T>> values)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            Values = values.ToList().AsReadOnly();
        }

        public override IEnumerable<T> GetValue(object? context) => context == null ? _nullContextCollection ??= new Collection(Values, context) : new Collection(Values, context);

        protected internal override ValueProvider<IEnumerable<T>> CloneInternal() => Values.All(v => ReferenceEquals(v.CloneInternal(), v)) ? this : new MultiValueProvider<T>(Values.Select(v => v.CloneInternal()));

        private sealed class Collection : IEnumerable<T>
        {
            private readonly IReadOnlyCollection<ValueProvider<T>> _values;
            private readonly object? _context;

            public Collection(IReadOnlyCollection<ValueProvider<T>> values, object? context)
            {
                _values = values;
                _context = context;
            }

            public IEnumerator<T> GetEnumerator()
            {
                foreach (var value in _values)
                {
                    yield return value.GetValue(_context);
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}