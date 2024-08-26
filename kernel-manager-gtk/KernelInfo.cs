
namespace KernelManagerGtk;

public class KernelInfo
{
  public string Version { get; set; } = "0.0.0";
  public bool IsDefault =>
      CliWrapper.Run("grubby", "--default-kernel").First().Trim() == InstallPath;
  public string Arch { get; set; } = "x86_64";
  public string InstallPath => $"/boot/vmlinuz-{Version}.{Arch}";
  public string PackageName => $"kernel-lt-{Version}.{Arch}";
  public bool IsInstalled => File.Exists(InstallPath);

  public static KernelInfo FromVersion(string version)
  {
    return new KernelInfo { Version = version };
  }
}

