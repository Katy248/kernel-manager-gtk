using Adw;
using Gtk;
using KernelManagerGtk.Extensions;
using NGettext;

namespace KernelManagerGtk;

public class KernelView : Gtk.Box
{
    private readonly KernelInfo _kernel;
    private readonly MainWindow _window;
    private readonly ICatalog _l;

    public KernelView(KernelInfo kernel, MainWindow window, ICatalog localeCatalog)
    : base()
    {
        _kernel = kernel;
        _window = window;
        _l = localeCatalog;
        this.Spacing(12).Vertical();

        this.MapHandler(ActivateHandler);
        _preferencesGroup = new();
        _setAsDefaultButton = Button.New();

        Append(_preferencesGroup);
        Append(_setAsDefaultButton);

    }

    private readonly Adw.PreferencesGroup _preferencesGroup;
    private readonly Button _setAsDefaultButton;

    private void ActivateHandler()
    {
        //var defKern = CliWrapper.Run("grubby", "--default-kernel").First();
        //var realKern = _kernel.InstallPath;

        // Console.WriteLine(
        // $"Default kernel: {defKern}, current item: {realKern}, is this item default: {_kernel.IsDefault}"
        // );

        _preferencesGroup.SetTitle(_kernel.Version + (_kernel.IsDefault ? " " + _l.GetString("(default)") : ""));
        if (_kernel.IsInstalled)
        {
            var row = Adw.ActionRow.New();
            row.AddCssClass("property");
            row.SetTitle(_l.GetString("Location"));
            row.SetSubtitle(_kernel.InstallPath);
            _preferencesGroup.Add(PropertyRow(_l.GetString("Location"), _kernel.InstallPath));
        }
        _preferencesGroup.Add(PropertyRow(_l.GetString("Package"), _kernel.PackageName));

        _setAsDefaultButton
            .ClickHandler(SetAsDefault)
              .Content(c =>
              {
                  c.SetLabel(_l.GetString("Set as default"));
                  c.SetIconName("object-select-symbolic");
              }).AddCssClass("pill");

        Update();
    }

    private ActionRow PropertyRow(string title, string property)
    {
        var row = new ActionRow();
        row.SetTitle(title);
        row.SetSubtitle(property);
        row.SetSubtitleSelectable(true);
        row.AddCssClass("property");

        return row;
    }

    public void Update()
    {

        if (_kernel.IsInstalled && !_kernel.IsDefault)
        {
            var suffix = Button
                .New()
                .Content(c => c.Icon("user-trash-symbolic").Text(_l.GetString("Delete")))
                .ClickHandler(Delete);
            suffix.AddCssClass("destructive-action");
            _preferencesGroup.SetHeaderSuffix(suffix);
        }
        else if (!_kernel.IsDefault)
        {
            var suffix = Button
                .New()
                .ClickHandler(Install)
                .Content(c => c.Icon("browser-download-symbolic").Text(_l.GetString("Install")));
            _preferencesGroup.SetHeaderSuffix(suffix);
        }
        _setAsDefaultButton.SetVisible(!_kernel.IsDefault && _kernel.IsInstalled);
    }

    private void SetAsDefault()
    {
        CliWrapper.Run($"grubby --set-default={_kernel.InstallPath}", true);
        Update();
    }

    private void Install()
    {
        CliWrapper.Run($"dnf install -y {_kernel.PackageName}", true);
        Update();
    }

    private void Delete()
    {
        CliWrapper.Run($"dnf remove -y {_kernel.PackageName}", true);
        Update();
    }
}
