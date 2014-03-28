﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Media;

using Input_API;
using MHTP_API;


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
        /// Handle input and return appropriate behaviour
        /// </summary>
        /// <param name="mhtp"></param>
        /// <returns></returns>
        public override Tuple<BEHAVIOUR_RULES, IBehaviour, IBehaviour> handleInput(MHTP mhtp)
        {
            return handleInput(mhtp, pointIsInPolyline(mhtp.position));
        }

        /// <summary>
        /// Handle a press.
        /// This method needs to be implemented if a new feature is wanted.
        /// </summary>
        /// <param name="mhtp"></param>
        public override void handlePress(MHTP mhtp)
        {
            // Do nothing
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

        protected override IBehaviour chooseBehaviour(MHTP mhtp)
        {
            List<Tuple<Point, Point>> lines = new List<Tuple<Point, Point>>();

            for (int i = 0; i < _points.Count() - 2; i++)
            {
                // XXX - pointIsClose to segment is called twice (in this method and from handle input)
                if (pointIsCloseToSegment(mhtp.position, _points[i], _points[i + 1], CORNER_NEARNESS_TOLLERANCE) &&
                    pointIsCloseToSegment(mhtp.position, _points[i + 1], _points[i + 2], CORNER_NEARNESS_TOLLERANCE))
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
                    if (pointIsCloseToSegment(mhtp.position, _points[i], _points[i + 1], NEARNESS_TOLLERANCE))
                    {
                        lines.Add(new Tuple<Point, Point>(_points[i], _points[i + 1]));
                        break;
                    }
                }
            }
            return new DirectionBehaviour(mhtp, lines);
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
            foreach (Point point in connectionPoints)
            { 
                drawingContext.DrawEllipse(null, new Pen(Brushes.Red, 1.0),
                point.toSysWinPoint(), NEARNESS_TOLLERANCE, NEARNESS_TOLLERANCE);
            }
        }
    }
}