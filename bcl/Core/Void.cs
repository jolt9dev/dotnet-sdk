
using Jolt9.Options;

namespace Jolt9;

public readonly struct Void
{
    public static Void Value { get; } = default;
    
    public static bool IsVoid(object? value)
    {
        return value is Void;
    }

    public static bool IsNil(object? value)
    {
        switch (value)
        {
            case null:
            case DBNull:
            case Void:
                return true;
            case IOption option:
                return option.IsNone;
            default:
                return false;
        }
    }
}