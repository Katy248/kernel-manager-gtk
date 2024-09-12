using KernelManagerGtk;
using NGettext;

#if RED_OS
Console.WriteLine("Current system RED OS");
#elif ARCH_LINUX
Console.WriteLine("Current system Arch Linux");
#else
Console.WriteLine("Unknown system");
#endif

const string AppId = "ru.katy248.kernel-manager-gtk";

var localeCatalog = new Catalog(AppId, "/usr/share/locale");
var app = Adw.Application.New(AppId, Gio.ApplicationFlags.DefaultFlags);

app.OnActivate += (s, args) =>
{
    var window = new MainWindow(localeCatalog, app);
    app.AddWindow(window);
    window.Present();
};
app.Run(args.Length, args);
