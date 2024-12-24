using System.Diagnostics;

namespace Jolt9;

#if NETLEGACY
[System.Serializable]
#endif
public class ResultException : System.Exception
{
    public ResultException()
    {
        this.TraceId = Activity.Current?.Id ?? string.Empty;
    }

    public ResultException(string? message)
        : base(message)
    {
        this.TraceId = Activity.Current?.Id ?? string.Empty;
    }

    public ResultException(string? message, System.Exception? inner)
        : base(message, inner)
    {
        this.TraceId = Activity.Current?.Id ?? string.Empty;
    }

    public ResultException(Error error)
        : base($"Result error: {error.Message}")
    {
        this.TraceId = error.TraceId ?? string.Empty;
        this.Target = error.Target;
        this.LineNumber = error.LineNumber;
        this.FilePath = error.FilePath;
    }

    public ResultException(Exception ex)
        : base($"Result error: {ex.Message}", ex)
    {
        this.TraceId = Activity.Current?.Id ?? string.Empty;
    }

#if NETLEGACY
    protected ResultException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context)
        : base(info, context)
    {
        this.TraceId = info.GetString("TraceId") ?? string.Empty;
    }
#endif

    public virtual string TraceId { get; set; }

    public virtual string Target { get; protected set; } = string.Empty;

    public virtual int LineNumber { get; protected set; }

    public virtual string FilePath { get; protected set; } = string.Empty;

    public static ResultException FromUnknown(object? error, string? message = null)
    {
        message ??= "Unknown error: ";
        if (error is Error e)
            return new ResultException(e);

        if (error is Exception ex)
            return new ResultException(ex);

        if (error is IError e2)
            return new ResultException($"{message} {e2.Message}");

        if (error != null)
            return new ResultException($"{message} {error}");

        return new ResultException(message);
    }

    public static ResultException FromError(Error error)
    {
        return new ResultException(error);
    }

    public static ResultException FromException(Exception ex)
    {
        return new ResultException(ex);
    }

    public ResultException TrackCallerInfo(
        [System.Runtime.CompilerServices.CallerLineNumber] int line = 0,
        [System.Runtime.CompilerServices.CallerFilePath] string file = "",
        [System.Runtime.CompilerServices.CallerMemberName] string target = "")
    {
        this.Target = target;
        this.LineNumber = line;
        this.FilePath = file;
        return this;
    }
}