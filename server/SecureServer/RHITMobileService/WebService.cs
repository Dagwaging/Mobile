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
        User GetUser(string authToken, string username);
        
        [OperationContract]
        User[] SearchUsers(string authToken, string search);

        [OperationContract]
        Course GetCourse(string authToken, int term, int crn);

        [OperationContract]
        Course[] SearchCourses(string authToken, string search);
        
        [OperationContract]
        string[] GetCourseEnrollment(string authToken, int term, int crn);
        
        [OperationContract]
        UserEnrollment[] GetUserEnrollment(string authToken, string username);
        
        [OperationContract]
        CourseTime[] GetCourseSchedule(string authToken, int term, int crn);
        
        [OperationContract]
        [FaultContract(typeof(AuthFault))]
        RoomSchedule[] GetRoomSchedule(string authToken, string room);
    }

    public class WebService : IWebService
    {
        private Authentication _auth;

        public WebService()
        {
            _auth = new Authentication();
        }

        public AuthenticationResponse Login(string username, string password)
        {
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
            _auth.Invalidate(authToken);
        }

        private void Authorize(string authToken)
        {
            if (!_auth.IsAuthenticated(authToken))
                throw new AuthFaultException();
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
    }
}
