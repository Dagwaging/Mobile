using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using Rhit.Applications.ViewModel.Utilities;

namespace Rhit.Applications.View.Converters {
    public class ActionToImageConverter : IValueConverter {
        private static string DefaultImageString;

        private static Dictionary<DirectionActionType, string> ActionImageStringDictionary;

        static ActionToImageConverter() {
            DefaultImageString = "TurnStraight";
            ActionImageStringDictionary = new Dictionary<DirectionActionType, string>() {
                {DirectionActionType.Arrive, ""},
                {DirectionActionType.Depart, ""},
                {DirectionActionType.Straight, "TurnStraight"},
                {DirectionActionType.CrossStreet, "TurnStraight"},
                {DirectionActionType.FollowPath, "TurnStraight"},
                {DirectionActionType.SlightLeft, "TurnSlightLeft"},
                {DirectionActionType.SlightRight, "TurnSlightRight"},
                {DirectionActionType.TurnLeft, "TurnLeft"},
                {DirectionActionType.TurnRight, "TurnRight"},
                {DirectionActionType.SharpLeft, "TurnSharpLeft"},
                {DirectionActionType.SharpRight, "TurnSharpRight"},
                {DirectionActionType.EnterBuilding, "TurnEnterBuilding"},
                {DirectionActionType.ExitBuilding, "TurnExitBuilding"},
                {DirectionActionType.GoUpStairs, "TurnUpStairs"},
                {DirectionActionType.GoDownStairs, "TurnDownStairs"},
            };
        }

        public ActionToImageConverter() { }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            DirectionActionType action = (DirectionActionType) value;
            string imageString;
            if(ActionImageStringDictionary.ContainsKey(action))
                imageString = ActionImageStringDictionary[action];
            else imageString = DefaultImageString;
            return new BitmapImage(new Uri(string.Format(@"/Assets/Icons/Directions/{0}.png", imageString), UriKind.Relative));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}