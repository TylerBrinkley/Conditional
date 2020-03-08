using System;
using System.Collections.Generic;
using EnumsNET;
using Newtonsoft.Json;

namespace Conditional
{
    public sealed class NumericCompoundCondition<T> : ValueCondition<T>
    {
        private ValueProvider<NumericOperator> _operator;
        private ValueProvider<CollectionOperator> _compoundOperator;
        private ValueProvider<IEnumerable<T>> _values;

        public ValueProvider<NumericOperator> Operator
        {
            get => _operator;
            set => _operator = value ?? throw new ArgumentNullException(nameof(value));
        }

        public ValueProvider<CollectionOperator> CompoundOperator
        {
            get => _compoundOperator;
            set => _compoundOperator = value ?? throw new ArgumentNullException(nameof(value));
        }

        public ValueProvider<IEnumerable<T>> Values
        {
            get => _values;
            set => _values = value ?? throw new ArgumentNullException(nameof(value));
        }

        public NumericCompoundCondition(NumericOperator @operator, CollectionOperator compoundOperator, ValueProvider<IEnumerable<T>> values)
            : this((ValueProvider<NumericOperator>)@operator.Validate(nameof(@operator)), compoundOperator.Validate(nameof(compoundOperator)), values)
        {
        }

        [JsonConstructor]
        public NumericCompoundCondition(ValueProvider<NumericOperator> @operator, ValueProvider<CollectionOperator> compoundOperator, ValueProvider<IEnumerable<T>> values)
        {
            CompoundOperator = compoundOperator;
            Operator = @operator;
            Values = values;
        }

        public override bool Evaluate(T value, object? context) => _compoundOperator.GetValue(context).Evaluate(_values.GetValue(context), v => _operator.GetValue(context).Evaluate(value, v));
    }
}