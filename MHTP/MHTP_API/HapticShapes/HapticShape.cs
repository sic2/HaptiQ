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
        protected enum STATE { down, move, up};
        protected STATE state;
        protected Geometry geometry;
        private const double NEARNESS_TOLLERANCE = 20.0;

        protected Object behaviourLock = new Object();

        protected String information;

        public List<Point> connectionPoints;

        protected IBehaviour _currentBehaviour;
        
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
        /// Returns true if a given point is near a straight line defined by two end points.
        /// TODO - add nearness tollerance as parameter
        /// TODO - move this function to helper? 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="startLine"></param>
        /// <param name="endLine"></param>
        /// <returns></returns>
        protected bool pointIsCloseToLine(Point point, Point startLine, Point endLine)
        {
            double d = Math.Abs(((endLine.X - startLine.X) * (startLine.Y - point.Y) -
                (startLine.X - point.X) * (endLine.Y - startLine.Y)) /
                (Math.Sqrt(Math.Pow(endLine.X - startLine.X, 2) + Math.Pow(endLine.Y - startLine.Y, 2))));
            return d <= NEARNESS_TOLLERANCE;
        }

        /// <summary>
        /// Method definition as in #IHapticObject. 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="orientation"></param>
        public abstract Tuple<int, IBehaviour, IBehaviour> handleInput(Point point, double orientation);

        // TODO - method abstract structure still not completely defined.
        public abstract void handlePress();
    }
}
