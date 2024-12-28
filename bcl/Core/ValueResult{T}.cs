namespace Jolt9;

public readonly struct ValueResult<T> : IResult<T, Error>,
    IEquatable<ValueResult<T>>
    where T : notnull
{
    private readonly T? value;

    private readonly Error? error;

    private readonly ResultState state;

    public ValueResult(T? value = default, Error? error = default, bool ok = false)
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
                throw new ResultException("ValueResult has an error.");

            return this.value!;
        }
    }

    public Error Error
    {
        get
        {
            if (this.state == ResultState.Ok)
                throw new ResultException("ValueResult does not have an error.");

            return this.error!;
        }
    }

    public static implicit operator ValueResult<T>(T? value)
        => new(value, default!, true);

    public static implicit operator ValueResult<T>(Error error)
        => new(default!, error, false);

    public static implicit operator ValueResult<T>(Exception exception)
        => new(default!, exception, false);

    public static implicit operator T(ValueResult<T> result)
        => result.Value;

    public static implicit operator Error(ValueResult<T> result)
        => result.Error;

    public static implicit operator Task<ValueResult<T>>(ValueResult<T> result)
        => Task.FromResult(result);

    public static implicit operator ValueTask<ValueResult<T>>(ValueResult<T> result)
        => new(result);

    public static ValueResult<T> Ok(T value)
        => new(value, default!, true);

    public static ValueResult<T> Fail(Error error)
        => new(default!, error, false);

    public ValueResult<U> And<U>(ValueResult<U> other)
        where U : notnull
        => this.IsOk ? other : new(default, this.Error, false);

    public ValueResult<U> And<U>(Func<ValueResult<U>> factory)
        where U : notnull
        => this.IsOk ? factory() : new(default, this.Error, false);

    public ValueResult<U> And<U>(U value)
        where U : notnull
        => this.IsOk ? new(value, default!, true) : new(default, this.Error, false);

    public bool Equals(ValueResult<T> other)
    {
        if (this.IsOk != other.IsOk)
            return false;

        return this.IsOk ? EqualityComparer<T>.Default.Equals(this.Value, other.Value) : EqualityComparer<Error>.Default.Equals(this.Error, other.Error);
    }

    public bool Equals(IResult<T, Error>? other)
    {
        if (other is null)
            return false;

        if (this.IsOk != other.IsOk)
            return false;

        return this.IsOk ? EqualityComparer<T>.Default.Equals(this.Value, other.Value) : EqualityComparer<Error>.Default.Equals(this.Error, other.Error);
    }

    public override bool Equals(object? obj)
    {
        if (obj is ValueResult<T, Error> other)
            return this.Equals(other);

        if (obj is IResult<T, Error> iother)
            return this.Equals(iother);

        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(this.IsOk, this.Value, this.Error);
    }

    public ValueResult<T> Inspect(Action<T> action)
    {
        if (this.IsOk)
            action(this.Value);

        return this;
    }

    public ValueResult<T> Inspect(Action<Error> action)
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

    public Error ExpectError(string message)
    {
        if (this.IsError)
            return this.Error;

        var ex = new ResultException(message);
        ex.TrackCallerInfo();
        throw ex;
    }

    public Error ExpectError(Exception ex)
    {
        if (this.IsError)
            return this.Error;

        throw ex;
    }

    public ValueResult<T> Or(ValueResult<T> other)
        => this.IsOk ? this : other;

    public ValueResult<T> Or(Func<ValueResult<T>> factory)
        => this.IsOk ? this : factory();

    public ValueResult<T> Or(T value)
        => this.IsOk ? this : new(value, default!, true);

    public ValueResult<T> Or(Func<T, bool> predicate, ValueResult<T> other)
        => (this.IsOk && predicate(this.Value)) ? this : other;

    public ValueResult<T> Or(Func<T, bool> predicate, Func<ValueResult<T>> factory)
        => (this.IsOk && predicate(this.Value)) ? this : factory();

    public ValueResult<U> Map<U>(Func<T, U> map)
        where U : notnull
        => this.IsOk ? new(map(this.Value), default!, true) : new(default(U), this.Error, false);

    public U MapOr<U>(Func<T, U> map, U defaultvalue)
        where U : notnull
        => this.IsOk ? map(this.Value) : defaultvalue;

    public U MapOr<U>(Func<T, U> map, Func<U> factory)
        where U : notnull
        => this.IsOk ? map(this.Value) : factory();

    public ValueResult<T, U> MapError<U>(Func<Error, U> map)
        where U : notnull
        => this.IsOk ? new(this.Value, default(U), true) : new(default(T), map(this.Error), false);

    public bool Test(Func<T, bool> predicate)
        => this.IsOk && predicate(this.Value);

    public bool TestError(Func<Error, bool> predicate)
        => this.IsError && predicate(this.Error);

    public Option<T> ToOption()
        => this.IsOk ? new(this.Value) : new();

    public Option<Error> ToErrorOption()
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

    public Error UnwrapError()
    {
        if (this.IsError)
            return this.Error;

        throw new ResultException("ValueResult does not have an error.");
    }

    public Error UnwrapErrorOr(Error defaultValue)
        => this.IsError ? this.Error : defaultValue;

    public Error UnwrapErrorOr(Func<Error> defaultValueFactory)
        => this.IsError ? this.Error : defaultValueFactory();
}