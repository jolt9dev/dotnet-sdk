using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Jolt9;

public class Error : IError
{
    public Error(string? message = null, IInnerError? innerError = null)
    {
        this.Message = message ?? "Unknown error";
        this.InnerError = innerError;
        this.TraceId = Activity.Current?.Id;
        this.Code = "Error";
    }

    [JsonPropertyName("message")]
    public string Message { get; set; }

    [JsonPropertyName("code")]
    public string Code { get; set; }

    [JsonPropertyName("traceId")]
    public string? TraceId { get; set; }

    [JsonIgnore]
    public virtual string Target { get; protected set; } = string.Empty;

    [JsonIgnore]
    public virtual int LineNumber { get; protected set; }

    [JsonIgnore]
    public virtual string FilePath { get; protected set; } = string.Empty;

    [JsonIgnore]
    public virtual string? StackTrace { get; protected set; }

    [JsonPropertyName("innerError")]
    public IInnerError? InnerError { get; set; }

    protected Exception? Exception { get; set; }

    public static implicit operator Error(Exception ex)
        => ErrorsConverter.Convert(ex);

    public Error TrackCallerInfo(
        [CallerLineNumber] int line = 0,
        [CallerFilePath] string file = "",
        [CallerMemberName] string target = "")
    {
        this.Target = target;
        this.LineNumber = line;
        this.FilePath = file;
        return this;
    }

    public bool Is(string code)
    {
        return this.Code == code;
    }

    public bool Is(Type type)
    {
        return type.IsInstanceOfType(this);
    }

    public bool Is<T>()
    {
        return this.Is(typeof(T));
    }

    public virtual Exception ToException()
    {
        this.Exception ??= new InvalidOperationException(this.Message);

        return this.Exception;
    }

    public override string ToString()
    {
        return this.Exception?.ToString() ?? this.Message;
    }
}