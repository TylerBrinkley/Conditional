using System;
using System.Collections.Generic;
using System.Linq;
using EnumsNET;

namespace Conditional
{
    public enum CollectionOperator
    {
        Any = 0,
        None = 1,
        All = 2,
        NotAll = 3
    }

    public static class CollectionOperatorExtensions
    {
        public static CollectionOperator Invert(this CollectionOperator @operator)
        {
            @operator.Validate(nameof(@operator));

            return @operator + (((int)@operator) % 2 == 0 ? 1 : -1);
        }

        public static bool Evaluate<T>(this CollectionOperator @operator, IEnumerable<T> collection, Func<T, bool> predicate)
        {
            @operator.Validate(nameof(@operator));
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            var isOdd = ((int)@operator) % 2 == 1;
            return EvaluateInternal(isOdd ? @operator + 1 : @operator) ^ isOdd;

            bool EvaluateInternal(CollectionOperator effectiveOperator)
            {
                return effectiveOperator switch
                {
                    CollectionOperator.Any => collection.Any(predicate),
                    CollectionOperator.All => collection.All(predicate),
                    _ => throw new NotSupportedException($"CollectionOperator of {@operator} is not supported"),
                };
            }
        }
    }
}