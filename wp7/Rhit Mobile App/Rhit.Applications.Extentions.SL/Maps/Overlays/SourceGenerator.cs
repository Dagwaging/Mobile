using System;

namespace Rhit.Applications.Extentions.Maps.Overlays {
    public class SourceGenerator {
        public SourceGenerator(string baseAddress) {
            BaseAddress = baseAddress;

        }

        public string BaseAddress { get; protected set; }

        public Overlay GenerateSource(string specifier) {
            string fullAddress;
            try {
                fullAddress = string.Format("{0}{1}/{2}.png",BaseAddress, specifier, "{0}");
            } catch(Exception e) {
                return null;
            }
            Overlay overlay = new Overlay(fullAddress) {
                Label = specifier,
            };
            return overlay;
        }
    }
}
