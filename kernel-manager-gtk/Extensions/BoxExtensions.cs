using System;
using Gtk;

namespace KernelManagerGtk.Extensions;

public static class BoxExtensions
{
    public static Box Vertical(this Box b)
    {
        b.SetOrientation(Orientation.Vertical);
        return b;
    }

    public static Box Horizontal(this Box b)
    {
        b.SetOrientation(Orientation.Horizontal);
        return b;
    }

    public static Box Spacing(this Box b, int spacing)
    {
        b.SetSpacing(spacing);
        return b;
    }
}
