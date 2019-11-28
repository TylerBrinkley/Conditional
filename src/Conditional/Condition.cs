using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Conditional
{
    public abstract class Condition<TConditionBase, TCondition>
        where TConditionBase : Condition<TConditionBase, TCondition>
        where TCondition : TConditionBase
    {
        private Joiner? _joiner;
        private IReadOnlyList<TConditionBase>? _conditions;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private Joiner? Joiner
        {
            get => ShouldSerializeJoiner() ? _joiner : null;
            set => _joiner = value;
        }

        protected virtual bool ShouldSerializeJoiner() => true;

        protected Joiner? GetJoiner() => _joiner;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, TypeNameHandling = TypeNameHandling.None)]
        private IReadOnlyList<TConditionBase>? Conditions
        {
            get => ShouldSerializeConditions() ? _conditions : null;
            set => _conditions = value;
        }

        protected virtual bool ShouldSerializeConditions() => true;

        protected IReadOnlyList<TConditionBase>? GetConditions() => _conditions;

        protected Condition()
        {
        }

        protected Condition(Joiner? joiner, IReadOnlyList<TConditionBase> conditions)
        {
            if (joiner.HasValue)
            {
                if (joiner < Conditional.Joiner.And || joiner > Conditional.Joiner.Or)
                {
                    throw new ArgumentException("must be 'And' or 'Or'", nameof(joiner));
                }
                if (conditions == null)
                {
                    throw new ArgumentNullException(nameof(conditions));
                }

                Joiner = joiner;
                Conditions = conditions;
            }
            if (conditions != null)
            {
                throw new ArgumentException("joiner and conditions must both be null or not null");
            }
        }

        protected TConditionBase And(TConditionBase condition) => Join(Conditional.Joiner.And, condition);

        protected TConditionBase Or(TConditionBase condition) => Join(Conditional.Joiner.Or, condition);

        private TConditionBase Join(Joiner joiner, TConditionBase condition)
        {
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
            }

            var conditions = new List<TConditionBase>();
            if (Joiner == joiner)
            {
                foreach (var c in Conditions!)
                {
                    conditions.Add(c.CloneInternal());
                }
            }
            else
            {
                conditions.Add(CloneInternal());
            }
            if (condition.Joiner == joiner)
            {
                foreach (var c in condition.Conditions!)
                {
                    conditions.Add(c.CloneInternal());
                }
            }
            else
            {
                conditions.Add(condition.CloneInternal());
            }
            return GetJoinedCondition(joiner, conditions.AsReadOnly());
        }

        protected virtual TConditionBase GetInvertedCondition()
        {
            if (Conditions == null)
            {
                throw new InvalidOperationException("To use, must override Invert in derived class");
            }

            var conditions = new List<TConditionBase>(Conditions.Count);
            foreach (var condition in conditions)
            {
                conditions.Add(condition.GetInvertedCondition());
            }
            return GetJoinedCondition(Joiner == Conditional.Joiner.And ? Conditional.Joiner.Or : Conditional.Joiner.And, conditions.AsReadOnly());
        }

        private TConditionBase GetJoinedCondition(Joiner joiner, IReadOnlyList<TConditionBase> conditions)
        {
            var joinedCondition = CreateJoinedCondition(joiner, conditions);
            if (joinedCondition == null || joinedCondition.Joiner != joiner || joinedCondition.Conditions != conditions)
            {
                throw new InvalidOperationException("CreateJoinedCondition is not implemented properly");
            }
            return joinedCondition;
        }

        protected abstract TConditionBase CreateJoinedCondition(Joiner joiner, IReadOnlyList<TConditionBase> conditions);

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

            return Evaluate(evaluator, andExpression, orExpression, shortCircuitEvaluator);
        }

        private TResult EvaluateInternal<TResult>(Func<TCondition, TResult> evaluator, Func<TResult, TResult, TResult> andExpression, Func<TResult, TResult, TResult> orExpression, Func<Joiner, TResult, bool>? shortCircuitEvaluator)
        {
            if (Conditions == null)
            {
                return evaluator((TCondition)this);
            }

            TResult result = default!;
            var first = true;
            var joiner = Joiner.GetValueOrDefault();
            foreach (var condition in Conditions)
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

        protected virtual TConditionBase CloneInternal() => (TConditionBase)this;
    }
}