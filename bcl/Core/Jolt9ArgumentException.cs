using System.Diagnostics;

namespace Jolt9;

#if NETLEGACY
[System.Serializable]
#endif
public class Jolt9ArgumentException : ArgumentException
{
    public Jolt9ArgumentException()
    {
        this.TraceId = Activity.Current?.Id ?? string.Empty;
    }

    public Jolt9ArgumentException(string argumentName, string? message = null, System.Exception? inner = null)
        : base(message ?? $"Argument ${argumentName}", argumentName, inner)
    {
        this.TraceId = Activity.Current?.Id ?? string.Empty;
    }

    #if NETLEGACY
    protected Jolt9ArgumentException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context)
        : base(info, context)
    {
    }
#endif

    public virtual string Code => "ArgumentException";

    public virtual string TraceId { get; set; } = string.Empty;

    public virtual string Target { get; protected set; } = string.Empty;

    public virtual int LineNumber { get; protected set; }

    public virtual string FilePath { get; protected set; } = string.Empty;

    public ArgumentException TrackCallerInfo(
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