using System;
using System.Globalization;
using Genumerics;
using Newtonsoft.Json;

namespace Conditional
{
    public abstract class NumericUnaryOperation<T> : ValueProvider<T>
    {
        public new ValueProvider<T> Value { get; }

        protected NumericUnaryOperation(ValueProvider<T> value)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }
    }

    public abstract class NumericBinaryOperation<T> : ValueProvider<T>
    {
        public ValueProvider<T> Left { get; }

        public ValueProvider<T> Right { get; }

        protected NumericBinaryOperation(ValueProvider<T> left, ValueProvider<T> right)
        {
            Left = left ?? throw new ArgumentNullException(nameof(left));
            Right = right ?? throw new ArgumentNullException(nameof(right));
        }
    }

    public sealed class NumericAbsOperation<T> : NumericUnaryOperation<T>
    {
        public NumericAbsOperation(ValueProvider<T> value)
            : base(value)
        {
        }

        public override T GetValue(object? context) => Number.Abs(Value.GetValue(context));
    }

    public sealed class NumericAddOperation<T> : NumericBinaryOperation<T>
    {
        public NumericAddOperation(ValueProvider<T> left, ValueProvider<T> right)
            : base(left, right)
        {
        }

        public override T GetValue(object? context) => Number.Add(Left.GetValue(context), Right.GetValue(context));
    }

    public sealed class NumericBitwiseAndOperation<T> : NumericBinaryOperation<T>
    {
        public NumericBitwiseAndOperation(ValueProvider<T> left, ValueProvider<T> right)
            : base(left, right)
        {
        }

        public override T GetValue(object? context) => Number.BitwiseAnd(Left.GetValue(context), Right.GetValue(context));
    }

    public sealed class NumericBitwiseOrOperation<T> : NumericBinaryOperation<T>
    {
        public NumericBitwiseOrOperation(ValueProvider<T> left, ValueProvider<T> right)
            : base(left, right)
        {
        }

        public override T GetValue(object? context) => Number.BitwiseOr(Left.GetValue(context), Right.GetValue(context));
    }

    public sealed class NumericCeilingOperation<T> : NumericUnaryOperation<T>
    {
        public NumericCeilingOperation(ValueProvider<T> value)
            : base(value)
        {
        }

        public override T GetValue(object? context) => Number.Ceiling(Value.GetValue(context));
    }

    public sealed class NumericClampOperation<T> : ValueProvider<T>
    {
        public new ValueProvider<T> Value { get; }

        public ValueProvider<T> Min { get; }

        public ValueProvider<T> Max { get; }

        public NumericClampOperation(ValueProvider<T> value, ValueProvider<T> min, ValueProvider<T> max)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
            Min = min ?? throw new ArgumentNullException(nameof(min));
            Max = max ?? throw new ArgumentNullException(nameof(max));
        }

        public override T GetValue(object? context) => Number.Clamp(Value.GetValue(context), Min.GetValue(context), Max.GetValue(context));
    }

    public sealed class NumericConvertOperation<TFrom, TTo> : ValueProvider<TTo>
    {
        public new ValueProvider<TFrom> Value { get; }

        public NumericConvertOperation(ValueProvider<TFrom> value)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public override TTo GetValue(object? context) => Number.Convert<TFrom, TTo>(Value.GetValue(context));
    }

    public sealed class NumericDivideOperation<T> : ValueProvider<T>
    {
        public ValueProvider<T> Dividend { get; }

        public ValueProvider<T> Divisor { get; }

        public NumericDivideOperation(ValueProvider<T> dividend, ValueProvider<T> divisor)
        {
            Dividend = dividend ?? throw new ArgumentNullException(nameof(dividend));
            Divisor = divisor ?? throw new ArgumentNullException(nameof(divisor));
        }

        public override T GetValue(object? context) => Number.Divide(Dividend.GetValue(context), Divisor.GetValue(context));
    }

    public sealed class NumericFloorOperation<T> : NumericUnaryOperation<T>
    {
        public NumericFloorOperation(ValueProvider<T> value)
            : base(value)
        {
        }

        public override T GetValue(object? context) => Number.Floor(Value.GetValue(context));
    }

    public sealed class NumericLeftShiftOperation<T> : ValueProvider<T>
    {
        public new ValueProvider<T> Value { get; }

        public ValueProvider<int> Shift { get; }

        public NumericLeftShiftOperation(ValueProvider<T> value, ValueProvider<int> shift)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
            Shift = shift ?? throw new ArgumentNullException(nameof(shift));
        }

        public override T GetValue(object? context) => Number.LeftShift(Value.GetValue(context), Shift.GetValue(context));
    }

    public sealed class NumericMaxOperation<T> : NumericBinaryOperation<T>
    {
        public NumericMaxOperation(ValueProvider<T> left, ValueProvider<T> right)
            : base(left, right)
        {
        }

        public override T GetValue(object? context) => Number.Max(Left.GetValue(context), Right.GetValue(context));
    }

    public sealed class NumericMinOperation<T> : NumericBinaryOperation<T>
    {
        public NumericMinOperation(ValueProvider<T> left, ValueProvider<T> right)
            : base(left, right)
        {
        }

        public override T GetValue(object? context) => Number.Min(Left.GetValue(context), Right.GetValue(context));
    }

    public sealed class NumericMultiplyOperation<T> : NumericBinaryOperation<T>
    {
        public NumericMultiplyOperation(ValueProvider<T> left, ValueProvider<T> right)
            : base(left, right)
        {
        }

        public override T GetValue(object? context) => Number.Multiply(Left.GetValue(context), Right.GetValue(context));
    }

    public sealed class NumericNegateOperation<T> : NumericUnaryOperation<T>
    {
        public NumericNegateOperation(ValueProvider<T> value)
            : base(value)
        {
        }

        public override T GetValue(object? context) => Number.Negate(Value.GetValue(context));
    }

    public sealed class NumericOnesComplementOperation<T> : NumericUnaryOperation<T>
    {
        public NumericOnesComplementOperation(ValueProvider<T> value)
            : base(value)
        {
        }

        public override T GetValue(object? context) => Number.OnesComplement(Value.GetValue(context));
    }

    public sealed class NumericParseOperation<T> : ValueProvider<T>
    {
        public new ValueProvider<string> Value { get; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ValueProvider<NumberStyles?>? Style { get; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ValueProvider<IFormatProvider?>? Provider { get; }

        public NumericParseOperation(ValueProvider<string> value, ValueProvider<NumberStyles?>? style = null, ValueProvider<IFormatProvider?>? provider = null)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
            Style = style;
            Provider = provider;
        }

        public override T GetValue(object? context) => Number.Parse<T>(Value.GetValue(context), Style?.GetValue(context), Provider?.GetValue(context));
    }

    public sealed class NumericParseOrDefaultOperation<T> : ValueProvider<T>
    {
        public new ValueProvider<string> Value { get; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ValueProvider<NumberStyles?>? Style { get; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ValueProvider<IFormatProvider?>? Provider { get; }

        public ValueProvider<T> Default { get; }

        public NumericParseOrDefaultOperation(ValueProvider<string> value, ValueProvider<T> @default, ValueProvider<NumberStyles?>? style = null, ValueProvider<IFormatProvider?>? provider = null)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
            Default = @default ?? throw new ArgumentNullException(nameof(@default));
            Style = style;
            Provider = provider;
        }

        public override T GetValue(object? context) => Number.TryParse<T>(Value.GetValue(context), Style?.GetValue(context), Provider?.GetValue(context), out var result) ? result : Default.GetValue(context);
    }

    public sealed class NumericRemainderOperation<T> : ValueProvider<T>
    {
        public ValueProvider<T> Dividend { get; }

        public ValueProvider<T> Divisor { get; }

        public NumericRemainderOperation(ValueProvider<T> dividend, ValueProvider<T> divisor)
        {
            Dividend = dividend ?? throw new ArgumentNullException(nameof(dividend));
            Divisor = divisor ?? throw new ArgumentNullException(nameof(divisor));
        }

        public override T GetValue(object? context) => Number.Remainder(Dividend.GetValue(context), Divisor.GetValue(context));
    }

    public sealed class NumericRightShiftOperation<T> : ValueProvider<T>
    {
        public new ValueProvider<T> Value { get; }

        public ValueProvider<int> Shift { get; }

        public NumericRightShiftOperation(ValueProvider<T> value, ValueProvider<int> shift)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
            Shift = shift ?? throw new ArgumentNullException(nameof(shift));
        }

        public override T GetValue(object? context) => Number.RightShift(Value.GetValue(context), Shift.GetValue(context));
    }

    public sealed class NumericRoundOperation<T> : ValueProvider<T>
    {
        public new ValueProvider<T> Value { get; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ValueProvider<int?>? Digits { get; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ValueProvider<MidpointRounding?>? Mode { get; }

        public NumericRoundOperation(ValueProvider<T> value, ValueProvider<int?>? digits = null, ValueProvider<MidpointRounding?>? mode = null)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
            Digits = digits;
            Mode = mode;
        }

        public override T GetValue(object? context) => Number.Round(Value.GetValue(context), Digits?.GetValue(context) ?? 0, Mode?.GetValue(context) ?? MidpointRounding.ToEven);
    }

    public sealed class NumericSubtractOperation<T> : NumericBinaryOperation<T>
    {
        public NumericSubtractOperation(ValueProvider<T> left, ValueProvider<T> right)
            : base(left, right)
        {
        }

        public override T GetValue(object? context) => Number.Subtract(Left.GetValue(context), Right.GetValue(context));
    }

    public sealed class NumericToStringOperation<T> : ValueProvider<string?>
    {
        public new ValueProvider<T> Value { get; }

        public ValueProvider<string?>? Format { get; }

        public ValueProvider<IFormatProvider?>? Provider { get; }

        public NumericToStringOperation(ValueProvider<T> value, ValueProvider<string?>? format = null, ValueProvider<IFormatProvider?>? provider = null)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
            Format = format;
            Provider = provider;
        }

        public override string? GetValue(object? context) => Number.ToString(Value.GetValue(context), Format?.GetValue(context), Provider?.GetValue(context));
    }

    public sealed class NumericTruncateOperation<T> : NumericUnaryOperation<T>
    {
        public NumericTruncateOperation(ValueProvider<T> value)
            : base(value)
        {
        }

        public override T GetValue(object? context) => Number.Truncate(Value.GetValue(context));
    }

    public sealed class NumericXorOperation<T> : NumericBinaryOperation<T>
    {
        public NumericXorOperation(ValueProvider<T> left, ValueProvider<T> right)
            : base(left, right)
        {
        }

        public override T GetValue(object? context) => Number.Xor(Left.GetValue(context), Right.GetValue(context));
    }
}