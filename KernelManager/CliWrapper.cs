using System.Diagnostics;

namespace KernelManager;

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

    /// <exception cref="Exception"></exception>
    public static string[] Run(string command, bool asSuperuser = false)
    {
        return RunAsync(command, asSuperuser, CancellationToken.None).GetAwaiter().GetResult();
    }

    static async Task<string> SudoCommand()
    {
        var user = Environment.GetEnvironmentVariable("USER");
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

public class CliCommand
{
    public CliCommand(string command)
    {
        Command = command;
    }

    public string Command { get; set; }
    public bool AsSudo { get; set; }

    public CliCommand WithSudo(bool val = true)
    {
        AsSudo = val;
        return this;
    }

    private string BuildCommand()
    {
        if (AsSudo)
        {
            var user = Environment.GetEnvironmentVariable("USER");
            if (user != "root")
                return "pkexec " + Command;
        }
        return Command;
    }

    public CliResult Run(CliRunner2 runner)
    {
        return runner.Run(BuildCommand());
    }
}

public class CliRunner2
{
    private object _cliLock = new();

    public CliResult Run(string command)
    {
        lock (_cliLock)
        {
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

            return CliResult.FromProcess(result);
        }
    }
}

public class CliResult
{
    public static CliResult FromProcess(Process process)
    {
        return new CliResult
        {
            _exitCode = process.ExitCode,
            _output = process.StandardOutput.ReadToEnd(),
            _errors = process.StandardError.ReadToEnd(),
        };
    }

    public static CliResult FromError(string error, int exitCode = -1)
    {
        return new CliResult { _exitCode = exitCode, _errors = error, };
    }

    private int _exitCode;
    private string? _output;
    private string? _errors;

    private CliResult() { }

    public CliResult Success(Action<string> action)
    {
        if (_exitCode == 0)
        {
            action.Invoke(_output ?? "");
        }
        return this;
    }

    public CliResult Fail(Action<string, string, int> action)
    {
        if (_exitCode != 0)
        {
            action.Invoke(_output ?? "", _errors ?? "", _exitCode);
        }
        return this;
    }
}
