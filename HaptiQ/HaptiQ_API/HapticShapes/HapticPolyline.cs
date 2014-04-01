using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Media;

using Input_API;
using HaptiQ_API;


namespace HapticClientAPI
{
    public class HapticPolyline : HapticShape
    {
        private List<Point> _points;

        public HapticPolyline(List<System.Windows.Point> points)
        {
            _points = new List<Point>();
            // Use Input_API points
            foreach (System.Windows.Point point in points)
            {
                _points.Add(new Point(point.X, point.Y));
            }
            GeometryGroup group = new GeometryGroup();
            for (int i = 0; i < _points.Count() - 1; i++)
            {
                group.Children.Add(new LineGeometry(_points[i].toSysWinPoint(),
                   _points[i + 1].toSysWinPoint()));
            }
            this.geometry = group;
        }

        /// <summary>
        /// Set the color of the HapticPolyline.
        /// Also automatically set the thickness of the HapticPolyline to be 20.
        /// </summary>
        /// <param name="brush"></param>
        public override void color(Brush brush)
        {
            this.Stroke = brush;
            this.StrokeThickness = 20;
        }

        /// <summary>
        /// Handle a press.
        /// This method needs to be implemented if a new feature is wanted.
        /// </summary>
        /// <param name="haptiQ"></param>
        public override void handlePress(HaptiQ haptiQ)
        {
            // Do nothing
        }

        protected override bool pointIsInside(Point point)
        {
            return pointIsInPolyline(point);
        }

        private bool pointIsInPolyline(Point point)
        {
            for (int i = 0; i < _points.Count() - 1; i++)
            {
                if (pointIsCloseToSegment(point, _points[i], _points[i + 1], NEARNESS_TOLLERANCE))
                   return true;
            }
            // Test corners, which have a different nearness tollerance factor
            for (int i = 0; i < _points.Count() - 2; i++)
            {
                if (pointIsCloseToSegment(point, _points[i], _points[i + 1], CORNER_NEARNESS_TOLLERANCE) &&
                   pointIsCloseToSegment(point, _points[i + 1], _points[i + 2], CORNER_NEARNESS_TOLLERANCE))
                   return true;
            }

            return false;
        }

        protected override IBehaviour chooseBehaviour(HaptiQ haptiQ)
        {
            List<Tuple<Point, Point>> lines = new List<Tuple<Point, Point>>();

            for (int i = 0; i < _points.Count() - 2; i++)
            {
                // XXX - pointIsClose to segment is called twice (in this method and from handle input)
                if (pointIsCloseToSegment(haptiQ.position, _points[i], _points[i + 1], CORNER_NEARNESS_TOLLERANCE) &&
                    pointIsCloseToSegment(haptiQ.position, _points[i + 1], _points[i + 2], CORNER_NEARNESS_TOLLERANCE))
                {
                    lines.Add(new Tuple<Point, Point>(_points[i + 1], _points[i]));
                    lines.Add(new Tuple<Point, Point>(_points[i + 1], _points[i + 2]));
                    break;
                }
            }

            if (lines.Count() == 0)
            {
                for (int i = 0; i < _points.Count() - 1; i++)
                {
                    if (pointIsCloseToSegment(haptiQ.position, _points[i], _points[i + 1], NEARNESS_TOLLERANCE))
                    {
                        lines.Add(new Tuple<Point, Point>(_points[i], _points[i + 1]));
                        break;
                    }
                }
            }
            return new DirectionBehaviour(haptiQ, lines);
        }

        /// <summary>
        /// Return the points that make this polyline
        /// </summary>
        /// <returns></returns>
        public List<Point> getPoints()
        {
            return _points;
        }

        /// <summary>
        /// Override OnRender to display tollerance borders of the shape
        /// </summary>
        /// <param name="drawingContext"></param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            foreach (Point point in _points)
            { 
                drawingContext.DrawEllipse(null, new Pen(Brushes.Red, 1.0),
                point.toSysWinPoint(), NEARNESS_TOLLERANCE, NEARNESS_TOLLERANCE);
            }
        }
    }
}
