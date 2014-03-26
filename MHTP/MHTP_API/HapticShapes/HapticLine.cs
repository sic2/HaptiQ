using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

using Input_API;
using MHTP_API;

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
            this.StrokeThickness = 20;
        }

        /// <summary>
        /// Handle input and return appropriate behaviour
        /// </summary>
        /// <param name="mhtp"></param>
        /// <returns></returns>
        public override Tuple<BEHAVIOUR_RULES, IBehaviour, IBehaviour> handleInput(MHTP mhtp)
        {
            return handleInput(mhtp, pointIsCloseToSegment(mhtp.position, _pair.Item1, _pair.Item2, NEARNESS_TOLLERANCE));
        }

        /// <summary>
        /// Handle a press.
        /// This method needs to be implemented if a new feature is wanted.
        /// </summary>
        /// <param name="point"></param>
        public override void handlePress(Point point)
        {
            // Do nothing
        }

        protected override IBehaviour chooseBehaviour(MHTP mhtp)
        {
            List<Tuple<Point, Point>> lines = new List<Tuple<Point, Point>>();
            lines.Add(_pair);
            state = STATE.down;
            return new DirectionBehaviour(mhtp, lines);
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
