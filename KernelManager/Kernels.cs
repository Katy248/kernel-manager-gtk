using System.Reactive.Disposables;

namespace KernelManager;

public class Kernels : IObservable<KernelInfo>
{
    private IEnumerable<string> GetAvailableVersions()
    {
        var lines = CliWrapper
            .Run("dnf list --showduplicates --all -q kernel-lt.x86_64")
            .Where(s => s.StartsWith("kernel-lt"));
        var versions = lines
            .Select(l =>
                l.Split(
                    ' ',
                    StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries
                )[1]
            )
            .Distinct();
        return versions;
    }

    public IDisposable Subscribe(IObserver<KernelInfo> o)
    {
        foreach (var ver in GetAvailableVersions())
        {
            o.OnNext(KernelInfo.FromVersion(ver));
        }
        o.OnCompleted();
        return Disposable.Empty;
    }
}
