using System.Reactive.Linq;
using Gtk;
using KernelManagerGtk.Extensions;
using NGettext;

namespace KernelManagerGtk;

public static class Actions
{
    public const string KeyboardShortcuts = "ks";
    public const string Help = "help";
    public const string About = "about";
}

public class MainWindow : Adw.ApplicationWindow
{
    private readonly IObservable<KernelInfo> _kernels = new Kernels();

    public MainWindow(ICatalog localeCatalog, Adw.Application app)
    {
        Application = app;
        Console.WriteLine("MainWindow ctor");
        _mainContent = new Box().Vertical().Spacing(30).WithMargin(12);
        _l = localeCatalog;

        var clamp = Adw.Clamp.New();
        clamp.SetChild(_mainContent);

        var scroll = ScrolledWindow.New();
        scroll.SetChild(clamp);

        this.Application.SetAccelsForAction(Actions.KeyboardShortcuts, ["<Ctrl>question"]);
        // this.Application.SetAccelsForAction(Actions.Help, ["F1"]);
        //
        _helpAction = Gio.SimpleAction.New(Actions.Help, null);
        _helpAction.SetEnabled(true);
        _helpAction.OnActivate += (sender, e) => Console.WriteLine("Fuck you asshole");
        this.Application.AddAction(_helpAction);
        this.Application.SetAccelsForAction(Actions.Help, ["F1"]);

        var menuModel = Gio.Menu.New();
        menuModel.AppendItem(
            Gio.MenuItem.New(_l.GetString("Keyboard shortcuts"), Actions.KeyboardShortcuts)
        );
        menuModel.AppendItem(Gio.MenuItem.New(_l.GetString("Help"), Actions.Help));
        menuModel.AppendItem(Gio.MenuItem.New(_l.GetString("About kernel manager"), Actions.About));
        var menu = Gtk.MenuButton.New();
        menu.SetIconName("open-menu");
        menu.SetMenuModel(menuModel);
        menu.SetPrimary(false);
        // menu.SetActive(true);

        var header = Adw
            .HeaderBar.New()
            .Title(t =>
                t.Title(_l.GetString("Kernel manager")).Subtitle(_l.GetString("by katy248"))
            );
        header.PackEnd(menu);

        var view = Adw.ToolbarView.New();
        view.AddTopBar(header);
        view.SetContent(scroll);
        _toastOverlay = new Adw.ToastOverlay();
        _toastOverlay.SetChild(view);
        Content = _toastOverlay;

        this.WidthRequest = 400;
        this.HeightRequest = 600;
        this.ShowHandler(ActivateHandler);
    }

    private readonly Gio.SimpleAction _helpAction;
    private readonly Box _mainContent;
    private readonly Adw.ToastOverlay _toastOverlay;
    private readonly ICatalog _l;

    public void Notify(string message)
    {
        var toast = Adw.Toast.New(message);
        toast.SetPriority(Adw.ToastPriority.High);
        _toastOverlay.AddToast(toast);
    }

    private void ActivateHandler()
    {
        using var kernels = _kernels.Subscribe(k =>
        {
            _mainContent.Append(new KernelView(k, this, _l));
        });
    }
}
