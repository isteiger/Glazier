using System;
using System.IO;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Newtonsoft.Json.Linq;
using Glazier.Models;
using System.Collections.ObjectModel;
using System.Drawing;
using Microsoft.UI.Xaml.Media.Imaging;
using System.Threading.Tasks;
using System.Drawing.Imaging;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Glazier;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class Computers : Page
{
    public ObservableCollection<Computer> computersList
    {
        get; set;
    }
    public string ComputersJSONFile;
    public Computers()
    {
        this.InitializeComponent();
        computersList = new ObservableCollection<Computer> { };
    }

    private async void AddPC_Click(object sender, RoutedEventArgs e)
    {
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

    private async void OpenFileButton(object sender, RoutedEventArgs e)
    {

        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.m_window); //get main window
        var filePicker = new Windows.Storage.Pickers.FileOpenPicker();
        filePicker.FileTypeFilter.Add(".json");
        filePicker.FileTypeFilter.Add(".glaz");
        WinRT.Interop.InitializeWithWindow.Initialize(filePicker, hwnd);
        var file = await filePicker.PickSingleFileAsync();
        if (file != null)
        {
            // Application now has read/write access to the picked file
            ComputersJSONFile = file.Path;
            var tempfilePicker = new Windows.Storage.Pickers.FileOpenPicker();
            tempfilePicker.FileTypeFilter.Add(".jpg");
            WinRT.Interop.InitializeWithWindow.Initialize(tempfilePicker, hwnd);

            var testfile = await tempfilePicker.PickSingleFileAsync();

            Bitmap defaultBmp = new Bitmap(testfile.Path);
            String json = File.ReadAllText(file.Path);
            JObject o = JObject.Parse(json);
            foreach (JToken puter in o["computers"])
            {
                Computer pc = new Computer
                {
                    DisplayName = (string)puter["displayName"],
                    HostName = (string)puter["hostname"],
                    UserName = (string)puter["username"],
                    Password = (string)puter["password"],
                    MacAddress = (string)puter["mac"],
                    Online = false,
                    IconColor = "Gray",
                    Background = BitmapToBitmapImage(defaultBmp)
                };
                /*System.Diagnostics.Debug.WriteLine(puter["displayName"]);
                Ping ping = new Ping();
                PingReply pingReply = ping.Send(pc.HostName);

                if (pingReply.Status == IPStatus.Success) {
                    pc.Online = true;
                } */
                Boolean isOnline = true;
                if (isOnline == true)
                {
                    pc.Online = true;
                    pc.IconColor = "LightGreen";
                } else {
                    var bmp = MakeGrayscale3(defaultBmp);
                    pc.Background = BitmapToBitmapImage(bmp);
                }
                computersList.Add(pc);
            };
            StartPanel.Visibility = Visibility.Collapsed;

        }

    }
    private BitmapImage BitmapToBitmapImage(Bitmap bmp)
    {
        BitmapImage bitmapImage = new BitmapImage();
        using (MemoryStream stream = new MemoryStream())
        {
            bmp.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            stream.Position = 0;
            bitmapImage.SetSource(stream.AsRandomAccessStream());
        }
        return bitmapImage;
    }
    public static Bitmap MakeGrayscale3(Bitmap original)
    {
        //create a blank bitmap the same size as original
        Bitmap newBitmap = new Bitmap(original.Width, original.Height);

        //get a graphics object from the new image
        using (Graphics g = Graphics.FromImage(newBitmap))
        {

            //create the grayscale ColorMatrix
            ColorMatrix colorMatrix = new ColorMatrix(
               new float[][]
               {
             new float[] {.3f, .3f, .3f, 0, 0},
             new float[] {.59f, .59f, .59f, 0, 0},
             new float[] {.11f, .11f, .11f, 0, 0},
             new float[] {0, 0, 0, 1, 0},
             new float[] {0, 0, 0, 0, 1}
               });

            //create some image attributes
            using (ImageAttributes attributes = new ImageAttributes())
            {

                //set the color matrix attribute
                attributes.SetColorMatrix(colorMatrix);

                //draw the original image on the new image
                //using the grayscale color matrix
                g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height),
                            0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);
            }
        }
        return newBitmap;
    }
}
