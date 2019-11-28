using System;
using EnumsNET;
using Newtonsoft.Json;

namespace Conditional
{
    public static class NumeriCondition
    {
        public static NumericCondition<T> Create<T>(NumericOperator @operator, T value) => new NumericCondition<T>(@operator, value);
    }

    public sealed class NumericCondition<T> : ValueCondition<T>
    {
        private NumericOperator _operator;
        private ValueProvider<T> _value;

        public NumericOperator Operator
        {
            get => _operator;
            set => _operator = value.Validate(nameof(value));
        }

        public ValueProvider<T> Value
        {
            get => _value;
            set => _value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public NumericCondition(NumericOperator @operator, T value)
            : this(@operator, (ValueProvider<T>)value)
        {
        }

        [JsonConstructor]
        public NumericCondition(NumericOperator @operator, ValueProvider<T> value)
        {
            Operator = @operator;
            Value = value;
        }

        public override bool Evaluate(T value, object? context) => _operator.Evaluate(value, Value.GetValue(context));

        public new NumericCondition<T> Clone() => new NumericCondition<T>(_operator, Value.CloneInternal());

        protected override ValueCondition<T> CloneInternal() => Clone();

        protected override ValueCondition<T> GetInvertedCondition() => new NumericCondition<T>(_operator.Invert(), Value.CloneInternal());
    }
}