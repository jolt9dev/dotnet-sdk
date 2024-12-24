namespace Jolt9;

public static class ErrorsConverter
{
    private static readonly List<IErrorConverter> s_converters = [new DefaultErrorConverter()];

    public static void PrependConverter(string name, Func<Exception, bool> canConvert,  Func<Exception, Error> convert)
    {
        s_converters.Insert(0, new DelegateErrorConverter(name, canConvert, convert));
    }

    public static void PrependConverter(IErrorConverter converter)
    {
        s_converters.Insert(0, converter);
    }

    public static void AppendConverter(string name, Func<Exception, bool> canConvert,  Func<Exception, Error> convert)
    {
        s_converters.Add(new DelegateErrorConverter(name, canConvert, convert));
    }

    public static void AppendConverter(IErrorConverter converter)
    {
        s_converters.Add(converter);
    }

    public static Error Convert(Exception ex)
    {
        foreach (var converter in s_converters)
        {
            if (converter.CanConvert(ex))
                return converter.Convert(ex);
        }

        return new ExceptionError(ex);
    }
}

public interface IErrorConverter
{
    string Name { get; }

    bool CanConvert(Exception ex);

    Error Convert(Exception ex);
}

public class DelegateErrorConverter : IErrorConverter
{
    public string Name { get; }

    private readonly Func<Exception, Error> converter;

    private readonly Func<Exception, bool> canConvert;

    public DelegateErrorConverter(string name, Func<Exception, bool> canConvert, Func<Exception, Error> converter)
    {
        this.Name = name;
        this.canConvert = canConvert;
        this.converter = converter;
    }

    public bool CanConvert(Exception ex)
        => this.canConvert(ex);

    public Error Convert(Exception ex)
        => this.converter(ex);
}

internal sealed class DefaultErrorConverter : IErrorConverter
{
    public string Name => "Default";

    public bool CanConvert(Exception ex)
        => true;

    public Error Convert(Exception ex)
    {
        if (ex is AggregateException aggrEx)
            return new AggregateError(aggrEx);

        if (ex is ArgumentOutOfRangeException rangeEx)
            return new ArgumentOutOfRangeError(rangeEx);

        if (ex is ArgumentNullException argNullEx)
            return new ArgumentNullError(argNullEx);

        if (ex is ArgumentException argEx)
            return new ArgumentError(argEx);

        return new ExceptionError(ex);
    }
}