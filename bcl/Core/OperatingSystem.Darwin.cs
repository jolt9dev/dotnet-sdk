using System;
using System.Runtime.InteropServices;

namespace Jolt9;

public static partial class OperatingSystem
{
    internal static Result<OperatingSystemInfo> GetOsRelease()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            var os = new OperatingSystemInfo();
            os.VersionCodename = GetMacOsCodeName();
            os.Id = "macos";
            os.Name = "macOS";
            os.IdLike = "darwin";
            os.VersionId = $"${Environment.Version.Major}.{Environment.Version.Minor}";
            os.Version = $"{os.VersionId} ${os.VersionCodename}";
            os.PrettyName = $"{os.Name} {os.Version} ({os.VersionCodename})";
            os.HomeUrl = "https://www.apple.com/macos";
            return os;
        }

#if NET5_0_OR_GREATER
        if (System.OperatingSystem.IsMacCatalyst())
        {
            var os = new OperatingSystemInfo();
            os.VersionCodename = GetMacOsCodeName();
            os.Id = "maccatalyst";
            os.Name = "Mac Catalyst";
            os.IdLike = "darwin";
            os.VersionId = $"{Environment.Version.Major}.{Environment.Version.Minor}";
            os.Version = os.VersionId;
            os.PrettyName = $"{os.Name} {os.Version}";
            os.HomeUrl = $"https://developer.apple.com/mac-catalyst";
            return os;
        }

        if (OperatingSystem.IsIOS())
        {
            var os = new OperatingSystemInfo();
            os.VersionCodename = GetMacIPhoneCodeName();
            os.Id = "ios";
            os.Name = "iOS";
            os.IdLike = "darwin";
            os.VersionId = $"{Environment.Version.Major}.{Environment.Version.Minor}";
            os.Version = os.VersionId;
            os.PrettyName = $"{os.Name} {os.Version}";
            os.HomeUrl = $"https://www.apple.com/ios";
            return os;
        }

        if (OperatingSystem.IsTvOS())
        {
            var os = new OperatingSystemInfo();
            os.Id = "tvos";
            os.Name = "tvOS";
            os.IdLike = "darwin";
            os.VersionId = $"{Environment.Version.Major}.{Environment.Version.Minor}";
            os.Version = os.VersionId;
            os.PrettyName = $"{os.Name} {os.Version}";
            os.HomeUrl = $"https://www.apple.com/ipados";
            return os;
        }
#endif
        throw new PlatformNotSupportedException("Unknown Darwin platform");
    }

#if NET5_0_OR_GREATER
    private static string GetMacIPhoneCodeName()
    {
        var v = Environment.OSVersion.Version;
        switch (v.Major)
        {
            case 15:
                return "Sky";
            case 14:
                return "Azul";
            case 13:
                return "Yukon";
            case 12:
                return "Hope";
            case 11:
                switch (v.Minor)
                {
                    case 4:
                        return "Fatsa";
                    case 3:
                        return "Emet";
                    case 2:
                        return "Cinar";
                    case 1:
                        return "Bursa";
                    default:
                        return "Tigris";
                }

            case 10:
                switch (v.Minor)
                {
                    case 3:
                        return "Erie";
                    case 2:
                        return "Corry";
                    case 1:
                        return "Butler";
                    default:
                        return "Whitetail";
                }

            case 9:
                switch (v.Minor)
                {
                    case 3:
                        return "Eagle";
                    case 2:
                        return "Castlerock";
                    case 1:
                        return "Boulder";
                    default:
                        return "Monarch";
                }

            case 8:
                switch (v.Minor)
                {
                    case 4:
                        return "Copper";
                    case 3:
                        return "Stowe";
                    case 2:
                        return "OkemoZurs";
                    case 1:
                        return "OkemoTaos";
                    default:
                        return "Okemo";
                }

            case 7:
                switch (v.Minor)
                {
                    case 1:
                        return "Sochi";
                    default:
                        return "Innsbruck";
                }

            default:
                return "Unknown";
        }
    }
#endif

    private static string GetMacOsCodeName()
    {
        var v = Environment.OSVersion.Version;
        switch (v.Major)
        {
            case 14:
                return "Sonoma";
            case 13:
                return "Ventura";
            case 12:
                return "Monterey";
            case 11:
                return "Big Sur";
            case 10:
                switch (v.Minor)
                {
                    case 15:
                        return "Catalina";
                    case 14:
                        return "Mojave";
                    case 13:
                        return "High Sierra";
                    case 12:
                        return "Sierra";
                    case 11:
                        return "El Capitan";
                    case 10:
                        return "Yosemite";
                    case 9:
                        return "Mavericks";
                    case 8:
                        return "Mountain Lion";
                    case 7:
                        return "Lion";
                    case 6:
                        return "Snow Leopard";
                    case 5:
                        return "Leopard";
                    case 4:
                        return "Tiger";
                    case 3:
                        return "Panther";
                    case 2:
                        return "Jaguar";
                    case 1:
                        return "Puma";
                    case 0:
                        return "Cheetah";
                    default:
                        return "Unknown";
                }

            default:
                return "Unknown";
        }
    }
}