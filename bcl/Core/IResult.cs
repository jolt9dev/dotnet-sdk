namespace Jolt9;

public interface IResult
{
    bool IsOk { get; }

    bool IsError { get; }

    object Unwrap();

    object UnwrapError();
}

public interface IResult<T, E> : IResult, IEquatable<IResult<T, E>>
    where T : notnull
    where E : notnull
{
    T Value { get; }

    E Error { get; }

    new T Unwrap();

    T UnwrapOr(T defaultValue);

    T UnwrapOr(Func<T> defaultValueFactory);

    new E UnwrapError();

    E UnwrapErrorOr(E defaultValue);

    E UnwrapErrorOr(Func<E> defaultValueFactory);
}

internal enum ResultState : byte
{
    Ok = 0,
    Error = 1,
}