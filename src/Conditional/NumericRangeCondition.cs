using System;
using EnumsNET;
using Genumerics;
using Newtonsoft.Json;

namespace Conditional
{
    public static class NumericRangeCondition
    {
        public static NumericRangeCondition<T> Create<T>(T min, T max, NumericRangeOptions options = NumericRangeOptions.Inclusive) => new NumericRangeCondition<T>(min, max, options);

        public static NumericRangeCondition<T> Create<T>(ValueProvider<T> min, ValueProvider<T> max, NumericRangeOptions options = NumericRangeOptions.Inclusive) => new NumericRangeCondition<T>(min, max, options);
    }

    public sealed class NumericRangeCondition<T> : ValueCondition<T>
    {
        private ValueProvider<T> _min;
        private ValueProvider<T> _max;
        private ValueProvider<NumericRangeOptions> _options;

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

        public ValueProvider<NumericRangeOptions> Options
        {
            get => _options;
            set => _options = value ?? throw new ArgumentNullException(nameof(value));
        }

        public NumericRangeCondition(T min, T max, NumericRangeOptions options = NumericRangeOptions.Inclusive)
            : this((ValueProvider<T>)min, max, options)
        {
        }

        public NumericRangeCondition(ValueProvider<T> min, ValueProvider<T> max, NumericRangeOptions options = NumericRangeOptions.Inclusive)
            : this(min, max, (ValueProvider<NumericRangeOptions>)options.Validate(nameof(options)))
        {
        }

        [JsonConstructor]
        public NumericRangeCondition(ValueProvider<T> min, ValueProvider<T> max, ValueProvider<NumericRangeOptions> options)
        {
            Min = min;
            Max = max;
            Options = options;
        }

        public override bool Evaluate(T value, object? context) => (Number.LessThan(value, _min.GetValue(context)) || Number.GreaterThan(value, _max.GetValue(context))) ^ (_options.GetValue(context) == NumericRangeOptions.Inclusive);
    }

    public enum NumericRangeOptions
    {
        Inclusive = 0,
        Exclusive = 1
    }
}