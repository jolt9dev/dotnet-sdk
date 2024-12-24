namespace Jolt9;

public class Result : IResult<Void, Error>
{
    private readonly Void value;

    private readonly Error? error;

    private readonly ResultState state;

    public Result(Void value = default, Error? error = default, bool ok = false)
    {
        this.value = value;
        this.error = error;
        this.state = ok ? ResultState.Ok : ResultState.Error;
    }

    public bool IsOk => this.state == ResultState.Ok;

    public bool IsError => this.state == ResultState.Error;

    public Void Value
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

    public static implicit operator Result(Void value)
        => new(value, default!, true);

    public static implicit operator Result(Error? error)
        => new(default!, error, false);

    public static implicit operator Result<Void, Error>(Result result)
        => new(result.Value, result.Error, result.IsOk);

    public static implicit operator Result<Void>(Result result)
        => new(result.Value, result.Error, result.IsOk);

    public static implicit operator Void(Result result)
        => result.Value;

    public static implicit operator Error(Result result)
        => result.Error;

    public static implicit operator Task<Result>(Result result)
        => Task.FromResult(result);

    public static implicit operator ValueTask<Result>(Result result)
        => new(result);

    public static Result Ok()
        => new(Void.Value, default!, true);

    public static Result Fail(Error error)
        => new(default!, error, false);

    public Result And(Result other)
        => this.IsOk ? other : new(default, this.Error, false);

    public Result<U> And<U>(Result<U> other)
        where U : notnull
        => this.IsOk ? other : new(default, this.Error, false);

    public Result<U, Error> And<U>(Func<Result<U, Error>> factory)
        where U : notnull
        => this.IsOk ? factory() : new(default, this.Error, false);

    public Result<U> And<U>(U value)
        where U : notnull
        => this.IsOk ? new(value, default!, true) : new(default, this.Error, false);

    public bool Equals(IResult<Void, Error>? other)
    {
        if (other is null)
            return false;

        if (this.IsError && other.IsError)
            return EqualityComparer<Error>.Default.Equals(this.Error, other.Error);

        if (this.IsOk != other.IsOk)
            return false;

        return EqualityComparer<Error>.Default.Equals(this.Error, other.Error);
    }

    public Result Inspect(Action<Error> action)
    {
        if (!this.IsOk)
            action(this.Error);

        return this;
    }

    public Void Expect(string message)
    {
        if (this.IsOk)
            return this.Value;

        var ex = new ResultException(message);
        ex.TrackCallerInfo();
        throw ex;
    }

    public Void Expect(Exception ex)
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

    public Result Or(Result other)
        => this.IsOk ? this : other;

    public Result<U> Map<U>(Func<Void, U> map)
        where U : notnull
        => this.IsOk ? new(map(this.Value), default!, true) : new(default(U), this.Error, false);

    public U MapOr<U>(Func<Void, U> map, U defaultvalue)
        where U : notnull
        => this.IsOk ? map(this.Value) : defaultvalue;

    public U MapOr<U>(Func<Void, U> map, Func<U> factory)
        where U : notnull
        => this.IsOk ? map(this.Value) : factory();

    public Result<Void, U> MapError<U>(Func<Error, U> map)
        where U : notnull
        => this.IsOk ? new(this.Value, default(U), true) : new(Void.Value, map(this.Error), false);

    public bool Test(Func<Void, bool> predicate)
        => this.IsOk && predicate(this.Value);

    public bool TestError(Func<Error, bool> predicate)
        => this.IsError && predicate(this.Error);

    public Option<Void> ToOption()
        => this.IsOk ? new(Void.Value) : new();

    public Option<Error> ToErrorOption()
        => this.IsOk ? new() : new(this.Error);

    object IResult.Unwrap()
        => this.Unwrap();

    public Void Unwrap()
    {
        if (this.IsOk)
            return this.Value;

        var ex = ResultException.FromUnknown(this.Error);
        ex.TrackCallerInfo();
        throw ex;
    }

    Void IResult<Void, Error>.UnwrapOr(Void defaultValue)
        => this.IsOk ? this.Value : Void.Value;

    Void IResult<Void, Error>.UnwrapOr(Func<Void> defaultValueFactory)
        => this.IsOk ? this.Value : Void.Value;

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