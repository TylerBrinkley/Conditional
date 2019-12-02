using EnumsNET;
using Newtonsoft.Json;

namespace Conditional
{
    public sealed class StringCondition : ValueCondition<string>
    {
        private StringOperator _operator;

        [JsonRequired]
        public StringOperator Operator
        {
            get => _operator;
            set => _operator = value.Validate(nameof(value));
        }

        public ValueProvider<string>? Value { get; set; }

        public StringCondition(StringOperator @operator, string? value)
            : this(@operator, ValueProvider.Create(value))
        {
        }

        [JsonConstructor]
        public StringCondition(StringOperator @operator, ValueProvider<string>? value)
        {
            Operator = @operator;
            Value = value;
        }

        public override bool Evaluate(string? value, object? context) => _operator.Evaluate(value, Value?.GetValue(context));

        public new StringCondition Clone() => new StringCondition(_operator, Value?.CloneInternal());

        protected override ValueCondition<string> CloneInternal() => Clone();
    }
}