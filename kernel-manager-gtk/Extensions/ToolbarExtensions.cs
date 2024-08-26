using System;
using Adw;

namespace KernelManagerGtk.Extensions;

public static class ToolbarExtensions
{
    public static HeaderBar Title(this HeaderBar header, Action<WindowTitle> action)
    {
        var title = new WindowTitle();

        action.Invoke(title);

        header.SetTitleWidget(title);

        return header;
    }

    public static WindowTitle Title(this WindowTitle title, string value)
    {
        title.SetTitle(value);
        return title;
    }

    public static WindowTitle Subtitle(this WindowTitle title, string value)
    {
        title.SetSubtitle(value);
        return title;
    }
}
