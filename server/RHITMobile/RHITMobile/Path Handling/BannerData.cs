using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RHITMobile.RhitPrivate;
using System.Collections.Specialized;

namespace RHITMobile {
    public class BannerHandler : SecurePathHandler {
        public static readonly IWebService Service = new WebServiceClient();

        public BannerHandler() {
            Redirects.Add("authenticate", new BannerAuthenticateHandler());
            Redirects.Add("action", new BannerActionHandler());
        }
    }

    public class BannerAuthenticateHandler : SecurePathHandler {
        public override IEnumerable<ThreadInfo> VerifyHeaders(ThreadManager TM, NameValueCollection headers, object state) {
            var currentThread = TM.CurrentThread;
            var loginData = (KerberosLoginData)state;
            var response = BannerHandler.Service.Login(loginData.Username, loginData.Password);
            yield return TM.Return(currentThread, new JsonResponse(new MessageResponse(response.Token))); //TODO
        }

        protected override IEnumerable<ThreadInfo> HandleNoPath(ThreadManager TM, Dictionary<string, string> query, object state) {
            var currentThread = TM.CurrentThread;
            yield return TM.Return(currentThread, new JsonResponse((JsonObject)state));
        }
    }

    public class BannerActionHandler : SecurePathHandler {
        public BannerActionHandler() {
            Redirects.Add("user", new BannerUserHandler());
            Redirects.Add("course", new BannerCourseHandler());
            Redirects.Add("room", new BannerRoomHandler());
        }

        public override IEnumerable<ThreadInfo> VerifyHeaders(ThreadManager TM, NameValueCollection headers, object state) {
            return base.VerifyHeaders(TM, headers, state); //TODO
        }
    }

    public class BannerUserHandler : SecurePathHandler {
        public BannerUserHandler() {
            Redirects.Add("data", new BannerUserDataHandler());
            Redirects.Add("search", new BannerUserSearchHandler());
            Redirects.Add("schedule", new BannerUserSearchHandler());
        }
    }

    public class BannerUserDataHandler : SecurePathHandler {
        public BannerUserDataHandler() {
            UnknownRedirect = new BannerUserDataUsernameHandler();
        }

        protected override IEnumerable<ThreadInfo> HandleUnknownPath(ThreadManager TM, string path, object state) {
            var currentThread = TM.CurrentThread;
            yield return TM.Return(currentThread, new BannerRequestData((string)state, path));
        }
    }

    public class BannerUserDataUsernameHandler : SecurePathHandler {
        protected override IEnumerable<ThreadInfo> HandleNoPath(ThreadManager TM, Dictionary<string, string> query, object state) {
            return base.HandleNoPath(TM, query, state); //TODO
        }
    }

    public class BannerUserSearchHandler : SecurePathHandler {

    }

    public class BannerUserScheduleHandler : SecurePathHandler {

    }

    public class BannerCourseHandler : SecurePathHandler {

    }

    public class BannerRoomHandler : SecurePathHandler {

    }

    public class KerberosLoginData {
        public string Username { get; set; }
        public string Password { get; set; }

        public KerberosLoginData(string username, string password) {
            Username = username;
            Password = password;
        }
    }

    public class BannerRequestData {
        public string Token { get; set; }
        public string Id { get; set; }

        public BannerRequestData(string token, string id) {
            Token = token;
            Id = id;
        }
    }

    public class BannerTermCrnData {
        public string Token { get; set; }
        public int Term { get; set; }
        public int Crn { get; set; }

        public BannerTermCrnData(string token, int term) {
            Token = token;
            Term = term;
            Crn = -1;
        }
    }
}
