namespace Jolt9;

public static class Results
{
    public static Result Ok()
        => new(default!, default!, true);

    public static Result<T> Ok<T>(T value)
        where T : notnull
        => new(value, default!, true);

    public static Result<T, E> Ok<T, E>(T value)
        where T : notnull
        where E : notnull
        => new(value, default!, true);

    public static ValueResult OkValue()
        => new();

    public static ValueResult<T> OkValue<T>(T value)
        where T : notnull
        => new(value, default!, true);

    public static ValueResult<T, E> OkValue<T, E>(T value)
        where T : notnull
        where E : notnull
        => new(value, default!, true);

    public static Result Fail(Error error)
        => new(default!, error, false);

    public static Result<T> Fail<T>(Exception exception)
        where T : notnull
        => new(default!, exception, false);

    public static Result<T> Fail<T>(Error error)
        where T : notnull
        => new(default!, error, false);

    public static Result<T, E> Fail<T, E>(E error)
        where T : notnull
        where E : notnull
        => new(default!, error, false);

    public static ValueResult FailValue(Error error)
        => new(error);

    public static ValueResult<T> FailValue<T>(Exception exception)
        where T : notnull
        => new(default!, exception, false);

    public static ValueResult<T> FailValue<T>(Error error)
        where T : notnull
        => new(default!, error, false);

    public static ValueResult<T, E> FailValue<T, E>(E error)
        where T : notnull
        where E : notnull
        => new(default!, error, false);

    public static Result Try(Action action)
    {
        try
        {
            action();
            return Ok();
        }
        catch (Exception ex)
        {
            return Fail(ex);
        }
    }

    public static Result<T> Try<T>(Func<T> func)
        where T : notnull
    {
        try
        {
            return Ok(func());
        }
        catch (Exception ex)
        {
            return Fail<T>(ex);
        }
    }

    public static Result<T, E> Try<T, E>(Func<T> func, Func<Exception, E> error)
        where T : notnull
        where E : notnull
    {
        try
        {
            return Ok<T, E>(func());
        }
        catch (Exception ex)
        {
            return Fail<T, E>(error(ex));
        }
    }

    public static async Task<Result> TryAsync(Func<Task> action)
    {
        try
        {
            await action();
            return Ok();
        }
        catch (Exception ex)
        {
            return Fail(ex);
        }
    }

    public static async Task<Result<T>> TryAsync<T>(Func<Task<T>> func)
        where T : notnull
    {
        try
        {
            return Ok(await func());
        }
        catch (Exception ex)
        {
            return Fail<T>(ex);
        }
    }

    public static async Task<Result<T, E>> TryAsync<T, E>(Func<Task<T>> func, Func<Exception, E> error)
        where T : notnull
        where E : notnull
    {
        try
        {
            var value = await func();
            return Ok<T, E>(value);
        }
        catch (Exception ex)
        {
            return Fail<T, E>(error(ex));
        }
    }

    public static ValueResult TryValue(Action action)
    {
        try
        {
            action();
            return OkValue();
        }
        catch (Exception ex)
        {
            return FailValue(ex);
        }
    }

    public static ValueResult<T> TryValue<T>(Func<T> func)
        where T : notnull
    {
        try
        {
            return OkValue(func());
        }
        catch (Exception ex)
        {
            return FailValue<T>(ex);
        }
    }

    public static ValueResult<T, E> TryValue<T, E>(Func<T> func, Func<Exception, E> error)
        where T : notnull
        where E : notnull
    {
        try
        {
            return OkValue<T, E>(func());
        }
        catch (Exception ex)
        {
            return FailValue<T, E>(error(ex));
        }
    }
}