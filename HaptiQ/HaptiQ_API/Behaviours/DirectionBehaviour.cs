using System;
using System.Collections.Generic;

using Input_API;

namespace HaptiQ_API
{
    public class DirectionBehaviour : Behaviour
    {
        private const int NUMBER_ACTUATORS_DIVIDER = 4;
        private const double HIGH_POSITION_PERCENTAGE = 0.8;

        private List<Tuple<Point, Point>> _lines;

        private int[] staticActuatorsMatrix = new int[] 
                         { 10, // 4-HaptiQ 1-line
                           68}; // 8 -HaptiQ 1-line

        private int[][] dynamicActuatorsMatrix = new int[][]
                        {
                            new int[]{9, 6}, // 4-HaptiQ 1-line
                            new int[]{66, 36}// 8-HaptiQ 1-line
                        };

        /// <summary>
        /// Constructor for direction behaviour. 
        /// </summary>
        /// <param name="haptiQ"></param>
        /// <param name="lines">Geometric lines indicating the direction</param>
        public DirectionBehaviour(HaptiQ haptiQ, List<Tuple<Point, Point>> lines)
            : base(haptiQ)
        {
            _lines = lines;
            TIME = 0;
            highPosition = HIGH_POSITION_PERCENTAGE;
            lowPosition = MIN_POSITION;
        }

        /// <summary>
        /// Play this behaviour. 
        /// </summary>
        /// <returns></returns>
        public override Dictionary<int, double> play()
        {
            Dictionary<int, double> retval = new Dictionary<int, double>();
            TIME++;

            // ASSUME number of actuators is either 4 or 8
            int numberActuators = _actuators.Count;
            bool isCorner = _lines.Count == 2 ? true : false;
            if (isCorner)
            {
                cornerBehaviour(ref retval);
            }
            else
            {
                segmentBehaviour(ref retval);
            }

            System.Threading.Thread.Sleep(200);
            return retval;
        }

        private void segmentBehaviour(ref Dictionary<int, double> output)
        {
            segmentBehaviour(_lines[0], ref output);
        }

        private void segmentBehaviour(Tuple<Point, Point> segment, ref Dictionary<int, double> output)
        {
            int sector = getSector(segment, _orientation, _actuators.Count, _actuators.Count);
            int matrixIndex = _actuators.Count / NUMBER_ACTUATORS_DIVIDER - 1;
            if (sector % 2 == 0) // Static sector
            {
                int activeActs = staticActuatorsMatrix[matrixIndex];
                activeActs = RshiftActs(activeActs, (int)(sector / 2), _actuators.Count);
                bitsToActuators(_actuators.Count, activeActs, false, true, ref output);
            }
            else // Pulsing sector
            {
                int[] acts = (int[])dynamicActuatorsMatrix[matrixIndex].Clone();
                acts[0] = RshiftActs(acts[0], (int)((sector - 1) / 2), _actuators.Count);
                acts[1] = RshiftActs(acts[1], (int)((sector - 1) / 2), _actuators.Count);
                bitsToActuators(_actuators.Count, acts[0], TIME % 2 == 0, false, ref output);
                bitsToActuators(_actuators.Count, acts[1], TIME % 2 != 0, false, ref output);
                setZerosToMinimum(_actuators.Count, (acts[0] | acts[1]), ref output);
            }
        }

