namespace Jolt9;

public static class FileSystem
{
    public static string ReadTextFile(string path)
        => File.ReadAllText(path);
}