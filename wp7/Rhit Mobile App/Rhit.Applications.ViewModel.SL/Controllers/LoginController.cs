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
using Rhit.Applications.Models.Services;

namespace Rhit.Applications.ViewModels.Controllers
{
    public class LoginController
    {
        private static LoginController _instance;

        #region Singleton Instance
        public static LoginController Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new LoginController();
                return _instance;
            }
        }
        #endregion

        public Boolean IsLoggedIn
        {
            get { return Connection.ServiceToken != null; }
        }
    }

}
