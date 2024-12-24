namespace Jolt9;

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

    public static Option<T> None { get; } = new();

    public bool IsSome => this.state == OptionState.Some;

    public bool IsNone => this.state == OptionState.None;

    public T Value
    {
        get
        {
            if (this.state == OptionState.None)
                throw new InvalidOperationException("Option is None");

            return this.value!;
        }
    }

    public static implicit operator T(Option<T> option)
        => option.Value;

    public static implicit operator Option<T>(T? value)
        => value is null ? new() : new(value);

    public static implicit operator Option<T>(ValueTuple value)
        => new();

    public static implicit operator ValueOption<T>(Option<T> option)
        => option.IsSome ? new(option.Value) : ValueOption<T>.None;

    public static implicit operator ValueTask<Option<T>>(Option<T> option)
        => new(option);

    public static implicit operator Task<Option<T>>(Option<T> option)
        => Task.FromResult(option);

    public static Option<T> Some(T value) => new(value);

    public static Option<T> From(T? value)
        => value is null or ValueTask or DBNull ? new() : new(value);

    public Option<U> And<U>(Option<U> other)
        where U : notnull
        => this.state == OptionState.Some ? other : new();

    public Option<U> And<U>(Func<Option<U>> otherFactory)
        where U : notnull
        => this.state == OptionState.Some ? otherFactory() : new();

    public Option<T> Or(Option<T> other)
        => this.state == OptionState.Some ? this : other;

    public Option<T> Or(T other)
        => this.state == OptionState.Some ? this : new(other);

    public Option<T> Or(Func<Option<T>> otherFactory)
        => this.state == OptionState.Some ? this : otherFactory();

    public override int GetHashCode()
    {
        return HashCode.Combine(this.state, this.value);
    }

    public bool Equals(IOption<T>? other)
    {
        if (other is null)
            return this.state == OptionState.None;

        if (this.state != OptionState.Some)
            return other.IsNone;

        return this.value!.Equals(other.Value);
    }

    public bool Equals(Option<T>? other)
    {
        if (ReferenceEquals(this, other))
            return true;

        if (other is null)
            return this.state == OptionState.None;

        if (this.state != OptionState.Some)
            return other.state == OptionState.None;

        return this.value!.Equals(other.value);
    }

    public bool Equals(T? other)
    {
        if (this.state != OptionState.Some)
            return other is null;

        return this.value!.Equals(other);
    }

    public override bool Equals(object? obj)
    {
        if (obj is Option<T> other)
            return this.Equals(other);

        if (obj is T otherValue)
            return this.Equals(otherValue);

        if (obj is IOption<T> otherOption)
            return this.Equals(otherOption);

        return false;
    }

    public T Expect(string message)
    {
        if (this.state == OptionState.None)
            throw new OptionException(message).TrackCallerInfo();

        return this.value!;
    }

    public T Expect(Exception ex)
    {
        if (this.state == OptionState.None)
            throw ex;

        return this.value!;
    }

    public Option<U> Map<U>(Func<T, U> mapper)
        where U : notnull
        => this.state == OptionState.Some ? new(mapper(this.value!)) : new();

    public U MapOr<U>(Func<T, U> mapper, U defaultValue)
        => this.state == OptionState.Some ? mapper(this.value!) : defaultValue;

    public U MapOr<U>(Func<T, U> mapper, Func<U> defaultValueFactory)
        => this.state == OptionState.Some ? mapper(this.value!) : defaultValueFactory();

    public Option<T> Inspect(Action<T> action)
    {
        if (this.state == OptionState.Some)
            action(this.value!);

        return this;
    }

    public bool Test(Func<T, bool> predicate)
    {
        if (this.state == OptionState.None)
            return false;

        return predicate(this.value!);
    }

    public bool TestNone(Func<bool> predicate)
    {
        if (this.state == OptionState.Some)
            return false;

        return predicate();
    }

    object IOption.Unwrap()
    {
        if (this.state == OptionState.None)
            throw new InvalidOperationException("Option is None");

        return this.value!;
    }

    public T Unwrap()
    {
        if (this.state == OptionState.None)
            throw new InvalidOperationException("Option is None");

        return this.value!;
    }

    public T UnwrapOr(T defaultValue)
        => this.state == OptionState.Some ? this.value! : defaultValue;

    public T UnwrapOr(Func<T> defaultValueFactory)
        => this.state == OptionState.Some ? this.value! : defaultValueFactory();

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