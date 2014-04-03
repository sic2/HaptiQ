using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Media;
using System.Globalization;

using Input_API;

namespace HaptiQ_API
{
    /// <summary>
    /// HapticRectangle class
    /// </summary>
    public class HapticRectangle : HapticShape
    {
        // Constant used to display border tollerance of rectangle
        private const int BORDERS_TOLLERANCE = (int) (NEARNESS_TOLLERANCE / 2);

        private double x;
        private double y;
        private double width;
        private double height;

        /// <summary>
        /// Constructor for HapticRectangle
        /// x, y represent the top-left coordinates relative to the main window
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public HapticRectangle(double x, double y, double width, double height) : base()
        {
            this.x = x; this.y = y; this.width = width; this.height = height;
            this.geometry = new RectangleGeometry(new System.Windows.Rect(x, y, width, height));

            // Mid-points
            connectionPoints.Add(new Point(x + (width / 2.0), y));
            connectionPoints.Add(new Point(x + (width / 2.0), y + height));
            connectionPoints.Add(new Point(x, y + (height / 2.0)));
            connectionPoints.Add(new Point(x + width, y + (height / 2.0)));

            // Corners
            connectionPoints.Add(new Point(x, y));
            connectionPoints.Add(new Point(x, y + height));
            connectionPoints.Add(new Point(x + width, y + height));
            connectionPoints.Add(new Point(x + width, y));
        }

        /// <summary>
        /// Output information content via audio if input was received 
        /// for a device currently in this haptic rectangle
        /// </summary>
        /// <param name="haptiQ"></param>
        public override void handlePress(HaptiQ haptiQ)
        {
            Tuple<STATE, IBehaviour> HaptiQState = _HaptiQBehaviours.ContainsKey(haptiQ.getID()) ? _HaptiQBehaviours[haptiQ.getID()] : null;
            if (pointIsInside(haptiQ.position) && HaptiQState != null && HaptiQState.Item1 == STATE.down)
            {
                if (_action != null)
                {
                    _action.run(haptiQ.getID(), haptiQ.getCurrentPressureData());
                }
                else
                {
                    SpeechOutput.Instance.speak(information);
                }
            }
        }

        /// <summary>
        /// Return true if point is inside this HapticRectangle
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        protected override bool pointIsInside(Point point)
        {
            return (point.X >= (x - BORDERS_TOLLERANCE) &&
                point.X <= (x + width + BORDERS_TOLLERANCE)) && 
                (point.Y >= (y - BORDERS_TOLLERANCE) &&
                point.Y <= (y + height + BORDERS_TOLLERANCE));
        }

        /// <summary>
        /// Return appropriate behaviour based on the position of the HaptiQ within 
        /// this HapticRectangle
        /// </summary>
        /// <param name="haptiQ"></param>
        /// <returns></returns>
        protected override IBehaviour chooseBehaviour(HaptiQ haptiQ)
        {
            IBehaviour behaviour = null;

            Point bottomLeft = new Point(x, y + height);
            Point bottomRight = new Point(x + width, y + height);
            Point topLeft = new Point(x, y);
            Point topRight = new Point(x + width, y);

            List<Tuple<Point, Point>> lines = new List<Tuple<Point, Point>>();
            if (pointIsCloseToSegment(haptiQ.position, bottomLeft, bottomRight, CORNER_NEARNESS_TOLLERANCE) &&
                pointIsCloseToSegment(haptiQ.position, bottomLeft, topLeft, CORNER_NEARNESS_TOLLERANCE)) // bottom-left corner
            {
                lines.Add(new Tuple<Point, Point>(bottomLeft, bottomRight));
                lines.Add(new Tuple<Point, Point>(bottomLeft, topLeft));
            }
            else if (pointIsCloseToSegment(haptiQ.position, topLeft, topRight, CORNER_NEARNESS_TOLLERANCE) &&
                    pointIsCloseToSegment(haptiQ.position, topLeft, bottomLeft, CORNER_NEARNESS_TOLLERANCE)) // top-left corner
            {
                lines.Add(new Tuple<Point, Point>(topLeft, topRight));
                lines.Add(new Tuple<Point, Point>(topLeft, bottomLeft));
            }
            else if (pointIsCloseToSegment(haptiQ.position, topRight, topLeft, CORNER_NEARNESS_TOLLERANCE) &&
                pointIsCloseToSegment(haptiQ.position, topRight, bottomRight, CORNER_NEARNESS_TOLLERANCE)) // top-right corner
            {
                lines.Add(new Tuple<Point, Point>(topRight, topLeft));
                lines.Add(new Tuple<Point, Point>(topRight, bottomRight));
            }
            else if (pointIsCloseToSegment(haptiQ.position, bottomRight, topRight, CORNER_NEARNESS_TOLLERANCE) &&
                pointIsCloseToSegment(haptiQ.position, bottomRight, bottomLeft, CORNER_NEARNESS_TOLLERANCE)) // bottom-right corner
            {
                lines.Add(new Tuple<Point, Point>(bottomRight, topRight));
                lines.Add(new Tuple<Point, Point>(bottomRight, bottomLeft));
            }
            else if (pointIsCloseToSegment(haptiQ.position, bottomLeft, bottomRight, NEARNESS_TOLLERANCE)) // horizontal
            {
                lines.Add(new Tuple<Point, Point>(bottomLeft, bottomRight));
            }
            else if (pointIsCloseToSegment(haptiQ.position, topLeft, topRight, NEARNESS_TOLLERANCE)) // horizontal
            {
                lines.Add(new Tuple<Point, Point>(topLeft, topRight));
            }
            else if (pointIsCloseToSegment(haptiQ.position, topLeft, bottomLeft, NEARNESS_TOLLERANCE)) // vertical
            {
                lines.Add(new Tuple<Point, Point>(topLeft, bottomLeft));
            }
            else if (pointIsCloseToSegment(haptiQ.position, topRight, bottomRight, NEARNESS_TOLLERANCE)) // vertical
            {
                lines.Add(new Tuple<Point, Point>(topRight, bottomRight));
            }
            else
            {
                behaviour = new BasicBehaviour(haptiQ, BasicBehaviour.TYPES.max);
            }
                   
            if (behaviour == null)
            {
                behaviour = new EdgeCornerBehaviour(haptiQ, lines);
            }
               
            return behaviour;
        }

        /// <summary>
        /// Override OnRender to display tollerance borders of the shape
        /// </summary>
        /// <param name="drawingContext"></param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            drawingContext.DrawRectangle(null, new Pen(Brushes.Red, 1.0),
                new System.Windows.Rect(x - BORDERS_TOLLERANCE, y - BORDERS_TOLLERANCE, 
                    width + 2*BORDERS_TOLLERANCE, height + 2*BORDERS_TOLLERANCE));

            drawingContext.DrawText(new FormattedText(information,
              CultureInfo.GetCultureInfo("en-us"),
              0,
              new Typeface("Verdana"),
              12, System.Windows.Media.Brushes.Black),
              new System.Windows.Point(x, y));
        }
    }
   
}