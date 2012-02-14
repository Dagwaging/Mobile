using System.Collections.Generic;
using System.Collections.ObjectModel;
using Rhit.Applications.Model;
using Rhit.Applications.Model.Events;
using Rhit.Applications.Model.Services;

namespace Rhit.Applications.ViewModel.Controllers {
    public class TagsController {
        private TagsController() {
            All = new ObservableCollection<Tag>();
            DataCollector.Instance.TagsReturned += new TagsEventHandler(TagsReturned);
            DataCollector.Instance.GetTags();
        }

        #region Singleton Instance
        private static TagsController _instance;
        public static TagsController Instance {
            get {
                if(_instance == null)
                    _instance = new TagsController();
                return _instance;
            }
        }
        #endregion

        private void TagsReturned(object sender, TagsEventArgs e) {
            Root = new TagsCategory(e.TagRoot);
            Root.Label = "Tour Tags";
            AddTagCategory(Root, -1);
            //OnCampusServicesUpdated();
        }

        private void AddTagCategory(TagsCategory category, int parentId) {
            foreach(Tag tag in category.Tags)
                All.Add(tag);
            CategoryDictionary[category.Id] = category;
            if(parentId >= 0)
                CategoryParentDictionary[category.Id] = parentId;
            foreach(TagsCategory child in category.Children)
                AddTagCategory(child, category.Id);
        }

        private TagsCategory Root { get; set; }

        public ObservableCollection<Tag> All { get; set; }

        private Dictionary<int, TagsCategory> CategoryDictionary { get; set; }

        private Dictionary<int, int> CategoryParentDictionary { get; set; }

    }

    public class TagsCategory {
        private static int _lastId = 0;

        public TagsCategory(TagsCategory_DC model) {
            Id = ++_lastId;
            Label = model.Name;
            Children = new ObservableCollection<TagsCategory>();
            foreach(TagsCategory_DC category in model.Children)
                Children.Add(new TagsCategory(category));
            Tags = new ObservableCollection<Tag>();
            foreach(Tag_DC tag in model.Tags)
                Tags.Add(new Tag(tag));
        }

        internal int Id { get; set; }

        public string Label { get; internal set; }

        public ObservableCollection<TagsCategory> Children { get; protected set; }

        public ObservableCollection<Tag> Tags { get; protected set; }
    }

    public class Tag {
        public Tag(Tag_DC model) {
            Id = model.Id;
            Label = model.Name;
        }

        internal int Id { get; set; }

        public string Label { get; protected set; }
    }
}
