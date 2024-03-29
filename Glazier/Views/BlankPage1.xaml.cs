// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System.Collections.ObjectModel;
using Glazier.Models;
using Newtonsoft.Json;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Glazier.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class BlankPage1 : Page {
    public ObservableCollection<Computer> computersList { get; set; }
    public BlankPage1() {
        this.InitializeComponent();
        /*Computer pcUno = new Computer {
            DisplayName = "ActualPC",
            HostName = "gamingpc",
            UserName = "isteiger",
                Password = "JoeBidenBalls",
            MacAddress = "asfsdfsdfad"
        };
        computersList = new ObservableCollection<Computer>{
            pcUno,
            pcUno
        };*/
        //String json = File.ReadAllText(@"/Properties/computers.json");
        //JsonTextReader reader = new JsonTextReader(new StringReader(json));
        //System.Diagnostics.Debug.WriteLine(json);


    }

}
