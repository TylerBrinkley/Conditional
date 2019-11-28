using System;
using EnumsNET;

namespace Conditional
{
    public enum StringOperator
    {
        Equals = 0,
        NotEquals = 1,
        EqualsExact = 2,
        NotEqualsExact = 3,
        Contains = 4,
        NotContains = 5,
        StartsWith = 6,
        NotStartsWith = 7,
        EndsWith = 8,
        NotEndsWIth = 9,
        EqualInLength = 10,
        NotEqualInLength = 11,
        ShorterThan = 12,
        LongerThanOrEqualInLength = 13,
        NotShorterThan = LongerThanOrEqualInLength,
        LongerThan = 14,
        ShorterThanOrEqualInLength = 15,
        NotLongerThan = ShorterThanOrEqualInLength
    }

    public static class StringOperatorExtensions
    {
        public static StringOperator Invert(this StringOperator @operator)
        {
            @operator.Validate(nameof(@operator));

            return @operator + (((int)@operator) % 2 == 0 ? 1 : -1);
        }

        public static bool Evaluate(this StringOperator @operator, string? value, string? comparisonValue)
        {
            @operator.Validate(nameof(@operator));

            var isOdd = ((int)@operator) % 2 == 1;
            return EvaluateInternal(isOdd ? @operator + 1 : @operator) ^ isOdd;

            bool EvaluateInternal(StringOperator effectiveOperator)
            {
                return effectiveOperator switch
                {
                    StringOperator.Equals => string.Equals(value, comparisonValue, StringComparison.OrdinalIgnoreCase),
                    StringOperator.EqualsExact => string.Equals(value, comparisonValue, StringComparison.Ordinal),
                    StringOperator.Contains => comparisonValue == null || (value == null ? comparisonValue.Length == 0 : value.IndexOf(comparisonValue, StringComparison.OrdinalIgnoreCase) >= 0),
                    StringOperator.StartsWith => comparisonValue == null || (value == null ? comparisonValue.Length == 0 : value.StartsWith(comparisonValue, StringComparison.OrdinalIgnoreCase)),
                    StringOperator.EndsWith => comparisonValue == null || (value == null ? comparisonValue.Length == 0 : value.EndsWith(comparisonValue, StringComparison.OrdinalIgnoreCase)),
                    StringOperator.EqualInLength => (value?.Length ?? 0) == (comparisonValue?.Length ?? 0),
                    StringOperator.ShorterThan => (value?.Length ?? 0) < (comparisonValue?.Length ?? 0),
                    StringOperator.LongerThan => (value?.Length ?? 0) > (comparisonValue?.Length ?? 0),
                    _ => throw new NotSupportedException($"StringOperator of {@operator} is not supported"),
                };
            }
        }
    }
}