        private void cornerBehaviour(ref Dictionary<int, double> output)
        {
            double sectorRange = (2 * Math.PI) / (2.0 * _actuators.Count);
            double normAngle = (_orientation > 0 ? _orientation : (2 * Math.PI + _orientation)) + (sectorRange / 2.0);
            int sector = (int)Math.Floor(normAngle / sectorRange) % (_actuators.Count * 2);

            // Two main actuators - Note that shifting is needed to apply orientation
            int actuator1 = vectorToActuator(_lines[0], _actuators.Count);
            int actuator2 = vectorToActuator(_lines[1], _actuators.Count);

            if (actuator1 == -1 || actuator2 == -1)
            {
                segmentBehaviour(_lines[0], ref output);
                //segmentBehaviour(_lines[1], ref output);
            }
            else if (sector % 2 == 0) // Combine the two actuators
            {
                actuator1 = actuator1 | actuator2;
                actuator1 = RshiftActs(actuator1, (int)(sector / 2), _actuators.Count);
                bitsToActuators(_actuators.Count, actuator1, false, true, ref output);
            }
            else // Combinations of actuators to virtually create corners
            {
                actuator1 = RshiftActs(actuator1, (int)((sector - 1) / 2), _actuators.Count);
                actuator2 = RshiftActs(actuator2, (int)((sector - 1) / 2), _actuators.Count);
                int actuator3 = RshiftActs(actuator1, 1, _actuators.Count);

                int actuator4 = 0;
                if (_actuators.Count == 8)
                {
                    actuator4 = RshiftActs(actuator2, 1, _actuators.Count);
                    bitsToActuators(_actuators.Count, actuator4, TIME % 2 != 0, false, ref output);
                }

                bitsToActuators(_actuators.Count, actuator1, TIME % 2 != 0, false, ref output);
                bitsToActuators(_actuators.Count, actuator2, TIME % 2 == 0, false, ref output);
                bitsToActuators(_actuators.Count, actuator3, TIME % 2 == 0, false, ref output);
                setZerosToMinimum(_actuators.Count, (actuator1 | actuator2 | actuator3 | actuator4), ref output); 
            }
        }
 
        // [ ASSUMPTION ] only angles at 90 degrees for 4 actuators
        // and angles at 90 and 135 degrees for 8 actuators
        // Order of points in segment is important. 
        // The coordinates used negate y (as in most graphics programs)
        // So the origin is to the top-left corner.
        private int vectorToActuator(Tuple<Point, Point> segment, int numberActuators)
        {
            if (numberActuators == 4)
            {
                if (segment.Item1.X == segment.Item2.X && segment.Item1.Y > segment.Item2.Y) return 1;
                if (segment.Item1.X < segment.Item2.X && segment.Item1.Y == segment.Item2.Y) return 2;
                if (segment.Item1.X == segment.Item2.X && segment.Item1.Y < segment.Item2.Y) return 4;
                if (segment.Item1.X > segment.Item2.X && segment.Item1.Y == segment.Item2.Y) return 8;
            }
            else // 8-HaptiQ
            {
                // right degrees angles
                if (segment.Item1.X == segment.Item2.X && segment.Item1.Y > segment.Item2.Y) return 1;
                if (segment.Item1.X < segment.Item2.X && segment.Item1.Y == segment.Item2.Y) return 4;
                if (segment.Item1.X == segment.Item2.X && segment.Item1.Y < segment.Item2.Y) return 16;
                if (segment.Item1.X > segment.Item2.X && segment.Item1.Y == segment.Item2.Y) return 64;
                // 45 degrees angles
                if (segment.Item1.X < segment.Item2.X && segment.Item1.Y > segment.Item2.Y) return 2;
                if (segment.Item1.X > segment.Item2.X && segment.Item1.Y > segment.Item2.Y) return 128;
                if (segment.Item1.X < segment.Item2.X && segment.Item1.Y < segment.Item2.Y) return 8;
                if (segment.Item1.X > segment.Item2.X && segment.Item1.Y < segment.Item2.Y) return 32;
            }
            return -1;
        }

        /// <summary>
        /// Override equals to allow DirectionBehaviour to be compared correctly.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(System.Object obj)
        {
            // If parameter is null return false
            if (obj == null) return false;

            // If parameter cannot be cast to Point return false
            DirectionBehaviour p = obj as DirectionBehaviour;
            if ((System.Object)p == null) return false;

            // Return true if the fields match
            return (p._lines == this._lines && 
                p._orientation == this._orientation);
        }

        // @see: http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode
        public override int GetHashCode()
        {
            unchecked 
            {
                int hash = 17;
                hash = hash * 23 + this._lines.GetHashCode();
                hash = hash * 23 + this._orientation.GetHashCode();
                return hash;
            }
        }
         
    }
}
