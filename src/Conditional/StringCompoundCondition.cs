using System;
using System.Collections.Generic;
using System.Linq;
using EnumsNET;
using Newtonsoft.Json;

namespace Conditional
{
    public sealed class StringCompoundCondition : ValueCondition<string>
    {
        private StringOperator _operator;
        private CollectionOperator _compoundOperator;
        private ValueProvider<IReadOnlyCollection<string>> _values;

        [JsonRequired]
        public StringOperator Operator
        {
            get => _operator;
            set => _operator = value.Validate(nameof(value));
        }

        [JsonRequired]
        public CollectionOperator CompoundOperator
        {
            get => _compoundOperator;
            set => _compoundOperator = value.Validate(nameof(value));
        }

        [JsonRequired]
        public ValueProvider<IReadOnlyCollection<string>> Values
        {
            get => _values;
            set => _values = value ?? throw new ArgumentNullException(nameof(value));
        }

        public StringCompoundCondition(StringOperator @operator, CollectionOperator compoundOperator, params string[] values)
            : this(@operator, compoundOperator, (IEnumerable<string>)values)
        {
        }

        public StringCompoundCondition(StringOperator @operator, CollectionOperator compoundOperator, IEnumerable<string> values)
            : this(@operator, compoundOperator, new ValueProvider<IReadOnlyCollection<string>>(values.ToList().AsReadOnly()))
        {
        }

        [JsonConstructor]
        public StringCompoundCondition(StringOperator @operator, CollectionOperator compoundOperator, ValueProvider<IReadOnlyCollection<string>> values)
        {
            CompoundOperator = compoundOperator;
            Operator = @operator;
            Values = values;
        }

        public override bool Evaluate(string value, object? context) => _compoundOperator.Evaluate(_values.GetValue(context), v => _operator.Evaluate(value, v));

        public new StringCompoundCondition Clone() => new StringCompoundCondition(_operator, _compoundOperator, _values.CloneInternal());

        protected override ValueCondition<string> CloneInternal() => Clone();

        protected override ValueCondition<string> GetInvertedCondition() => new StringCompoundCondition(_operator, _compoundOperator.Invert(), _values.CloneInternal());
    }
}