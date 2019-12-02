using System;
using Genumerics;
using Newtonsoft.Json;

namespace Conditional
{
    public sealed class NumericRangeCondition<T> : ValueCondition<T>
    {
        private ValueProvider<T> _min;
        private ValueProvider<T> _max;

        public ValueProvider<T> Min
        {
            get => _min;
            set => _min = value ?? throw new ArgumentNullException(nameof(value));
        }

        public ValueProvider<T> Max
        {
            get => _max;
            set => _max = value ?? throw new ArgumentNullException(nameof(value));
        }

        public bool Inclusive { get; set; }

        public NumericRangeCondition(T min, T max, bool inclusive = true)
            : this((ValueProvider<T>)min, max, inclusive)
        {
        }

        [JsonConstructor]
        public NumericRangeCondition(ValueProvider<T> min, ValueProvider<T> max, bool inclusive = true)
        {
            Min = min;
            Max = max;
            Inclusive = inclusive;
        }

        public override bool Evaluate(T value, object? context) => (Number.LessThan(value, _min.GetValue(context)) || Number.GreaterThan(value, _max.GetValue(context))) ^ Inclusive;

        public new NumericRangeCondition<T> Clone() => new NumericRangeCondition<T>(_min.CloneInternal(), _max.CloneInternal(), Inclusive);

        protected override ValueCondition<T> CloneInternal() => Clone();
    }
}