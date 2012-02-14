using System.Device.Location;
using System.Windows;
using Rhit.Applications.Model;
using System.Collections.Generic;

namespace Rhit.Applications.ViewModel.Utilities {
    public class PathNode : DependencyObject {
        private static int LastNumber;

        private static Dictionary<string, DirectionActionType> ActionCodeDictionary;

        static PathNode() {
            LastNumber = 0;
            ActionCodeDictionary = new Dictionary<string, DirectionActionType>() {
                {"GS", DirectionActionType.Straight},
                {"CS", DirectionActionType.CrossStreet},
                {"FP", DirectionActionType.FollowPath},
                {"L1", DirectionActionType.SlightLeft},
                {"R1", DirectionActionType.SlightRight},
                {"L2", DirectionActionType.TurnLeft},
                {"R2", DirectionActionType.TurnRight},
                {"L3", DirectionActionType.SharpLeft},
                {"R3", DirectionActionType.SharpRight},
                {"EN", DirectionActionType.EnterBuilding},
                {"EX", DirectionActionType.ExitBuilding},
                {"US", DirectionActionType.GoUpStairs},
                {"DS", DirectionActionType.GoDownStairs},
            };
        }

        public PathNode(DirectionPath_DC model) {
            Number = ++LastNumber;
            Center = new GeoCoordinate(model.Latitude, model.Longitude);
            Action = GetActionType(model.Action);
        }

        internal PathNode Next { get; set; }

        internal PathNode Previous { get; set; }

        internal static void Reset() {
            LastNumber = 0;
        }

        internal static DirectionActionType GetActionType(string actionCode) {
            if(!ActionCodeDictionary.ContainsKey(actionCode))
                return DirectionActionType.None;
            return ActionCodeDictionary[actionCode];
        }

        public int Number { get; protected set; }

        public DirectionActionType Action { get; protected set; }

        public GeoCoordinate Center { get; protected set; }

        #region bool IsSelected
        public bool IsSelected {
            get { return (bool) GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public static readonly DependencyProperty IsSelectedProperty =
           DependencyProperty.Register("IsSelected", typeof(bool), typeof(PathNode), new PropertyMetadata(false));
        #endregion
    }
}
