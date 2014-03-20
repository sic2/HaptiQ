using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Media;

using Input_API;
using MHTP_API;

namespace HapticClientAPI
{
    public class HapticRectangle : HapticShape
    {
        private const int BORDERS_TOLLERANCE = 10;

        private double x;
        private double y;
        private double width;
        private double height;

        public HapticRectangle(double x, double y, double width, double height) : base()
        {
            this.x = x; this.y = y; this.width = width; this.height = height;
            this.geometry = new RectangleGeometry(new System.Windows.Rect(x, y, width, height));
            
            connectionPoints.Add(new Point(x + (width / 2.0), y));
            connectionPoints.Add(new Point(x + (width / 2.0), y + height));
            connectionPoints.Add(new Point(x, y + (height / 2.0)));
            connectionPoints.Add(new Point(x + width, y + (height / 2.0)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="orientation"></param>
        public override Tuple<int, IBehaviour, IBehaviour> handleInput(Point point, double orientation)
        {
            if (pointIsInside(point))
            {
                if (state == STATE.up)
                {
                    SpeechOutput.Instance.speak("Haptic Rectangle");
                    state = STATE.down;
                }
                IBehaviour prevBehaviour = _currentBehaviour;
                _currentBehaviour = chooseBehaviour(point, orientation);
                int rule = 2;
                if (_currentBehaviour.Equals(prevBehaviour))
                {
                    rule = 3;
                }
                return new Tuple<int, IBehaviour, IBehaviour>(rule, _currentBehaviour, prevBehaviour);
            }
            else if (state == STATE.down)
            {
                state = STATE.up;
                IBehaviour prevBehaviour = _currentBehaviour;
                _currentBehaviour = new BasicBehaviour(BasicBehaviour.TYPES.flat);
                int rule = 2;
                if (_currentBehaviour.Equals(prevBehaviour))
                {
                    rule = 3;
                }
                return new Tuple<int, IBehaviour, IBehaviour>(rule, _currentBehaviour, prevBehaviour);
            }
            return new Tuple<int, IBehaviour, IBehaviour>(1, _currentBehaviour, null);
        }

        public override void handlePress()
        {
            // TODO - speak
        }

        private bool pointIsInside(Point point)
        {
            return (point.X >= (x - BORDERS_TOLLERANCE) &&
                point.X <= (x + width + BORDERS_TOLLERANCE)) && 
                (point.Y >= (y - BORDERS_TOLLERANCE) &&
                point.Y <= (y + height + BORDERS_TOLLERANCE));
        }

        private IBehaviour chooseBehaviour(Point point, double orientation)
        {
            IBehaviour behaviour = null;
            lock (behaviourLock)
            {
                state = STATE.move;

                Point bottomLeft = new Point(x, y + height);
                Point bottomRight = new Point(x + width, y + height);
                Point topLeft = new Point(x, y);
                Point topRight = new Point(x + width, y);

                List<Tuple<Point, Point>> lines = new List<Tuple<Point, Point>>();
                if (pointIsCloseToLine(point, bottomLeft, bottomRight) && 
                    pointIsCloseToLine(point, topLeft, bottomLeft)) // bottom-left corner
                {
                    lines.Add(new Tuple<Point, Point>(bottomLeft, bottomRight));
                    lines.Add(new Tuple<Point, Point>(topLeft, bottomLeft));
                }
                else if (pointIsCloseToLine(point, topLeft, topRight) &&
                        pointIsCloseToLine(point, topLeft, bottomLeft)) // top-left corner
                {
                    lines.Add(new Tuple<Point, Point>(topLeft, topRight));
                    lines.Add(new Tuple<Point, Point>(topLeft, bottomLeft));
                }
                else if (pointIsCloseToLine(point, topLeft, topRight) &&
                    pointIsCloseToLine(point, topRight, bottomRight)) // top-right corner
                {
                    lines.Add(new Tuple<Point, Point>(topLeft, topRight));
                    lines.Add(new Tuple<Point, Point>(topRight, bottomRight));
                }
                else if (pointIsCloseToLine(point, topRight, bottomRight) &&
                    pointIsCloseToLine(point, bottomLeft, bottomRight)) // bottom-right corner
                {
                    lines.Add(new Tuple<Point, Point>(topRight, bottomRight));
                    lines.Add(new Tuple<Point, Point>(bottomLeft, bottomRight));
                }
                else if (pointIsCloseToLine(point, bottomLeft, bottomRight)) // horizontal
                {
                    lines.Add(new Tuple<Point, Point>(bottomLeft, bottomRight));
                }
                else if (pointIsCloseToLine(point, topLeft, topRight)) // horizontal
                {
                    lines.Add(new Tuple<Point, Point>(topLeft, topRight));
                }
                else if (pointIsCloseToLine(point, topLeft, bottomLeft)) // vertical
                {
                    lines.Add(new Tuple<Point, Point>(topLeft, bottomLeft));
                }
                else if (pointIsCloseToLine(point, topRight, bottomRight)) // vertical
                {
                    lines.Add(new Tuple<Point, Point>(topRight, bottomRight));
                }
                else
                {
                    behaviour = new BasicBehaviour(BasicBehaviour.TYPES.notification);
                }
                   
                if (behaviour == null)
                {
                    behaviour = new DirectionBehaviour(lines, orientation);
                }
                
                state = STATE.down;
                
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
        }
    }
   
}