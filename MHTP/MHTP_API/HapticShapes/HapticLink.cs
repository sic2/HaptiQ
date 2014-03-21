using System;

using System.Windows.Media;

using Input_API;
using MHTP_API;

namespace HapticClientAPI
{
    /// <summary>
    /// HapticLink defines a object that links two other objects.
    /// Special haptic behaviour are used to indicate the direction 
    /// of the connection.
    /// </summary>
    public class HapticLink : HapticShape
    {
        private HapticShape _hapticShapeSrc;
        private HapticShape _hapticShapeDst;
        private bool _hasDirection;

        private Tuple<Point, Point> _pair;

        public HapticLink(HapticShape hapticShapeSrc, HapticShape hapticShapeDst, bool hadDirection) : base()
        {
            _hapticShapeSrc = hapticShapeSrc;
            _hapticShapeDst = hapticShapeDst;
            _hasDirection = hadDirection;

            _pair = Helper.findNearestPoints(hapticShapeSrc.connectionPoints, hapticShapeDst.connectionPoints);
            this.geometry = new LineGeometry(_pair.Item1.toSysWinPoint(), _pair.Item2.toSysWinPoint());
        }

        /// <summary>
        /// Set the color of the HapticLink.
        /// Also automatically set the thickness of the HapticLink to be 20.
        /// </summary>
        /// <param name="brush"></param>
        public override void color(Brush brush)
        {
            this.Stroke = brush;
            this.StrokeThickness = 20;
        } 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="orientation"></param>
        public override Tuple<BEHAVIOUR_RULES, IBehaviour, IBehaviour> handleInput(Point point, double orientation)
        {
            if (pointIsCloseToSegment(point, _pair.Item1, _pair.Item2, NEARNESS_TOLLERANCE))
            {
                if (state == STATE.up)
                {
                    SpeechOutput.Instance.speak("Haptic Link");
                    state = STATE.down;
                }
                IBehaviour prevBehaviour = _currentBehaviour;
                _currentBehaviour = chooseBehaviour(point, orientation);
                BEHAVIOUR_RULES rule = BEHAVIOUR_RULES.SUBS;
                if (_currentBehaviour.Equals(prevBehaviour))
                {
                    rule = BEHAVIOUR_RULES.NOPE;
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
            return new Tuple<BEHAVIOUR_RULES, IBehaviour, IBehaviour>(BEHAVIOUR_RULES.REMOVE, _currentBehaviour, null);
        }

        public override void handlePress(Point point)
        {
            // TODO
        }

        private IBehaviour chooseBehaviour(Point point, double orientation)
        {
            IBehaviour behaviour;
            lock (behaviourLock)
            {
                // XXX - apply non-linear function !?
                double highFrequency = 100 * (Helper.distanceBetweenTwoPoints(point, _pair.Item1) / Helper.distanceBetweenTwoPoints(_pair.Item2, _pair.Item1)); // TODO - what about item2? do not hardcode this
                double orientationDeg = Helper.radsToDegrees(orientation);
                double shift = orientationDeg >= 0 ? (orientationDeg + 22.5) / 45.0 : 360 + (orientationDeg + 22.5) / 45.0; // XXX use 45 degrees because 8 zones are identified in a full circle.
                
                // TODO - establish actuators based on orientation of hapticlink too
                behaviour = new PulsationBehaviour((int)shift, highFrequency);
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
            drawingContext.DrawEllipse(null, new Pen(Brushes.Red, 1.0), 
                _pair.Item1.toSysWinPoint(), NEARNESS_TOLLERANCE, NEARNESS_TOLLERANCE);
            drawingContext.DrawEllipse(null, new Pen(Brushes.Red, 1.0), 
                _pair.Item2.toSysWinPoint(), NEARNESS_TOLLERANCE, NEARNESS_TOLLERANCE);

            drawingContext.DrawEllipse(null, new Pen(Brushes.Red, 1.0),
                new Point(0, 0).toSysWinPoint(), NEARNESS_TOLLERANCE, NEARNESS_TOLLERANCE);
        }
    }
}
