using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glazier.Models;

public class Computer
{
    public string DisplayName
    {
        get; set;
    }
    public string HostName
    {
        get; set;
    }
    public string UserName
    {
        get; set;
    }
    public string Password
    {
        get; set;
    }
    public string MacAddress
    {
        get; set;
    }
    public Boolean Online
    {
        get; set;
    }
    public string Uuid
    {
        get; set;
    }
    public ImageSource Background {get; set;}
    public string IconColor { get; set; }
}
