using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Conditional
{
    public class ValueCondition<T> : Condition<ValueCondition<T>, ValueCondition<T>>, ICloneable
    {
        protected ValueCondition()
        {
        }

        [JsonConstructor]
        private ValueCondition(Joiner? joiner, IReadOnlyList<ValueCondition<T>>? conditions)
            : base(joiner, conditions)
        {
        }

        protected sealed override ValueCondition<T> CreateJoinedCondition(Joiner joiner, IReadOnlyList<ValueCondition<T>> conditions) => new ValueCondition<T>(joiner, conditions);

        protected sealed override bool ShouldSerializeConditions() => true;

        protected sealed override bool ShouldSerializeJoiner() => true;

        public bool Evaluate(T value) => Evaluate(value, null);

        public virtual bool Evaluate(T value, object? context) => Evaluate(c => c.Evaluate(value, context));

        public ValueCondition<T> Clone()
        {
            var conditions = GetConditions();
            if (conditions == null)
            {
                throw new InvalidOperationException("Must override CloneInternal in derived classes");
            }

            var newConditions = new List<ValueCondition<T>>(conditions.Count);
            foreach (var condition in conditions)
            {
                newConditions.Add(condition.CloneInternal());
            }
            return new ValueCondition<T>(GetJoiner(), newConditions.AsReadOnly());
        }

        protected override ValueCondition<T> CloneInternal() => Clone();

        object ICloneable.Clone() => CloneInternal();
    }
}