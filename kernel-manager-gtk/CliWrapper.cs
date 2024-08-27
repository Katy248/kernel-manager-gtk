using System.Diagnostics;

namespace KernelManagerGtk;

public static class CliWrapper
{
    /// <exception cref="Exception"></exception>
    public static async Task<string[]> RunAsync(
        string command,
        bool asSuperuser = false,
        CancellationToken ct = default
    )
    {
        if (asSuperuser)
        {
            System.Console.WriteLine("Command should be run as root");
            command = await SudoCommand() + command;
        }
        else
        {
            Console.WriteLine("Command shouldn't be run as root");
        }

        var baseCommand = command.Split(' ').First();
        var args = command.Replace(baseCommand, "");

        Console.WriteLine("Coomand to run: {0}", command);

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
            throw new Exception(await result.StandardError.ReadToEndAsync(ct));
        }

        var output = await result.StandardOutput.ReadToEndAsync(ct);
        var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        return lines;
    }

    public static string[] Run(string command, bool asSuperuser = false)
    {
        return RunAsync(command, asSuperuser, CancellationToken.None).GetAwaiter().GetResult();
    }

    static async Task<string> SudoCommand()
    {
        var user = (await RunAsync("id -un", false)).First();
        if (user == "root")
        {
            System.Console.WriteLine("Current user is root, no need for additional auth");
            return "";
        }
        else
        {
            return "pkexec ";
        }
    }
}
