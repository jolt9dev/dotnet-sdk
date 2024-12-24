using System.Diagnostics.CodeAnalysis;

namespace Jolt9;

public class Result<T, E> : IResult<T, E>,
    IEquatable<Result<T, E>>
    where T : notnull
    where E : notnull
{
    private readonly T? value;

    private readonly E? error;

    private readonly ResultState state;

    public Result(T? value = default, E? error = default, bool ok = false)
    {
        this.value = value;
        this.error = error;
        this.state = ok ? ResultState.Ok : ResultState.Error;
    }

    public bool IsOk => this.state == ResultState.Ok;

    public bool IsError => this.state == ResultState.Error;

    public T Value
    {
        get
        {
            if (this.state == ResultState.Error)
                throw new ResultException("Result has an error.");

            return this.value!;
        }
    }

    public E Error
    {
        get
        {
            if (this.state == ResultState.Ok)
                throw new ResultException("Result does not have an error.");

            return this.error!;
        }
    }

    public static implicit operator Result<T, E>(T? value)
        => new(value, default!, true);

    public static implicit operator Result<T, E>(E? error)
        => new(default!, error, false);

    public static implicit operator T(Result<T, E> result)
        => result.Value;

    public static implicit operator E(Result<T, E> result)
        => result.Error;

    public static implicit operator Task<Result<T, E>>(Result<T, E> result)
        => Task.FromResult(result);

    public static implicit operator ValueTask<Result<T, E>>(Result<T, E> result)
        => new(result);

    public static Result<T, E> Ok(T value)
        => new(value, default!, true);

    public static Result<T, E> Fail(E error)
        => new(default!, error, false);

    public Result<U, E> And<U>(Result<U, E> other)
        where U : notnull
        => this.IsOk ? other : new(default, this.Error, false);

    public Result<U, E> And<U>(Func<Result<U, E>> factory)
        where U : notnull
        => this.IsOk ? factory() : new(default, this.Error, false);

    public Result<U, E> And<U>(U value)
        where U : notnull
        => this.IsOk ? new(value, default!, true) : new(default, this.Error, false);

    public bool Equals(Result<T, E>? other)
    {
        if (other is null)
            return false;

        if (this.IsOk != other.IsOk)
            return false;

        return this.IsOk ? EqualityComparer<T>.Default.Equals(this.Value, other.Value) : EqualityComparer<E>.Default.Equals(this.Error, other.Error);
    }

    public bool Equals(IResult<T, E>? other)
    {
        if (other is null)
            return false;

        if (this.IsOk != other.IsOk)
            return false;

        return this.IsOk ? EqualityComparer<T>.Default.Equals(this.Value, other.Value) : EqualityComparer<E>.Default.Equals(this.Error, other.Error);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj))
            return true;

        if (obj is Result<T, E> other)
            return this.Equals(other);

        if (obj is IResult<T, E> iother)
            return this.Equals(iother);

        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(this.IsOk, this.Value, this.Error);
    }

    public Result<T, E> Inspect(Action<T> action)
    {
        if (this.IsOk)
            action(this.Value);

        return this;
    }

    public Result<T, E> Inspect(Action<E> action)
    {
        if (!this.IsOk)
            action(this.Error);

        return this;
    }

    public T Expect(string message)
    {
        if (this.IsOk)
            return this.Value;

        var ex = new ResultException(message);
        ex.TrackCallerInfo();
        throw ex;
    }

    public T Expect(Exception ex)
    {
        if (this.IsOk)
            return this.Value;

        throw ex;
    }

    public E ExpectError(string message)
    {
        if (this.IsError)
            return this.Error;

        var ex = new ResultException(message);
        ex.TrackCallerInfo();
        throw ex;
    }

    public E ExpectError(Exception ex)
    {
        if (this.IsError)
            return this.Error;

        throw ex;
    }

    public Result<T, E> Or(Result<T, E> other)
        => this.IsOk ? this : other;

    public Result<T, E> Or(Func<Result<T, E>> factory)
        => this.IsOk ? this : factory();

    public Result<T, E> Or(T value)
        => this.IsOk ? this : new(value, default!, true);

    public Result<T, E> Or(Func<T, bool> predicate, Result<T, E> other)
        => (this.IsOk && predicate(this.Value)) ? this : other;

    public Result<T, E> Or(Func<T, bool> predicate, Func<Result<T, E>> factory)
        => (this.IsOk && predicate(this.Value)) ? this : factory();

    public Result<U, E> Map<U>(Func<T, U> map)
        where U : notnull
        => this.IsOk ? new(map(this.Value), default!, true) : new(default(U), this.Error, false);

    public U MapOr<U>(Func<T, U> map, U defaultvalue)
        where U : notnull
        => this.IsOk ? map(this.Value) : defaultvalue;

    public U MapOr<U>(Func<T, U> map, Func<U> factory)
        where U : notnull
        => this.IsOk ? map(this.Value) : factory();

    public Result<T, U> MapError<U>(Func<E, U> map)
        where U : notnull
        => this.IsOk ? new(this.Value, default(U), true) : new(default(T), map(this.Error), false);

    public bool Test(Func<T, bool> predicate)
        => this.IsOk && predicate(this.Value);

    public bool TestError(Func<E, bool> predicate)
        => this.IsError && predicate(this.Error);

    public Option<T> ToOption()
        => this.IsOk ? new(this.Value) : new();

    public Option<E> ToErrorOption()
        => this.IsOk ? new() : new(this.Error);

    object IResult.Unwrap()
        => this.Unwrap();

    public T Unwrap()
    {
        if (this.IsOk)
            return this.Value;

        var ex = ResultException.FromUnknown(this.Error);
        ex.TrackCallerInfo();
        throw ex;
    }

    public T UnwrapOr(T defaultValue)
        => this.IsOk ? this.Value : defaultValue;

    public T UnwrapOr(Func<T> defaultValueFactory)
        => this.IsOk ? this.Value : defaultValueFactory();

    object IResult.UnwrapError()
        => this.UnwrapError();

    public E UnwrapError()
    {
        if (this.IsError)
            return this.Error;

        throw new ResultException("Result does not have an error.");
    }

    public E UnwrapErrorOr(E defaultValue)
        => this.IsError ? this.Error : defaultValue;

    public E UnwrapErrorOr(Func<E> defaultValueFactory)
        => this.IsError ? this.Error : defaultValueFactory();
}