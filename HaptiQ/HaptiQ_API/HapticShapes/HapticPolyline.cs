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
        List<Tuple<Point, Point>> _lines;
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
        }

        protected override bool pointIsInside(Point point)
        {
            return pointIsInPolyline(point);
        }

        private bool pointIsInPolyline(Point point)
        {
            _lines = new List<Tuple<Point, Point>>();

            bool retval = false;
            for (int i = 0; i < _points.Count() - 2; i++)
            {
                if (pointIsCloseToSegment(point, _points[i], _points[i + 1], CORNER_NEARNESS_TOLLERANCE) &&
                    pointIsCloseToSegment(point, _points[i + 1], _points[i + 2], CORNER_NEARNESS_TOLLERANCE))
                {
                    _lines.Add(new Tuple<Point, Point>(_points[i + 1], _points[i]));
                    _lines.Add(new Tuple<Point, Point>(_points[i + 1], _points[i + 2]));

                    double dst0 = distancePointToSegment(point, _points[i], _points[i + 1]);
                    double dst1 = distancePointToSegment(point, _points[i + 1], _points[i + 2]);
                    if (dst0 < dst1)
                        beepFeedback(point, _points[i], _points[i + 1], CORNER_NEARNESS_TOLLERANCE); 
                    else
                        beepFeedback(point, _points[i + 1], _points[i + 2], CORNER_NEARNESS_TOLLERANCE);
                    retval = true;
                    break;
                }
            }

            for (int i = 0; i < _points.Count() - 1; i++)
            {
                if (pointIsCloseToSegment(point, _points[i], _points[i + 1], NEARNESS_TOLLERANCE))
                {
                    if (_lines.Count() == 0)
                    {
                        _lines.Add(new Tuple<Point, Point>(_points[i], _points[i + 1]));
                        beepFeedback(point, _points[i], _points[i + 1], CORNER_NEARNESS_TOLLERANCE);
                    }
                    retval = true;
                    break;
                }
            }

            return retval;
        }

        protected override IBehaviour chooseBehaviour(HaptiQ haptiQ)
        {
            return new DirectionBehaviour(haptiQ, _lines);
        }

        private void beepFeedback(Point point, Point startLine, Point endLine, double TOLLERANCE)
        {
            double dst = distancePointToSegment(point, startLine, endLine);
            int duration = - (int)dst * ((BeepOutput.MIN_DURATION - BeepOutput.MAX_DURATION) / (int)TOLLERANCE);
            if (dst <= TOLLERANCE)
            {
                Console.WriteLine("dst " + dst + " duration " + duration);
                BeepOutput.Beep(duration);
            }
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
