using CommunityToolkit.Mvvm.ComponentModel;

namespace KernelManager;

public class KernelInfo : ObservableObject
{
    private static object _defaultKernelLock = new();
    private static object _setAsDefaultLock = new();
    private static object _installDeleteLock = new();

    private string _arch = "x86_64";

    public string Version { get; set; } = "0.0.0";
    public bool IsDefault => KernelInfo.DefaultKernel == InstallPath;
    public string Arch { get => _arch; set => SetProperty(ref _arch, value); }
    public string InstallPath => $"/boot/vmlinuz-{Version}.{Arch}";
    public string PackageName => $"kernel-lt-{Version}.{Arch}";
    public bool IsInstalled => File.Exists(InstallPath);

    private static string? _defaultKernel;
    public static string DefaultKernel
    {
        get
        {
#if RED_OS
            lock (_defaultKernelLock)
            {
                if (_defaultKernel is null)
                {
                    GetDefaultCommand
                        .Run(_runner)
                        .Success(result =>
                        {
                            Console.WriteLine("Result: {0}", result);
                            _defaultKernel = result.Trim();
                            System.Console.WriteLine("Default kernel: {0}", _defaultKernel);
                        })
                        .Fail(
                            (_, error, _) =>
                            {
                                throw new Exception(
                                    $"Error while executing '{GetDefaultCommand.Command}': '{error}'"
                                );
                            }
                        );
                }
                return _defaultKernel;
            }
#else
            return "kernel-lt-1.1.1.red80.x86_64";
#endif
        }
    }

    public static void DefaultKernelUpdated()
    {
        _defaultKernel = null;
        OnGlobalInfoUpdated?.Invoke();
    }

    private void RaiseInfoUpdated()
    {
        OnInfoUpdated?.Invoke();
    }

    public event Action? OnInfoUpdated;

    public static event Action? OnGlobalInfoUpdated;

    public KernelInfo()
    {
        OnGlobalInfoUpdated += () =>
        {
            RaiseInfoUpdated();
        };
    }

    public static KernelInfo FromVersion(string version)
    {
        return new KernelInfo { Version = version };
    }


    public static KernelInfo FromPackage(string package)
    {
        var info = new KernelInfo();
        info.Arch = package.Split('.').Last();
        System.Console.WriteLine(info.Arch);

        package = package.Replace($".{info.Arch}", "");
        info.Version = package.Replace("kernel-lt-", "");
        System.Console.WriteLine(info.Version);

        return info;
    }

    private static readonly CliRunner2 _runner = new();

    private CliCommand InstallCommand => new CliCommand($"dnf install -y {PackageName}").WithSudo();
    private CliCommand DeleteCommand => new CliCommand($"dnf remove -y {PackageName}").WithSudo();
    private CliCommand SetAsDefaultCommand =>
        new CliCommand($"grubby --set-default={InstallPath}").WithSudo();
    private static CliCommand GetDefaultCommand =>
        new CliCommand($"grubby --default-kernel").WithSudo();

    public CliResult Install()
    {
        if (IsInstalled)
            return CliResult.FromError($"Kernel '{PackageName}' already installed");
        CliResult result;
        lock (_setAsDefaultLock)
        {
            result = InstallCommand.Run(_runner);
        }
        OnInfoUpdated?.Invoke();
        return result;
    }

    public CliResult Delete()
    {
        if (IsInstalled)
            return CliResult.FromError($"Kernel '{PackageName}' isn't installed");
        CliResult result;
        lock (_setAsDefaultLock)
        {
            result = DeleteCommand.Run(_runner);
        }
        OnInfoUpdated?.Invoke();
        return result;
    }

    public CliResult SetAsDefault()
    {
        CliResult result;
        lock (_setAsDefaultLock)
        {
            result = SetAsDefaultCommand
                .Run(_runner)
                .Success(_ =>
                {
                    DefaultKernelUpdated();
                });
        }
        OnInfoUpdated?.Invoke();
        return result;
    }
}
