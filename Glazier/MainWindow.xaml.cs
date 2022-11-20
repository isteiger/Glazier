using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;




// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Glazier;
/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : Window
{

    public MainWindow() {
        this.InitializeComponent();
        this.Title = "Glazier";
        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);
   

    }
    private void NavigationView_SelectionChanged(
    NavigationView sender,
    NavigationViewSelectionChangedEventArgs args)
    {
        SetCurrentNavigationViewItem(args.SelectedItemContainer as NavigationViewItem);
    }
    public void SetCurrentNavigationViewItem(
    NavigationViewItem item)
    {
        if (item == null) {
            return;
        }

        if (item.Tag == null) {
            return;
        }

        if (item.Tag.ToString() == "Settings") {
            ContentFrame.Navigate(Type.GetType("Glazier.Settings"),item.Content);
        } else {
            ContentFrame.Navigate(Type.GetType(item.Tag.ToString()), item.Content);
        }

        NavigationView.Header = item.Content;
        NavigationView.SelectedItem = item;
    }
    private void NavigationView_Loaded( object sender, RoutedEventArgs e) {
        // Navigates, but does not update the Menu.
        // ContentFrame.Navigate(typeof(HomePage));

        SetCurrentNavigationViewItem(GetNavigationViewItems(typeof(Computers)).First());
    }
    public List<NavigationViewItem> GetNavigationViewItems() {
        var result = new List<NavigationViewItem>();
        var items = NavigationView.MenuItems.Select(i => (NavigationViewItem)i).ToList();
        items.AddRange(NavigationView.FooterMenuItems.Select(i => (NavigationViewItem)i));
        result.AddRange(items);

        foreach (var mainItem in items) {
            result.AddRange(mainItem.MenuItems.Select(i => (NavigationViewItem)i));
        }

        return result;
    }

    public List<NavigationViewItem> GetNavigationViewItems(Type type) {
        return GetNavigationViewItems().Where(i => i.Tag.ToString() == type.FullName).ToList();
    }

    public List<NavigationViewItem> GetNavigationViewItems(
        Type type,
        string title)
    {
        return GetNavigationViewItems(type).Where(ni => ni.Content.ToString() == title).ToList();
    }
    public NavigationViewItem GetCurrentNavigationViewItem()
    {
        return NavigationView.SelectedItem as NavigationViewItem;
    }

}
