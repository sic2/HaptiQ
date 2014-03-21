using System;
using System.Collections.Generic;

using Input_API;

namespace MHTP_API
{
    public class DirectionBehaviour : IBehaviour
    {
        private int TIME;
        private double _orientation;

        private double highPosition;
        private double lowPosition;

        private const int NUMBER_ACTUATORS_DIVIDER = 4;
        private const int FOUR_ACTUATORS = 4;
        private const int EIGHT_ACTUATORS = 8;

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
            highPosition = 40;
            lowPosition = 0;
        }

        /// <summary>
        /// Play this behaviour. 
        /// </summary>
        /// <param name="actuators"></param>
        /// <param name="pressureData"></param>
        /// <returns></returns>
        public Dictionary<int, double> play(SerializableDictionary<int, SerializableTuple<int, int>> actuators,
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
            // Determines angle between lines in radians
            double angle = Math.Atan2(_lines[0].Item1.Y - _lines[0].Item2.Y,
                                        _lines[0].Item1.X - _lines[0].Item2.X);
            // Map angle to 0-(2*PI)
            angle = (angle > 0 ? angle : (2 * Math.PI + angle));
            angle += _orientation; // Total angle between lines and device orientation
            // Normalise angle
            double sectorRange = (2 * Math.PI) / (2.0 * numberActuators);
            double normAngle = angle + (sectorRange / 2.0);
            int sector = (int)Math.Floor(normAngle / sectorRange) % numberActuators;

            int matrixIndex = numberActuators / NUMBER_ACTUATORS_DIVIDER - 1;
            if (sector % 2 == 0) // Static sector
            {
                int activeActs = staticActuatorsMatrix[matrixIndex];
                activeActs = shiftActs(activeActs, (int)(sector / 2), numberActuators);
                bitsToActuators(actuators, activeActs, false, true, ref output);
            }
            else // Pulsing sector
            {
                int[] acts = dynamicActuatorsMatrix[matrixIndex];
                acts[0] = shiftActs(acts[0], (int)(sector / 2), numberActuators);
                acts[1] = shiftActs(acts[1], (int)(sector / 2), numberActuators);

                bitsToActuators(actuators, acts[0], TIME % 2 == 0, true, ref output);
                bitsToActuators(actuators, acts[1], TIME % 2 != 0, true, ref output);
            }
        }

        private void cornerBehaviour(SerializableDictionary<int, SerializableTuple<int, int>> actuators,
            Dictionary<int, double> pressureData, int numberActuators, ref Dictionary<int, double> output)
        {
            double sectorRange = (2 * Math.PI) / (2.0 * numberActuators);
            double normAngle = _orientation + (sectorRange / 2.0);
            int sector = (int)Math.Floor(normAngle / sectorRange) % numberActuators;

            int actuator1 = vectorToActuator(_lines[0], numberActuators);
            int actuator2 = shiftActs(actuator1, 1, numberActuators);
            if (sector % 2 == 0)
            {
                actuator1 += actuator2;
                actuator1 = shiftActs(actuator1, (int)(sector / 2), numberActuators);
                bitsToActuators(actuators, actuator1, false, true, ref output);
            }
            else
            {
                int actuator3 = shiftActs(actuator2, 1, numberActuators);

                actuator1 = shiftActs(actuator1, (int)(sector / 2), numberActuators);
                actuator2 = shiftActs(actuator2, (int)(sector / 2), numberActuators);
                actuator3 = shiftActs(actuator3, (int)(sector / 2), numberActuators);

                bitsToActuators(actuators, actuator1, TIME % 2 == 0, false, ref output);
                bitsToActuators(actuators, actuator2, TIME % 2 != 0, false, ref output);
                bitsToActuators(actuators, actuator3, TIME % 2 == 0, false, ref output);
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
        /// Shift acts to the left with carry by offset.
        /// Number of bits is determined by numberActuators.
        /// </summary>
        /// <param name="acts"></param>
        /// <param name="offset"></param>
        /// <param name="numberActuators"></param>
        /// <returns></returns>
        private int shiftActs(int acts, int offset, int numberActuators)
        {
            int limit = (int)(Math.Pow(2, numberActuators));
            return (acts << offset |
                    acts >> (numberActuators - offset)) & (limit - 1);
        }

        private void bitsToActuators(SerializableDictionary<int, SerializableTuple<int, int>> actuators, int activeActuators, bool switchPositionOrder, bool setZeros, ref Dictionary<int, double> output)
        {
            double pos0 = highPosition;
            double pos1 = lowPosition;
            if (switchPositionOrder)
            {
                double tmp = pos0;
                pos0 = pos1;
                pos1 = tmp;
            }
            for (int i = 0; i < actuators.Count; i++)
            {
                output[i] = actuators[i].Item1;
                if ((activeActuators & 1) != 0)
                {
                    output[i] += pos0;
                }
                else if (setZeros)
                {
                    output[i] += pos1;
                }
                activeActuators = activeActuators >> 1;
            }
        }
        
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
