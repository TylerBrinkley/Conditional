using System;
using EnumsNET;
using Genumerics;

namespace Conditional
{
    public enum NumericOperator
    {
        Equals = 0,
        NotEquals = 1,
        LessThan = 2,
        GreaterThanOrEqual = 3,
        NotLessThan = GreaterThanOrEqual,
        GreaterThan = 4,
        LessThanOrEqual = 5,
        NotGreaterThan = LessThanOrEqual
    }

    public static class NumericOperatorExtensions
    {
        public static NumericOperator Invert(this NumericOperator @operator)
        {
            @operator.Validate(nameof(@operator));

            return @operator + (((int)@operator) % 2 == 0 ? 1 : -1);
        }

        public static bool Evaluate<T>(this NumericOperator @operator, T value, T comparisonValue)
        {
            @operator.Validate(nameof(@operator));

            var isOdd = ((int)@operator) % 2 == 1;
            return EvaluateInternal(isOdd ? @operator + 1 : @operator) ^ isOdd;

            bool EvaluateInternal(NumericOperator effectiveOperator)
            {
                return effectiveOperator switch
                {
                    NumericOperator.Equals => Number.Equals(value, comparisonValue),
                    NumericOperator.LessThan => Number.LessThan(value, comparisonValue),
                    NumericOperator.GreaterThan => Number.GreaterThan(value, comparisonValue),
                    _ => throw new NotSupportedException($"NumericOperator of {@operator} is not supported"),
                };
            }
        }
    }
}