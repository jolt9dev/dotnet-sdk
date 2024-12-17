
using System.Diagnostics;

namespace Jolt9.Options;

#if NETLEGACY
[System.Serializable]
#endif
public class OptionException : System.Exception
{
    public virtual string TraceId  { get; set;}

    public virtual string Target { get; protected set; } = string.Empty;

    public virtual int LineNumber { get; protected set; }

    public virtual string FilePath { get; protected set; } = string.Empty;

    public OptionException() 
    {
        this.TraceId = Activity.Current?.Id ?? string.Empty;
    }

    public OptionException(string? message)
        : base(message)
    { 
        this.TraceId = Activity.Current?.Id ?? string.Empty;
    }

    public OptionException(string? message, System.Exception? inner) 
        : base(message, inner)
    {
        this.TraceId = Activity.Current?.Id ?? string.Empty;
    }

    public OptionException TrackCallerInfo(
        [System.Runtime.CompilerServices.CallerLineNumber] int line = 0,
        [System.Runtime.CompilerServices.CallerFilePath] string file = "",
        [System.Runtime.CompilerServices.CallerMemberName] string target = "")
    {
        this.Target = target;
        this.LineNumber = line;
        this.FilePath = file;
        return this;
    }

#if NETLEGACY
    protected OptionException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) 
        : base(info, context) 
    {
        this.TraceId = info.GetString("TraceId") ?? string.Empty;
    }
#endif
}