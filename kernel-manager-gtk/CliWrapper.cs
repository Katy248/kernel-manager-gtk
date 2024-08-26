using System.Diagnostics;

namespace KernelManagerGtk;

public static class CliWrapper
{
    /// <exception cref="Exception"></exception>
    public static string[] Run(string command, bool asSuperuser = false)
    {
        if (asSuperuser)
        {
            System.Console.WriteLine("Command should be run as root");
            command = SudoCommand() + command;
        }
        else
        {
            System.Console.WriteLine("Command shouldn't be run as root");
        }

        var baseCommand = command.Split(' ').First();
        var args = command.Replace(baseCommand, "");

        System.Console.WriteLine("Coomand to run: {0}", command);

        var info = new ProcessStartInfo
        {
            FileName = baseCommand,
            Arguments = args,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
        };
        var result = Process.Start(info)!;
        result.WaitForExit();
        if (result.ExitCode != 0)
        {
            throw new Exception(result.StandardError.ReadToEnd());
        }
        var lines = result.StandardOutput
            .ReadToEnd()
            .Split('\n', StringSplitOptions.RemoveEmptyEntries);
        return lines;
    }

    static string SudoCommand()
    {
        var user = Run("id -un", false).First();
        if (user == "root")
        {
            System.Console.WriteLine("Current user is root, no need for additional auth");
            return "";
        }
        else { return "pkexec "; }
    }
}
