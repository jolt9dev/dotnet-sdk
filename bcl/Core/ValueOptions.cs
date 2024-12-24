namespace Jolt9;

public static class ValueOptions
{
    public static ValueOption<T> Some<T>(T value)
        where T : notnull
    {
        return new ValueOption<T>(value);
    }

    public static ValueOption<T> None<T>()
        where T : notnull
    {
        return ValueOption<T>.None;
    }

    public static ValueOption<T> From<T>(T? value)
        where T : notnull
    {
        return value is null ? None<T>() : Some(value);
    }
}