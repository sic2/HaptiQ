using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows;

namespace SurfaceApp1
{
    public class Helper
    {
        private static Point _topLeftCorner;

        private const int OFFSET = 20;

        public static void setTopLeftCorner(Point point)
        {
            _topLeftCorner = point;
        }

        /// <summary>
        /// Return appropriate brush.
        /// Return white brush if color not valid
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static SolidColorBrush getBrush(String color)
        {
            switch (color)
            {
                case "Blue":
                    return Brushes.Blue;
                case "Orange":
                    return Brushes.Orange;
                case "Red":
                    return Brushes.Red;
                case "Green":
                    return Brushes.Green;
                case "Yellow":
                    return Brushes.Yellow;
                default:
                    break;
            }

            return Brushes.White;
        }

        public static Point adjustPoint(Point point)
        {
            return new Point(point.X + _topLeftCorner.X + OFFSET, point.Y + OFFSET);
        }
    }
}
