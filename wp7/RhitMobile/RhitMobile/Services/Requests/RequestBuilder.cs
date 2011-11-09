using System;

namespace RhitMobile.Services.Requests {
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
            Version = version;
            SearchText = searchText;
            HighlightSearch = highlightSearch;
            PartUrl = "{0}";
        }
        #endregion

        public double Version { get; set; }

        public string SearchText { get; set; }

        public bool HighlightSearch { get; set; }

        protected override string FullUrl {
            get {
                string url = BaseUrl + PartUrl;
                if(Version != 0) url += "?version=" + Version.ToString();
                if(SearchText != null && SearchText != "") {
                    if(HighlightSearch) url += "?sh=" + SearchText;
                    else url += "?s=" + SearchText;
                }
                return url;
            }
        }

        public LocationRequestPart Locations {
            get { return new LocationRequestPart(FullUrl); }
        }

        public DirectionsRequestPart Directions {
            get { return new DirectionsRequestPart(FullUrl); }
        }

        public override string ToString() {
            throw new InvalidOperationException("Not a valid request string.");
        }
    }
}
