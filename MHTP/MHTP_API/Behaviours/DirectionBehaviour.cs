using System;
using System.Collections.Generic;

using Input_API;

namespace MHTP_API
{
    public class DirectionBehaviour : Behaviour
    {
        public int TIME { get; set; }

        private double _orientation;

        private const int NUMBER_ACTUATORS_DIVIDER = 4;
        private const int FOUR_ACTUATORS = 4;
        private const int EIGHT_ACTUATORS = 8;
        private const double HIGH_POSITION_PERCENTAGE = 0.8;

        private List<Tuple<Point, Point>> _lines;

        private int[] staticActuatorsMatrix = new int[] 
                         { 10, // 4-MHTP 1-line
                           68}; // 8 -MHTP 1-line

        private int[][] dynamicActuatorsMatrix = new int[][]
                        {
                            new int[]{12, 3}, // 4-MHTP 1-line
                            new int[]{132, 72}// 8-MHTP 1-line
                        };

        /// <summary>
        /// Constructor for direction behaviour. 
        /// </summary>
        /// <param name="lines">Geometric lines indicating the direction</param>
        /// <param name="orientation">Orientation of the device in radians</param>
        public DirectionBehaviour(List<Tuple<Point, Point>> lines, double orientation)
        {
            _lines = lines;
            _orientation = orientation;
            TIME = 0;
            highPosition = HIGH_POSITION_PERCENTAGE;
            lowPosition = MIN_POSITION;
        }

        /// <summary>
        /// Play this behaviour. 
        /// </summary>
        /// <param name="actuators"></param>
        /// <param name="pressureData"></param>
        /// <returns></returns>
        public override Dictionary<int, double> play(SerializableDictionary<int, SerializableTuple<int, int>> actuators,
            Dictionary<int, double> pressureData)
        {
            Dictionary<int, double> retval = new Dictionary<int, double>();
            TIME++;

            // Normalise number of actuators. This is necessary, otherwise the methods used 
            // below do not behave correctly
            int numberActuators = actuators.Count <= FOUR_ACTUATORS ? FOUR_ACTUATORS : EIGHT_ACTUATORS;
            bool isCorner = _lines.Count == 2 ? true : false;
            if (isCorner)
            {
                cornerBehaviour(actuators, pressureData, numberActuators, ref retval);
            }
            else
            {
                segmentBehaviour(actuators, pressureData, numberActuators, ref retval);
            }

            System.Threading.Thread.Sleep(200);
            return retval;
        }

        private void segmentBehaviour(SerializableDictionary<int, SerializableTuple<int, int>> actuators,
            Dictionary<int, double> pressureData, int numberActuators, ref Dictionary<int, double> output)
        {
            int sector = getSector(_lines[0], _orientation, numberActuators, numberActuators);
            int matrixIndex = numberActuators / NUMBER_ACTUATORS_DIVIDER - 1;
            if (sector % 2 == 0) // Static sector
            {
                int activeActs = staticActuatorsMatrix[matrixIndex];
                activeActs = RshiftActs(activeActs, (int)(sector / 2), numberActuators);
                bitsToActuators(actuators, activeActs, false, true, ref output);
            }
            else // Pulsing sector
            {
                int[] acts = dynamicActuatorsMatrix[matrixIndex];
                acts[0] = RshiftActs(acts[0], (int)((sector - 1) / 2), numberActuators);
                acts[1] = RshiftActs(acts[1], (int)((sector - 1) / 2), numberActuators);
                bitsToActuators(actuators, acts[0], TIME % 2 == 0, false, ref output);
                bitsToActuators(actuators, acts[1], TIME % 2 != 0, false, ref output);
                setZerosToMinimum(actuators, (acts[0] | acts[1]), ref output); 
            }
        }

        private void cornerBehaviour(SerializableDictionary<int, SerializableTuple<int, int>> actuators,
            Dictionary<int, double> pressureData, int numberActuators, ref Dictionary<int, double> output)
        {
            double sectorRange = (2 * Math.PI) / (2.0 * numberActuators);
            double normAngle = _orientation + (sectorRange / 2.0);
            int sector = (int)Math.Floor(normAngle / sectorRange) % numberActuators;

            int actuator1 = vectorToActuator(_lines[0], numberActuators);
            int actuator2 = vectorToActuator(_lines[1], numberActuators);
            if (sector % 2 == 0)
            {
                actuator1 += actuator2;
                actuator1 = LshiftActs(actuator1, (int)(sector / 2), numberActuators);
                bitsToActuators(actuators, actuator1, false, true, ref output);
            }
            else
            {
                int actuator3 = LshiftActs(actuator2, 1, numberActuators);

                actuator1 = LshiftActs(actuator1, (int)(sector / 2), numberActuators);
                actuator2 = LshiftActs(actuator2, (int)(sector / 2), numberActuators);
                actuator3 = LshiftActs(actuator3, (int)(sector / 2), numberActuators);

                bitsToActuators(actuators, actuator1, TIME % 2 == 0, false, ref output);
                bitsToActuators(actuators, actuator2, TIME % 2 != 0, false, ref output);
                bitsToActuators(actuators, actuator3, TIME % 2 == 0, false, ref output);
                setZerosToMinimum(actuators, (actuator1 | actuator2 | actuator3), ref output); 
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
            else // 8-MHTP
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
