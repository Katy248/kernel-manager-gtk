using System;
using Adw;
using Gtk;

namespace KernelManagerGtk.Extensions;

public static class ButtonExtensions
{
    public static Button Content(this Button btn, Action<ButtonContent> action)
    {
        var content = ButtonContent.New();
        btn.SetChild(content);

        action.Invoke(content);

        return btn;
    }

    public static ButtonContent Icon(this ButtonContent c, string iconName)
    {
        c.SetIconName(iconName);
        return c;
    }

    public static ButtonContent Text(this ButtonContent c, string text)
    {
        c.SetLabel(text);
        return c;
    }

    public static Button ClickHandler(this Button b, Action handler)
    {
        var th = new Thread(() =>
        {
            handler();
        });

        b.OnClicked += (_, _) =>
        {
            th.Start();
        };

        return b;
    }
}
