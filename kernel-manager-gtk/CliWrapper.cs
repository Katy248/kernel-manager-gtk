using System;
using System.Diagnostics;

namespace KernelManagerGtk;

public static class CliWrapper
{
  /// <exception cref="Exception"></exception>
  public static string[] Run(string command, string args)
  {
    System.Console.WriteLine("Coomand to run: {0} {1}", command, args);
    var info = new ProcessStartInfo
    {
      FileName = command,
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
    var lines =
        result?.StandardOutput.ReadToEnd().Split('\n', StringSplitOptions.RemoveEmptyEntries)
        ?? [];
    return lines;
  }
}
