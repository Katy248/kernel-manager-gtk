using System.Reactive.Linq;
using Gtk;
using KernelManagerGtk.Extensions;
using NGettext;

namespace KernelManagerGtk;

public class MainWindow : Adw.ApplicationWindow
{
  private readonly IObservable<KernelInfo> _kernels = new Kernels();

  public MainWindow(ICatalog localeCatalog)
  {
    Console.WriteLine("MainWindow ctor");
    _mainContent = new Box().Vertical().Spacing(30).WithMargin(12);
    _l = localeCatalog;

    var clamp = Adw.Clamp.New();
    clamp.SetChild(_mainContent);

    var scroll = ScrolledWindow.New();
    scroll.SetChild(clamp);

    var view = Adw.ToolbarView.New();
    view.AddTopBar(Adw
        .HeaderBar.New()
        .Title(t => t.Title(_l.GetString("Kernel manager")).Subtitle(_l.GetString("by katy248"))));
    view.SetContent(scroll);
    Content = view;

    this.WidthRequest = 400;
    this.HeightRequest = 600;
    this.ShowHandler(ActivateHandler);
  }

  private readonly Box _mainContent;
  private readonly ICatalog _l;

  private void ActivateHandler()
  {
    using var kernels = _kernels.Subscribe(k =>
    {
      _mainContent.Append(new KernelView(k, this, _l));
    });
  }
}
