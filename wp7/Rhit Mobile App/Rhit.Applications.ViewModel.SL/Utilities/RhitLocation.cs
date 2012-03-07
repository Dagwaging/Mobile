using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using Microsoft.Maps.MapControl;
using Rhit.Applications.Models;


namespace Rhit.Applications.ViewModels.Utilities {
    public class RhitLocation : SimpleRhitLocation {
        public RhitLocation(LocationData location) : base(location) {
            InitilizeCallbacks();
        }

        private void InitilizeCallbacks() {
            AltNames.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChanged);
            Corners.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChanged);
            Links.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChanged);
        }

        public List<string> CheckChanges() {
            LocationData location = OriginalLocation;
            List<string> changes = new List<string>();
            if(Center != location.Center) changes.Add("Center");
            if(Description != location.Description) changes.Add("Description");
            if(Floor != location.Floor) changes.Add("Floor");
            if(Id != location.Id) changes.Add("Id");
            if(Label != location.Label) changes.Add("Label");
            if(LabelOnHybrid != location.LabelOnHybrid) changes.Add("LabelOnHybrid");
            if(MinZoom != location.MinZoomLevel) changes.Add("MinZoomLevel");
            if(ParentId != location.ParentId) changes.Add("ParentId");
            if(Type != location.Type) changes.Add("Type");

            if(Links.Count != location.Links.Count) changes.Add("Links");
            else {
                foreach(Link link in Links) {
                    if(location.Links.Contains(link)) continue;
                    changes.Add("Links");
                    break;
                }
            }

            if(AltNames.Count != location.AltNames.Count) changes.Add("AltNames");
            else {
                foreach(AlternateName altName in AltNames) {
                    if(location.AltNames.Contains(altName.Name)) continue;
                    changes.Add("AltNames");
                    break;
                }
            }

            if(changes.Count > 0) HasChanged = true;
            else HasChanged = false;
            return changes;
        }

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            CheckChanges();
        }

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            (d as RhitLocation).CheckChanges();
        }

        #region Description
        public override string Description {
            get { return (string) GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(SimpleRhitLocation), new PropertyMetadata("", new PropertyChangedCallback(OnPropertyChanged)));
        #endregion

        #region Floor
        public override int Floor {
            get { return (int) GetValue(FloorProperty); }
            set { SetValue(FloorProperty, value); }
        }

        public static readonly DependencyProperty FloorProperty =
            DependencyProperty.Register("Floor", typeof(int), typeof(SimpleRhitLocation), new PropertyMetadata(0, new PropertyChangedCallback(OnPropertyChanged)));
        #endregion

        #region Center
        public Location Center {
            get { return (Location) GetValue(CenterProperty); }
            set { SetValue(CenterProperty, value); }
        }

        public static readonly DependencyProperty CenterProperty =
            DependencyProperty.Register("Center", typeof(Location), typeof(SimpleRhitLocation), new PropertyMetadata(new Location(), new PropertyChangedCallback(OnPropertyChanged)));
        #endregion

        #region HasChanged
        public bool HasChanged {
            get { return (bool) GetValue(HasChangedProperty); }
            set { SetValue(HasChangedProperty, value); }
        }

        public static readonly DependencyProperty HasChangedProperty =
            DependencyProperty.Register("HasChanged", typeof(bool), typeof(SimpleRhitLocation), new PropertyMetadata(false));
        #endregion

        #region Id
        public override int Id {
            get { return (int) GetValue(IdProperty); }
            set { SetValue(IdProperty, value); }
        }

        public static readonly DependencyProperty IdProperty =
            DependencyProperty.Register("Id", typeof(int), typeof(SimpleRhitLocation), new PropertyMetadata(0, new PropertyChangedCallback(OnPropertyChanged)));
        #endregion

        #region Label
        public override string Label {
            get { return (string) GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        public static readonly DependencyProperty NameProperty =
         DependencyProperty.Register("Label", typeof(string), typeof(SimpleRhitLocation), new PropertyMetadata("", new PropertyChangedCallback(OnPropertyChanged)));
        #endregion

        #region LabelOnHybrid
        public override bool LabelOnHybrid {
            get { return (bool) GetValue(LabelOnHybridProperty); }
            set { SetValue(LabelOnHybridProperty, value); }
        }

        public static readonly DependencyProperty LabelOnHybridProperty =
            DependencyProperty.Register("LabelOnHybrid", typeof(bool), typeof(SimpleRhitLocation), new PropertyMetadata(false, new PropertyChangedCallback(OnPropertyChanged)));
        #endregion

        #region MinZoom
        public override int MinZoom {
            get { return (int) GetValue(MinZoomProperty); }
            set { SetValue(MinZoomProperty, value); }
        }

        public static readonly DependencyProperty MinZoomProperty =
            DependencyProperty.Register("MinZoom", typeof(int), typeof(SimpleRhitLocation), new PropertyMetadata(0, new PropertyChangedCallback(OnPropertyChanged)));
        #endregion

        #region ParentId
        public override int ParentId {
            get { return (int) GetValue(ParentIdProperty); }
            set { SetValue(ParentIdProperty, value); }
        }

        public static readonly DependencyProperty ParentIdProperty =
            DependencyProperty.Register("ParentId", typeof(int), typeof(SimpleRhitLocation), new PropertyMetadata(0, new PropertyChangedCallback(OnPropertyChanged)));
        #endregion

        #region Type
        public override LocationType Type {
            get { return (LocationType) GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }

        public static readonly DependencyProperty TypeProperty =
            DependencyProperty.Register("Type", typeof(LocationType), typeof(SimpleRhitLocation), new PropertyMetadata(LocationType.NormalLocation, new PropertyChangedCallback(OnPropertyChanged)));
        #endregion
    }
}
