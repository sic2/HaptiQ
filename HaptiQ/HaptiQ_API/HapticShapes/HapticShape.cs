using System;
using System.Collections.Generic;

using System.Windows.Shapes;
using System.Windows.Media;

using Input_API;

namespace HaptiQ_API
{
    /// <summary>
    /// Any custom HapticShape must extend this class.
    /// HapticShape extends Shape, allowing geometric figures to be displayed 
    /// by WPF applications as well.
    /// In addition, HapticShape plays the role of the observer of the HaptiQsManager.
    /// HaptiQsManager notifies HapticShape everytime a new valid input is read. 
    /// 
    /// Issue:
    /// - It is possible to extend event handlers, such as touchDown, touchMove, touchUp, etc..,
    ///   from Shape. This would lead to cleaner code, since not HaptiQsManager
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
        protected enum STATE 
        { 
            /// <summary>
            /// Device is inside this shape
            /// </summary>
            down, 
            /// <summary>
            /// Device is not inside this shape
            /// </summary>
            up 
        };

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
        /// Associate an HaptiQ with a specific current behaviour
        /// </summary>
        protected Dictionary<uint, Tuple<STATE, IBehaviour>> _HaptiQBehaviours;

        /// <summary>
        /// Points structure for this shape
        /// </summary>
        public List<Point> connectionPoints;

        /// <summary>
        /// List of Tuples, where each tuple contains:
        /// a connection point of this HapticShape and 
        /// the hapticLink leaving from such point.
        /// </summary>
        public List<Tuple<Point, HapticLink>> connections;

        /// <summary>
        /// Action to be executed when pressure input is received
        /// </summary>
        protected IAction _action;

        private bool isSelectable;

        private Brush _color;
        private Brush SELECTION_COLOR = Brushes.Silver;
        
        /// <summary>
        /// Default constructor for HapticShape. 
        /// Registers this object ad an observer to the 
        /// HaptiQsManager.
        /// </summary>
        public HapticShape()
        {
            connectionPoints = new List<Point>();
            connections = new List<Tuple<Point, HapticLink>>();
            _HaptiQBehaviours = new Dictionary<uint, Tuple<STATE, IBehaviour>>();

            isSelected = false;
            isSelectable = true;
            this.StrokeThickness = 20;
            this.Unloaded += new System.Windows.RoutedEventHandler(HapticShape_Unloaded);

            HaptiQsManager.Instance.addObserver(this);
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
            if (_color == null)
                _color = brush; // set initial color
            this.Fill = brush;
        }

