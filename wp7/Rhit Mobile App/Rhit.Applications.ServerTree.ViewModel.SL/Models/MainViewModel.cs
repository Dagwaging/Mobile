using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;
using Rhit.Applications.Model.Events;
using Rhit.Applications.Model.Services;
using Rhit.Applications.Model.Services.Requests;

namespace Rhit.Applications.ViewModel.Models {
    public class MainViewModel : DependencyObject {
        public string BaseAddress = "http://mobilewin.csse.rose-hulman.edu:5600";

        public MainViewModel() {
            DataCollector.Instance.BaseAddress = BaseAddress;
            ResponseHandler.ResponseReceived += new Model.Events.ServerEventHandler(ResponseHandler_ResponseReceived);
            Id = 0;
            CreateTree();
        }

        private void CreateTree() {
            RequestTree = new ObservableCollection<RequestNode>();

            RequestNode node = new RequestNode(typeof(LocationRequestPart), new RequestBuilder(BaseAddress).Locations as RequestPart);
            node.Name = "Locations";
            RequestTree.Add(node);

            node = new RequestNode(typeof(DirectionsRequestPart), new RequestBuilder(BaseAddress).Directions);
            node.Name = "Directions";
            RequestTree.Add(node);

            //node = new RequestNode(typeof(AdminRequestPart), new RequestBuilder(BaseAddress).Admin);
            //RequestTree.Add(node);
        }

        private void ResponseHandler_ResponseReceived(object sender, ServerEventArgs e) {
            CurrentRequest.Response = e.RawResponse;
            Response = e.RawResponse;
            HttpStatusCode = e.ServerResponse.ToString();
        }

        #region Properties
        public ObservableCollection<RequestNode> RequestTree { get; set; }

        public RequestNode CurrentRequest { get; set; }
        #endregion

        #region Response
        public string Response {
            get { return (string) GetValue(ResponseProperty); }
            set { SetValue(ResponseProperty, value); }
        }

        public static readonly DependencyProperty ResponseProperty =
            DependencyProperty.Register("Response", typeof(string), typeof(MainViewModel), new PropertyMetadata(""));
        #endregion

        #region HttpStatusCode
        public string HttpStatusCode {
            get { return (string) GetValue(HttpStatusCodeProperty); }
            set { SetValue(HttpStatusCodeProperty, value); }
        }

        public static readonly DependencyProperty HttpStatusCodeProperty =
            DependencyProperty.Register("HttpStatusCode", typeof(string), typeof(MainViewModel), new PropertyMetadata(""));
        #endregion

        #region RequestString
        public string RequestString {
            get { return (string) GetValue(RequestStringProperty); }
            set { SetValue(RequestStringProperty, value); }
        }

        public static readonly DependencyProperty RequestStringProperty =
            DependencyProperty.Register("RequestString", typeof(string), typeof(MainViewModel), new PropertyMetadata(""));
        #endregion

        #region Id
        public int Id {
            get { return (int) GetValue(IdProperty); }
            set { SetValue(IdProperty, value); }
        }

        public static readonly DependencyProperty IdProperty =
            DependencyProperty.Register("Id", typeof(int), typeof(MainViewModel), new PropertyMetadata(0));
        #endregion

        public void ChangeRequest(object selectedObject) {
            CurrentRequest = selectedObject as RequestNode;
            if(!CurrentRequest.ReEvaluate(Id)) {
                if(CurrentRequest.Response != null && CurrentRequest.Response != string.Empty) {
                    RequestString = CurrentRequest.Request.ToString();
                    Response = CurrentRequest.Response;
                    HttpStatusCode = "Retrieved From Cache";
                    return;
                }
            }
            RequestString = CurrentRequest.Request.ToString();
            Response = "Waitin for a reply from the server...";
            HttpStatusCode = "";
            Connection.MakeRequest(Dispatcher, CurrentRequest.Request);
        }
    }
}
