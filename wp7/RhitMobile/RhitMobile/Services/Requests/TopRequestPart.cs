﻿
namespace RhitMobile.Services.Requests {
    public class TopRequestPart : RequestPart {
        public TopRequestPart(string baseUrl) : base(baseUrl) {
            PartUrl = "/top{0}";
        }
    }
}