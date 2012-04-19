using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using RHITMobile.Secure.Data;

namespace RHITMobile.Secure
{
    public class AuthenticationResponse
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
    }

    public class AuthFault
    {
    }

    public class AuthFaultException : FaultException<AuthFault>
    {
        public AuthFaultException()
            : base(new AuthFault(), "Invalid authorization token")
        { }
    }

    [ServiceContract(Namespace="http://mobileprivate.rose-hulman.edu")]
    public interface IWebService
    {
        [OperationContract]
        AuthenticationResponse Login(string username, string password);

        [OperationContract(IsOneWay = true)]
        void Logout(string authToken);

        [OperationContract]
        [FaultContract(typeof(AuthFault))]
        User GetUser(string authToken, string username);
        
        [OperationContract]
        [FaultContract(typeof(AuthFault))]
        User[] SearchUsers(string authToken, string search);

        [OperationContract]
        [FaultContract(typeof(AuthFault))]
        Course GetCourse(string authToken, int term, int crn);

        [OperationContract]
        [FaultContract(typeof(AuthFault))]
        Course[] SearchCourses(string authToken, string search);
        
        [OperationContract]
        [FaultContract(typeof(AuthFault))]
        string[] GetCourseEnrollment(string authToken, int term, int crn);
        
        [OperationContract]
        [FaultContract(typeof(AuthFault))]
        UserEnrollment[] GetUserEnrollment(string authToken, string username);

        [OperationContract]
        [FaultContract(typeof(AuthFault))]
        UserEnrollment[] GetInstructorSchedule(string authToken, string username);

        [OperationContract]
        [FaultContract(typeof(AuthFault))]
        CourseTime[] GetCourseSchedule(string authToken, int term, int crn);
        
        [OperationContract]
        [FaultContract(typeof(AuthFault))]
        RoomSchedule[] GetRoomSchedule(string authToken, string room);

        [OperationContract]
        bool RequestUpdate();

        [OperationContract]
        ServerState GetState();
    }

    public struct ServerState
    {
        public DateTime LastUpdateTime { get; set; }
        public bool IsUpdateQueued { get; set; }
        public int ActiveRequests { get; set; }
        public int ActiveUserCount { get; set; }
        public int LastRecordsAffected { get; set; }
        public TimeSpan Uptime { get; set; }
        public int RequestCount { get; set; }
    }

    [ServiceBehavior(InstanceContextMode=InstanceContextMode.Single, ConcurrencyMode=ConcurrencyMode.Multiple)]
    public class WebService : IWebService
    {
        private Authentication _auth;

        private int _requests;

        public WebService()
        {
            _auth = new Authentication();
            _requests = 0;
        }

        public int LogRequest()
        {
            lock (this)
            {
                _requests++;
                return _requests;
            }
        }

        public AuthenticationResponse Login(string username, string password)
        {
            LogRequest();

            AuthenticationResponse response = new AuthenticationResponse();

            DateTime expiration;
            response.Token = _auth.AuthenticateUser(username, password, out expiration);
            response.Expiration = expiration;

            if (response.Token == null)
                return null;

            return response;
        }

        public void Logout(string authToken)
        {
            LogRequest();

            _auth.Invalidate(authToken);
        }

        private void Authorize(string authToken)
        {
            if (!_auth.IsAuthenticated(authToken))
                throw new AuthFaultException();
            
            LogRequest();
        }

        public User GetUser(string authToken, string username)
        {
            Authorize(authToken);

            return DB.Instance.GetUser(username);
        }

        public User[] SearchUsers(string authToken, string search)
        {
            Authorize(authToken);

            return DB.Instance.SearchUsers(search);
        }

        public Course GetCourse(string authToken, int term, int crn)
        {
            Authorize(authToken);

            return DB.Instance.GetCourse(term, crn);
        }

        public Course[] SearchCourses(string authToken, string search)
        {
            Authorize(authToken);

            return DB.Instance.SearchCourses(search);
        }

        public string[] GetCourseEnrollment(string authToken, int term, int crn)
        {
            Authorize(authToken);

            return DB.Instance.GetCourseEnrollment(term, crn);
        }

        public UserEnrollment[] GetUserEnrollment(string authToken, string username)
        {
            Authorize(authToken);

            return DB.Instance.GetUserEnrollment(username);
        }

        public UserEnrollment[] GetInstructorSchedule(string authToken, string username)
        {
            Authorize(authToken);

            return DB.Instance.GetInstructorSchedule(username);
        }

        public CourseTime[] GetCourseSchedule(string authToken, int term, int crn)
        {
            Authorize(authToken);

            return DB.Instance.GetCourseSchedule(term, crn);
        }

        public RoomSchedule[] GetRoomSchedule(string authToken, string room)
        {
            Authorize(authToken);

            return DB.Instance.GetRoomSchedule(room);
        }

        public bool RequestUpdate()
        {
            LogRequest();

            return WindowsService.Instance.DataMonitor.Updater.Update();
        }

        public ServerState GetState()
        {
            ServerState state = new ServerState();

            WindowsService service = WindowsService.Instance;
            DataUpdater updater = service.DataMonitor.Updater;

            state.LastUpdateTime = updater.LastUpdateTime;
            state.IsUpdateQueued = updater.IsUpdateQueued;
            state.ActiveRequests = DB.Instance.ActiveReaderCount;
            state.ActiveUserCount = _auth.ActiveUsers;
            state.LastRecordsAffected = DB.Instance.AffectedRows;
            state.Uptime = service.Uptime;
            state.RequestCount = LogRequest();
        
            return state;
        }
    }
}
