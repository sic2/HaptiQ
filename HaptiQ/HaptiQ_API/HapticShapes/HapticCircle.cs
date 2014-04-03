using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Media;
using System.Globalization;

using Input_API;
using HaptiQ_API;

namespace HapticClientAPI
{
    /// <summary>
    /// HapticCircle
    /// </summary>
    public class HapticCircle : HapticShape
    {
        private double x;
        private double y;
        private double radius;

        /// <summary>
        /// Construct an HapticCircle
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="radius"></param>
        public HapticCircle(double x, double y, double radius) : base()
        {
            this.x = x; this.y = y; this.radius = radius;
            this.geometry = new EllipseGeometry(new System.Windows.Point(x, y), radius, radius);

            // Credit to Regis Ongaro-Carcy
            connectionPoints.Add(new Point(x + radius, y));
            connectionPoints.Add(new Point(x - radius, y));
            connectionPoints.Add(new Point(x, y + radius));
            connectionPoints.Add(new Point(x, y - radius));
            double sqrtTwo = Math.Sqrt(2) / 2;
            connectionPoints.Add(new Point(x + sqrtTwo * radius, y + sqrtTwo * radius));
            connectionPoints.Add(new Point(x + sqrtTwo * radius, y - sqrtTwo * radius));
            connectionPoints.Add(new Point(x - sqrtTwo * radius, y + sqrtTwo * radius));
            connectionPoints.Add(new Point(x - sqrtTwo * radius, y - sqrtTwo * radius));
        }

        /// <summary>
        /// Return a notificationBehaviour with frequency dictated by the 
        /// position of the HaptiQ within this shape
        /// </summary>
        /// <param name="haptiQ"></param>
        /// <returns></returns>
        protected override IBehaviour chooseBehaviour(HaptiQ haptiQ)
        {
            return new NotificationBehaviour(haptiQ, getFrequency(haptiQ.position));
        }

        /// <summary>
        /// Return true if point is inside this HapticCircle
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        protected override bool pointIsInside(Point point)
        {
            return Math.Pow((point.X - x), 2) + Math.Pow(point.Y - y, 2) < Math.Pow(radius + NEARNESS_TOLLERANCE, 2);
        }

        private double getFrequency(Point point)
        {
            double dst = dstFromCenter(point);
            if (dst == 0) return 1;
            return (-1.0 / (2 * radius)) * dst + 1;
        }

        /// <summary>
        /// Returns the distance between a point and the center of the HapticCircle
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        protected virtual double dstFromCenter(Point point)
        {
            return Math.Sqrt(Math.Pow((point.X - x), 2) + Math.Pow(point.Y - y, 2));
        }

        /// <summary>
        /// Override OnRender to display tollerance borders of the shape
        /// </summary>
        /// <param name="drawingContext"></param>
        protected override void OnRender(DrawingContext drawingContext)
        {

            base.OnRender(drawingContext);
            drawingContext.DrawEllipse(null, new Pen(Brushes.Red, 1.0),
                new System.Windows.Point(x, y), radius + NEARNESS_TOLLERANCE, radius + NEARNESS_TOLLERANCE);

            drawingContext.DrawText(new FormattedText(information,
              CultureInfo.GetCultureInfo("en-us"),
              0,
              new Typeface("Verdana"),
              12, System.Windows.Media.Brushes.Black),
              new System.Windows.Point(x, y));
        }

    }
}
