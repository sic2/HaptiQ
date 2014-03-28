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
        protected enum STATE { down, up };

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
        protected String information = "";

        /// <summary>
        /// Associate an MHTP with a specific current behaviour
        /// </summary>
        protected Dictionary<uint, Tuple<STATE, IBehaviour>> _mhtpBehaviours;

        /// <summary>
        /// Points structure for this shape
        /// </summary>
        public List<Point> connectionPoints;

        /// <summary>
        /// Action to be executed when pressure input is received
        /// </summary>
        protected IAction _action;
        
        /// <summary>
        /// Default constructor for HapticShape. 
        /// Registers this object ad an observer to the 
        /// MHTPsManager.
        /// </summary>
        public HapticShape()
        {
            connectionPoints = new List<Point>();
            _mhtpBehaviours = new Dictionary<uint, Tuple<STATE, IBehaviour>>();

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

        /// <summary>
        /// Handle behaviours on input
        /// </summary>
        /// <param name="mhtp"></param>
        /// <param name="pointIsInside"></param>
        /// <returns></returns>
        public Tuple<BEHAVIOUR_RULES, IBehaviour, IBehaviour> handleInput(MHTP mhtp, bool pointIsInside)
        {
            Tuple<STATE, IBehaviour> mhtpState = _mhtpBehaviours.ContainsKey(mhtp.getID()) ? _mhtpBehaviours[mhtp.getID()] : null;
            if (pointIsInside)
            {
                IBehaviour prevBehaviour = mhtpState != null ? mhtpState.Item2 : null;
                IBehaviour currentBehaviour = chooseBehaviour(mhtp);
                currentBehaviour.updateNext(prevBehaviour);

                BEHAVIOUR_RULES rule = BEHAVIOUR_RULES.SUBS;
                if (currentBehaviour.Equals(prevBehaviour))
                {
                    rule = BEHAVIOUR_RULES.NOPE;
                }
                _mhtpBehaviours[mhtp.getID()] = new Tuple<STATE, IBehaviour>(STATE.down, currentBehaviour);
                return new Tuple<BEHAVIOUR_RULES, IBehaviour, IBehaviour>(rule, currentBehaviour, prevBehaviour);
            }
            else if (mhtpState != null && mhtpState.Item1 == STATE.down)
            {
                IBehaviour prevBehaviour = mhtpState.Item2;
                IBehaviour currentBehaviour = new BasicBehaviour(mhtp, BasicBehaviour.TYPES.flat);
                BEHAVIOUR_RULES rule = BEHAVIOUR_RULES.SUBS;
                if (currentBehaviour.Equals(prevBehaviour))
                {
                    rule = BEHAVIOUR_RULES.NOPE;
                }
                _mhtpBehaviours[mhtp.getID()] = new Tuple<STATE, IBehaviour>(STATE.up, currentBehaviour);
                return new Tuple<BEHAVIOUR_RULES, IBehaviour, IBehaviour>(rule, currentBehaviour, prevBehaviour);
            }

            Tuple<BEHAVIOUR_RULES, IBehaviour, IBehaviour> retval = 
                new Tuple<BEHAVIOUR_RULES, IBehaviour, IBehaviour>(BEHAVIOUR_RULES.REMOVE,
                    _mhtpBehaviours.ContainsKey(mhtp.getID()) ? _mhtpBehaviours[mhtp.getID()].Item2 : null, null);
            _mhtpBehaviours[mhtp.getID()] = new Tuple<STATE, IBehaviour>(STATE.up, null);
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
        /// <param name="mhtp"></param>
        public abstract void handlePress(MHTP mhtp);

        /// <summary>
        /// Register action for this HapticShape
        /// </summary>
        /// <param name="action"></param>
        public void registerAction(IAction action)
        {
            _action = action;
        }
    }
}
