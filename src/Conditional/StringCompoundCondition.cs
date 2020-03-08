using System;
using System.Collections.Generic;
using EnumsNET;
using Newtonsoft.Json;

namespace Conditional
{
    public sealed class StringCompoundCondition : ValueCondition<string>
    {
        private ValueProvider<StringOperator> _operator;
        private ValueProvider<CollectionOperator> _compoundOperator;
        private ValueProvider<IEnumerable<string?>> _values;

        public ValueProvider<StringOperator> Operator
        {
            get => _operator;
            set => _operator = value ?? throw new ArgumentNullException(nameof(value));
        }

        public ValueProvider<CollectionOperator> CompoundOperator
        {
            get => _compoundOperator;
            set => _compoundOperator = value ?? throw new ArgumentNullException(nameof(value));
        }

        public ValueProvider<IEnumerable<string?>> Values
        {
            get => _values;
            set => _values = value ?? throw new ArgumentNullException(nameof(value));
        }

        public StringCompoundCondition(StringOperator @operator, CollectionOperator compoundOperator, params string?[] values)
            : this(@operator, compoundOperator, (IEnumerable<string?>)values)
        {
        }

        public StringCompoundCondition(StringOperator @operator, CollectionOperator compoundOperator, IEnumerable<string?> values)
            : this(@operator, compoundOperator, (ValueProvider<IEnumerable<string?>>)(values ?? throw new ArgumentNullException(nameof(values))))
        {
        }

        public StringCompoundCondition(StringOperator @operator, CollectionOperator compoundOperator, ValueProvider<IEnumerable<string?>> values)
            : this((ValueProvider<StringOperator>)@operator.Validate(nameof(@operator)), compoundOperator.Validate(nameof(compoundOperator)), values)
        {
        }

        [JsonConstructor]
        public StringCompoundCondition(ValueProvider<StringOperator> @operator, ValueProvider<CollectionOperator> compoundOperator, ValueProvider<IEnumerable<string?>> values)
        {
            CompoundOperator = compoundOperator;
            Operator = @operator;
            Values = values;
        }

        public override bool Evaluate(string value, object? context) => _compoundOperator.GetValue(context).Evaluate(_values.GetValue(context), v => _operator.GetValue(context).Evaluate(value, v));
    }
}