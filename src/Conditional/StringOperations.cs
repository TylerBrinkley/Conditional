using System;

namespace Conditional
{
    public sealed class StringOperations : ValueProvider<string?>
    {
        public new ValueProvider<string?> Value { get; }

        public StringOperations(ValueProvider<string?> value)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public override string? GetValue(object? context) => Value.GetValue(context)?.Trim();
    }
}