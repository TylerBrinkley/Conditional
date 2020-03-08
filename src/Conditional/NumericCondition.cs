using System;
using EnumsNET;
using Newtonsoft.Json;

namespace Conditional
{
    public static class NumeriCondition
    {
        public static NumericCondition<T> Create<T>(NumericOperator @operator, T value) => new NumericCondition<T>(@operator, value);

        public static NumericCondition<T> Create<T>(NumericOperator @operator, ValueProvider<T> value) => new NumericCondition<T>(@operator, value);
    }

    public sealed class NumericCondition<T> : ValueCondition<T>
    {
        private ValueProvider<NumericOperator> _operator;
        private ValueProvider<T> _value;

        public ValueProvider<NumericOperator> Operator
        {
            get => _operator;
            set => _operator = value ?? throw new ArgumentNullException(nameof(value));
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

        public NumericCondition(NumericOperator @operator, ValueProvider<T> value)
            : this((ValueProvider<NumericOperator>)@operator.Validate(nameof(@operator)), value)
        {
        }

        [JsonConstructor]
        public NumericCondition(ValueProvider<NumericOperator> @operator, ValueProvider<T> value)
        {
            Operator = @operator;
            Value = value;
        }

        public override bool Evaluate(T value, object? context) => _operator.GetValue(context).Evaluate(value, Value.GetValue(context));
    }
}