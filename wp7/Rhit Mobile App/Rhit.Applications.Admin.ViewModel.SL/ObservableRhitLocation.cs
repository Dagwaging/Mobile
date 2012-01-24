using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Rhit.Applications.Model;
using Microsoft.Maps.MapControl;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace Rhit.Applications.ViewModel
{
    public class ObservableRhitLocation : DependencyObject
    {
        public ObservableRhitLocation(RhitLocation location)
        {
            OriginalLocation = location;
            InitilizeData();
        }

        private void InitilizeData()
        {
            AltNames = new ObservableCollection<AlternateName>();
            foreach (string name in OriginalLocation.AltNames) AltNames.Add(new AlternateName(name));
            Corners = new ObservableCollection<Location>();
            foreach (Location location in OriginalLocation.Corners) Corners.Add(location);
            Links = new ObservableCollection<Link>();
            foreach (ILink link in OriginalLocation.Links)
                Links.Add(new Link() { Name = link.Name, Address = link.Address, });

            Center = OriginalLocation.Center;
            Description = OriginalLocation.Description;
            Floor = OriginalLocation.Floor;
            Id = OriginalLocation.Id;
            LabelOnHybrid = OriginalLocation.LabelOnHybrid;
            MinZoom = OriginalLocation.MinZoomLevel;
            Label = OriginalLocation.Label;
            ParentId = OriginalLocation.ParentId;
            Type = OriginalLocation.Type;

            AltNames.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChanged);
            Corners.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChanged);
            Links.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChanged);
        }

        public List<string> CheckChanges()
        {
            RhitLocation location = OriginalLocation;
            List<string> changes = new List<string>();
            if (Center != location.Center) changes.Add("Center");
            if (Description != location.Description) changes.Add("Description");
            if (Floor != location.Floor) changes.Add("Floor");
            if (Id != location.Id) changes.Add("Id");
            if (Label != location.Label) changes.Add("Label");
            if (LabelOnHybrid != location.LabelOnHybrid) changes.Add("LabelOnHybrid");
            if (MinZoom != location.MinZoomLevel) changes.Add("MinZoomLevel");
            if (ParentId != location.ParentId) changes.Add("ParentId");
            if (Type != location.Type) changes.Add("Type");

            if (Links.Count != location.Links.Count) changes.Add("Links");
            else
            {
                foreach (Link link in Links)
                {
                    if (location.Links.Contains(link)) continue;
                    changes.Add("Links");
                    break;
                }
            }

            if (AltNames.Count != location.AltNames.Count) changes.Add("AltNames");
            else
            {
                foreach (AlternateName altName in AltNames)
                {
                    if (location.AltNames.Contains(altName.Name)) continue;
                    changes.Add("AltNames");
                    break;
                }
            }

            if (changes.Count > 0) HasChanged = true;
            else HasChanged = false;
            return changes;
        }

        public RhitLocation OriginalLocation { get; private set; }

        public ObservableCollection<AlternateName> AltNames { get; set; }

        public ObservableCollection<Location> Corners { get; private set; }

        public ObservableCollection<Link> Links { get; private set; }

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            CheckChanges();
        }

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ObservableRhitLocation).CheckChanges();
        }

        #region Description
        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(ObservableRhitLocation), new PropertyMetadata("", new PropertyChangedCallback(OnPropertyChanged)));
        #endregion

        #region Floor
        public int Floor
        {
            get { return (int)GetValue(FloorProperty); }
            set { SetValue(FloorProperty, value); }
        }

        public static readonly DependencyProperty FloorProperty =
            DependencyProperty.Register("Floor", typeof(int), typeof(ObservableRhitLocation), new PropertyMetadata(0, new PropertyChangedCallback(OnPropertyChanged)));
        #endregion

        #region Center
        public Location Center
        {
            get { return (Location)GetValue(CenterProperty); }
            set { SetValue(CenterProperty, value); }
        }

        public static readonly DependencyProperty CenterProperty =
            DependencyProperty.Register("Center", typeof(Location), typeof(ObservableRhitLocation), new PropertyMetadata(new Location(), new PropertyChangedCallback(OnPropertyChanged)));
        #endregion

        #region HasChanged
        public bool HasChanged
        {
            get { return (bool)GetValue(HasChangedProperty); }
            set { SetValue(HasChangedProperty, value); }
        }

        public static readonly DependencyProperty HasChangedProperty =
            DependencyProperty.Register("HasChanged", typeof(bool), typeof(ObservableRhitLocation), new PropertyMetadata(false));
        #endregion

        #region Id
        public int Id
        {
            get { return (int)GetValue(IdProperty); }
            set { SetValue(IdProperty, value); }
        }

        public static readonly DependencyProperty IdProperty =
            DependencyProperty.Register("Id", typeof(int), typeof(ObservableRhitLocation), new PropertyMetadata(0, new PropertyChangedCallback(OnPropertyChanged)));
        #endregion

        #region Label
        public string Label
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        public static readonly DependencyProperty NameProperty =
         DependencyProperty.Register("Label", typeof(string), typeof(ObservableRhitLocation), new PropertyMetadata("", new PropertyChangedCallback(OnPropertyChanged)));
        #endregion

        #region LabelOnHybrid
        public bool LabelOnHybrid
        {
            get { return (bool)GetValue(LabelOnHybridProperty); }
            set { SetValue(LabelOnHybridProperty, value); }
        }

        public static readonly DependencyProperty LabelOnHybridProperty =
            DependencyProperty.Register("LabelOnHybrid", typeof(bool), typeof(ObservableRhitLocation), new PropertyMetadata(false, new PropertyChangedCallback(OnPropertyChanged)));
        #endregion

        #region MinZoom
        public int MinZoom
        {
            get { return (int)GetValue(MinZoomProperty); }
            set { SetValue(MinZoomProperty, value); }
        }

        public static readonly DependencyProperty MinZoomProperty =
            DependencyProperty.Register("MinZoom", typeof(int), typeof(ObservableRhitLocation), new PropertyMetadata(0, new PropertyChangedCallback(OnPropertyChanged)));
        #endregion

        #region ParentId
        public int ParentId
        {
            get { return (int)GetValue(ParentIdProperty); }
            set { SetValue(ParentIdProperty, value); }
        }

        public static readonly DependencyProperty ParentIdProperty =
            DependencyProperty.Register("ParentId", typeof(int), typeof(ObservableRhitLocation), new PropertyMetadata(0, new PropertyChangedCallback(OnPropertyChanged)));
        #endregion

        #region Type
        public LocationType Type
        {
            get { return (LocationType)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }

        public static readonly DependencyProperty TypeProperty =
            DependencyProperty.Register("Type", typeof(LocationType), typeof(ObservableRhitLocation), new PropertyMetadata(LocationType.NormalLocation, new PropertyChangedCallback(OnPropertyChanged)));
        #endregion
    }
}
