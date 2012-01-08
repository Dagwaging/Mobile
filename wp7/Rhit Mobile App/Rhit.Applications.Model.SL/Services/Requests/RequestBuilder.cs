﻿using System;

namespace Rhit.Applications.Model.Services.Requests {
    /// \ingroup services
    /// <summary>
    /// 
    /// </summary>
    public class RequestBuilder : RequestPart {
        #region Constructors
        public RequestBuilder(string baseUrl) : this(baseUrl, 0) { }

        public RequestBuilder(string baseUrl, double version) 
            : this(baseUrl, version, null, false) { }

        public RequestBuilder(string baseUrl, string searchText)
            : this(baseUrl, 0, searchText, false) { }

        public RequestBuilder(string baseUrl, double version, string searchText)
            : this(baseUrl, version, searchText, false) { }

        public RequestBuilder(string baseUrl, string searchText, bool highlightSearch)
            : this(baseUrl, 0, searchText, highlightSearch) { }

        public RequestBuilder(string baseUrl, double version, string searchText, bool highlightSearch)
            : base(baseUrl) {
            CurrentVersion = version;
            SearchText = searchText;
            HighlightSearch = highlightSearch;
            PartUrl = "{0}";
        }
        #endregion

        public double CurrentVersion { get; set; }

        public string SearchText { get; set; }

        public bool HighlightSearch { get; set; }

        protected override string FullUrl {
            get {
                string url = BaseUrl + PartUrl;
                if(CurrentVersion != 0) url += "?version=" + CurrentVersion.ToString();
                if(SearchText != null && SearchText != "") {
                    if(HighlightSearch) url += "?sh=" + SearchText;
                    else url += "?s=" + SearchText;
                }
                return url;
            }
        }

        public AdminRequestPart Admin(Guid token, string storedProcedure) {
            return new AdminRequestPart(FullUrl, token, storedProcedure);
        }

        public AdminRequestPart Admin(string username, string password) {
            return new AdminRequestPart(FullUrl, username, password);
        }

        public AdminRequestPart Admin(Guid token, double version) {
            return new AdminRequestPart(FullUrl, token, version);
        }


        public DirectionsRequestPart Directions {
            get { return new DirectionsRequestPart(FullUrl); }
        }


        public LocationRequestPart Locations {
            get { return new LocationRequestPart(FullUrl); }
        }


        public VersionRequestPart Version {
            get { return new VersionRequestPart(FullUrl); }
        }

        [Obsolete("Not a valid request end point")]
        public override string ToString() {
            return base.ToString();
        }
    }
}
