using System;

namespace Rhit.Applications.Models.Events {
    public delegate void TagsEventHandler(Object sender, TagsEventArgs e);

    public class TagsEventArgs : ServiceEventArgs {
        public TagsEventArgs(ServiceEventArgs baseArgs) : base() {
            Copy(baseArgs);
        }

        public TagsCategory_DC TagRoot { get; set; }
    }
}
