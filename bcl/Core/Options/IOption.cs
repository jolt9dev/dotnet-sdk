
namespace Jolt9.Options; 

public interface IOption
{
    bool IsSome { get; }

    bool IsNone { get; }

    object Unwrap();
}

public interface IOption<T> : IOption, IEquatable<IOption<T>>
    where T : notnull
{

    T Value { get; }

    new T Unwrap();

    T UnwrapOr(T defaultValue);

    T UnwrapOr(Func<T> defaultValueFactory);
}

internal enum OptionState : byte 
{
    None = 0,
    Some = 1
}