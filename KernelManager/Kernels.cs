using System.Reactive.Disposables;
using KernelManager;

namespace KernelManager;

public class Kernels : IObservable<KernelInfo>
{
    private IEnumerable<string> GetAvailablePackages()
    {
    #if RED_OS
        var lines = CliWrapper
            .Run("rpm -qa kernel-lt");
        return lines;
    #else
        return ["kernel-lt-1.1.1.red80.x86_64"];
    #endif
    }

    public IDisposable Subscribe(IObserver<KernelInfo> o)
    {
        foreach (var package in GetAvailablePackages())
        {
            System.Console.WriteLine(package);
            o.OnNext(KernelInfo.FromPackage(package));
        }
        o.OnCompleted();
        return Disposable.Empty;
    }
}
