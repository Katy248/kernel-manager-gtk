using CommunityToolkit.Mvvm.ComponentModel;

namespace KernelManager;

public class ObservableValue<T> : ObservableObject
{
    private T _value;
    public static implicit operator T(ObservableValue<T> a)
    {
        return a.Value;
    }
    public T Value
    {
        get => _value;
        set => SetProperty(ref _value, value);
    }
}
