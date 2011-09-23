using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace RHITMobile
{
    public static class Program
    {
        public static void Main()
        {
            var TM = new ThreadManager();
            TM.Enqueue(WebController.HandleClients(TM));
            TM.Start(1);
        }

        public static double ServerVersion = 0.1;
        public static string ConnectionString = @"Data Source=mobilewin.csse.rose-hulman.edu\RHITMobile;Initial Catalog=MapData;User Id=server;Password=rhitMobile56";
    }
}