using System;
using System.IO;
using Microsoft.UI.Xaml;
using System.Threading.Tasks;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Glazier;
/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        InitializeComponent();
        Task.Run(() => DirectoryCheck());
    }

    /// <summary>
    /// Invoked when the application is launched normally by the end user.  Other entry points
    /// will be used such as when the application is launched to open a specific file.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        m_window.Activate();
        var manager = WinUIEx.WindowManager.Get(m_window);
        manager.Backdrop = new WinUIEx.MicaSystemBackdrop();

    }

    public static Window m_window = new MainWindow();
    /// <summary>
    /// Creates the cache directories if they don't exist.
    /// </summary>
    private static void DirectoryCheck() {
        String cacheDirectory = Windows.Storage.ApplicationData.Current.LocalFolder.Path;
        if (!Directory.Exists(cacheDirectory + "\\ImageCache\\Backgrounds\\BW")) {
            Directory.CreateDirectory(cacheDirectory + "\\ImageCache\\Backgrounds\\BW");
        }
        if (!Directory.Exists(cacheDirectory + "\\ImageCache\\Backgrounds\\Color"))
        {
            Directory.CreateDirectory(cacheDirectory + "\\ImageCache\\Backgrounds\\Color");
        }
    }
}
