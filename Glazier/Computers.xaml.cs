using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers;
using Windows.Storage;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Glazier;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class Computers : Page {

    public Computers() {
        this.InitializeComponent();
    }

    private async void AddPC_Click(object sender, RoutedEventArgs e) {
        ContentDialog dialog = new ContentDialog();

        // XamlRoot must be set in the case of a ContentDialog running in a Desktop app
        dialog.XamlRoot = this.XamlRoot;
        dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
        dialog.Title = "Add a Computer";
        dialog.PrimaryButtonText = "Add";
        dialog.CloseButtonText = "Cancel";
        dialog.DefaultButton = ContentDialogButton.Primary;
        dialog.Content = new AddPCDialog();

        var result = await dialog.ShowAsync();
    }

    private async void OpenFileButton(object sender, RoutedEventArgs e) {
        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.m_window); //get main window
        var filePicker = new Windows.Storage.Pickers.FileOpenPicker();
        filePicker.FileTypeFilter.Add(".json");
        filePicker.FileTypeFilter.Add(".glaz");
        WinRT.Interop.InitializeWithWindow.Initialize(filePicker, hwnd);
        var file = await filePicker.PickSingleFileAsync();

        if (file != null) {
            // Application now has read/write access to the picked file
            fileNameText.Text = "Opened File: " + file.Name;
            String json = File.ReadAllText(file.Path);
            JObject o = JObject.Parse(json);
            foreach (JToken puter in o["computers"]){
                System.Diagnostics.Debug.WriteLine(puter["displayName"]);
            }
        }
        else {
            fileNameText.Text = "Operation cancelled.";
        }

    }

}
