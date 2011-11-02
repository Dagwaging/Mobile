using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace RhitMobile.Events {
    public delegate void SearchEventHandler(Object sender, SearchEventArgs e);

    public class SearchEventArgs : ServiceEventArgs {
        //TODO: Put Search Results here
    }
}
