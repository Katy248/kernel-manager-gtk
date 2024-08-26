using System;
using Adw;
using Gtk;

namespace KernelManagerGtk.Extensions;

public static class WidgetExtensions
{
    public static TWidget WithMarginX<TWidget>(this TWidget w, int margin)
        where TWidget : Gtk.Widget
    {
        w.MarginStart = margin;
        w.MarginEnd = margin;
        return w;
    }

    public static TWidget WithMarginY<TWidget>(this TWidget w, int margin)
        where TWidget : Gtk.Widget
    {
        w.MarginTop = margin;
        w.MarginBottom = margin;
        return w;
    }

    public static TWidget WithMargin<TWidget>(this TWidget w, int margin)
        where TWidget : Gtk.Widget
    {
        WithMarginX(w, margin);
        WithMarginY(w, margin);
        return w;
    }

    /* public static TWidget Child<TWidget>(this TWidget w, Widget child)
        where TWidget : Gtk.Accessible
    {
        w.S;
        return w;
    } */

    public static TWidget ShowHandler<TWidget>(this TWidget w, Action handler)
        where TWidget : Widget
    {
        var th = new Thread(() =>
        {
            handler();
        });

        w.OnShow += (_, _) =>
        {
            th.Start();
        };

        return w;
    }

    public static TWidget MapHandler<TWidget>(this TWidget w, Action handler)
        where TWidget : Widget
    {
        var th = new Thread(() =>
        {
            handler();
        });

        w.OnMap += (_, _) =>
        {
            th.Start();
        };

        return w;
    }
}
