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
        }

        // TODO - refactor
        public override Tuple<BEHAVIOUR_RULES, IBehaviour, IBehaviour> handleInput(Point point, double orientation)
        {
            if (pointIsInside(point))
            {
                if (state == STATE.up)
                {
                    SpeechOutput.Instance.speak("Haptic Circle");
                    state = STATE.down;
                }
                IBehaviour prevBehaviour = _currentBehaviour;
                BasicBehaviour prev = prevBehaviour as BasicBehaviour;
                if (prev != null)
                    _currentBehaviour = new BasicBehaviour(BasicBehaviour.TYPES.notification, prev.currentActiveActuators, prev.prevActiveActuators, getFrequency(point));
                else
                {
                    _currentBehaviour = new BasicBehaviour(BasicBehaviour.TYPES.notification);
                }
                BEHAVIOUR_RULES rule = BEHAVIOUR_RULES.SUBS;
                if (_currentBehaviour.Equals(prevBehaviour))
                {
                    rule = BEHAVIOUR_RULES.NOPE;
                }
                else
                {
                     // Updating behaviour time to allow continuous pulsing
                    _currentBehaviour.TIME = prevBehaviour != null ? prevBehaviour.TIME++ : 0;
                }
                return new Tuple<BEHAVIOUR_RULES, IBehaviour, IBehaviour>(rule, _currentBehaviour, prevBehaviour);
            }
            else if (state == STATE.down)
            {
                state = STATE.up;
                IBehaviour prevBehaviour = _currentBehaviour;
                _currentBehaviour = new BasicBehaviour(BasicBehaviour.TYPES.flat);
                BEHAVIOUR_RULES rule = BEHAVIOUR_RULES.SUBS;
                if (_currentBehaviour.Equals(prevBehaviour))
                {
                    rule = BEHAVIOUR_RULES.NOPE;
                }
                return new Tuple<BEHAVIOUR_RULES, IBehaviour, IBehaviour>(rule, _currentBehaviour, prevBehaviour);
            }

            Tuple<BEHAVIOUR_RULES, IBehaviour, IBehaviour> retval = new Tuple<BEHAVIOUR_RULES, IBehaviour, IBehaviour>(BEHAVIOUR_RULES.REMOVE, _currentBehaviour, null);
            _currentBehaviour = null;
            return retval;
        
        }

        public override void handlePress(Point point)
        {
            // TODO
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
