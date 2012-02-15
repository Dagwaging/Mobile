using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using Rhit.Applications.ViewModel.Utilities;

namespace Rhit.Applications.View.Converters {
    public class ActionToStringConverter : IValueConverter {
        private static string DefaultMessage;

        private static Dictionary<DirectionActionType, string> ActionMessageDictionary;

        static ActionToStringConverter() {
            DefaultMessage = "Go Straight";
            ActionMessageDictionary = new Dictionary<DirectionActionType, string>() {
                {DirectionActionType.Arrive, "Arrive"},
                {DirectionActionType.Depart, "Depart"},
                {DirectionActionType.Straight, "Go Straight"},
                {DirectionActionType.CrossStreet, "Cross the Street"},
                {DirectionActionType.FollowPath, "Follow the Path"},
                {DirectionActionType.SlightLeft, "Slight Left"},
                {DirectionActionType.SlightRight, "Slight Right"},
                {DirectionActionType.TurnLeft, "Turn Left"},
                {DirectionActionType.TurnRight, "Turn Right"},
                {DirectionActionType.SharpLeft, "Sharp Left"},
                {DirectionActionType.SharpRight, "Sharp Right"},
                {DirectionActionType.EnterBuilding, "Enter the Building"},
                {DirectionActionType.ExitBuilding, "Exit the Building"},
                {DirectionActionType.GoUpStairs, "Go Up the Stairs"},
                {DirectionActionType.GoDownStairs, "Go Down the Stairs"},
            };
        }

        public ActionToStringConverter() { }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            DirectionActionType action = (DirectionActionType) value;
            if(ActionMessageDictionary.ContainsKey(action))
                return ActionMessageDictionary[action];
            return DefaultMessage;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}