namespace Jolt9;

public static class Options
{
    public static Option<T> Some<T>(T value)
        where T : notnull
    {
        return new Option<T>(value);
    }

    public static Option<T> None<T>()
        where T : notnull
    {
        return Option<T>.None;
    }

    public static Option<T> From<T>(T? value)
        where T : notnull
    {
        return value is null ? None<T>() : Some(value);
    }

    public static ValueOption<T> SomeValue<T>(T value)
        where T : notnull
    {
        return new ValueOption<T>(value);
    }

    public static ValueOption<T> NoValue<T>()
        where T : notnull
    {
        return ValueOption<T>.None;
    }

    public static ValueOption<T> FromValue<T>(T? value)
        where T : notnull
    {
        return value is null ? ValueOption<T>.None : SomeValue(value);
    }
}