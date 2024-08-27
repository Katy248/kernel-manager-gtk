namespace KernelManagerGtk;

public class KernelInfo
{
    static KernelInfo()
    {
        // _defaultKernel = CliWrapper
        // .Run("grubby --default-kernel", true)
        // .First()
        // .Trim();
    }
    private static object _defaultKernelLock = new();

    public string Version { get; set; } = "0.0.0";
    public bool IsDefault =>
      KernelInfo.DefaultKernel == InstallPath;
    public string Arch { get; set; } = "x86_64";
    public string InstallPath => $"/boot/vmlinuz-{Version}.{Arch}";
    public string PackageName => $"kernel-lt-{Version}.{Arch}";
    public bool IsInstalled => File.Exists(InstallPath);


    private static string? _defaultKernel;
    public static string DefaultKernel
    {
        get
        {
            lock (_defaultKernelLock)
            {
                _defaultKernel ??= CliWrapper.Run("grubby --default-kernel", true).First().Trim();
                return _defaultKernel;
            }
        }
    }

    public static void DefaultKernelUpdated()
    {
        _defaultKernel = null;
    }

    public static KernelInfo FromVersion(string version)
    {
        return new KernelInfo { Version = version };
    }
}

