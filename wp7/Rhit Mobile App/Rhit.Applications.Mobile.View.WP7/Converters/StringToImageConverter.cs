using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using Rhit.Applications.ViewModel.Utilities;
using System.Collections.Generic;

namespace Rhit.Applications.View.Converters {
    public class StringToImageConverter : IValueConverter {
        private static Dictionary<DirectionActionType, string> ActionMessageDictionary;

        static StringToImageConverter() {
            ActionMessageDictionary = new Dictionary<DirectionActionType, string>() {
                {DirectionActionType.Straight},
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

        public StringToImageConverter() { }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            string imageString;
            switch((string) value) {
                case "Go Straight":
                case "Cross the Street":
                case "Follow the Path":
                    imageString = @"/Assets/Icons/Directions/TurnStraight.png";
                    break;
                case "Slight Left":
                    imageString = @"/Assets/Icons/Directions/TurnSlightLeft.png";
                    break;
                case "Slight Right":
                    imageString = @"/Assets/Icons/Directions/TurnSlightRight.png";
                    break;
                case "Turn Left":
                    imageString = @"/Assets/Icons/Directions/TurnLeft.png";
                    break;
                case "Turn Right":
                    imageString = @"/Assets/Icons/Directions/TurnRight.png";
                    break;
                case "Sharp Left":
                    imageString = @"/Assets/Icons/Directions/TurnSharpLeft.png";
                    break;
                case "Sharp Right":
                    imageString = @"/Assets/Icons/Directions/TurnSharpRight.png";
                    break;
                case "Enter the Building":
                    imageString = @"/Assets/Icons/Directions/TurnEnterBuilding.png";
                    break;
                case "Exit the Building":
                    imageString = @"/Assets/Icons/Directions/TurnExitBuilding.png";
                    break;
                case "Go Up the Stairs":
                    imageString = @"/Assets/Icons/Directions/TurnUpStairs.png";
                    break;
                case "Go Down the Stairs":
                    imageString = @"/Assets/Icons/Directions/TurnDownStairs.png";
                    break;
                default:
                    imageString = @"/Assets/Icons/Directions/TurnStraight.png";
                    break;

            }
            return new BitmapImage(new Uri(imageString, UriKind.Relative));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}