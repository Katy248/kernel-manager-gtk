using System.Reactive.Disposables;

namespace KernelManagerGtk;

public class Kernels : IObservable<KernelInfo>
{
    private IEnumerable<string> GetAvailablePackages()
    {
        var lines = CliWrapper
            .Run("rpm -qa kernel-lt");
        return lines;
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
