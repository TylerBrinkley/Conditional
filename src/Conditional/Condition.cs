using System;
using System.Collections.Generic;
using System.Diagnostics;
using EnumsNET;
using Newtonsoft.Json;

namespace Conditional
{
    public abstract class Condition<TCompoundCondition, TCondition>
        where TCompoundCondition : Condition<TCompoundCondition, TCondition>
        where TCondition : TCompoundCondition
    {
        private readonly Joiner? _joiner;
        private readonly IReadOnlyList<TCompoundCondition>? _conditions;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
#pragma warning disable IDE0051 // Remove unused private members, used only for serialization
        private Joiner? Joiner => ShouldSerializeJoiner() ? _joiner : null;
#pragma warning restore IDE0051 // Remove unused private members

        protected virtual bool ShouldSerializeJoiner() => true;

        protected Joiner? GetJoiner() => _joiner;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, TypeNameHandling = TypeNameHandling.None)]
#pragma warning disable IDE0051 // Remove unused private members, used only for serialization
        private IReadOnlyList<TCompoundCondition>? Conditions => ShouldSerializeConditions() ? _conditions : null;
#pragma warning restore IDE0051 // Remove unused private members

        protected virtual bool ShouldSerializeConditions() => true;

        protected IReadOnlyList<TCompoundCondition>? GetConditions() => _conditions;

        protected Condition()
        {
            if (GetType() == typeof(TCompoundCondition))
            {
                throw new InvalidOperationException($"cannot use this constructor for {typeof(TCompoundCondition)}");
            }
        }

        protected Condition(Joiner? joiner, IReadOnlyList<TCompoundCondition>? conditions)
        {
            if (GetType() == typeof(TCompoundCondition))
            {
                if (!joiner.HasValue)
                {
                    throw new ArgumentNullException(nameof(joiner));
                }
                joiner.GetValueOrDefault().Validate(nameof(joiner));
                if (conditions == null)
                {
                    throw new ArgumentNullException(nameof(conditions));
                }

                _joiner = joiner;
                _conditions = conditions;
            }
            else if (joiner.HasValue)
            {
                throw new ArgumentException("must be null", nameof(joiner));
            }
            else if (conditions != null)
            {
                throw new ArgumentException("must be null", nameof(conditions));
            }
        }

        public TCompoundCondition And(TCompoundCondition condition) => Join(Conditional.Joiner.And, condition);

        public TCompoundCondition Or(TCompoundCondition condition) => Join(Conditional.Joiner.Or, condition);

        private TCompoundCondition Join(Joiner joiner, TCompoundCondition condition)
        {
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
            }

            var conditions = new List<TCompoundCondition>();
            if (_joiner == joiner)
            {
                foreach (var c in _conditions!)
                {
                    conditions.Add(c.CloneInternal());
                }
            }
            else
            {
                conditions.Add(CloneInternal());
            }
            if (condition._joiner == joiner)
            {
                foreach (var c in condition._conditions!)
                {
                    conditions.Add(c.CloneInternal());
                }
            }
            else
            {
                conditions.Add(condition.CloneInternal());
            }
            var readOnlyConditions = conditions.AsReadOnly();
            var joinedCondition = CreateJoinedCondition(joiner, readOnlyConditions);
            if (joinedCondition == null || joinedCondition._joiner != joiner || joinedCondition._conditions != readOnlyConditions)
            {
                throw new InvalidOperationException("CreateJoinedCondition is not implemented properly");
            }
            Debug.Assert(joinedCondition.GetType() == typeof(TCompoundCondition));
            return joinedCondition;
        }

        protected abstract TCompoundCondition CreateJoinedCondition(Joiner joiner, IReadOnlyList<TCompoundCondition> conditions);

        protected bool Evaluate(Func<TCondition, bool> evaluator, bool shortCircuit = true)
        {
            if (evaluator == null)
            {
                throw new ArgumentNullException(nameof(evaluator));
            }

            return EvaluateInternal(evaluator, (a, b) => a & b, (a, b) => a | b, shortCircuit ? (joiner, result) => (joiner == Conditional.Joiner.And) ^ result : (Func<Joiner, bool, bool>?)null);
        }

        protected TResult Evaluate<TResult>(Func<TCondition, TResult> evaluator, Func<TResult, TResult, TResult> andExpression, Func<TResult, TResult, TResult> orExpression, Func<Joiner, TResult, bool>? shortCircuitEvaluator = null)
        {
            if (evaluator == null)
            {
                throw new ArgumentNullException(nameof(evaluator));
            }
            if (andExpression == null)
            {
                throw new ArgumentNullException(nameof(andExpression));
            }
            if (orExpression == null)
            {
                throw new ArgumentNullException(nameof(orExpression));
            }

            return EvaluateInternal(evaluator, andExpression, orExpression, shortCircuitEvaluator);
        }

        private TResult EvaluateInternal<TResult>(Func<TCondition, TResult> evaluator, Func<TResult, TResult, TResult> andExpression, Func<TResult, TResult, TResult> orExpression, Func<Joiner, TResult, bool>? shortCircuitEvaluator)
        {
            if (_conditions == null)
            {
                return evaluator((TCondition)this);
            }

            TResult result = default!;
            var first = true;
            var joiner = _joiner.GetValueOrDefault();
            foreach (var condition in _conditions)
            {
                var evaluationResult = condition.EvaluateInternal(evaluator, andExpression, orExpression, shortCircuitEvaluator);
                if (first)
                {
                    result = evaluationResult;
                    first = false;
                }
                else
                {
                    result = joiner == Conditional.Joiner.And ? andExpression(result, evaluationResult) : orExpression(result, evaluationResult);
                }
                if (shortCircuitEvaluator?.Invoke(joiner, result) == true)
                {
                    break;
                }
            }
            return result;
        }

        protected virtual TCompoundCondition CloneInternal() => (TCompoundCondition)this;
    }
}