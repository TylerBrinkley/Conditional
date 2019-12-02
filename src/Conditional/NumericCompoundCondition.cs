using System;
using System.Collections.Generic;
using EnumsNET;
using Newtonsoft.Json;

namespace Conditional
{
    public sealed class NumericCompoundCondition<T> : ValueCondition<T>
    {
        private NumericOperator _operator;
        private CollectionOperator _compoundOperator;
        private ValueProvider<IEnumerable<T>> _values;

        [JsonRequired]
        public NumericOperator Operator
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
        public ValueProvider<IEnumerable<T>> Values
        {
            get => _values;
            set => _values = value ?? throw new ArgumentNullException(nameof(value));
        }

        public NumericCompoundCondition(NumericOperator @operator, CollectionOperator compoundOperator, ValueProvider<IEnumerable<T>> values)
        {
            CompoundOperator = compoundOperator;
            Operator = @operator;
            Values = values;
        }

        public override bool Evaluate(T value, object? context) => _compoundOperator.Evaluate(_values.GetValue(context), v => _operator.Evaluate(value, v));

        public new NumericCompoundCondition<T> Clone() => new NumericCompoundCondition<T>(_operator, _compoundOperator, _values.CloneInternal());

        protected override ValueCondition<T> CloneInternal() => Clone();
    }
}