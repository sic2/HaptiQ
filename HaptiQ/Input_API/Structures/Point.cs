using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Input_API
{
    /// <summary>
    /// Generic Point class 
    /// Add methods to convert to other Point classes if necessary
    /// such as System.Windows.Point, System.Drawing.Point, etc.
    /// </summary>
    public class Point
    {
        /// <summary>
        /// X value for this Point
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Y value for this Point
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// Constructor for a Point
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        public Point(double X, double Y)
        {
            this.X = X;
            this.Y = Y;
        }

        /// <summary>
        /// Convert this Point to a System.Windows.Point
        /// </summary>
        /// <returns></returns>
        public System.Windows.Point toSysWinPoint()
        {
            return new System.Windows.Point(X, Y);
        }

        /// <summary>
        /// Convert this Point to a System.Drawing.Point
        /// </summary>
        /// <returns></returns>
        public System.Drawing.Point toSysDrwPoint()
        {
            return new System.Drawing.Point((int)X, (int)Y);
        }

        /// <summary>
        /// Adds a vector to this Point
        /// If vect is null, then return this point
        /// </summary>
        /// <param name="vect"></param>
        /// <returns></returns>
        public Point add(Point vect)
        {
            if (vect == null)
                return this;
            return new Point(X + vect.X, Y + vect.Y);
        }

        /// <summary>
        /// Subtract a Point to this Point
        /// If point is null, then return this point
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Point substract(Point point)
        {
            if (point == null)
                return this;
            return new Point(X - point.X, Y - point.Y);
        }

        /// <summary>
        /// Compares this Point with another given Point, SysPoint, or DrwPoint
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(System.Object obj)
        {
            // If parameter is null return false
            if (obj == null) return false;

            if (obj.GetType() == typeof(System.Windows.Point))
            {
                System.Windows.Point p = (System.Windows.Point) obj;
                return (p.X == this.X && p.Y == this.Y);
            }
            else if (obj.GetType() == typeof(System.Drawing.Point))
            {
                System.Drawing.Point p = (System.Drawing.Point)obj;
                return (p.X == this.X && p.Y == this.Y);
            }

            // If parameter cannot be cast to Point return false
            Point point = obj as Point;
            if ((System.Object)point == null) return false;

            // Return true if the fields match
            return (point.X == this.X && point.Y == this.Y);
        }

        /// <summary>
        /// Overriding this method to be able to compare two points using
        /// the equals method.
        /// @see: http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + this.X.GetHashCode();
                hash = hash * 23 + this.Y.GetHashCode();
                return hash;
            }
        }
    }
}
