using System.Diagnostics.CodeAnalysis;

namespace Jolt9;

public readonly struct ValueOption<T> : IOption<T>, IEquatable<ValueOption<T>>
    where T : notnull
{
    private readonly OptionState state;

    private readonly T? value;

    public ValueOption(T value)
    {
        this.value = value;
        this.state = value is ValueTuple or DBNull ? OptionState.None : OptionState.Some;
    }

    public ValueOption()
    {
        this.value = default;
        this.state = OptionState.None;
    }

    public static ValueOption<T> None { get; } = new();

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

    public static implicit operator T(ValueOption<T> option)
        => option.Value;

    public static implicit operator ValueOption<T>(T? value)
        => value is null ? new() : new(value);

    public static implicit operator ValueOption<T>(ValueTuple value)
        => new();

    public static implicit operator Option<T>(ValueOption<T> option)
        => option.IsSome ? new(option.value!) : Option<T>.None;

    public static implicit operator ValueTask<ValueOption<T>>(ValueOption<T> option)
        => new(option);

    public static implicit operator Task<ValueOption<T>>(ValueOption<T> option)
        => Task.FromResult(option);

    public static ValueOption<T> Some(T value) => new(value);

    public static ValueOption<T> From(T? value)
        => value is null or ValueTask or DBNull ? new() : new(value);

    public ValueOption<U> And<U>(ValueOption<U> other)
        where U : notnull
        => this.state == OptionState.Some ? other : new();

    public ValueOption<U> And<U>(Func<ValueOption<U>> otherFactory)
        where U : notnull
        => this.state == OptionState.Some ? otherFactory() : new();

    public ValueOption<T> Or(ValueOption<T> other)
        => this.state == OptionState.Some ? this : other;

    public ValueOption<T> Or(T other)
        => this.state == OptionState.Some ? this : new(other);

    public ValueOption<T> Or(Func<ValueOption<T>> otherFactory)
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

    public bool Equals(ValueOption<T> other)
    {
        if (this.state == OptionState.None)
            return other.state == OptionState.None;

        return EqualityComparer<T>.Default.Equals(this.value!, other.value!);
    }

    public bool Equals(T? other)
    {
        if (other is null)
            return this.state == OptionState.None;

        return EqualityComparer<T>.Default.Equals(this.value!, other);
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is ValueOption<T> other)
            return this.Equals(other);

        if (obj is T value)
            return this.Equals(value);

        if (obj is IOption<T> option)
            return this.Equals(option);

        return base.Equals(obj);
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

    public ValueOption<U> Map<U>(Func<T, U> mapper)
        where U : notnull
        => this.state == OptionState.Some ? new(mapper(this.value!)) : new();

    public U MapOr<U>(Func<T, U> mapper, U defaultValue)
        => this.state == OptionState.Some ? mapper(this.value!) : defaultValue;

    public U MapOr<U>(Func<T, U> mapper, Func<U> defaultValueFactory)
        => this.state == OptionState.Some ? mapper(this.value!) : defaultValueFactory();

    public ValueOption<T> Inspect(Action<T> action)
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

    public ValueOption<T> Xor(ValueOption<T> other)
    {
        if (this.IsSome)
            return other.IsNone ? this : new();

        return other.IsSome ? other : new();
    }

    public ValueOption<(T, U)> Zip<U>(Option<U> other)
        where U : notnull
    {
        if (this.IsNone || other.IsNone)
            return new();

        return new((this.value!, other.Value));
    }
}