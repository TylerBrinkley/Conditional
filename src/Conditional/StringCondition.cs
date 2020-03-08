using EnumsNET;
using Newtonsoft.Json;
using System;

namespace Conditional
{
    public sealed class StringCondition : ValueCondition<string?>
    {
        private ValueProvider<StringOperator> _operator;
        private ValueProvider<string?> _value;

        public ValueProvider<StringOperator> Operator
        {
            get => _operator;
            set => _operator = value ?? throw new ArgumentNullException(nameof(value));
        }

        public ValueProvider<string?> Value
        {
            get => _value;
            set => _value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public StringCondition(StringOperator @operator, string? value)
            : this(@operator, (ValueProvider<string?>)value)
        {
        }

        public StringCondition(StringOperator @operator, ValueProvider<string?> value)
            : this((ValueProvider<StringOperator>)@operator.Validate(nameof(value)), value)
        {
        }

        [JsonConstructor]
        public StringCondition(ValueProvider<StringOperator> @operator, ValueProvider<string?> value)
        {
            Operator = @operator;
            Value = value;
        }

        public override bool Evaluate(string? value, object? context) => _operator.GetValue(context).Evaluate(value, _value.GetValue(context));
    }
}