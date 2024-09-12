using Gtk;
using CommunityToolkit.Mvvm.ComponentModel;

namespace KernelManagerGtk.Extensions;

public static class BindExtensions
{
    public static T BindOneWay<T>(this T widget, ObservableObject obj) where T : Widget
    {
        obj.PropertyChanged += (sender, args) => {
            
        };

        return widget;
    }
}
