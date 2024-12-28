using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Jolt9;

#pragma warning disable S3400 // Methods should not return constants
public static partial class OperatingSystem
{
#if NETLEGACY
    private static readonly Dictionary<string, OSPlatform> s_platforms = [];
#endif

    public static string EndOfLine { get; } = Environment.NewLine;

#if NETLEGACY
    [SupportedOSPlatformGuard("windows")]
    public static bool IsWindows()
        => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

    [SupportedOSPlatformGuard("linux")]
    public static bool IsLinux()
        => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

    [SupportedOSPlatformGuard("macos")]
    public static bool IsMacOS()
        => RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

    public static bool IsMacCatalyst()
        => RuntimeInformation.IsOSPlatform(GetPlatform("maccataslyst"));

    public static bool IsIOS()
        => RuntimeInformation.IsOSPlatform(GetPlatform("ios"));

    public static bool IsTvOS()
        => false;

    public static bool IsWatchOS()
        => false;

    public static bool IsFreeBSD()
        => RuntimeInformation.IsOSPlatform(GetPlatform("freebsd"));

    private static OSPlatform GetPlatform(string id)
    {
        if (s_platforms.TryGetValue(id, out var platform))
            return platform;

        platform = OSPlatform.Create(id);
        s_platforms.Add(id, platform);
        return platform;
    }

#else
    [SupportedOSPlatformGuard("windows")]
    public static bool IsWindows()
        => System.OperatingSystem.IsWindows();

    [SupportedOSPlatformGuard("linux")]
    public static bool IsLinux()
        => System.OperatingSystem.IsLinux();

    [SupportedOSPlatformGuard("macos")]
    public static bool IsMacOS()
        => System.OperatingSystem.IsMacOS();

    public static bool IsMacCatalyst()
        => System.OperatingSystem.IsMacCatalyst();

    public static bool IsIOS()
        => System.OperatingSystem.IsIOS();

    public static bool IsTvOS()
        => System.OperatingSystem.IsTvOS();

    public static bool IsWatchOS()
        => System.OperatingSystem.IsWatchOS();

    public static bool IsFreeBSD()
        => System.OperatingSystem.IsFreeBSD();

    public static bool IsAndroid()
        => System.OperatingSystem.IsAndroid();

    public static bool IsBrowser()
        => System.OperatingSystem.IsBrowser();

    public static bool IsWasi()
        => System.OperatingSystem.IsWasi();
#endif
}