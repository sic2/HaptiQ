using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

using Input_API;
using HaptiQ_API;

namespace HapticClientAPI
{
    /// <summary>
    /// Represent an haptic line.
    /// </summary>
    public class HapticLine : HapticShape
    {
        private Tuple<Point, Point> _pair;

        /// <summary>
        /// Constructor accepting System.Windows Points
        /// </summary>
        /// <param name="v"></param>
        /// <param name="w"></param>
        public HapticLine(System.Windows.Point v, System.Windows.Point w)
        {
            _pair = new Tuple<Point, Point>(new Point(v.X, v.Y), new Point(w.X, w.Y));
            this.geometry = new LineGeometry(_pair.Item1.toSysWinPoint(), _pair.Item2.toSysWinPoint());
        }

        /// <summary>
        /// Set the color of the HapticLine.
        /// Also automatically set the thickness of the HapticLine to be 20.
        /// </summary>
        /// <param name="brush"></param>
        public override void color(Brush brush)
        {
            this.Stroke = brush;
        }

        /// <summary>
        /// Return true if point is in the HapticLine
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        protected override bool pointIsInside(Point point)
        {
            return pointIsCloseToSegment(point, _pair.Item1, _pair.Item2, NEARNESS_TOLLERANCE);
        }

        /// <summary>
        /// Returns true if a given point is near a segment.
        /// Play sound, changing duration when getting further away.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="startLine"></param>
        /// <param name="endLine"></param>
        /// <param name="TOLLERANCE"></param>
        /// <returns></returns>
        protected override bool pointIsCloseToSegment(Point point, Point startLine, Point endLine, double TOLLERANCE)
        {
            double dst = distancePointToSegment(point, startLine, endLine);
            int duration = - (int) dst * ((BeepOutput.MIN_DURATION - BeepOutput.MAX_DURATION) / (int) TOLLERANCE);
            if (dst <= TOLLERANCE)
            {
                BeepOutput.Beep(duration);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns an edge-corner behaviour based on the position of the HaptiQ
        /// on the HapticLine
        /// </summary>
        /// <param name="haptiQ"></param>
        /// <returns></returns>
        protected override IBehaviour chooseBehaviour(HaptiQ haptiQ)
        {
            List<Tuple<Point, Point>> lines = new List<Tuple<Point, Point>>();
            lines.Add(_pair);
            return new EdgeCornerBehaviour(haptiQ, lines);
        }

        /// <summary>
        /// Return the two end points of this haptic line
        /// </summary>
        /// <returns></returns>
        public Tuple<Point, Point> getEndPoints()
        {
            return _pair;
        }

        /// <summary>
        /// Override OnRender to display tollerance borders of the shape
        /// </summary>
        /// <param name="drawingContext"></param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            drawingContext.DrawEllipse(null, new Pen(Brushes.Red, 1.0),
                _pair.Item1.toSysWinPoint(), NEARNESS_TOLLERANCE, NEARNESS_TOLLERANCE);
            drawingContext.DrawEllipse(null, new Pen(Brushes.Red, 1.0),
                _pair.Item2.toSysWinPoint(), NEARNESS_TOLLERANCE, NEARNESS_TOLLERANCE);
         }
    }
}
