namespace Jolt9;

public class Result<T> : IResult<T, Error>
    where T : notnull
{
    private readonly T? value;

    private readonly Error? error;

    private readonly ResultState state;

    public Result(T? value = default, Error? error = default, bool ok = false)
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

    public Error Error
    {
        get
        {
            if (this.state == ResultState.Ok)
                throw new ResultException("Result does not have an error.");

            return this.error!;
        }
    }

    public static implicit operator Result<T>(T? value)
        => new(value, default!, true);

    public static implicit operator Result<T>(Error? error)
        => new(default!, error, false);

    public static implicit operator Result<T, Error>(Result<T> result)
        => new(result.Value, result.Error, result.IsOk);

    public static implicit operator T(Result<T> result)
        => result.Value;

    public static implicit operator Error(Result<T> result)
        => result.Error;

    public static implicit operator Task<Result<T>>(Result<T> result)
        => Task.FromResult(result);

    public static implicit operator ValueTask<Result<T>>(Result<T> result)
        => new(result);

    public static Result<T, Error> Ok(T value)
        => new(value, default!, true);

    public static Result<T, Error> Fail(Error error)
        => new(default!, error, false);

    public Result<U, Error> And<U>(Result<U, Error> other)
        where U : notnull
        => this.IsOk ? other : new(default, this.Error, false);

    public Result<U, Error> And<U>(Func<Result<U, Error>> factory)
        where U : notnull
        => this.IsOk ? factory() : new(default, this.Error, false);

    public Result<U> And<U>(U value)
        where U : notnull
        => this.IsOk ? new(value, default!, true) : new(default, this.Error, false);

    public bool Equals(IResult<T, Error>? other)
    {
        if (other is null)
            return false;

        if (this.IsOk != other.IsOk)
            return false;

        return this.IsOk ? EqualityComparer<T>.Default.Equals(this.Value, other.Value) : EqualityComparer<Error>.Default.Equals(this.Error, other.Error);
    }

    public Result<T> Inspect(Action<T> action)
    {
        if (this.IsOk)
            action(this.Value);

        return this;
    }

    public Result<T> Inspect(Action<Error> action)
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

    public Result<T> Or(Result<T> other)
        => this.IsOk ? this : other;

    public Result<T> Or(Func<Result<T>> factory)
        => this.IsOk ? this : factory();

    public Result<T> Or(T value)
        => this.IsOk ? this : new(value, default!, true);

    public Result<T> Or(Func<T, bool> predicate, Result<T> other)
        => (this.IsOk && predicate(this.Value)) ? this : other;

    public Result<T> Or(Func<T, bool> predicate, Func<Result<T>> factory)
        => (this.IsOk && predicate(this.Value)) ? this : factory();

    public Result<U> Map<U>(Func<T, U> map)
        where U : notnull
        => this.IsOk ? new(map(this.Value), default!, true) : new(default(U), this.Error, false);

    public U MapOr<U>(Func<T, U> map, U defaultvalue)
        where U : notnull
        => this.IsOk ? map(this.Value) : defaultvalue;

    public U MapOr<U>(Func<T, U> map, Func<U> factory)
        where U : notnull
        => this.IsOk ? map(this.Value) : factory();

    public Result<T, U> MapError<U>(Func<Error, U> map)
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

        throw new ResultException("Result does not have an error.");
    }

    public Error UnwrapErrorOr(Error defaultValue)
        => this.IsError ? this.Error : defaultValue;

    public Error UnwrapErrorOr(Func<Error> defaultValueFactory)
        => this.IsError ? this.Error : defaultValueFactory();
}