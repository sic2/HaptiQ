using System;
using System.Windows.Media;

using Input_API;
using HaptiQ_API;

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

        private Tuple<Point, Point> _pair;

        /// <summary>
        /// Constructor for HapticLink. 
        /// </summary>
        /// <param name="hapticShapeSrc"></param>
        /// <param name="hapticShapeDst"></param>
        /// <param name="hadDirection"></param>
        public HapticLink(HapticShape hapticShapeSrc, HapticShape hapticShapeDst) : base()
        {
            _hapticShapeSrc = hapticShapeSrc;
            _hapticShapeDst = hapticShapeDst;

            _pair = Helper.findNearestPoints(hapticShapeSrc.connectionPoints, hapticShapeDst.connectionPoints);
            hapticShapeSrc.connections.Add(new Tuple<Point, HapticLink>(_pair.Item1, this));
            hapticShapeDst.connections.Add(new Tuple<Point, HapticLink>(_pair.Item2, this));

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
        /// Handle a press.
        /// This method needs to be implemented if a new feature is wanted.
        /// </summary>
        /// <param name="haptiQ"></param>
        public override void handlePress(HaptiQ haptiQ)
        {
            // Do nothing
        }

        protected override IBehaviour chooseBehaviour(HaptiQ haptiQ)
        {
            double highFrequency = 100 * (Helper.distanceBetweenTwoPoints(haptiQ.position, _pair.Item2) / 
                Helper.distanceBetweenTwoPoints(_pair.Item2, _pair.Item1));
            IBehaviour behaviour = new PulsationBehaviour(haptiQ, new Tuple<Point, Point>(_pair.Item1, _pair.Item2), highFrequency);
            return behaviour;
        }

        protected override bool pointIsInside(Point point)
        {
            return pointIsCloseToSegment(point, _pair.Item1, _pair.Item2, NEARNESS_TOLLERANCE);
        }

        /// <summary>
        /// Return a tuple of haptic shapes connected by this haptic link
        /// </summary>
        /// <returns></returns>
        public Tuple<HapticShape, HapticShape> getLinkedHapticShapes()
        {
            return new Tuple<HapticShape, HapticShape>(_hapticShapeSrc, _hapticShapeDst);
        }

        /// <summary>
        /// Return the two end points of this haptic link
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
            drawingContext.DrawEllipse(Brushes.Red, new Pen(Brushes.Red, 1.0), 
                _pair.Item1.toSysWinPoint(), NEARNESS_TOLLERANCE, NEARNESS_TOLLERANCE);
            drawingContext.DrawEllipse(Brushes.Green, new Pen(Brushes.Green, 1.0), 
                _pair.Item2.toSysWinPoint(), NEARNESS_TOLLERANCE, NEARNESS_TOLLERANCE);

        }
    }
}
