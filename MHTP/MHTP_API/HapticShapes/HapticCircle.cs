using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Media;
using System.Globalization;

using Input_API;
using MHTP_API;

namespace HapticClientAPI
{
    public class HapticCircle : HapticShape
    {
        private double x;
        private double y;
        private double radius;

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

        public override Tuple<BEHAVIOUR_RULES, IBehaviour, IBehaviour> handleInput(MHTP mhtp)
        {
            return handleInput(mhtp, pointIsInside(mhtp.position));
        }

        public override void handlePress(Point point)
        {
            // TODO
        }

        protected override IBehaviour chooseBehaviour(MHTP mhtp)
        {
            return new BasicBehaviour(mhtp, BasicBehaviour.TYPES.notification, getFrequency(mhtp.position));
        }

        private bool pointIsInside(Point point)
        {
            return Math.Pow((point.X - x), 2) + Math.Pow(point.Y - y, 2) < Math.Pow(radius + NEARNESS_TOLLERANCE, 2);
        }

        private double getFrequency(Point point)
        {
            double dst = dstFromCenter(point);
            if (dst == 0) return 1;
            return (-1.0 / (2 * radius)) * dst + 1;
        }

        private double dstFromCenter(Point point)
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
