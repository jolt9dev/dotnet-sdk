namespace Jolt9.Options;

public class Option<T> : IOption<T>,
    IEquatable<Option<T>>, 
    IEquatable<T>
    where T : notnull
{
    private readonly OptionState state;

    private readonly T? value;

    public Option(T value)
    {
        this.value = value;
        this.state = value is ValueTuple or DBNull ? OptionState.None : OptionState.Some; 
    }

    public Option()
    {
        this.value = default;
        this.state = OptionState.None;
    }
    public bool IsSome => state == OptionState.Some;

    public bool IsNone => state == OptionState.None;

    public T Value
    {
        get
        {
            if (state == OptionState.None)
                throw new InvalidOperationException("Option is None");

            return value!;
        }
    }

    public static implicit operator T(Option<T> option)
        => option.Value;

    public static implicit operator Option<T>(T? value)
        => value is null ? new() : new(value);

    public static implicit operator Option<T>(ValueTuple value)
        => new();

    public static implicit operator ValueTask<Option<T>>(Option<T> option)
        => new(option);

    public static implicit operator Task<Option<T>>(Option<T> option)
        => Task.FromResult(option);

    public static Option<T> None { get; } = new();

    public static Option<T> Some(T value) => new(value);

    public static Option<T> From(T? value)
        => value is null or ValueTask or DBNull ? new() : new(value);

    public Option<U> And<U>(Option<U> other)
        where U : notnull
        => state == OptionState.Some ? other : new();

    public Option<U> And<U>(Func<Option<U>> otherFactory)
        where U : notnull
        => state == OptionState.Some ? otherFactory() : new();

    public Option<T> Or(Option<T> other)
        => state == OptionState.Some ? this : other;

    public Option<T> Or(T other)
        => state == OptionState.Some ? this : new(other);

    public Option<T> Or(Func<Option<T>> otherFactory)
        => state == OptionState.Some ? this : otherFactory();

    public bool Equals(IOption<T>? other)
    {
        if (other is null)
            return state == OptionState.None;

        if (state != OptionState.Some)
            return other.IsNone;

        return this.value!.Equals(other.Value);
    }

    public bool Equals(Option<T>? other)
    {
        if (ReferenceEquals(this, other))
            return true;

        if (other is null)
            return state == OptionState.None;

        if (state != OptionState.Some)
            return other.state == OptionState.None;

        return this.value!.Equals(other.value);
    }

    public bool Equals(T? other)
    {
        if (state != OptionState.Some)
            return other is null;

        return this.value!.Equals(other);
    }

    public T Expect(string message)
    {
        if (state == OptionState.None)
            throw new OptionException(message).TrackCallerInfo();

        return this.value!;
    }

    public T Expect(Exception ex)
    {
        if (state == OptionState.None)
            throw ex;

        return this.value!;
    }

    public Option<U> Map<U>(Func<T, U> mapper)
        where U : notnull
        => state == OptionState.Some ? new(mapper(this.value!)) : new();

    public U MapOr<U>(Func<T, U> mapper, U defaultValue)
        => state == OptionState.Some ? mapper(this.value!) : defaultValue;

    public U MapOr<U>(Func<T, U> mapper, Func<U> defaultValueFactory)
        => state == OptionState.Some ? mapper(this.value!) : defaultValueFactory();

    public Option<T> Inspect(Action<T> action)
    {
        if (state == OptionState.Some)
            action(this.value!);

        return this;
    }

    public bool Test(Func<T, bool> predicate)
    {
        if (state == OptionState.None)
            return false;

        return predicate(this.value!);
    }

    public bool TestNone(Func<bool> predicate)
    {
        if (state == OptionState.Some)
            return false;

        return predicate();
    }

    object IOption.Unwrap()
    {
        if (state == OptionState.None)
            throw new InvalidOperationException("Option is None");

        return value!;
    }


    public T Unwrap()
    {
        if (state == OptionState.None)
            throw new InvalidOperationException("Option is None");

        return value!;
    }

    public T UnwrapOr(T defaultValue)
        => state == OptionState.Some ? value! : defaultValue;

    public T UnwrapOr(Func<T> defaultValueFactory)
        => state == OptionState.Some ? value! : defaultValueFactory();

    public Option<T> Xor(Option<T> other)
    {
        if (this.IsSome)
            return other.IsNone ? this : new();

        return other.IsSome ? other : new();
    }

    public Option<(T, U)> Zip<U>(Option<U> other)
        where U : notnull
    {
        if (this.IsNone || other.IsNone)
            return new();

        return new((this.value!, other.Value));
    }
}