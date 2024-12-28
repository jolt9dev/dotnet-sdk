namespace Jolt9;

public static partial class FileSystem
{
#if NET8_0_OR_GREATER
    public static void MakeDir(string path, UnixFileMode mode)
        => Directory.CreateDirectory(path, mode);

    public static ValueResult MakeDirSafe(string path, UnixFileMode mode)
    {
        try
        {
            MakeDir(path, mode);
            return Results.OkValue();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
#endif
}