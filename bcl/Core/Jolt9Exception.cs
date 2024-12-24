using System.Diagnostics;

namespace Jolt9;

#if NETLEGACY
[System.Serializable]
#endif
public class Jolt9Exception : System.Exception
{
    public Jolt9Exception()
    {
        this.TraceId = Activity.Current?.Id ?? string.Empty;
    }

    public Jolt9Exception(string? message)
        : base(message)
    {
        this.TraceId = Activity.Current?.Id ?? string.Empty;
    }

    public Jolt9Exception(string? message, System.Exception? inner)
        : base(message, inner)
    {
        this.TraceId = Activity.Current?.Id ?? string.Empty;
    }

#if NETLEGACY
    protected Jolt9Exception(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context)
        : base(info, context)
    {
    }
#endif

    public virtual string Code => "Error";

    public virtual string? TraceId { get; set; }

    public virtual string Target { get; protected set; } = string.Empty;

    public virtual int LineNumber { get; protected set; }

    public virtual string FilePath { get; protected set; } = string.Empty;

    public Jolt9Exception TrackCallerInfo(
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