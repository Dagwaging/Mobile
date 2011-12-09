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

namespace Rhit.Admin.Model.Services.Requests
{
    public class RequestQuery : RequestPart
    {
        public RequestQuery(string baseUrl, string name, object value)
            : base(baseUrl)
        {
            PartUrl = String.Format("?{0}={1}", name, value);
        }

        protected override string FullUrl {
            get { return BaseUrl + PartUrl; }
        }

        public override RequestQuery AddQueryParameter(string name, object value)
        {
            PartUrl += String.Format("&{0}={1}", name, value);
            return this;
        }
    }
}
