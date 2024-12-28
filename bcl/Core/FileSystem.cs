using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Jolt9;

public static partial class FileSystem
{
    public static void CopyFile(string source, string destination, bool overwrite = false)
        => File.Copy(source, destination, overwrite);

    public static ValueResult CopyFileSafe(string source, string destination, bool overwrite = false)
    {
        try
        {
            CopyFile(source, destination, overwrite);
            return Result.Ok();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public static void Copy(string source, string destination, bool overwrite = false)
    {
        var fi = new FileInfo(source);
        if (!fi.Exists)
            throw new FileNotFoundException("Source file or directory does not exist.", source);

        if (fi.Attributes.HasFlag(FileAttributes.Directory))
        {
            if (!Directory.Exists(destination))
                Directory.CreateDirectory(destination);

            foreach (var file in Directory.EnumerateFiles(source))
            {
                var name = Path.GetFileName(file);
                var dest = Path.Combine(destination, name);
                File.Copy(file, dest, overwrite);
            }

            foreach (var dir in Directory.EnumerateDirectories(source))
            {
                var name = Path.GetFileName(dir);
                var dest = Path.Combine(destination, name);
                Copy(dir, dest, overwrite);
            }
        }
        else
        {
            fi.CopyTo(destination, overwrite);
        }
    }

    public static ValueResult CopySafe(string source, string destination, bool overwrite = false)
    {
        try
        {
            var fi = new FileInfo(source);
            if (!fi.Exists)
                return new FileNotFoundException("Source file or directory does not exist.", source);

            if (fi.Attributes.HasFlag(FileAttributes.Directory))
            {
                if (!Directory.Exists(destination))
                {
                    var res = MakeDirSafe(destination);
                    if (res.IsError)
                        return res;
                }

                foreach (var file in Directory.EnumerateFiles(source))
                {
                    var name = Path.GetFileName(file);
                    var dest = Path.Combine(destination, name);
                    var res = CopyFileSafe(file, dest, overwrite);
                    if (res.IsError)
                        return res;
                }

                foreach (var dir in Directory.EnumerateDirectories(source))
                {
                    var name = Path.GetFileName(dir);
                    var dest = Path.Combine(destination, name);
                    var res = CopySafe(dir, dest, overwrite);
                    if (res.IsError)
                        return res;
                }
            }
            else
            {
                fi.CopyTo(destination, overwrite);
            }

            return Result.Ok();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public static bool IsFile(string path)
        => File.Exists(path);

    public static bool IsDir(string path)
        => Directory.Exists(path);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void MakeDir(string path)
        => Directory.CreateDirectory(path);

    public static ValueResult MakeDirSafe(string path)
    {
        try
        {
            MakeDir(path);
            return Result.Ok();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public static void MoveFile(string source, string destination)
        => File.Move(source, destination);

    public static ValueResult MoveFileSafe(string source, string destination)
    {
        try
        {
            MoveFile(source, destination);
            return Result.Ok();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] ReadFile(string path)
        => File.ReadAllBytes(path);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string[] ReadFileLines(string path)
        => File.ReadAllLines(path);

    public static ValueResult<string[]> ReadFileLinesSafe(string path)
    {
        try
        {
            return File.ReadAllLines(path);
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public static ValueResult<byte[]> ReadFileSafe(string path)
    {
        try
        {
            return ReadFile(path);
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ReadTextFile(string path)
        => File.ReadAllText(path);

    public static ValueResult<string> ReadTextFileSafe(string path)
    {
        try
        {
            return ReadTextFile(path);
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public static void Rename(string source, string destination)
    {
        if (File.GetAttributes(source).HasFlag(FileAttributes.Directory))
            Directory.Move(source, destination);
        else
            File.Move(source, destination);
    }

    public static ValueResult RenameSafe(string source, string destination)
    {
        try
        {
            Rename(source, destination);
            return Results.OkValue();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RenameFile(string source, string destination)
        => File.Move(source, destination);

    public static ValueResult RenameFileSafe(string source, string destination)
    {
        try
        {
            RenameFile(source, destination);
            return Result.Ok();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RenameDir(string source, string destination)
        => Directory.Move(source, destination);

    public static ValueResult RenameDirSafe(string source, string destination)
    {
        try
        {
            Directory.Move(source, destination);
            return Result.Ok();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public static void Remove(string path, bool recursive = false)
    {
        if (IsDir(path))
            Directory.Delete(path, recursive);
        else
            File.Delete(path);
    }

    public static void RemoveFile(string path)
        => File.Delete(path);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteFile(string path, byte[] content, bool append = false)
        => File.WriteAllBytes(path, content);

    public static ValueResult WriteFileSafe(string path, byte[] content)
    {
        try
        {
            File.WriteAllBytes(path, content);
            return Result.Ok();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public static void WriteFileLines(string path, string[] content, bool append = false)
    {
        if (append)
            File.AppendAllLines(path, content);
        else
            File.WriteAllLines(path, content);
    }

    public static ValueResult WriteFileLinesSafe(string path, string[] content, bool append = false)
    {
        try
        {
            if (append)
                File.AppendAllLines(path, content);
            else
                File.WriteAllLines(path, content);

            return Result.Ok();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteTextFile(string path, string content, bool append = false, Encoding? encoding = null)
    {
        if (encoding == null)
        {
            if (append)
                File.AppendAllText(path, content);
            else
                File.WriteAllText(path, content);

            return;
        }

        if (append)
            File.AppendAllText(path, content, encoding);
        else
            File.WriteAllText(path, content, encoding);
    }

    public static ValueResult WriteTextFileSafe(string path, string content, bool append = false, Encoding? encoding = null)
    {
        try
        {
            if (encoding == null)
            {
                if (append)
                    File.AppendAllText(path, content);
                else
                    File.WriteAllText(path, content);

                return Result.Ok();
            }

            if (append)
                File.AppendAllText(path, content, encoding);
            else
                File.WriteAllText(path, content, encoding);

            return Result.Ok();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
}