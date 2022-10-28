using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WinRT; // required to support Window.As<ICompositionSupportsSystemBackdrop>()


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
        this.InitializeComponent();
    }

    /// <summary>
    /// Invoked when the application is launched normally by the end user.  Other entry points
    /// will be used such as when the application is launched to open a specific file.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        m_window = new MainWindow();
        m_window.Activate();
        var mica = new MicaBackground(m_window);
        mica.TrySetMicaBackdrop();

    }
    public class WindowsSystemDispatcherQueueHelper
    {
        private object? _dispatcherQueueController;

        [StructLayout(LayoutKind.Sequential)]
        internal struct DispatcherQueueOptions
        {
            internal int dwSize;
            internal int threadType;
            internal int apartmentType;
        }

        [DllImport("CoreMessaging.dll")]
        private static extern int CreateDispatcherQueueController([In] DispatcherQueueOptions options, [In, Out, MarshalAs(UnmanagedType.IUnknown)] ref object? dispatcherQueueController);

        public void EnsureWindowsSystemDispatcherQueueController()
        {
            if (Windows.System.DispatcherQueue.GetForCurrentThread() != null)
            {
                // one already exists, so we'll just use it.
                return;
            }

            if (_dispatcherQueueController == null)
            {
                DispatcherQueueOptions options;
                options.dwSize = Marshal.SizeOf(typeof(DispatcherQueueOptions));
                options.threadType = 2;
                options.apartmentType = 2;

                CreateDispatcherQueueController(options, ref _dispatcherQueueController);
            }
        }
    }
    public class MicaBackground
    {
        private readonly Window _window;
        private MicaController _micaController = new();
        private SystemBackdropConfiguration _backdropConfiguration = new();
        private readonly WindowsSystemDispatcherQueueHelper _dispatcherQueueHelper = new();

        public MicaBackground(Window window)
        {
            _window = window;
        }

        public bool TrySetMicaBackdrop()
        {
            if (MicaController.IsSupported())
            {
                _dispatcherQueueHelper.EnsureWindowsSystemDispatcherQueueController();
                _window.Activated += WindowOnActivated;
                _window.Closed += WindowOnClosed;
                _backdropConfiguration.IsInputActive = true;
                _backdropConfiguration.Theme = _window.Content switch
                {
                    FrameworkElement { ActualTheme: ElementTheme.Dark } => SystemBackdropTheme.Dark,
                    FrameworkElement { ActualTheme: ElementTheme.Light } => SystemBackdropTheme.Light,
                    FrameworkElement { ActualTheme: ElementTheme.Default } => SystemBackdropTheme.Default,
                    _ => throw new InvalidOperationException("Unknown theme")
                };

                _micaController.AddSystemBackdropTarget(_window.As<ICompositionSupportsSystemBackdrop>());
                _micaController.SetSystemBackdropConfiguration(_backdropConfiguration);
                return true;
            }

            return false;
        }

        private void WindowOnClosed(object sender, WindowEventArgs args)
        {
            _micaController.Dispose();
            _micaController = null!;
            _window.Activated -= WindowOnActivated;
            _backdropConfiguration = null!;
        }

        private void WindowOnActivated(object sender, WindowActivatedEventArgs args)
        {
            _backdropConfiguration.IsInputActive = args.WindowActivationState is not WindowActivationState.Deactivated;
        }
    }
    private Window m_window;
}
