using System;
using System.Collections.Generic;

using System.Windows.Shapes;
using System.Windows.Media;

using Input_API;
using MHTP_API;

namespace HapticClientAPI
{
    /// <summary>
    /// Any custom HapticShape must extend this class.
    /// HapticShape extends Shape, allowing geometric figures to be displayed 
    /// by WPF applications as well.
    /// In addition, HapticShape plays the role of the observer of the MHTPsManager.
    /// MHTPsManager notifies HapticShape everytime a new valid input is read. 
    /// 
    /// Issue:
    /// - It is possible to extend event handlers, such as touchDown, touchMove, touchUp, etc..,
    ///   from Shape. This would lead to cleaner code, since not MHTPsManager
    ///   notifies HapticShape objects at any input (even when outside). 
    ///   However, I found out that such event handlers were getting called at a lower frequency
    ///   compared to how often we can pull input data from out displaying device. 
    /// Solution:
    /// - Use of gylphs
    /// </summary>
    abstract public class HapticShape : Shape, IHapticObject
    {
        /// <summary>
        /// STATE enum used for indicating the current state of an haptic shape
        /// </summary>
        protected enum STATE { down, move, up};
        /// <summary>
        /// State of this shape
        /// </summary>
        protected STATE state;
        /// <summary>
        /// Geometry used to render this shape
        /// </summary>
        protected Geometry geometry;
        /// <summary>
        /// Tollerance value for lines
        /// </summary>
        protected const double NEARNESS_TOLLERANCE = 20.0;
        /// <summary>
        /// Tollerance value for corners (two adjacent lines)
        /// </summary>
        protected const double CORNER_NEARNESS_TOLLERANCE = 40;

        /// <summary>
        /// Information contained in this shape
        /// </summary>
        protected String information = "No information available";

        protected Object behaviourLock = new Object();
        /// <summary>
        /// Current behaviour for this shape
        /// </summary>
        protected IBehaviour _currentBehaviour;

        /// <summary>
        /// Points structure for this shape
        /// </summary>
        public List<Point> connectionPoints;
        
        /// <summary>
        /// Default constructor for HapticShape. 
        /// Registers this object ad an observer to the 
        /// MHTPsManager.
        /// </summary>
        public HapticShape()
        {
            connectionPoints = new List<Point>();

            state = STATE.up;
            MHTPsManager.Instance.addObserver(this);
        }

        /// <summary>
        /// Return the geometry used for rendering this shape
        /// </summary>
        protected override Geometry DefiningGeometry
        {
            get
            {
                return geometry;
            }
        }

        /// <summary>
        /// Set the color of this HapticShape
        /// </summary>
        /// <param name="brush"></param>
        public virtual void color(Brush brush)
        {
            this.Fill = brush;
        }

        /// <summary>
        /// Add any information to this HapticShape.
        /// Information can be output as sound if necessary
        /// </summary>
        /// <param name="information"></param>
        public void addInformation(String information)
        {
            this.information = information;
        }

        /// <summary>
        /// Returns true if a given point is near a segment. 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="startLine"></param>
        /// <param name="endLine"></param>
        /// <param name="TOLLERANCE"></param>
        /// <returns></returns>
        protected bool pointIsCloseToSegment(Point point, Point startLine, Point endLine, double TOLLERANCE)
        {
            double d = Math.Sqrt(distanceToSegmentSquared(point, startLine, endLine));
            return d <= TOLLERANCE;
        }

        private double distanceSquared(Point v, Point w)
        {
            return Math.Pow(v.X - w.X, 2) + Math.Pow(v.Y - w.Y, 2);
        }

        // @see: // http://stackoverflow.com/questions/849211/shortest-distance-between-a-point-and-a-line-segment
        private double distanceToSegmentSquared(Point p, Point v, Point w)
        {
            double segmentLenghtSqr = distanceSquared(v, w);
            if (segmentLenghtSqr == 0) return distanceSquared(p, v);
            double t = ((p.X - v.X) * (w.X - v.X) + (p.Y - v.Y) * (w.Y - v.Y)) / segmentLenghtSqr;
            if (t < 0) return distanceSquared(p, v);
            if (t > 1) return distanceSquared(p, w);
            return distanceSquared(p, new Point(v.X + t * (w.X - v.X), v.Y + t * (w.Y - v.Y)));
        }

        public Tuple<BEHAVIOUR_RULES, IBehaviour, IBehaviour> handleInput(MHTP mhtp, bool pointIsInside)
        {
            if (pointIsInside)
            {
                if (state == STATE.up)
                {
                    state = STATE.down;
                }
                IBehaviour prevBehaviour = _currentBehaviour;
                _currentBehaviour = chooseBehaviour(mhtp);
                _currentBehaviour.updateNext(prevBehaviour);

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
                _currentBehaviour = new BasicBehaviour(mhtp, BasicBehaviour.TYPES.flat);
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

        /// <summary>
        /// Return a behaviour for this haptic shape
        /// </summary>
        /// <param name="mhtp"></param>
        /// <returns></returns>
        protected abstract IBehaviour chooseBehaviour(MHTP mhtp);

        /// <summary>
        /// Method definition as in #IHapticObject. 
        /// </summary>
        /// <param name="mhtp"></param>
        public abstract Tuple<BEHAVIOUR_RULES, IBehaviour, IBehaviour> handleInput(MHTP mhtp);

        /// <summary>
        /// Handle an actuator press. 
        /// [ TODO ] Currently this event is called only when an actuator is pressed.
        /// Multiple presses are not supported. 
        /// </summary>
        /// <param name="point"></param>
        public abstract void handlePress(Point point);
    }
}
