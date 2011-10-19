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
using System.Net.NetworkInformation;

namespace RhitMobile.Services {
    public static class InternetConnection {
        public static bool TestConnection() {
            if(!NetworkInterface.GetIsNetworkAvailable()) {
                MessageBox.Show("No Internet Connection\nThere will be limited functionality until connection to the internet is restored.");
                return false;
            }
            return true;
        }
    }
}
