using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Conditional
{
    public sealed class MultiValueProvider<T> : ValueProvider<IReadOnlyCollection<T>>
    {
        private IReadOnlyCollection<T>? _nullContextCollection;

        [JsonRequired]
        public IReadOnlyCollection<ValueProvider<T>> Values { get; }

        public MultiValueProvider(params ValueProvider<T>[] values)
            : this((IEnumerable<ValueProvider<T>>)values)
        {
        }

        [JsonConstructor]
        public MultiValueProvider(IEnumerable<ValueProvider<T>> values)
        {
            Values = values.ToList().AsReadOnly();
        }

        public override IReadOnlyCollection<T> GetValue(object? context) => context == null ? _nullContextCollection ?? (_nullContextCollection = new Collection(Values, context)) : new Collection(Values, context);

        protected internal override ValueProvider<IReadOnlyCollection<T>> CloneInternal() => Values.All(v => v.CloneInternal() == v) ? this : new MultiValueProvider<T>(Values.Select(v => v.CloneInternal()));

        private sealed class Collection : IReadOnlyCollection<T>
        {
            private readonly IReadOnlyCollection<ValueProvider<T>> _values;
            private readonly object? _context;

            public int Count => _values.Count;

            public Collection(IReadOnlyCollection<ValueProvider<T>> values, object? context)
            {
                _values = values;
                _context = context;
            }

            public IEnumerator<T> GetEnumerator() => new Enumerator(_values.GetEnumerator(), _context);

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            private sealed class Enumerator : IEnumerator<T>
            {
                private readonly IEnumerator<ValueProvider<T>> _enumerator;
                private readonly object? _context;

                public T Current => _enumerator.Current.GetValue(_context);

                object? IEnumerator.Current => Current;

                public Enumerator(IEnumerator<ValueProvider<T>> enumerator, object? context)
                {
                    _enumerator = enumerator;
                    _context = context;
                }

                public void Dispose() => _enumerator.Dispose();

                public bool MoveNext() => _enumerator.MoveNext();

                public void Reset() => _enumerator.Reset();
            }
        }
    }
}