        /// <summary>
        /// Set the thickness for this HapticShape. 
        /// </summary>
        /// <param name="value"></param>
        public virtual void thickness(int value)
        {
            this.StrokeThickness = value;
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
        /// Return true if point is inside this HapticShape
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        protected abstract bool pointIsInside(Point point);

        /// <summary>
        /// Returns true if a given point is near a segment. 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="startLine"></param>
        /// <param name="endLine"></param>
        /// <param name="TOLLERANCE"></param>
        /// <returns></returns>
        protected virtual bool pointIsCloseToSegment(Point point, Point startLine, Point endLine, double TOLLERANCE)
        {
            double dst = Math.Sqrt(distanceToSegmentSquared(point, startLine, endLine));
            return dst <= TOLLERANCE;
        }

        /// <summary>
        /// Returns the distance between a point and a segment
        /// </summary>
        /// <param name="point"></param>
        /// <param name="startLine"></param>
        /// <param name="endLine"></param>
        /// <returns></returns>
        protected double distancePointToSegment(Point point, Point startLine, Point endLine)
        {
            double dst = Math.Sqrt(distanceToSegmentSquared(point, startLine, endLine));
            return dst;
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
        /// <param name="haptiQ"></param>
        /// <returns></returns>
        public Tuple<BEHAVIOUR_RULES, IBehaviour, IBehaviour> handleInput(HaptiQ haptiQ)
        {
            if (this.Parent != null)
            {
                Tuple<STATE, IBehaviour> HaptiQState = _HaptiQBehaviours.ContainsKey(haptiQ.getID()) ? _HaptiQBehaviours[haptiQ.getID()] : null;
                if (pointIsInside(haptiQ.position))
                {
                    IBehaviour prevBehaviour = HaptiQState != null ? HaptiQState.Item2 : null;
                    IBehaviour currentBehaviour = chooseBehaviour(haptiQ);
                    currentBehaviour.updateNext(prevBehaviour);

                    BEHAVIOUR_RULES rule = BEHAVIOUR_RULES.SUBS;
                    if (currentBehaviour.Equals(prevBehaviour))
                    {
                        rule = BEHAVIOUR_RULES.NOPE;
                    }
                    _HaptiQBehaviours[haptiQ.getID()] = new Tuple<STATE, IBehaviour>(STATE.down, currentBehaviour);
                    return new Tuple<BEHAVIOUR_RULES, IBehaviour, IBehaviour>(rule, currentBehaviour, prevBehaviour);
                }
                else if (HaptiQState != null && HaptiQState.Item1 == STATE.down)
                {
                    IBehaviour prevBehaviour = HaptiQState.Item2;
                    IBehaviour currentBehaviour = new BasicBehaviour(haptiQ, BasicBehaviour.TYPES.flat);
                    BEHAVIOUR_RULES rule = BEHAVIOUR_RULES.SUBS;
                    if (currentBehaviour.Equals(prevBehaviour))
                    {
                        rule = BEHAVIOUR_RULES.NOPE;
                    }
                    _HaptiQBehaviours[haptiQ.getID()] = new Tuple<STATE, IBehaviour>(STATE.up, currentBehaviour);
                    return new Tuple<BEHAVIOUR_RULES, IBehaviour, IBehaviour>(rule, currentBehaviour, prevBehaviour);
                }
            }

            Tuple<BEHAVIOUR_RULES, IBehaviour, IBehaviour> retval = 
                new Tuple<BEHAVIOUR_RULES, IBehaviour, IBehaviour>(BEHAVIOUR_RULES.REMOVE,
                    _HaptiQBehaviours.ContainsKey(haptiQ.getID()) ? _HaptiQBehaviours[haptiQ.getID()].Item2 : null, null);
            _HaptiQBehaviours[haptiQ.getID()] = new Tuple<STATE, IBehaviour>(STATE.up, null);
            return retval;
        }

        /// <summary>
        /// Return a behaviour for this haptic shape
        /// </summary>
        /// <param name="haptiQ"></param>
        /// <returns></returns>
        protected abstract IBehaviour chooseBehaviour(HaptiQ haptiQ);


        /// <summary>
        /// Handle an actuator press. 
        /// [ XXX ] Currently this event is called only when an actuator is pressed.
        /// Multiple presses are not supported. 
        /// </summary>
        /// <param name="haptiQ"></param>
        public virtual void handlePress(HaptiQ haptiQ)
        {
            Tuple<STATE, IBehaviour> HaptiQState = _HaptiQBehaviours.ContainsKey(haptiQ.getID()) ? _HaptiQBehaviours[haptiQ.getID()] : null;
            if (pointIsInside(haptiQ.position) && HaptiQState != null && HaptiQState.Item1 == STATE.down)
            {
                if (_action != null)
                {
                    _action.run(haptiQ.getID(), haptiQ.getCurrentPressureData());
                }
            }
        }

        /// <summary>
        /// Register action for this HapticShape
        /// </summary>
        /// <param name="action"></param>
        public void registerAction(IAction action)
        {
            _action = action;
        }

        /// <summary>
        /// Defines the actions to execute when rendering this HapticShape
        /// </summary>
        /// <param name="drawingContext"></param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
        }

        /// <summary>
        /// This method is called whenever this HapticShape is removed from its visual parent
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void HapticShape_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {}

        /// <summary>
        /// Make this HapticShape selectable or not.
        /// </summary>
        /// <param name="selectable"></param>
        public void makeObjectSelectable(bool selectable)
        {
            isSelectable = !isSelectable;
        }


        private bool _isSelected;
        /// <summary>
        /// True if this HapticShape is selected. False otherwise.
        /// </summary>
        public bool isSelected 
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                if (_isSelected)
                {
                    color(SELECTION_COLOR);
                    HaptiQsManager.Instance.selectObject(this);
                }
                else
                {
                    color(_color);
                    HaptiQsManager.Instance.deselectObject(this);
                }
            }
        }

        /// <summary>
        /// Called when this object is entered
        /// </summary>
        /// <param name="e"></param>
        protected override void OnTouchEnter(System.Windows.Input.TouchEventArgs e)
        {
            base.OnTouchDown(e);
            if (isSelectable)
            {
                isSelected = !isSelected;
            }
        }
    }
}
