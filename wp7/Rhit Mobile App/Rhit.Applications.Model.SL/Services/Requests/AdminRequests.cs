using System;

namespace Rhit.Applications.Models.Services.Requests {
    public class AdminRequestPart : RequestPart {
        public AdminRequestPart(string baseUrl)
            : base(baseUrl) {
            PartUrl = "/admin{0}";
        }

        public PathDataRequestPart PathData(Guid token) {
            return new PathDataRequestPart(FullUrl, token);
        }

        public UpdateVersionRequestPart UpdateVersion(Guid token, double version) {
            return new UpdateVersionRequestPart(FullUrl, token, "locations", version);
        }

        public UpdateVersionRequestPart UpdateServicesVersion(Guid token, double version)
        {
            return new UpdateVersionRequestPart(FullUrl, token, "services", version);
        }

        public AuthenticateRequestPart Authenticate(string username, string password) {
            return new AuthenticateRequestPart(FullUrl, username, password);
        }

        public StoredProcedureRequestPart StoredProcedure(Guid token, string spName) {
            return new StoredProcedureRequestPart(FullUrl, token, spName);
        }

        public FileHostRequestPart FileHost(Guid token) {
            return new FileHostRequestPart(FullUrl, token);
        }

        [Obsolete("Not a valid request end point")]
        public override string ToString() {
            return base.ToString();
        }
    }

    public class AuthenticateRequestPart : RequestPart {
        public AuthenticateRequestPart(string baseUrl, string username, string password)
            : base(baseUrl) {
            UserName = username;
            Password = password;
            PartUrl = "/authenticate/{1}/{2}{0}";
        }

        protected override string FullUrl {
            get { return String.Format(String.Format(BaseUrl, PartUrl), "{0}", UserName, Password); }
        }

        public string UserName { get; set; }

        public string Password { get; set; }

        public override string ToString() {
            return String.Format(FullUrl, "");
        }
    }

    public class PathDataRequestPart : RequestPart {
        public PathDataRequestPart(string baseUrl, Guid token)
            : base(baseUrl) {
            Token = token;
            PartUrl = "/{1}/pathdata{0}";
        }
        protected override string FullUrl {
            get { return String.Format(String.Format(BaseUrl, PartUrl), "{0}", Token); }
        }

        public Guid Token { get; set; }


        public override string ToString() {
            return String.Format(FullUrl, "");
        }
    }

    public class StoredProcedureRequestPart : RequestPart {
        public StoredProcedureRequestPart(string baseUrl, Guid token, string spName)
            : base(baseUrl) {
            Token = token;
            Name = spName;
            PartUrl = "/{1}/storedproc/{2}{0}";
        }

        protected override string FullUrl {
            get { return String.Format(String.Format(BaseUrl, PartUrl), "{0}", Token, Name); }
        }

        public Guid Token { get; set; }

        public string Name { get; set; }

        public override string ToString() {
            return String.Format(FullUrl, "");
        }
    }

    public class FileHostRequestPart : RequestPart {
        public FileHostRequestPart(string baseUrl, Guid token)
            : base(baseUrl) {
            Token = token;
            PartUrl = "/{1}/filehost/{0}";
        }

        protected override string FullUrl {
            get { return String.Format(String.Format(BaseUrl, PartUrl), "{0}", Token); }
        }

        public Guid Token { get; set; }

        public UploadRequestPart Upload(string folder) {
            return new UploadRequestPart(FullUrl, folder);
        }

        public DeleteFolderRequestPart Remove(string folder) {
            return new DeleteFolderRequestPart(FullUrl, folder);
        }

        public FoldersRequestPart Folders {
            get { return new FoldersRequestPart(FullUrl); }
        }

        [Obsolete("Not a valid request end point")]
        public override string ToString() {
            return base.ToString();
        }
    }

    public class UploadRequestPart : RequestPart {
        public UploadRequestPart(string baseUrl, string folder)
            : base(baseUrl) {
            PartUrl = "/upload/{0}{1}";
            Folder = folder;
        }

        public string Folder { get; set; }

        protected override string FullUrl {
            get { return String.Format(String.Format(BaseUrl, PartUrl), Folder, "{0}"); }
        }
    }

    public class FoldersRequestPart : RequestPart {
        public FoldersRequestPart(string baseUrl)
            : base(baseUrl) {
            PartUrl = "/folders{0}";
        }
    }

    public class DeleteFolderRequestPart : RequestPart {
        public DeleteFolderRequestPart(string baseUrl, string folder)
            : base(baseUrl) {
            PartUrl = "/remove/{0}{1}";
            Folder = folder;
        }

        public string Folder { get; set; }

        protected override string FullUrl {
            get { return String.Format(String.Format(BaseUrl, PartUrl), Folder, "{0}"); }
        }
    }

    public class UpdateVersionRequestPart : RequestPart {
        public UpdateVersionRequestPart(string baseUrl, Guid token, String type, double version)
            : base(baseUrl) {
            Token = token;
            Version = version;
            VersionType = type;
            PartUrl = "/{0}/updateversion?{1}={2}";
        }

        protected override string FullUrl {
            get { return String.Format(String.Format(BaseUrl, PartUrl), Token, VersionType, Version); }
        }

        public Guid Token { get; set; }

        public String VersionType { get; set; }

        public double Version { get; set; }

        public override string ToString() {
            return String.Format(FullUrl, "");
        }
    }
}
