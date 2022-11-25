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
using System.Net.NetworkInformation;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.Storage;
using Newtonsoft.Json;
using Glazier.Modules;
using System.Security.Cryptography;
using System.Globalization;
using System.Net.Sockets;
using System.Net;


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
        var dialog = new ContentDialog();

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
        if (file != null) {
            loadingBar.Visibility = Visibility.Visible;
            // Application now has read/write access to the picked file
            ComputersJSONFile = file.Path;
            var defaultBmp = new Bitmap("C:\\Windows\\Web\\Wallpaper\\Windows\\img0.jpg");
            var json = File.ReadAllText(file.Path);
            var o = JObject.Parse(json);
            void OnlineCheck(Computer pc, Bitmap defaultBmp) {
                Microsoft.UI.Dispatching.DispatcherQueue dispatcherQueue = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();

                // use ping to check if online. if ping fails, it errors out which is why this is in a try catch
                try
                {
                    Ping ping = new Ping();
                    PingReply pingReply = ping.Send(pc.HostName);
                    
                    // if succeed, set online
                    if (pingReply.Status == IPStatus.Success) {
                        pc.Online = true;
                    }
                } catch{
                    // errored out, so must be offline
                    pc.Online = false;
                }
                
                // set parameters for online & offline 
                if (pc.Online) {
                    System.Diagnostics.Debug.WriteLine("pc online");
                    var imgPath = Windows.Storage.ApplicationData.Current.LocalFolder.Path + "\\ImageCache\\Backgrounds\\Color\\" + pc.Uuid + ".jpg";
                    if (File.Exists(imgPath)) {
                        dispatcherQueue.TryEnqueue(() =>
                        {
                            pc.Background = new BitmapImage(new Uri(imgPath));
                        });
                    } else {
                        dispatcherQueue.TryEnqueue(() =>
                        {
                            pc.Background = new BitmapImage(new Uri("C:\\Windows\\Web\\Wallpaper\\Windows\\img0.jpg"));
                        });
                    }
                    dispatcherQueue.TryEnqueue(() =>
                    {
                        pc.IconColor = "LightGreen";
                    });
                } else {
                    System.Diagnostics.Debug.WriteLine("pc offline");

                    var imgPath = Windows.Storage.ApplicationData.Current.LocalFolder.Path + "\\ImageCache\\Backgrounds\\BW\\" + pc.Uuid + ".jpg";
                    if (File.Exists(imgPath)) {
                        pc.Background = new BitmapImage(new Uri(imgPath));
                    } else {
                        var bmp = MakeGrayscale3(defaultBmp);
                        pc.Background = BitmapToBitmapImage(bmp);
                        using var stream = File.Create(imgPath);
                        bmp.Save(stream, ImageFormat.Jpeg);
                    }
                }
                pc.Loading = false;

            }

            // for each computer in the json file
            foreach (var puter in o["computers"]) {

                var pc = new Computer {
                    DisplayName = (string)puter["displayName"],
                    HostName = (string)puter["hostname"],
                    UserName = (string)puter["username"],
                    Password = (string)puter["password"],
                    MacAddress = (string)puter["mac"],
                    Online = false,
                    IconColor = "Gray",
                    Uuid = (string)puter["uuid"],
                    Background = BitmapToBitmapImage(defaultBmp),
                    Loading = true
                };



                // assume the pc is offline
                var imgPath = Windows.Storage.ApplicationData.Current.LocalFolder.Path + "\\ImageCache\\Backgrounds\\BW\\" + pc.Uuid + ".jpg";
                if (File.Exists(imgPath)) {
                    pc.Background = new BitmapImage(new Uri(imgPath));
                } else {
                    var bmp = MakeGrayscale3(defaultBmp);
                    pc.Background = BitmapToBitmapImage(bmp);
                    using var stream = File.Create(imgPath);
                    bmp.Save(stream, ImageFormat.Jpeg);
                }
                computersList.Add(pc);
                OnlineCheck(pc, defaultBmp); //todo: multithread this function so it can run in the background and let other cards generate
            };
            StartPanel.Visibility = Visibility.Collapsed;
            loadingBar.Visibility = Visibility.Collapsed;

        }

    }
    private BitmapImage BitmapToBitmapImage(Bitmap bmp)
    {
        var bitmapImage = new BitmapImage();
        using (var stream = new MemoryStream())
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
        var newBitmap = new Bitmap(original.Width, original.Height);

        //get a graphics object from the new image
        using (var g = Graphics.FromImage(newBitmap))
        {

            //create the grayscale ColorMatrix
            var colorMatrix = new ColorMatrix(
               new float[][]
               {
             new float[] {.3f, .3f, .3f, 0, 0},
             new float[] {.59f, .59f, .59f, 0, 0},
             new float[] {.11f, .11f, .11f, 0, 0},
             new float[] {0, 0, 0, 1, 0},
             new float[] {0, 0, 0, 0, 1}
               });

            //create some image attributes
            using var attributes = new ImageAttributes();

            //set the color matrix attribute
            attributes.SetColorMatrix(colorMatrix);

            //draw the original image on the new image
            //using the grayscale color matrix
            g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height),
                        0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);
        }
        return newBitmap;
    }
    void PowerOnClick(object sender, RoutedEventArgs e) {
        var macaddress = (string)((Button)sender).Tag;
        UdpClient udpClient = new UdpClient();

        // enable UDP broadcasting for UdpClient
        udpClient.EnableBroadcast = true;

        var dgram = new byte[1024];

        // 6 magic bytes
        for (int i = 0; i < 6; i++)
        {
            dgram[i] = 255;
        }

        // convert MAC-address to bytes
        byte[] address_bytes = new byte[6];
        for (int i = 0; i < 6; i++)
        {
            address_bytes[i] = byte.Parse(macaddress.Substring(3 * i, 2), NumberStyles.HexNumber);
        }

        // repeat MAC-address 16 times in the datagram
        var macaddress_block = dgram.AsSpan(6, 16 * 6);
        for (int i = 0; i < 16; i++)
        {
            address_bytes.CopyTo(macaddress_block.Slice(6 * i));
        }

        // send datagram using UDP and port 0
        udpClient.Send(dgram, dgram.Length, new System.Net.IPEndPoint(IPAddress.Broadcast, 0));
        udpClient.Close();
    }

    private async void SaveSoftwareBitmapToFile(SoftwareBitmap softwareBitmap, StorageFile outputFile)
    {
        using (var stream = await outputFile.OpenAsync(FileAccessMode.ReadWrite))
        {
            // Create an encoder with the desired format
            var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream);

            // Set the software bitmap
            // https://stackoverflow.com/questions/56469760/convert-a-system-drawing-bitmap-to-windows-graphics-imaging-softwarebitmap
            // https://learn.microsoft.com/en-us/uwp/api/windows.graphics.imaging.bitmapencoder?view=winrt-22621
            //https://stackoverflow.com/questions/35804375/how-do-i-save-a-bitmapimage-from-memory-into-a-file-in-wpf-c
            encoder.SetSoftwareBitmap(softwareBitmap);

            // Set additional encoding parameters, if needed
            encoder.BitmapTransform.ScaledWidth = 320;
            encoder.BitmapTransform.ScaledHeight = 240;
            encoder.BitmapTransform.Rotation = Windows.Graphics.Imaging.BitmapRotation.Clockwise90Degrees;
            encoder.BitmapTransform.InterpolationMode = BitmapInterpolationMode.Fant;
            encoder.IsThumbnailGenerated = true;

            try
            {
                await encoder.FlushAsync();
            }
            catch (Exception err)
            {
                const int WINCODEC_ERR_UNSUPPORTEDOPERATION = unchecked((int)0x88982F81);
                switch (err.HResult)
                {
                    case WINCODEC_ERR_UNSUPPORTEDOPERATION:
                        // If the encoder does not support writing a thumbnail, then try again
                        // but disable thumbnail generation.
                        encoder.IsThumbnailGenerated = false;
                        break;
                    default:
                        throw;
                }
            }

            if (encoder.IsThumbnailGenerated == false)
            {
                await encoder.FlushAsync();
            }


        }
    }
}
